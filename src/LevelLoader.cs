using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class LevelLoader
{
	const string LEVEL_NAME = "{0}/level{1}.txt";

	const string EMPTY = ".";
	const string WALL = "#";
	const string SWORD = "B";
	const string SHIELD = "S";
	const string HEART = "H";
	const string KEY = "K";
	const string PLAYER = "P";
	const string MONSTER = "M";
	const string DOOR = "D";
	const string FIRE = "F";
	const string LOVER = "L";

	BitArray walls = new BitArray(256, false);
	HashSet<Collectible> items = new HashSet<Collectible>();
	HashSet<Interractable> obstacles = new HashSet<Interractable>();

	SpriteBatch spriteBatch;
	ContentManager content;
	SpriteSheet collectibleSheet;
	SpriteSheet obstacleSheet;
	SpriteSheet wallSheet;

	public int LevelCount { get; private set; }

	public LevelLoader(SpriteBatch _spriteBatch, ContentManager _content)
	{
		spriteBatch = _spriteBatch;
		content = _content;
		collectibleSheet = LoadSpriteSheet("collectibles" + GridPosition.CELL_SIZE, 2, 2);
		obstacleSheet = LoadSpriteSheet("obstacles" + GridPosition.CELL_SIZE, 2, 2);
		wallSheet = LoadSpriteSheet("walls" + GridPosition.CELL_SIZE, 2, 2);
		LevelCount = GetLevelCount();
	}

	int GetLevelCount()
	{
		string partialName = "level";
		DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, content.RootDirectory));
		FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles(partialName + "*.*");

		return filesInDir.Length;
	}

	SpriteSheet LoadSpriteSheet(string textureName, int rows, int columns)
	{
		var texture = content.Load<Texture2D>(textureName);
		return new SpriteSheet(spriteBatch, texture, rows, columns);
	}

	public Level Load(int levelId)
	{
		GridPosition playerPosition = null;
		walls.SetAll(false);
		items.Clear();
		obstacles.Clear();
		string fileName = string.Format(LEVEL_NAME, content.RootDirectory, levelId);
		using (StreamReader reader = new StreamReader(TitleContainer.OpenStream(fileName)))
		{
			for (int j = 0; j < GridPosition.GRID_SIZE; j++)
			{
				string line = reader.ReadLine();
				for (int i = 0; i < GridPosition.GRID_SIZE; i++)
				{
					switch (line.Substring(i, 1))
					{
						case EMPTY:
							break;
						case WALL:
							walls[i + j * GridPosition.GRID_SIZE] = true;
							break;
						case SWORD:
							CreateItem(Collectible.Type.SWORD, i, j);
							break;
						case SHIELD:
							CreateItem(Collectible.Type.SHIELD, i, j);
							break;
						case HEART:
							CreateItem(Collectible.Type.HEART, i, j);
							break;
						case KEY:
							CreateItem(Collectible.Type.KEY, i, j);
							break;
						case PLAYER:
							playerPosition = new GridPosition(i, j);
							break;
						case MONSTER:
							CreateObstacle(Collectible.Type.SWORD, i, j);
							break;
						case DOOR:
							CreateObstacle(Collectible.Type.KEY, i, j);
							break;
						case FIRE:
							CreateObstacle(Collectible.Type.SHIELD, i, j);
							break;
						case LOVER:
							CreateObstacle(Collectible.Type.HEART, i, j);
							break;
					}
				}
			}
		}
		return new Level(walls, items, obstacles, wallSheet, playerPosition);
	}

	void CreateItem(Collectible.Type type, int x, int y)
	{
		var newItem = new Collectible(collectibleSheet, type);
		newItem.MoveTo(x, y);
		items.Add(newItem);
	}

	void CreateObstacle(Collectible.Type type, int x, int y)
	{
		var newObstacle = new Interractable(obstacleSheet, type);
		newObstacle.MoveTo(x, y);
		obstacles.Add(newObstacle);
	}
}