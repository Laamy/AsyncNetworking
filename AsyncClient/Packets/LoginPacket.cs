using AsyncNetworking.Packets;

class LoginPacket : Packet
{
    public string Username { get; set; }
    //public string Password { get; set; }

    public LoginPacket() : base("LoginPacket") {}
}