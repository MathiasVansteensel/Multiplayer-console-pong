using Pumpkin.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Console_Pong._Engine;
internal static class MultiplayerClient
{
    //i really dont care
#pragma warning disable SYSLIB0011

	const byte dcByte = 0x13;
    const ushort Port = ushort.MaxValue - 6969;

    internal static IPAddress CurrentIp { get; private set; }

	private static BinaryFormatter packetSerializer = new();

	internal static Dictionary<string, Player> PlayerUpdateBuffer { get; private set; } = new Dictionary<string, Player>();

	static MultiplayerClient()
    {
        Network.Initialize(Port);
        Network.DatagramStreamReceived += Network_DatagramStreamReceived;
        CurrentIp = GetLocalIp();
    }

    private static IPAddress GetLocalIp() 
    {
		using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
		{
			socket.Connect("8.8.8.8", 65530);

			IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
			if (endPoint != null)
			{
				return endPoint.Address;
			}
		}

		return null;
	}

    private static void Network_DatagramStreamReceived(MemoryStream packetStream, IPAddress sender)
    {
        if (!Engine.RegisteredGame.IsMultiplayerEnabled | packetStream is null | packetStream.Length <= 1 | sender.Address == CurrentIp.Address) return;
        packetStream.Seek(0, SeekOrigin.Begin);
		if (packetStream.ReadByte() != dcByte) return;
        
		NetworkObject obj = (NetworkObject)packetSerializer.Deserialize(packetStream);
        Player p = obj.Player;

        PlayerUpdateBuffer[p.Name] = p;
    }

    public static void RenderNetworkPlayers() 
    {
		if (!Engine.RegisteredGame.IsMultiplayerEnabled) return;

		if (PlayerUpdateBuffer.Count < 1) return;
        for (int i = 0; i < PlayerUpdateBuffer.Count; i++) PlayerUpdateBuffer.ElementAt(i).Value.Render();
    }

    public static async void SendPlayerToClients(Player player)
    {
		if (!Engine.RegisteredGame.IsMultiplayerEnabled) return;

        using (MemoryStream packetStream = new())
        {
            packetStream.WriteByte(dcByte);
            packetSerializer.Serialize(packetStream, new NetworkObject(player));
			Network.Send(packetStream.ToArray(), IPAddress.Broadcast.ToString(), Port);
		}
    }
}
