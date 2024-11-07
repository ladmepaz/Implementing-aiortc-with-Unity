import argparse
import asyncio
import json
import logging
import os
import platform
from aiortc import RTCPeerConnection, RTCSessionDescription
from aiortc.contrib.media import MediaPlayer, MediaRelay
from aiortc.rtcrtpsender import RTCRtpSender
import websockets

relay = None
webcam = None
pcs = set()


# Función para crear las pistas de video
def create_local_tracks(play_from, decode):
    global relay, webcam

    if play_from:
        player = MediaPlayer(play_from, decode=decode)
        return player.video
    else:
        options = {"framerate": "30", "video_size": "640x480", "rtbufsize": "512M"}
        if relay is None:
            if platform.system() == "Darwin":
                webcam = MediaPlayer(
                    "default:none", format="avfoundation", options=options
                )
            elif platform.system() == "Windows":
                webcam = MediaPlayer(
                    "video=Integrated Camera", format="dshow", options=options
                )
            else:
                webcam = MediaPlayer("/dev/video0", format="v4l2", options=options)
            relay = MediaRelay()
        return None, relay.subscribe(webcam.video)


# Forzar codec de video o audio
def force_codec(pc, sender, forced_codec):
    kind = forced_codec.split("/")[0]
    codecs = RTCRtpSender.getCapabilities(kind).codecs
    transceiver = next(t for t in pc.getTransceivers() if t.sender == sender)
    transceiver.setCodecPreferences(
        [codec for codec in codecs if codec.mimeType == forced_codec]
    )

# Función para manejar las conexiones WebSocket
async def handle_websocket(websocket, path):
    print("WebSocket connection established")

    try:
        async for message in websocket:
            data = json.loads(message)
            print("Mensaje: ", data)
            # Verificar si el mensaje es una oferta SDP
            if "sdp" in data and "type" in data and data["type"] == "offer":
                offer = RTCSessionDescription(sdp=data["sdp"], type=data["type"])
                # print(f"Received offer: {offer.sdp}")

                print("Creating PeerConnection ...")

                pc = RTCPeerConnection()
                pcs.add(pc)
                print(pcs)

                @pc.on("icecandidate")
                async def on_icecandidate(candidate):
                    if candidate:
                        print("Sending ICE candidate")
                        # Serializamos el ICE candidate
                        ice_candidate = {
                            "candidate": candidate.candidate,
                            "sdpMid": candidate.sdpMid,
                            "sdpMLineIndex": candidate.sdpMLineIndex,
                        }
                        # Enviamos el ICE candidate por WebSocket
                        await websocket.send(json.dumps(ice_candidate))

                @pc.on("connectionstatechange")
                async def on_connectionstatechange():
                    print("Connection state is %s" % pc.connectionState)
                    if pc.connectionState == "failed":
                        await pc.close()
                        pcs.discard(pc)

                # Crear las pistas de video/audio
                # video = create_local_tracks(play_from=None, decode=True)[1]
                # #print("Audio: ", audio)

                # open media source
                audio, video = create_local_tracks(
                    args.play_from, decode=not args.play_without_decoding
                )
                print("Video: ", video)
                if video:
                    video_sender = pc.addTrack(video)
                    # video = pc.addTrack(video)
                    if args.video_codec:
                        force_codec(pc, video_sender, args.video_codec)
                    elif args.play_without_decoding:
                        raise Exception(
                            "You must specify the video codec using --video-codec"
                        )
                if audio:
                    audio_sender = pc.addTrack(audio)
                    if args.audio_codec:
                        force_codec(pc, audio_sender, args.audio_codec)
                    elif args.play_without_decoding:
                        raise Exception(
                            "You must specify the audio codec using --audio-codec"
                        )

                # print("HOALS", pc.getTransceivers())
                for transceiver in pc.getTransceivers():
                    print(f"Transceiver direction: {transceiver.direction}")
                    print(f"Transceiver mid: {transceiver.mid}")

                await pc.setRemoteDescription(offer)
                print("Creating answer ...")
                answer = await pc.createAnswer()
                print(answer)
                await pc.setLocalDescription(answer)
                print("---Separador---")
                print(pc.setLocalDescription)

                # Enviar la respuesta al cliente (Unity)
                await websocket.send(
                    json.dumps(
                        {
                            "sdp": pc.localDescription.sdp,
                            "type": pc.localDescription.type,
                        }
                    )
                )
                print("Sent answer back to client")
                # This is a SDP send
                print(pc.localDescription.sdp)
            
                

    except Exception as e:
        print(f"Error: {e}")

    finally:
        await websocket.close()
        print("WebSocket connection closed")


# Limpiar conexiones al cerrar la aplicación
async def on_shutdown():
    coros = [pc.close() for pc in pcs]
    await asyncio.gather(*coros)
    pcs.clear()


# Ejecutar el servidor WebSocket
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="WebRTC WebSocket server")
    parser.add_argument(
        "--play-without-decoding",
        help=(
            "Read the media without decoding it (experimental). "
            "For now it only works with an MPEGTS container with only H.264 video."
        ),
        action="store_true",
    )
    parser.add_argument("--play-from", help="Read the media from a file and sent it.")
    parser.add_argument("--verbose", "-v", action="count")
    parser.add_argument(
        "--audio-codec", help="Force a specific audio codec (e.g. audio/opus)"
    )
    parser.add_argument(
        "--video-codec", help="Force a specific video codec (e.g. video/H264)"
    )

    args = parser.parse_args()

    if args.verbose:
        logging.basicConfig(level=logging.DEBUG)
    else:
        logging.basicConfig(level=logging.INFO)

    start_server = websockets.serve(handle_websocket, "0.0.0.0", 8765)

    loop = asyncio.get_event_loop()
    loop.run_until_complete(start_server)
    print("WebSocket server started on ws://0.0.0.0:8765")

    try:
        loop.run_forever()
    except KeyboardInterrupt:
        pass
    finally:
        loop.run_until_complete(on_shutdown())

#'''
