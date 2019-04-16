using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame;

public class LevelEditor
{
	const int MENU_X = 17;
	const int SAVE_Y = 15;
	const int NUMBER_Y = 13;
	const int SCROLL_THRESHOLD = 13500;

	SpriteSheet gameAtlas;
	SpriteSheet editorAtlas;
	MouseState mouse;
	int brush;
	Sprite saveButton;
	int lastScroll;

	public Level Target { get; set; }
	public Player Player { get; set; }
	public int LevelId { get; set; }

	public LevelEditor(SpriteSheet _gameAtlas, SpriteSheet _editorAtlas)
	{
		gameAtlas = _gameAtlas;
		editorAtlas = _editorAtlas;
		saveButton = new Sprite(editorAtlas, 12);
		saveButton.MoveTo(MENU_X, SAVE_Y);
	}

	public void Update()
	{
		mouse = Mouse.GetState();
		if (mouse.LeftButton == ButtonState.Pressed)
		{
			GridPosition click = GridPosition.FromWindowCoordinates(mouse.X, mouse.Y);
			if (IsOutOfBounds(click)) return; // Nothing to do out of bounds

			if (click.X < GridPosition.GRID_SIZE)
				Paint(click);
			else if (IsWithinBrushSelector(click))
				SelectBrush(click.Y);
			else if (click.X == MENU_X && click.Y == SAVE_Y)
				Save();
		}
		else if (mouse.RightButton == ButtonState.Pressed)
		{
			GridPosition click = GridPosition.FromWindowCoordinates(mouse.X, mouse.Y);
			if (IsOutOfBounds(click)) return; // Nothing to do out of bounds

			if (click.X < GridPosition.GRID_SIZE)
				Target.Empty(click.Index);
		}
		if (mouse.ScrollWheelValue != 0)
		{
			int delta = mouse.ScrollWheelValue - lastScroll;
			if (delta * delta > SCROLL_THRESHOLD)
			{
				brush = MathHelper.Clamp(brush + (delta > 0 ? 1 : -1), 0, 10);
				lastScroll = mouse.ScrollWheelValue;
			}
		}
	}

	bool IsOutOfBounds(GridPosition pos)
	{
		return !(pos.X >= 0 && pos.X < 19 && pos.Y >= 0 && pos.Y < 16);
	}

	public void Draw()
	{
		saveButton.Draw();
		DrawLevelNumber();
		DrawBrushes();
		DrawCursor();
	}

	void DrawLevelNumber()
	{
		int tens = LevelId / 10;
		int units = LevelId % 10;
		GridPosition pos = new GridPosition(MENU_X, NUMBER_Y);

		editorAtlas.Draw(tens, pos.ToVector());
		pos.X++;
		editorAtlas.Draw(units, pos.ToVector());
	}

	void DrawBrushes()
	{
		GridPosition brushSelector = new GridPosition(17, 0);
		Vector2 pos = brushSelector.ToVector();

		for (int i = 0; i < 11; i++)
		{
			gameAtlas.Draw(i, pos);
			pos.Y += GridPosition.CELL_SIZE;
		}
	}

	void DrawCursor()
	{
		editorAtlas.Draw(14, GridPosition.ToVector(MENU_X - 1, brush));
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
			Target.StartingPlayerPosition = position;
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

	void Save()
	{
		Target.StartingPlayerPosition.SetFromIndex(Player.Index);

		string s = Target.Serialize();
		string fileName = LevelLoader.GetFullLevelPath(LevelId);

		if (File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		else
		{
			LevelLoader.LevelCount++;
			SaveGame.AddLevel();
		}

		using (StreamWriter sw = File.CreateText(fileName))
			sw.Write(s.ToCharArray());
	}
}