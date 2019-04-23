using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Sprite
{
	public enum Direction
	{
		UP = -Level.SIZE,
		DOWN = Level.SIZE,
		LEFT = -1,
		RIGHT = 1,
		NONE = 0,
	}

	SpriteSheet sheet;
	int cellId;
	GridPosition position;
	List<int> adjacentCells;

	public int Index { get => position.Index; }
	public int X { get => position.X; protected set => position.X = value; }
	public int Y { get => position.Y; protected set => position.Y = value; }

	protected event System.Action _onDeath = delegate { };
	public event System.Action OnDeath
	{
		add { _onDeath += value; }
		remove { _onDeath -= value; }
	}

	public Sprite(SpriteSheet _sheet, int _cellId)
	{
		sheet = _sheet;
		cellId = _cellId;
		position = new GridPosition(0, 0);
	}

	public void Draw()
	{
		sheet.Draw(cellId, position.ToVector());
	}

	public void MoveTo(int x, int y)
	{
		X = x;
		Y = y;
		RefreshAdjacentCells();
	}

	public void MoveTo(GridPosition newPos)
	{
		X = newPos.X;
		Y = newPos.Y;
		RefreshAdjacentCells();
	}

	public virtual void Move(Direction direction, int distance = 1)
	{
		switch (direction)
		{
			case Direction.UP:
				Y -= distance;
				break;
			case Direction.DOWN:
				Y += distance;
				break;
			case Direction.LEFT:
				X -= distance;
				break;
			case Direction.RIGHT:
				X += distance;
				break;
			default:
				break;
		}
		RefreshAdjacentCells();
	}

	public bool IsAdjacentTo(int cell)
	{
		return adjacentCells.Contains(cell);
	}

	void RefreshAdjacentCells()
	{
		adjacentCells = new List<int>{
			Index - GridPosition.GRID_SIZE,
			Index + GridPosition.GRID_SIZE,
			Index - 1,
			Index + 1
		};
	}

	public void Kill()
	{
		_onDeath();
	}
}