using AsyncNetworking.Packets;

class MessagePacket : Packet
{
    public string Message { get; set; }

    public MessagePacket() : base("MessagePacket") { }
}