using System.Collections.Generic;

public class Player : Sprite
{
	IDictionary<Direction, Collectible> attachedItems = new Dictionary<Direction, Collectible>(4);

	public Level CurrentLevel { get; set; }

	public Player(SpriteSheet sheet, int cellId) : base(sheet, cellId)
	{
	}

	public void Reset()
	{
		attachedItems.Clear();
	}

	public override void Move(Direction direction, int distance = 1)
	{
		if (CurrentLevel.IsImpassible(Index, (int)direction)) return;
		foreach (var item in attachedItems.Values)
			if (CurrentLevel.IsImpassible(item.Index, (int)direction)) return;

		Score.Increase(1);
		base.Move(direction);
		foreach (var item in attachedItems.Values)
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
				fetchedItem.OnDeath += () => attachedItems.Remove(direction);
				fetchedItem.IsCollected = true;
				return true;
			}
		}
		return false;
	}
}