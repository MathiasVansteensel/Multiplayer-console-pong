using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console_Pong._Engine;
using Console_Pong._Engine._Input;

namespace Console_Pong;
internal class Program
{
    static void Main(string[] args)
    {


        Console.OutputEncoding = Encoding.Unicode;
		PongGame game = new PongGame();
        Engine.Init(game);
        Engine.Start();
    }
}
