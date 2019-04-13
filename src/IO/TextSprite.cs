using System.Collections.Generic;

public class TextSprite
{
	public enum Alignement
	{
		LEFT,
		RIGHT,
		CENTER
	}

	const int LETTER_OFFSET = -81;

	SpriteSheet font;
	GridPosition position;
	IList<int> sequence;
	string text;
	Alignement alignement;

	IDictionary<char, int> symbolMapping = new Dictionary<char, int>()
	{
		{':', 42},
		{'-', 43},
		{' ', 44},
		{'/', 45}
	};

	public TextSprite(SpriteSheet _font, Alignement align = Alignement.LEFT, int x = 0, int y = 0)
	{
		font = _font;
		position = new GridPosition(x, y);
	}

	public void SetText(string _text)
	{
		text = _text.ToLower();
		sequence = new List<int>(_text.Length);
		foreach (char c in _text)
			if (c >= 'a' && c <= 'z')
				sequence.Add(c + LETTER_OFFSET);
			else if (symbolMapping.ContainsKey(c))
				sequence.Add(symbolMapping[c]);
	}

	public void Draw()
	{
		GridPosition cache = new GridPosition(position);
		if (alignement == Alignement.RIGHT)
			cache.X -= sequence.Count + 1;
		else if (alignement == Alignement.CENTER)
			cache.X -= (sequence.Count - 1) / 2;

		for (int i = 0; i < sequence.Count; i++)
		{
			font.Draw(sequence[i], cache.ToVector());
			cache.X += 1;
		}
	}
}