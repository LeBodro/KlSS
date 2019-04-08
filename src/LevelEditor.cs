using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame;

public class LevelEditor
{
	SpriteSheet gameAtlas;
	SpriteSheet editorAtlas;
	MouseState mouse;
	int brush;

	public Level Target { get; set; }
	public Player Player { get; set; }

	public LevelEditor(SpriteSheet _gameAtlas, SpriteSheet _editorAtlas)
	{
		gameAtlas = _gameAtlas;
		editorAtlas = _editorAtlas;
	}

	public void Update()
	{
		mouse = Mouse.GetState();
		if (mouse.LeftButton == ButtonState.Pressed)
		{
			GridPosition gridPos = GridPosition.FromWindowCoordinates(mouse.X, mouse.Y);
			if (gridPos.X < GridPosition.GRID_SIZE)
				Paint(gridPos);
			else if (IsWithinBrushSelector(gridPos))
				SelectBrush(gridPos.Y);
		}
	}

	public void Draw()
	{
		GridPosition brushSelector = new GridPosition(17, 0);
		Vector2 pos = brushSelector.ToVector();

		for (int i = 0; i < 11; i++)
		{
			gameAtlas.Draw(i, pos);
			pos.Y += GridPosition.CELL_SIZE;
			if (brush == i)
			{
				GridPosition cursor = new GridPosition(16, i);
				editorAtlas.Draw(14, cursor.ToVector());
			}
		}
	}

	void Paint(GridPosition position)
	{
		if (brush < 0 || brush > 10) return; // Invalid brush ID
		if (Player.Index == position.Index) return; // Can't overwrite player

		if (brush < 4)
			CreateObstacle((Collectible.Type)brush, position);
		else if (brush < 8)
			CreateItem((Collectible.Type)(brush - 4), position);
		else if (brush == 8)
			Target.Empty(position.Index);
		else if (brush == 9)
			Target.SetWall(position.Index);
		else if (brush == 10)
		{
			Player.MoveTo(position);
			Target.Empty(Player.Index);
		}
	}

	bool IsWithinBrushSelector(GridPosition pos)
	{
		return pos.X == GridPosition.GRID_SIZE + 1 && pos.Y < 11;
	}

	void SelectBrush(int index)
	{
		brush = index;
		// TODO: indicate selection
	}

	void CreateItem(Collectible.Type type, GridPosition pos)
	{
		var newItem = new Collectible(gameAtlas, type);
		newItem.MoveTo(pos);
		Target.Add(newItem);
	}

	void CreateObstacle(Collectible.Type type, GridPosition pos)
	{
		var newObstacle = new Interractable(gameAtlas, type);
		newObstacle.MoveTo(pos);
		Target.Add(newObstacle);
	}
}