public class Score
{
	int _total;
	public static int Total { get => Instance._total; }

	static Score _instance;
	static Score Instance
	{
		get
		{
			if (_instance == null) _instance = new Score();
			return _instance;
		}
	}

	Score() { }

	public static int Increase(int value)
	{
		Instance._total += value;
		return Instance._total;
	}

	public static void Reset()
	{
		Instance._total = 0;
	}
}