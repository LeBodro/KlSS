using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class GridMouseButton
{
	public event Action<GridPosition> OnClick = delegate { };
	public event Action<GridPosition> OnDrag = delegate { };

	GridPosition lastPosition = new GridPosition();
	ButtonState lastState = ButtonState.Released;

	bool Pressed(ButtonState s) => s == ButtonState.Pressed;
	bool Moved(GridPosition p) => p != lastPosition;

	public void Update(ButtonState state, GridPosition position)
	{
		if (Pressed(state))
		{
			if (Pressed(lastState))
			{
				if (Moved(position))
					OnDrag(position);
			}
			else
			{
				OnClick(position);
			}
		}

		lastPosition = position;
		lastState = state;
	}
}

public class GridMouse
{
	public GridMouseButton Left { get; private set; }
	public GridMouseButton Right { get; private set; }
	public event Action<int> OnScroll = delegate { };

	MouseState mouse;
	int lastWheelValue;

	public GridMouse()
	{
		Left = new GridMouseButton();
		Right = new GridMouseButton();
	}

	public void Update()
	{
		mouse = Mouse.GetState();
		GridPosition pos = GridPosition.FromWindowCoordinates(mouse.Position);
		Left.Update(mouse.LeftButton, pos);
		Right.Update(mouse.RightButton, pos);

		OnScroll(lastWheelValue - mouse.ScrollWheelValue);
		lastWheelValue = mouse.ScrollWheelValue;
	}
}