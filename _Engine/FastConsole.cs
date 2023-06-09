using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine;

public static class FastConsole
{
	#region Imports
	// Import the necessary Win32 API functions
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, Coord dwCursorPosition);

	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern bool WriteConsoleOutputCharacterW(IntPtr hConsoleOutput, char[] lpCharacter, uint nLength, Coord dwWriteCoord, out uint lpNumberOfCharsWritten);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

	// Define the COORD and CONSOLE_SCREEN_BUFFER_INFO structures used by the Win32 API functions
	[StructLayout(LayoutKind.Sequential)]
	public struct Coord
	{
        public short X;
		public short Y;
		public Coord(short x, short y)
		{
			X = x;
			Y = y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SmallRect
	{
		public short Left;
		public short Top;
		public short Right;
		public short Bottom;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConsoleScreenBufferInfo
	{
		public Coord dwSize;
		public Coord dwCursorPosition;
		public short wAttributes;
		public SmallRect srWindow;
		public Coord dwMaximumWindowSize;
	}

	#endregion

	//const double TargetFps = 30;
	//static double fps = 0;
	//static double targetTickDelay = Stopwatch.Frequency / TargetFps;
	public static IntPtr ConsoleOutHandle { get; private set; } = GetStdHandle(-11);
	public static IntPtr ConsoleInHandle { get; private set; } = GetStdHandle(-10);

	public static int ConsoleSize { get; private set; }

	static FastConsole() 
	{
		ConsoleSize = GetConsoleSize(ConsoleOutHandle);
	}

	public static void Clear() => Fill(' ');

	public static void Fill(char character)
	{
		char[] charBuffer = new string(character, ConsoleSize).ToCharArray();
		WriteConsoleOutputCharacterW(ConsoleOutHandle, charBuffer, (uint)charBuffer.LongLength, new Coord { X = 0, Y = 0 }, out _);
	}

	internal static int GetConsoleSize(IntPtr consoleOutput)
	{
		ConsoleScreenBufferInfo consoleInfo;
		GetConsoleScreenBufferInfo(consoleOutput, out consoleInfo);
		int width = consoleInfo.srWindow.Right - consoleInfo.srWindow.Left + 1;
		int height = consoleInfo.srWindow.Bottom - consoleInfo.srWindow.Top + 1;
		return (width * height);
	}

	public static void WriteAt(object value, Coord coord) 
	{
		char[] charBuffer = (value?.ToString() ?? "¿?").ToCharArray();
		WriteConsoleOutputCharacterW(ConsoleOutHandle, charBuffer, (uint)charBuffer.LongLength, coord, out _);
	}
}
