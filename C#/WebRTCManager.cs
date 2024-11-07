// Create by Over A. Mejia. R. and Ronald M. Ceballos L.
// Date: 2024/10/27
// This code is under license Open, you can use, modify but no distribute.
// If you want to distribute, please refer to the authors.
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Unity.WebRTC;
using WebRTCSerial.DTO;

public class WebRTCManager : MonoBehaviour
{
    public event Action<Texture> RemoteVideoReceived;
    private RTCPeerConnection peerConnection;
    private WebSocket _ws;
    [SerializeField] private string _serverIp = "192.168.0.105"; // Cambia esto según la IP de tu Raspberry Pi
    [SerializeField] private string _port = "8765";
    [SerializeField] private RawImage rawImage;

    private void Start()
    {
        WebRTC.Initialize();
        Debug.Log("Inicializando la PeerConnection");
        CreatePeerConnection();
        Debug.Log("Conectando al WebSocket");
        ConnectWebSocket();
    }

    private void CreatePeerConnection()
    {
        StartCoroutine(WebRTC.Update());
        var config = new RTCConfiguration
        {
            iceServers = new[] 
            {
                new RTCIceServer
                {
                    urls = new[]
                    { 
                        "stun:stun.l.google.com:19302" 
                    } 
                } 
            }
        };

        peerConnection = new RTCPeerConnection(ref config);
        peerConnection.AddTransceiver(TrackKind.Video);
        peerConnection.OnIceCandidate += OnIceCandidate;
       
    }
    private void OnTrack(RTCTrackEvent trackEvent)
    {
        Debug.Log("OnTrack received, type: " + trackEvent.Track.Kind);

        if (trackEvent.Track is VideoStreamTrack videoStreamTrack)
        {
            Debug.Log("Hola goku");
            videoStreamTrack.OnVideoReceived += OnVideoReceived;
        }
        else
        {
            Debug.LogError(
                $"Unhandled track of type: {trackEvent.Track.GetType()}. In this tutorial, we're handling only video tracks.");
        }
    }
    private void OnVideoReceived(Texture texture)
    {
        Debug.Log($"Video received, resolution: {texture.width}x{texture.height}");
        RemoteVideoReceived?.Invoke(texture);
        rawImage.texture = texture;
    }

    private void ConnectWebSocket()
    {
        var url = $"ws://{_serverIp}:{_port}";
        _ws = new WebSocket(url);
        _ws.OnOpen += OnOpen;
        _ws.OnMessage += OnMessage;
        _ws.OnError += OnError;
        _ws.Connect();

        Debug.Log("Conectando al WebSocket: " + url);
    }

    private void OnOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket conectado. Enviando SDP offer...");
        StartCoroutine(CreateOffer());
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Mensaje recibido del WebSocket: " + e.Data);
        var data = JsonUtility.FromJson<SdpDTO>(e.Data);
        if (data.type == "answer")
        {
            Debug.Log("Answer SDP recibido");
            var desc = new RTCSessionDescription { sdp = data.sdp, type = RTCSdpType.Answer };
            if (string.IsNullOrEmpty(desc.sdp) || desc.type != RTCSdpType.Answer)
            {
                Debug.LogError("SDP o tipo inválido. No se puede proceder con SetRemoteDescription.");
                return;
            }
            Debug.Log("Estableciendo descripción remota...");
            SetRemoteDescription(desc);
            peerConnection.OnTrack += OnTrack;
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("Error en WebSocket: " + e.Message);
    }

    private void OnIceCandidate(RTCIceCandidate candidate)
    {
        if (candidate != null && candidate.Candidate != null)
        {
            Debug.Log("Enviando candidato ICE");

            var iceCandidate = new ICECanddidateDTO
            {
                Candidate = candidate.Candidate,
                SdpMid = candidate.SdpMid,
                SdpMLineIndex = candidate.SdpMLineIndex
            };

            string iceCandidateJson = JsonUtility.ToJson(iceCandidate);
            Debug.Log("Candidato ICE: " + iceCandidateJson);
            _ws.Send(iceCandidateJson);
        }
    }

    private IEnumerator CreateOffer()
    {
        Debug.Log("Creando oferta SDP");
        var offerOp = peerConnection.CreateOffer();
        yield return offerOp;

        if (offerOp.IsError)
        {
            Debug.LogError("Error al crear la oferta: " + offerOp.Error.message);
            yield break;
        }

        var offer = offerOp.Desc;
        var setLocalDescOp = peerConnection.SetLocalDescription(ref offer);
        yield return setLocalDescOp;

        if (setLocalDescOp.IsError)
        {
            Debug.LogError("Error al establecer la descripción local: " + setLocalDescOp.Error.message);
            yield break;
        }

        var sdpOffer = new SdpDTO
        {
            sdp = offer.sdp,
            type = offer.type.ToString().ToLower()
        };

        string jsonOffer = JsonUtility.ToJson(sdpOffer);
        Debug.Log("Enviando oferta SDP: " + jsonOffer);
        _ws.Send(jsonOffer);
    }

    private void SetRemoteDescription(RTCSessionDescription desc)
    {
        var op = peerConnection.SetRemoteDescription(ref desc);
        if (op.IsError)
        {
            Debug.LogError("Error al establecer la descripción remota: " + op.Error.message);
        }
        else
        {
            Debug.Log("Descripción remota establecida correctamente");
        }
    }
    private void OnDestroy()
    {
        _ws?.Close();
        peerConnection?.Close();
        peerConnection?.Dispose();
        WebRTC.Dispose();
    }
}
