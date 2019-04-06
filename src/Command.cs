using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

public class Command
{
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
	HashSet<Keys> downKeys = new HashSet<Keys>();

	public void Update()
	{
		kstate = Keyboard.GetState();
		var unpressedKeys = new HashSet<Keys>(downKeys);
		foreach (Keys k in kstate.GetPressedKeys())
		{
			if (!downKeys.Contains(k))
			{
				downKeys.Add(k);
				if (justDown.ContainsKey(k))
					justDown[k]();
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