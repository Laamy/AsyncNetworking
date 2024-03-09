using AsyncNetworking.Server;

using System;

namespace AsyncServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            server.OnClientHandshake += (packet) =>
            {
                Console.WriteLine($"Client {packet.ClientId} has connected!");
            };

            server.OnClientDisconnected += (clientId) =>
            {
                Console.WriteLine($"Client {clientId} has disconnected!");
            };

            server.OnMessageReceived += (clientId, message) =>
            {
                Console.WriteLine($"Client {clientId} sent: {message}");

                //Console.WriteLine(message["PacketType"]);
                switch (message["PacketType"])
                {
                    case "MessagePacket":
                        //create new messagepacket
                        MessagePacket messagePacket = new MessagePacket()
                        {
                            ClientId = clientId,
                            Message = message["Message"]
                        };

                        // send messagepacket to all clients connected
                        server.SendToAll(messagePacket).GetAwaiter().GetResult();

                        return false;
                }

                return false; // invalid packet
            };

            Console.WriteLine("Server started...");
            server.Start().GetAwaiter().GetResult(); // start server & await result

            Console.WriteLine("Server stopped...");
            Console.ReadKey();
        }
    }
}
