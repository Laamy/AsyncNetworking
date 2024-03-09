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
                return true;
            };

            Console.WriteLine("Server started...");
            server.Start().GetAwaiter().GetResult(); // start server & await result

            Console.WriteLine("Server stopped...");
            Console.ReadKey();
        }
    }
}
