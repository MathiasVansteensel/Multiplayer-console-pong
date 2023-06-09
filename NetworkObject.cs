using Console_Pong._Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong;

[Serializable]
internal class NetworkObject
{
	public Player Player { get; private set; }
    //TODO: add other shit to (de)serialize

    public NetworkObject(Player player)
    {
        Player = player;
    }
}