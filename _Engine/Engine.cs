using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine;
public static class Engine
{
    public static long Time 
    {
        get => 
    }

    public static bool IsRunning { get; private set; } = false;
    public static bool IsPaused { get; private set; } = false;

    private static float _targetUpdateFps;
    public static float TargetUpdateFps 
    {
        get => _targetUpdateFps;
        set => targetUpdateFpsTicks = 10_000_000 / (_targetUpdateFps = value);
    }

    private static float _targetFixedUpdateFps;
    public static float TargetFixedUpdateFps
    {
        get => _targetFixedUpdateFps;
        set => targetFixedUpdateFpsTicks = 10_000_000 / (_targetFixedUpdateFps = value);
    }

    private static float _targetInputUpdateFps;
    public static float TargetInputUpdateFps
    {
        get => _targetInputUpdateFps;
        set => targetInputUpdateFpsTicks = 10_000_000 / (_targetInputUpdateFps = value);
    }

    private static float _targetUIUpdateFps;
    public static float TargetUIUpdateFps
    {
        get => _targetUIUpdateFps;
        set => targetUIUpdateFpsTicks = 10_000_000 / (_targetUIUpdateFps = value);
    }

    public static bool FpsCounterShown { get; private set; } = true;

    private static float targetUpdateFpsTicks;
    private static float targetFixedUpdateFpsTicks;
    private static float targetInputUpdateFpsTicks;
    private static float targetUIUpdateFpsTicks;

	private static Thread consoleThread = null;
	private static Thread updateThread = null;
    private static Thread fixedUpdateThread = null;
    private static Thread inputThread = null;
    private static Thread uiThread = null;

    public static IGameBehaviour RegisteredGame { get; private set; } = null;

    static Engine()
    {
        //init accessors otherwise they're 0 and keep em here for easy access
        //set to number to limit fps (30 recommended) or to -1 to leave unlimited
        TargetUpdateFps = 30;
        TargetFixedUpdateFps = 60;
        TargetInputUpdateFps = -1;
        TargetUIUpdateFps = -1;
    }

    private static Size prevConsoleSize = new(Console.BufferWidth, Console.BufferHeight);

    public static float GetUpdateFps() => 1 / updateDT;
    public static float GetFixedUpdateFps() => 1 / fixedUpdateDT;
    public static float GetInputUpdateFps() => 1 / inputUpdateDT;
    public static float GetUIUpdateFps() => 1 / uiUpdateDT;

    public static void Init<T>(T game) where T : IGameBehaviour => RegisteredGame = game;

    public static void Start()
    {
        if (RegisteredGame is null) throw new ArgumentNullException($"'{nameof(Engine)}.{nameof(RegisteredGame)}' is null, use {nameof(Engine)}.{nameof(Init)}() to set the game environment");
        if (IsRunning) return;
        else if (IsPaused) IsPaused = false;
        else
        {
            IsRunning = true;
            IsPaused = false;

            FastConsole.Clear();

            Console.CursorVisible = false;
            RegisteredGame?.Initialize();

			updateThread = new(() => 
            {
                while (true) 
                {
                    while (!IsPaused) internalUpdate();
                    if (updateWatch.IsRunning) updateWatch.Stop();
                } 
            });

            fixedUpdateThread = new(() => 
            {
                while (true)
                {
                    while (!IsPaused) internalFixedUpdate();
                    if (FixedUpdateWatch.IsRunning)
                    {
                        FixedUpdateWatch.Stop();
                    }
                }
            });

            inputThread = new(() => 
            {
                while (true)
                {
                    internalInputUpdate(); //continues to run if paused
                    if (inputUpdateWatch.IsRunning) inputUpdateWatch.Stop();
                }
            });

			uiThread = new(() => 
            {
                while (true)
                {
                    internalUIUpdate(); //continues to run if paused
					if (uiUpdateWatch.IsRunning) uiUpdateWatch.Stop();
                }
            });

            updateThread.Start();
            fixedUpdateThread.Start();
            inputThread.Start();
            uiThread.Start();
        }        
    }

    public static void Pause() => IsRunning = !(IsPaused = true);
    public static void UnPause() => IsRunning = IsPaused = false;
    public static void Stop() => IsRunning = IsPaused = false;

    #region Updates

    private static Stopwatch timeWatch = new();

