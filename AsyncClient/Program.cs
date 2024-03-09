using AsyncNetworking;
using AsyncNetworking.Client;

using System;

namespace AsyncClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            client.OnServerHandshake += (clientId) =>
            {
                Console.WriteLine("Server handshake received. ClientId: " + clientId);
            };

            client.OnMessageReceived += (packet) =>
            {
                switch (packet["PacketType"])
                {
                    case "MessagePacket":
                        Console.WriteLine($"[{packet["ClientId"]}] {packet["Message"]}");
                        return true;
                }

                return false;
            };

            client.OnServerDisconnect += () =>
            {
                Console.WriteLine("Server disconnected");
            };

            Console.WriteLine("Connecting to server...");
            client.Connect("127.0.0.1", 3060); // connect to server

            while (true)
            {
                string message = Console.ReadLine();

                MessagePacket messagePacket = new MessagePacket()
                {
                    Message = message,
                    ClientId = client.ClientId // this is ignored anyways so we dont have to set it
                };

                client.PostPacket(messagePacket);
                Console.WriteLine(ObjectSerializer.Serialize(messagePacket));
            }

            Console.WriteLine("Client disconnected");
            Console.ReadKey();
        }
    }
}
