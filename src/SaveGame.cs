using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;

public class SaveGame
{
	public struct Data
	{
		public int Weekly;
		public List<int> Levels;
		public List<int> Scores;
	}

	static SaveGame _instance;
	static SaveGame Instance
	{
		get
		{
			if (_instance == null) _instance = new SaveGame();
			return _instance;
		}
	}

	public static string FullPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "savegame.txt");
	public static int LevelCount { get => Instance.data.Levels.Count; }
	public static int Weekly
	{
		get => Instance.data.Weekly;
		set
		{
			Instance.data.Weekly = value;
			Instance._Save();
		}
	}

	Data data;

	SaveGame() { }

	public static void Save()
	{
		Instance._Save();
	}

	public static void Load()
	{
		Instance._Load();
	}

	public static void MoveLevelDown(int index)
	{
		Instance._MoveLevelDown(index);
	}

	public static void MoveLevelUp(int index)
	{
		Instance._MoveLevelUp(index);
	}

	public static void KeepHighScore(int index, int score)
	{
		Instance._SetHighScore(index, score);
	}

	public static void AddLevel()
	{
		Instance._AddLevel();
	}

	public static int GetLevelId(int id)
	{
		return id < Instance.data.Levels.Count ? Instance.data.Levels[id] : -1;
	}

	public static int GetScore(int id)
	{
		return id < Instance.data.Scores.Count ? Instance.data.Scores[id] : 0;
	}

	void _Save()
	{
		string json = JsonConvert.SerializeObject(data);

		if (File.Exists(FullPath))
			File.Delete(FullPath);

		using (StreamWriter writer = File.CreateText(FullPath))
			writer.Write(json.ToCharArray());
	}

	void _Load()
	{
		if (File.Exists(FullPath))
		{
			string json = File.ReadAllText(FullPath);
			data = JsonConvert.DeserializeObject<Data>(json);
		}
		else
		{
			MonoGame.Log.Print("No savegame found at: " + FullPath);
			data.Weekly = -1;
			int count = LevelLoader.LevelCount;
			data.Levels = new List<int>();
			data.Scores = new List<int>();
			for (int i = 0; i < count; i++)
			{
				data.Levels.Add(i);
				data.Scores.Add(0);
			}
		}
	}

	void _MoveLevelDown(int index)
	{
		data.Levels.MoveDown(index);
		data.Scores.MoveDown(index);
		_Save();
	}

	void _MoveLevelUp(int index)
	{
		data.Levels.MoveUp(index);
		data.Scores.MoveUp(index);
		_Save();
	}

	void _SetHighScore(int index, int score)
	{
		if (data.Scores[index] <= 0 || score < data.Scores[index])
		{
			data.Scores[index] = score;
			_Save();
		}
	}

	void _AddLevel()
	{
		data.Levels.Add(data.Levels.Count);
		data.Scores.Add(0);
		_Save();
	}
}

public static class ListExtension
{
	public static void MoveUp<T>(this List<T> self, int index)
	{
		if (index < self.Count - 1)
		{
			T item = self[index];
			self.RemoveAt(index);
			self.Insert(index + 1, item);
		}
	}

	public static void MoveDown<T>(this List<T> self, int index)
	{
		if (index > 0)
		{
			T item = self[index];
			self.RemoveAt(index);
			self.Insert(index - 1, item);
		}
	}
}