using System.Collections.Generic;

public class TextSprite
{
	const int LETTER_OFFSET = -81;

	SpriteSheet font;
	GridPosition position;
	IList<int> sequence;
	string text;

	IDictionary<char, int> symbolMapping = new Dictionary<char, int>()
	{
		{':', 26},
		{'-', 27},
		{' ', 28}
	};

	public TextSprite(SpriteSheet _font, int x = 0, int y = 0)
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
		for (int i = 0; i < sequence.Count; i++)
		{
			font.Draw(sequence[i], cache.ToVector());
			cache.X += 1;
		}
	}
}