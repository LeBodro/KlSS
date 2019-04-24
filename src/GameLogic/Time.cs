using Microsoft.Xna.Framework;

public static class Time
{
	public static double deltaSeconds { get; private set; }
	public static double totalSeconds { get; private set; }
	public static int currentMilliseconds { get; private set; }
	public static int currentSeconds { get; private set; }

	public static void Update(GameTime time)
	{
		deltaSeconds = time.ElapsedGameTime.TotalSeconds;
		totalSeconds = time.TotalGameTime.TotalSeconds;
		currentMilliseconds = time.TotalGameTime.Milliseconds;
		currentSeconds = time.TotalGameTime.Seconds;
	}
}