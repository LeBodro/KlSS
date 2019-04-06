using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Level
{
	public const int SIZE = GridPosition.GRID_SIZE;

	SpriteSheet sheet;
	BitArray walls = new BitArray(256, false);
	HashSet<Collectible> items = new HashSet<Collectible>();
	HashSet<Interractable> obstacles = new HashSet<Interractable>();

	public GridPosition StartingPlayerPosition { get; private set; }

	public event System.Action OnDone = delegate { };

	public Level(BitArray _walls, HashSet<Collectible> _items, HashSet<Interractable> _obstacles, SpriteSheet _sheet, GridPosition startingPlayerPosition)
	{
		walls = _walls;
		items = _items;
		obstacles = _obstacles;
		sheet = _sheet;
		StartingPlayerPosition = startingPlayerPosition;

		foreach (var item in items)
			if (item.type == Collectible.Type.HEART)
				item.OnDeath += EndLevel;
	}

	void EndLevel()
	{
		OnDone();
	}

	bool CheckForInterraction(Collectible item)
	{
		HashSet<Interractable> obstaclesToDestroy = new HashSet<Interractable>();
		foreach (var obstacle in obstacles)
		{
			if (item.IsAdjacentTo(obstacle.Index) && obstacle.TryUnlocking(item.type))
			{
				obstaclesToDestroy.Add(obstacle);
			}
		}
		foreach (var obstacle in obstaclesToDestroy)
			obstacles.Remove(obstacle);

		return obstaclesToDestroy.Count > 0;
	}

	public void UpdateStep()
	{
		HashSet<Collectible> itemsToDestroy = new HashSet<Collectible>();
		foreach (var item in items)
		{
			if (item.HasMoved)
			{
				item.HasMoved = false;
				if (CheckForInterraction(item))
					itemsToDestroy.Add(item);
			}
		}

		foreach (var item in itemsToDestroy)
		{
			item.Kill();
			items.Remove(item);
		}
	}

	public void Draw()
	{
		Vector2 p = Vector2.Zero;
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i])
			{
				p.X = i % SIZE * GridPosition.CELL_SIZE;
				p.Y = i / SIZE * GridPosition.CELL_SIZE;
				sheet.Draw(0, p);
			}
		}

		foreach (var item in items)
			item.Draw();

		foreach (var obstacle in obstacles)
			obstacle.Draw();
	}

	public bool IsImpassible(int tileIndex, int indexDelta)
	{
		int indexToCheck = tileIndex + indexDelta;
		if (ContainsObstacle(indexToCheck))
			return true;

		int furtherIndex = indexToCheck + indexDelta;
		bool couldPushItem = IsEmpty(furtherIndex);

		foreach (var item in items)
		{
			if (!item.IsCollected && item.Index == indexToCheck)
			{
				if (couldPushItem) item.Move((Sprite.Direction)indexDelta);
				else return true;
			}
		}
		return false;
	}

	bool IsEmpty(int tileIndex)
	{
		if (ContainsObstacle(tileIndex))
			return false;

		foreach (var item in items)
			if (item.Index == tileIndex)
				return false;

		return true;
	}

	bool ContainsObstacle(int tileIndex)
	{
		if (tileIndex < 0 || tileIndex >= 256 || walls[tileIndex])
			return true;
		foreach (var obstacle in obstacles)
			if (obstacle.Index == tileIndex)
				return true;
		return false;
	}

	public Collectible FetchItem(int tileIndex)
	{
		foreach (var item in items)
		{
			if (item.Index == tileIndex)
				return item;
		}
		return null;
	}
}