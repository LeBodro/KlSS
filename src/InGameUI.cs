using System;
using System.Collections.Generic;

public struct Button
{
	public Sprite Sprite { get; private set; }
	public Action Callback { get; private set; }
	public Button(Sprite sprite, Action callback) { Sprite = sprite; Callback = callback; }
}

public class InGameUI
{
	const int MENU_Y = 16;
	const int MENU_X_OFFSET = 7;

	GridMouse mouse = new GridMouse();
	IList<Button> buttons = new List<Button>();

	public InGameUI()
	{
		mouse.Left.OnClick += ProcessClick;
	}

	public void AddButton(Sprite sprite, Action callback)
	{
		sprite.MoveTo(MENU_X_OFFSET + buttons.Count, MENU_Y);
		buttons.Add(new Button(sprite, callback));
	}

	void ProcessClick(GridPosition position)
	{
		if (position.Y != MENU_Y) return;
		foreach (var btn in buttons)
			if (btn.Sprite.X == position.X)
				btn.Callback();
	}

	public void Update()
	{
		mouse.Update();
	}

	public void Draw()
	{
		foreach (var btn in buttons)
			btn.Sprite.Draw();
	}
}