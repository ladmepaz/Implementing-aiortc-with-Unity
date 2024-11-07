namespace WebRTCSerial.DTO
{
    [System.Serializable]
    public class ICECanddidateDTO
    {
        public string Candidate;
        public string SdpMid;
        public int? SdpMLineIndex;
    }
}