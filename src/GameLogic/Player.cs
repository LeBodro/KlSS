using System.Collections.Generic;

public class Player : Sprite
{
	IDictionary<Direction, Collectible> attachedItems = new Dictionary<Direction, Collectible>(4);

	public Level CurrentLevel { get; set; }

	public Player(SpriteSheet sheet, int cellId) : base(sheet, cellId) { }

	public void Reset()
	{
		attachedItems.Clear();
		cellId = 0;
	}

	public override void Move(Direction direction, int distance = 1)
	{
		List<Collectible> pushables = new List<Collectible>(4);
		if (CurrentLevel.IsImpassible(Index, (int)direction)) return;
		foreach (var item in attachedItems.Values)
		{
			Collectible pushable = null;
			if (CurrentLevel.IsImpassible(item.Index, (int)direction, ref pushable)) return;
			if (pushable != null)
				pushables.Add(pushable);
		}

		Score.Increase(1);
		base.Move(direction);
		foreach (var item in attachedItems.Values)
			item.Move(direction);
		foreach (var item in pushables)
			item.Move(direction);

		bool fetched = false;
		fetched |= FetchItemAt(Direction.UP);
		fetched |= FetchItemAt(Direction.DOWN);
		fetched |= FetchItemAt(Direction.LEFT);
		fetched |= FetchItemAt(Direction.RIGHT);
		if (fetched)
			AudioLibrary.Instance.Play("Stick");
		else
			AudioLibrary.Instance.Play("Step");
	}

	bool FetchItemAt(Direction direction)
	{
		if (!attachedItems.ContainsKey(direction))
		{
			var fetchedItem = CurrentLevel.FetchItem(Index + (int)direction);
			if (fetchedItem != null)
			{
				attachedItems.Add(direction, fetchedItem);
				fetchedItem.OnDeath += () => { attachedItems.Remove(direction); RefreshCellId(); };
				fetchedItem.IsCollected = true;
				RefreshCellId();
				return true;
			}
		}
		return false;
	}

	void RefreshCellId()
	{
		var occupiedSlots = attachedItems.Keys;
		int id = 0;
		foreach (var direction in occupiedSlots)
		{
			if (direction == Direction.LEFT) id += 1;
			if (direction == Direction.RIGHT) id += 2;
			if (direction == Direction.UP) id += 4;
			if (direction == Direction.DOWN) id += 8;
		}
		cellId = id;
	}
}