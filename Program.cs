using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
		Console.InputEncoding = Encoding.Unicode;
		Console.WriteLine("==========CONSOLE PONG==========");
        Console.Write("Username: ");
        string userName = Console.ReadLine();

		Console.Write("Character: ");
		string playerString = Console.ReadLine();

        userName = string.IsNullOrWhiteSpace(userName = userName.Trim()) ? null : userName;
		playerString = string.IsNullOrWhiteSpace(playerString = playerString.Trim()) ? null : playerString;

		PongGame game = new PongGame(userName, playerString);
        Engine.Init(game);
        Engine.Start();
    }
}
