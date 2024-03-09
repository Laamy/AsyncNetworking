# Packets

the Client & server both have access to 2 default packets aswell as a packet class to inherit (contains packet per packet information)</b></b>

Packet:</b>
  string PacketType - kind of packet being sent</b>
  int ClientId - used mainly in the handshake packet (use the client connected to get this ID and not the one in the packet)</b></b>
  
HandshakePacket,Packet:</b>
  bool Success - true if the client is allowed to connect, else false</b>
  string Message - information for if the client connect was denied</b></b>
  
DisconnectPacket,Packet:</b>
  string Reason - the reason on why this packet was sent to the client</b>

# Client

the client currently has 3 events you can hook, OnMessageRecieved which has the parsed packet as a dictionary (for performance) & a return for if the message/packet should be denied</b>
OnServerDisconnect for when a client is kicked/disconnects (no arguments)</b>
OnServerHandshake which has the ClientIDof the connecting client

# Server

the server currently also has 3 events but they differ slightly</b>
OnMessager Received is the same as the client/has the parsed packet as a dictionary & a return value for if the packet was accepted or not</b>
OnClientDisconnect which has the DisconnectPacket given as the argument</b>
OnClientHandshake which has the HandshakePacket as the argument

# Packet Serializer/ObjectSerializer

this is a custom serializer that stores classes as a pretty simple format (key1:value1;key2:value2)</b>
this class currently has a Serialize method that thats in an object (the class to serialize) & returns the serialized object/class as a string</b>
Deserialize takes in a serialized string (the serialized packet) and returns it as T</b>
ParseSerializedString taks in a string (the serialized packet) and returns it as a dictionary of string keys & string values (recommend using this instead of parsing it back to the packet class)
