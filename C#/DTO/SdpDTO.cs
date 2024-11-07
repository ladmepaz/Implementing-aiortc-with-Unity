namespace WebRTCSerial.DTO
{
    /// <summary>
    /// DTO (Data Transfer Object) to send/receive SDP Offer or Answer through the network. This DTO maps to <see cref="RTCSessionDescription"/>
    /// </summary>
    [System.Serializable]
    public class SdpDTO
    {
        public string sdp;
        public string type;
       
    }
}