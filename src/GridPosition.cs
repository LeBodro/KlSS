using Microsoft.Xna.Framework;
using MonoGame;

public class GridPosition
{
	public string FORMAT = "[{0} ; {1}]";
	public const int CELL_SIZE = 8;
	public const int GRID_SIZE = 16;
	public const int SCALED_CELL_SIZE = CELL_SIZE * Root.SCALE;

	int _x;
	public int X { get => _x; set { _x = value; Index = X + GRID_SIZE * Y; } }
	int _y;
	public int Y { get => _y; set { _y = value; Index = X + GRID_SIZE * Y; } }
	public int Index { get; private set; }

	public static GridPosition FromWindowCoordinates(int x, int y)
	{
		return new GridPosition(x / SCALED_CELL_SIZE, y / SCALED_CELL_SIZE);
	}

	public GridPosition(int x = 0, int y = 0)
	{
		_x = x;
		_y = y;
		Index = X + GRID_SIZE * Y;
	}

	public static bool operator ==(GridPosition a, GridPosition b)
	{
		return a.X == b.X && a.Y == b.Y;
	}

	public static bool operator !=(GridPosition a, GridPosition b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
			return false;

		return this == (GridPosition)obj;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public Vector2 ToVector()
	{
		return CELL_SIZE * new Vector2(X, Y);
	}

	public override string ToString()
	{
		return string.Format(FORMAT, X, Y);
	}
}