using Pumpkin.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Console_Pong._Engine;
internal static class MultiplayerClient
{
    static MultiplayerClient()
    {
        Network.Initialize(ushort.MaxValue - 6969);
        Network.DatagramReceived += Network_DatagramReceived;
    }

    private static void Network_DatagramReceived(string msg, IPAddress sender)
    {

    }

    public static void UpdatePos()
    {

    }
}
