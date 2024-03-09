namespace AsyncNetworking.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class Client
    {
        // the client and the stream (private)
        private TcpClient client;
        private NetworkStream stream;

        public int ClientId { get; private set; } // the id of the client

        // events
        public event Func<Dictionary<string, string>, bool> OnMessageReceived;
        public event Action OnServerDisconnect;
        public event Action<int> OnServerHandshake;

        public async Task Connect(string ip, int port)
        {
            client = new TcpClient();

            try
            {
                await client.ConnectAsync(ip, port); // connect to server
                stream = client.GetStream(); // get the stream

                // Wait for handshake
                bool result = await GetHandshake();

                if (result)
                    await ReceiveMessages();
                else // failed to handshake
                    throw new Exception("Failed to handshake with server");

                OnServerDisconnect?.Invoke();
            }
            catch
            {
                throw new Exception("Failed to connect to server");
            }
        }

        private async Task<bool> GetHandshake()
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Dictionary<string, string> packet = ObjectSerializer.ParseSerializedString(data);

                if (packet.ContainsKey("PacketType") && packet["PacketType"] == "HandshakePacket")
                {
                    ClientId = int.Parse(packet["ClientId"]);
                    OnServerHandshake?.Invoke(int.Parse(packet["ClientId"]));
                    return true;
                }
            }

            // something went wrong (possibly server disconnected or bad packet that wasn't handshake)
            // close the client
            return false;
        }

        public void PostPacket<T>(T packet)
        {
            // serialize packet
            string serializedPacket = ObjectSerializer.Serialize(packet);

            // serialize packet to bytes
            byte[] buffer = Encoding.UTF8.GetBytes(serializedPacket);

            // send the packet
            stream.Write(buffer, 0, buffer.Length);
        }

        private async Task ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Dictionary<string, string> packet = ObjectSerializer.ParseSerializedString(data);

                        if (OnMessageReceived != null)
                            OnMessageReceived(packet); // send it off to the event
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error '{e.Message}'");

                // disconnect if not already
                if (client.Connected)
                    client.Close();

                OnServerDisconnect?.Invoke();
            }
        }
    }
}