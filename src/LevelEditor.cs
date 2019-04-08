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
		}
	}

	void Paint(GridPosition position)
	{
		if (brush < 0 || brush > 10) return; // Invalid brush ID

		if (brush < 8)
		{
			// TODO: Remove existing item, Paint dynamic item
		}
		else if (brush < 10)
		{
			// TODO: Remove existing item, Paint wall/empty
		}
		else if (brush == 10)
		{
			Player.MoveTo(position);
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
}