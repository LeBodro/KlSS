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

		AudioLibrary.Instance.Play("Step");
		Score.Increase(1);
		base.Move(direction);
		foreach (var item in attachedItems.Values)
			item.Move(direction);

		FetchItemAt(Direction.UP);
		FetchItemAt(Direction.DOWN);
		FetchItemAt(Direction.LEFT);
		FetchItemAt(Direction.RIGHT);
	}

	void FetchItemAt(Direction direction)
	{
		if (!attachedItems.ContainsKey(direction))
		{
			var fetchedItem = CurrentLevel.FetchItem(Index + (int)direction);
			if (fetchedItem != null)
			{
				attachedItems.Add(direction, fetchedItem);
				fetchedItem.OnDeath += () => attachedItems.Remove(direction);
				fetchedItem.IsCollected = true;
			}
		}
	}
}