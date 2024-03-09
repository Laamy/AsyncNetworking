namespace AsyncNetworking.Packets
{
    public abstract class Packet
    {
        public string PacketType { get; set; }
        public int ClientId { get; set; }

        public Packet(string packetType)
        {
            PacketType = packetType;
        }
    }
}