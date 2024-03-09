namespace AsyncNetworking.Packets
{
    public class HandshakePacket : Packet
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public HandshakePacket() : base("HandshakePacket") { }
    }
}