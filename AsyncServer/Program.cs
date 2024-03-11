using AsyncNetworking.Packets;
using AsyncNetworking.Server;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncServer
{
    internal class Program
    {
        static Dictionary<int, string> usernames = new Dictionary<int, string>()
        {
            { 0, "Server" }, // reserved for server messages (for example global notifications, joining, leaving, etc.)
            { 1, "System" }, // reserved for system messages (updates, restarts, etc.)
        };

        static Server server;

        static void Main(string[] args)
        {
            server =  new Server();

            server.OnClientHandshake += (packet) =>
            {
                Console.WriteLine($"Client {packet.ClientId} has connected!");
            };

            server.OnClientDisconnected += (clientId) =>
            {
                Console.WriteLine($"Client {clientId} has disconnected!");
            };

            // clientId part here is VERIFIED so use this instead of whats in the message packet
            // what is in message packet is what the client says it is, so it can be faked
            server.OnMessageReceived += (clientId, message) =>
            {
                //Console.WriteLine($"Client {clientId} sent: {message}");

                //Console.WriteLine(message["PacketType"]);
                switch (message["PacketType"])
                {
                    case "MessagePacket":
                        //create new messagepacket

                        if (!usernames.ContainsKey(clientId))
                        {
                            Console.WriteLine("Client not logged in");
                            return false; // client not logged in
                        }

                        MessagePacket messagePacket = new MessagePacket()
                        {
                            ClientId = clientId,
                            Message = message["Message"],
                            Username = usernames[clientId]
                        };

                        // send messagepacket to all clients connected
                        Console.WriteLine($"[USER] [{messagePacket.Username}] {messagePacket.Message}");
                        server.SendToAll(messagePacket).GetAwaiter().GetResult();

                        return false;

                    case "LoginPacket":
                        Console.WriteLine("Login packet received");
                        if (usernames.ContainsKey(clientId))
                        {
                            // client already connected & registered before under that client id
                            // send error message
                            server.DisconnectClient(clientId, "You are already connected!");
                            Console.WriteLine("User already connected");
                        }
                        else
                        {
                            // pause thread until username is verified 2 avoid bugs
                            VerifyUsername(message["Username"], clientId).GetAwaiter().GetResult();
                            Console.WriteLine($"User {usernames[clientId]} verified");
                        }
                        return false;
                }

                return false; // invalid packet
            };

            Console.WriteLine("Server started...");
            server.Start().GetAwaiter().GetResult(); // start server & await result

            Console.WriteLine("Server stopped...");
            Console.ReadKey();
        }

        private static async Task VerifyUsername(string username, int clientId)
        {
            string output = username;

            // limit username to 15 characters
            if (username.Length > 15)
                output = username.Substring(0, 15);

            // strip spaces to _
            if (username.Contains(" "))
                output = username.Replace(" ", "_");

            // no special characters
            List<string> allowedChars = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_"
            };

            foreach (char c in username)
            {
                if (!allowedChars.Contains(c.ToString()))
                {
                    output = username.Replace(c.ToString(), "");
                }
            }

            // add discriminator to username
            int randomNum = new Random(clientId).Next(1000, 9999); // 4 digit number & unique based on usernames already existing
            output += "#" + randomNum;

            // add username to list
            usernames.Add(clientId, output);
        }
    }
}
