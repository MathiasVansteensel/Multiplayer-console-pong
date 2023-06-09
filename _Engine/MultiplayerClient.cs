using Pumpkin.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Console_Pong._Engine;
internal static class MultiplayerClient
{
    const byte dcByte = 0x13;
    const ushort Port = ushort.MaxValue - 6969;

    internal static Dictionary<string, Player> PlayerUpdateBuffer { get; private set; } = new Dictionary<string, Player>();

	static MultiplayerClient()
    {
        Network.Initialize();
        Network.DatagramStreamReceived += Network_DatagramStreamReceived;
    }

    private static async void Network_DatagramStreamReceived(MemoryStream packetStream, IPAddress sender)
    {
        if (!Engine.RegisteredGame.IsMultiplayerEnabled | packetStream is null | packetStream.Length <= 1) return;
        packetStream.Seek(0, SeekOrigin.Begin);
		if (packetStream.ReadByte() != dcByte) return;

        NetworkObject obj = await JsonSerializer.DeserializeAsync<NetworkObject>(packetStream);
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
		string msg;

        using (MemoryStream packetStream = new())
        {
            await JsonSerializer.SerializeAsync(packetStream, new NetworkObject(player));
            msg = Encoding.Unicode.GetString(packetStream.ToArray());
        }
		
        Network.Send((char)dcByte + msg, IPAddress.Broadcast.ToString(), Port);
    }
}
