using AsyncNetworking.Client;

using System;
using System.Collections.Generic;

namespace AsyncClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            Console.WriteLine("Enter a username:");
            Console.Write(">");
            string username = Console.ReadLine();

            // limit username to 15 characters
            if (username.Length > 15)
                username = username.Substring(0, 15);

            // strip spaces to _
            if (username.Contains(" "))
                username = username.Replace(" ", "_");

            // no special characters
            List<string> allowedChars = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_"
            };

            // strip any characters that are not in allowedChars
            foreach (char c in username)
            {
                if (!allowedChars.Contains(c.ToString()))
                    username = username.Replace(c.ToString(), "");
            }

            // print new username
            Console.WriteLine("Your username is: " + username);

            client.OnServerHandshake += (clientId) =>
            {
                Console.WriteLine("Server handshake received. ClientId: " + clientId);

                // lets pass the login packet
                LoginPacket loginPacket = new LoginPacket()
                {
                    Username = username,
                    ClientId = clientId
                };

                Console.WriteLine("Sent login packet");

                client.PostPacket(loginPacket);
            };

            client.OnMessageReceived += (packet) =>
            {
                switch (packet["PacketType"])
                {
                    case "MessagePacket":
                        Console.WriteLine($"[{packet["Username"]}] {packet["Message"]}");
                        return true;
                }

                return false;
            };

            client.OnServerDisconnect += () =>
            {
                Console.WriteLine("Server disconnected");
            };

            Console.WriteLine("Connecting to server...");
            client.Connect("147.185.221.18", 57115); // connect to server (147.185.221.18:57115)
            //client.Connect("127.0.0.1", 3060); // debug server

            while (true)
            {
                string message = Console.ReadLine();

                MessagePacket messagePacket = new MessagePacket()
                {
                    Message = message,
                    ClientId = client.ClientId // this is ignored anyways so we dont have to set it
                };

                client.PostPacket(messagePacket);
                //Console.WriteLine(ObjectSerializer.Serialize(messagePacket));
            }

            Console.WriteLine("Client disconnected");
            Console.ReadKey();
        }
    }
}
