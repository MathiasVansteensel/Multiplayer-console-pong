using System.Numerics;
using System.Diagnostics;
using Console_Pong._Engine;
using System.Drawing;

namespace Console_Pong;

internal class PongGame : IGameBehaviour
{
    #region GameParams

    const float MovementSpeed = 15f;
    const string PlayerString = "🫵👁👄👁🤏";

    #endregion


    public void Initialize()
    {
        //Engine.ShowFpsCounter(CornerAlignment.TopRight);
        Input.OnKeyDown += Input_OnKeyDown;
    }

    private void Input_OnKeyDown(object? sender, ConsoleKeyInfo e)
    {
		switch (e.Key)
        {
            case ConsoleKey.Escape:
                if (Engine.IsPaused) Engine.UnPause(); else Engine.Pause();
                break;
            default:
                return;
        }
    }

    public void FixedUpdate(float deltaTime)
    {
        
    }

    public void UIUpdate(float deltaTime)
    {
        
    }

    float x = 0, y = 0;
    float prevX = 0, prevY = 0;

    public void Update(float deltaTime)
    {
		Console.Title = $"{1f / deltaTime:0.00} fps";
		switch (Input.KeyPressed.Key)
        {
            case ConsoleKey.UpArrow:
                y -= MovementSpeed * deltaTime;
                break;
            case ConsoleKey.DownArrow:
                y += MovementSpeed * deltaTime;
				break;
            case ConsoleKey.RightArrow:
                x += MovementSpeed * deltaTime + 1;
				break;
            case ConsoleKey.LeftArrow:
                x -= MovementSpeed * deltaTime + 1;
				break;
            default:
                break;
        }

		int playerStrLen = PlayerString.Length;

		x = Engine.Clamp(x, 0, Console.BufferWidth - playerStrLen);
        y = Engine.Clamp(y, 0, Console.BufferHeight - 1);


		prevX = x;
        prevY = y;

		short x16 = (short)MathF.Round(x);
		short y16 = (short)MathF.Round(y);
		FastConsole.WriteAt(PlayerString, new(x16, y16));
		if (prevX != x || prevY != y) FastConsole.WriteAt(' ', new((short)MathF.Round(prevX), (short)MathF.Round(prevY)));
	}


}
