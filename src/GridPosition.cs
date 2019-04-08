using Microsoft.Xna.Framework;

public class GridPosition
{
	public const int CELL_SIZE = 8;
	public const int GRID_SIZE = 16;

	int _x;
	public int X { get => _x; set { _x = value; Index = X + GRID_SIZE * Y; } }
	int _y;
	public int Y { get => _y; set { _y = value; Index = X + GRID_SIZE * Y; } }
	public int Index { get; private set; }

	public static GridPosition FromWindowCoordinates(int x, int y)
	{
		return new GridPosition(x / CELL_SIZE, y / CELL_SIZE);
	}

	public GridPosition(int x = 0, int y = 0)
	{
		_x = x;
		_y = y;
		Index = X + GRID_SIZE * Y;
	}

	public Vector2 ToVector()
	{
		return CELL_SIZE * new Vector2(X, Y);
	}
}