    //-----Update-----
    private static Stopwatch updateWatch = new();
    private static float updateDT;
    private static void internalUpdate()
    {
        updateDT = updateWatch.ElapsedTicks / 10_000_000f;
        if (updateWatch.IsRunning) updateWatch.Restart();
        else updateWatch.Start();

		FastConsole.Clear();
		RegisteredGame?.Update(updateDT);
		Input.ResetLastKey();
		if (FpsCounterShown) ShowFpsCounter(fpsCounterCorner);
        while (updateWatch.ElapsedTicks < targetUpdateFpsTicks) Thread.SpinWait(128);
    }

    //-----Fixed Update-----
    private static Stopwatch FixedUpdateWatch = new();
    private static float fixedUpdateDT;
    private static void internalFixedUpdate()
    {
        fixedUpdateDT = FixedUpdateWatch.ElapsedTicks / 10_000_000f;
        if (FixedUpdateWatch.IsRunning) FixedUpdateWatch.Restart();
        else FixedUpdateWatch.Start();

        RegisteredGame?.FixedUpdate(fixedUpdateDT);
        while (FixedUpdateWatch.ElapsedTicks < targetFixedUpdateFpsTicks) Thread.SpinWait(128);
    }

    //-----Input Update-----
    private static Stopwatch inputUpdateWatch = new();
    private static float inputUpdateDT;
    private static void internalInputUpdate()
    {
        inputUpdateDT = inputUpdateWatch.ElapsedTicks / 10_000_000f;
        if (inputUpdateWatch.IsRunning) inputUpdateWatch.Restart();
        else inputUpdateWatch.Start();
        Input.InternalInputUpdate(inputUpdateDT);
        while (inputUpdateWatch.ElapsedTicks < targetInputUpdateFpsTicks) Thread.SpinWait(128);
    }

    //-----UI Update-----
    private static Stopwatch uiUpdateWatch = new();
    private static float uiUpdateDT;
    private static void internalUIUpdate()
    {
        uiUpdateDT = uiUpdateWatch.ElapsedTicks / 10_000_000f;
        if (uiUpdateWatch.IsRunning) uiUpdateWatch.Restart();
        else uiUpdateWatch.Start();

        RegisteredGame?.UIUpdate(uiUpdateDT);
        while (uiUpdateWatch.ElapsedTicks < targetUIUpdateFpsTicks) Thread.SpinWait(128);
    }

    #endregion

    #region Utils

    private static CornerAlignment fpsCounterCorner;
    private static int lastFpsStrLen = 0;

    public static void ShowFpsCounter(CornerAlignment corner) 
    {
        if (fpsCounterCorner != corner) fpsCounterCorner = corner;
        FpsCounterShown = true;

        string fpsStr = $"{1 / updateDT:0.#}fps";
        lastFpsStrLen = fpsStr.Length;

        Point curPos = GetCornerAlignmentPos(corner, lastFpsStrLen);

        Console.SetCursorPosition(curPos.X, curPos.Y);
        Console.Write(fpsStr);
    }

    public static void HideFpsCounter() 
    {
        if (!FpsCounterShown) return;
        string emptyStr = string.Empty;
        for (int i = 0; i < lastFpsStrLen; i++) emptyStr += ' ';

        Point curPos = GetCornerAlignmentPos(fpsCounterCorner, lastFpsStrLen);

        Console.SetCursorPosition(curPos.X, curPos.Y);
        Console.Write(emptyStr);
    }

    private static Point GetCornerAlignmentPos(CornerAlignment corner, int strLen) 
    {
        Point curPos;
        RetryDefault:

        switch (corner)
        {
            case CornerAlignment.TopRight:
                curPos = new Point(Console.BufferWidth - strLen, 0);
                break;
            case CornerAlignment.TopLeft:
                curPos = new Point(0, 0);
                break;
            case CornerAlignment.BottomRight:
                curPos = new Point(Console.BufferWidth - strLen, Console.BufferHeight);
                break;
            case CornerAlignment.BottomLeft:
                curPos = new Point(Console.BufferHeight, 0);
                break;
            default:
                corner = CornerAlignment.TopRight;
                goto RetryDefault;
        }

        return curPos;
    }

    public static T Clamp<T>(T value, T min, T max) where T : struct, IComparable, IConvertible, IComparable<T>, IEquatable<T>
	{
        if (typeof(T).IsPrimitive && IsNumericType(value))
        {
            dynamic val = value;
            return (T)(val > max ? max : (val < min ? min : val));
        }
        return value;
    }

	private static bool IsNumericType(dynamic val) => val is decimal || val is byte || val is sbyte || val is ushort || val is short || val is uint || val is int || val is ulong || val is long || val is float || val is double;

	#endregion
}
