using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class LevelLoader
{
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

	SpriteSheet atlas;
	ContentManager content;

	static int _levelCount = -1;
	public static int LevelCount
	{
		get
		{
			if (_levelCount == -1)
				_levelCount = GetLevelCount();
			return _levelCount;
		}
		set
		{
			_levelCount = value;
		}
	}

	public static string GetLevelPath(int id)
	{
		return Path.Combine(".", "Content", "level" + id + ".txt");
	}

	public static string GetFullLevelPath(int id)
	{
		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "level" + id + ".txt");
	}

	public LevelLoader(SpriteSheet _atlas, ContentManager _content)
	{
		atlas = _atlas;
		content = _content;
		DownloadLevelOfTheWeek();
	}

	static int GetLevelCount()
	{
		string partialName = "level";
		string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
		var directory = System.IO.Path.GetDirectoryName(path);
		DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Path.GetDirectoryName(GetFullLevelPath(0)));
		FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles(partialName + "*.txt");

		return filesInDir.Length;
	}

	public Level Load(int levelId)
	{
		GridPosition playerPosition = null;
		walls.SetAll(false);
		items.Clear();
		obstacles.Clear();
		string instruction = string.Empty;
		string fileName = GetLevelPath(levelId);
		if (levelId == -1 || !File.Exists(GetFullLevelPath(levelId)))
			fileName = Path.Combine(".", "Content", "TEMPLATE.txt");
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
			instruction = reader.ReadLine();
		}
		Level level = new Level(walls, items, obstacles, atlas, playerPosition);
		if (!string.IsNullOrWhiteSpace(instruction))
			level.Instruction = instruction;
		return level;
	}

	void DownloadLevelOfTheWeek()
	{
		Console.WriteLine("Fetching new level.");
		try { FetchLevelOnline(); }
		catch (System.Net.WebException e) { Console.WriteLine(string.Format("Could not reach level: {0}", e.Message)); }
	}

	void FetchLevelOnline()
	{
		WebClient client = new WebClient();
		int id = -1;
		byte[] raw = client.DownloadData("https://raw.githubusercontent.com/LeBodro/KlSS/master/weekly.txt");
		string data = System.Text.Encoding.ASCII.GetString(raw);
		int length = data.IndexOf('\n');
		string rawId = data.Substring(0, length + 1);
		if (!int.TryParse(rawId, out id))
			Console.WriteLine("Failed to parse \"" + rawId + "\" as id.");

		if (id > SaveGame.Weekly)
		{
			Console.WriteLine("Adding downloaded level.");
			data = data.Replace(rawId, string.Empty);
			SaveGame.Weekly = id;
			using (StreamWriter sw = File.CreateText(GetFullLevelPath(LevelCount)))
				sw.Write(data.ToCharArray());
			LevelCount++;
			SaveGame.AddLevel();
		}
		else
		{
			Console.WriteLine("Already up to date.");
		}
	}

	void CreateItem(Collectible.Type type, int x, int y)
	{
		var newItem = new Collectible(atlas, type);
		newItem.MoveTo(x, y);
		items.Add(newItem);
	}

	void CreateObstacle(Collectible.Type type, int x, int y)
	{
		var newObstacle = new Interractable(atlas, type);
		newObstacle.MoveTo(x, y);
		obstacles.Add(newObstacle);
	}
}