public class DisconnectPacket : Packet
{
    public string Reason { get; set; }

    public DisconnectPacket() : base("DisconnectPacket") { }
}