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
                Console.WriteLine("Message received: " + packet["Message"]);
                return false;
            };

            client.OnServerDisconnect += () =>
            {
                Console.WriteLine("Server disconnected");
            };

            Console.WriteLine("Connecting to server...");
            client.Connect("127.0.0.1", 3060).GetAwaiter().GetResult(); // connect to server & await

            Console.WriteLine("Client disconnected");
            Console.ReadKey();
        }
    }
}
