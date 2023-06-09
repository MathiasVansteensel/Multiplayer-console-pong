using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Pong._Engine;
public class Player : IEquatable<Player>
{
	internal const int MinChar = 0x1F5FF;
	internal const int MaxChar = 0x1F64B;

	internal const short BufferHeightMargin = 1;

	public event Action<Player> OnPlayerMove;

	private static Random randomCharacter = new();

	private PointF _position = new PointF(0, 0);
	public PointF Position { get => _position; internal set => _position = value; } 
	public string Name { get; internal set; }
	public bool IsBoundByViewport { get; internal set; } = true;
	public float MovementSpeed { get; internal set; } = 15f;

	private string _playerString = ((char)randomCharacter.Next(MinChar, MaxChar)).ToString();
	public string PlayerString 
	{
		get => _playerString;
		internal set => PlayerStringLength = (_playerString = value).Length;
	}

	public int PlayerStringLength { get; private set; }

    public Player() => PlayerStringLength = _playerString.Length;

    public Player(string name, string playerString = null, float movementSpeed = 15f, Point position = default)
    {
        Name = name;
		if (playerString is not null) PlayerString = playerString;
		MovementSpeed = movementSpeed;
		Position = position;
    }

	public void Render() => FastConsole.WriteAt(PlayerString, GetViewportPos());

	public void MoveTo(float x, float y)
	{
		Size bufferSize = Engine.ConsoleBufferSize;

		if (IsBoundByViewport)
		{
			x = Engine.Clamp(x, 0, bufferSize.Width - PlayerStringLength);
			y = Engine.Clamp(y, 0, bufferSize.Height - 1);
		}

		Position = new(x / bufferSize.Width, y / bufferSize.Height);
		OnPlayerMove?.Invoke(this);
	}

	public void MoveBy(float x, float y)
	{
		Size bufferSize = Engine.ConsoleBufferSize;
		int maxX = GetMaxX(bufferSize.Width, PlayerStringLength),
			maxY = GetMaxY(bufferSize.Height, BufferHeightMargin);

		_position.X += x / maxX;
		_position.Y += y / maxY;

		if (IsBoundByViewport) 
		{
			_position.X = Engine.Clamp(Position.X, 0f, 1f);
			_position.Y = Engine.Clamp(Position.Y, 0f, 1f);
		}
		OnPlayerMove?.Invoke(this);
	}

	public FastConsole.Coord GetViewportPos()
	{
		Size bufferSize = Engine.ConsoleBufferSize;
		int maxX = GetMaxX(bufferSize.Width, PlayerStringLength),
			maxY = GetMaxY(bufferSize.Height, BufferHeightMargin);
		return new((short)MathF.Round(Position.X * maxX), (short)MathF.Round(Position.Y * maxY));
	}

	private int GetMaxX(int bufferWidth, int playerStrLen) => bufferWidth - playerStrLen;
	private int GetMaxY(int bufferHeight, int bottomHeightMargin) => bufferHeight - bottomHeightMargin;

	public bool Equals(Player? other) => other is not null & Name == other?.Name & PlayerString == other?.PlayerString;
}
