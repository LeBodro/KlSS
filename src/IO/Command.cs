using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

public class Command
{
	const double REPETITION = 0.16f;

	public enum Event
	{
		JUST_DOWN,
		JUST_UP,
		DOWN,
	}

	KeyboardState kstate;
	IDictionary<Keys, System.Action> justDown = new Dictionary<Keys, System.Action>();
	IDictionary<Keys, System.Action> justUp = new Dictionary<Keys, System.Action>();
	IDictionary<Keys, System.Action> down = new Dictionary<Keys, System.Action>();
	IDictionary<Keys, double> repeat = new Dictionary<Keys, double>();
	HashSet<Keys> downKeys = new HashSet<Keys>();

	public void Update(double time)
	{
		kstate = Keyboard.GetState();
		var unpressedKeys = new HashSet<Keys>(downKeys);
		foreach (Keys k in kstate.GetPressedKeys())
		{
			if (!downKeys.Contains(k))
			{
				downKeys.Add(k);
				if (justDown.ContainsKey(k))
				{
					justDown[k]();
					repeat[k] = REPETITION;
				}
			}
			else if (justDown.ContainsKey(k))
			{
				repeat[k] -= time;
				if (repeat[k] <= 0 && justDown.ContainsKey(k))
				{
					System.Console.WriteLine(time.ToString());
					justDown[k]();
					repeat[k] = REPETITION;
				}
			}
			if (down.ContainsKey(k))
				down[k]();

			unpressedKeys.Remove(k);
		}

		foreach (Keys k in unpressedKeys)
		{
			if (justUp.ContainsKey(k))
				justUp[k]();
			downKeys.Remove(k);
			repeat.Remove(k);
		}
	}

	public void Map(Keys k, Event e, System.Action callback)
	{
		switch (e)
		{
			case Event.JUST_DOWN:
				justDown.Add(k, callback);
				break;
			case Event.JUST_UP:
				justUp.Add(k, callback);
				break;
			case Event.DOWN:
				down.Add(k, callback);
				break;
		}
	}
}