using System.Numerics;
using System.Diagnostics;
using Console_Pong._Engine;
using System.Drawing;
using Console_Pong._Engine._Input;

namespace Console_Pong;

internal class PongGame : IGameBehaviour
{
    #region GameParams

    const float MovementSpeed = 15f;
    const string PlayerString = "🫵👁👄👁🤏";
    Player player = new("galla153", PlayerString, MovementSpeed);
    public bool IsMultiplayerEnabled { get; set; } = false;

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

    PointF prevPlayerPos = new();

    public void FixedUpdate(float deltaTime)
    {
        if (player.Position != prevPlayerPos)
        {
			MultiplayerClient.SendPlayerToClients(player);
            prevPlayerPos = player.Position;
		}
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
                player.MoveBy(0, -MovementSpeed * deltaTime);
                break;
            case ConsoleKey.DownArrow:
                player.MoveBy(0, MovementSpeed * deltaTime);
				break;
            case ConsoleKey.RightArrow:
				player.MoveBy(MovementSpeed * deltaTime + 1, 0);
				break;
            case ConsoleKey.LeftArrow:
				player.MoveBy(-MovementSpeed * deltaTime - 1, 0);
				break;
            default:
                break;
        }

#error TODO: Test multiplayer on different devices

        MultiplayerClient.RenderNetworkPlayers();
		player.Render();
	}
}
