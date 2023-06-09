using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine._Input;
public static class Input
{
    public static ConsoleKeyInfo KeyPressed { get; private set; }
    public static ConsoleKeyInfo LastKeyPressed { get; private set; }

    public static readonly ConsoleKeyInfo NullKey = new ConsoleKeyInfo('\0', 0, false, false, false);

    public static event EventHandler<ConsoleKeyInfo> OnKeyDown;

	internal static void InternalInputUpdate(double deltaTime)
    {

		KeyPressed = Console.ReadKey(true);
		LastKeyPressed = KeyPressed;
		OnKeyDown?.Invoke(null, KeyPressed);
    }

    internal static void ResetLastKey() => KeyPressed = NullKey;
}
