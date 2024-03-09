using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Server
{
    private TcpListener tcpListener;
    private readonly ConcurrentDictionary<int, TcpClient> clients = new ConcurrentDictionary<int, TcpClient>();
    private readonly Random ran = new Random();

    public int MaxBufferSize { get; set; } = 4096;

    // events
    /// <summary>
    /// Event that is called when a message is received from a client (return true to disconnect the client, invalid packet)
    /// </summary>
    public event Func<int, Dictionary<string, string>, bool> OnMessageReceived;
    public event Action<DisconnectPacket> OnClientDisconnected;
    public event Action<HandshakePacket> OnClientHandshake;

    /// <summary>
    /// Start the server on port 3060(optional port)
    /// </summary>
    public async Task Start(int port = 3060)
    {
        tcpListener = new TcpListener(System.Net.IPAddress.Any, port);
        tcpListener.Start();

        while (true)
        {
            TcpClient curClient = await tcpListener.AcceptTcpClientAsync();
            int clientId = GetUnqiueID();
            clients.TryAdd(clientId, curClient);

            // Sent client handsake
            HandshakeClient(clientId, curClient);

            _ = HandleClientAsync(clientId, curClient);
        }
    }

    /// <summary>
    /// Get a unique id that no client uses
    /// </summary>
    public int GetUnqiueID()
    {
        int id = ran.Next(100000, 999999);

        if (clients.ContainsKey(id))
            return GetUnqiueID();

        return id;
    }

    public void HandshakeClient(int clientId, TcpClient curClient)
    {
        HandshakePacket handshakePacket = new HandshakePacket()
        {
            ClientId = clientId,
            Success = true
        };

        string serialized = ObjectSerializer.Serialize(handshakePacket);

        byte[] data = Encoding.ASCII.GetBytes(serialized);

        curClient.GetStream().Write(data, 0, data.Length);

        OnClientHandshake?.Invoke(handshakePacket);
    }

    public void DisconnectClient(int clientId, string reason)
    {
        DisconnectPacket disconnectPacket = new DisconnectPacket()
        {
            ClientId = clientId,
            Reason = reason
        };

        string serialized = ObjectSerializer.Serialize(disconnectPacket);

        byte[] data = Encoding.ASCII.GetBytes(serialized);

        clients[clientId].GetStream().Write(data, 0, data.Length); // send the disconnect packet bytes
        clients[clientId].Close(); // close the connection

        clients.TryRemove(clientId, out _); // remove the client from the list
    }

    private async Task HandleClientAsync(int clientId, TcpClient curClient)
    {
        try
        {
            NetworkStream stream = curClient.GetStream();
            byte[] buffer = new byte[MaxBufferSize]; // 4KB buffer should be enough

            while (true)
            {
                // read the data from the client stream
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead <= 0) continue; // lets skip this

                // convert the bytes to a string
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // deserialize packet into a simplified dictionary
                Dictionary<string, string> packet = ObjectSerializer.ParseSerializedString(message);

                // lets pass this off to the events
                if (OnMessageReceived != null)
                {
                    bool invalid = OnMessageReceived(clientId, packet); // sent it off to the event

                    if (invalid)
                    {
                        DisconnectClient(clientId, "Invalid packet received");
                        return;
                    }
                }
            }
        }
        catch
        {
            // invalid packet or error on the server so lets disconenct them
            DisconnectClient(clientId, "Internal server error");
        }
    }
}