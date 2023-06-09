//using Console_Pong._Engine._Input;
//using Console_Pong._Engine;
//using System;
//using System.Reflection;
//using System.Runtime.InteropServices;

//internal static class KeyboardHook
//{
//	private const int WH_KEYBOARD_LL = 13;
//	private const int WM_KEYDOWN = 0x0100;
//	private const int WM_SYSKEYDOWN = 0x0104;

//	private static IntPtr hookHandle;
//	private static IntPtr currentModuleHandle;

//	[StructLayout(LayoutKind.Sequential)]
//	private struct KBDLLHOOKSTRUCT
//	{
//		public uint vkCode;
//		public uint scanCode;
//		public uint flags;
//		public uint time;
//		public IntPtr dwExtraInfo;
//	}

//	public static event Action<VirtualKeys> OnKeyDown;

//	private delegate IntPtr KeyDownCallback(int nCode, IntPtr wParam, IntPtr lParam);

//	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//	private static extern IntPtr SetWindowsHookEx(int idHook, KeyDownCallback lpfn, IntPtr hMod, uint dwThreadId);

//	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

//	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//	private static extern IntPtr GetModuleHandle(string lpModuleName);

//	private static IntPtr KeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
//	{
//		if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
//		{
//			KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
//			//OnKeyDown?.Invoke((VirtualKeys)hookStruct.vkCode); <= Disabled for debugging
//			FastConsole.WriteAt((VirtualKeys)hookStruct.vkCode, new(5, 5)); //tmp
//		}

//		return CallNextHookEx(hookHandle, nCode, wParam, lParam);
//	}

//	public static void Hook() 
//	{
//		currentModuleHandle = GetModuleHandle(Assembly.GetExecutingAssembly().Modules.ElementAt(0).Name);
//		KeyDownCallback keyboardProc = KeyboardCallback;
//		hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, currentModuleHandle, 0);
//	}

//	public static void Unhook() => UnhookWindowsHookEx(hookHandle);
//}
