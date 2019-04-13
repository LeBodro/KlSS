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
	const int NUMBER_OFFSET = -48;

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
		alignement = align;
	}

	public void SetText(string _text)
	{
		text = _text.ToLower();
		sequence = new List<int>(_text.Length);
		foreach (char c in _text)
			sequence.Add(ToSheetCell(c));
	}

	int ToSheetCell(char c)
	{
		if (c >= 'a' && c <= 'z')
			return c + LETTER_OFFSET;
		else if (c >= '0' && c <= '9')
			return c + NUMBER_OFFSET;
		else if (symbolMapping.ContainsKey(c))
			return symbolMapping[c];
		return symbolMapping[' '];
	}

	public void Draw()
	{
		GridPosition cache = new GridPosition(position);
		if (alignement == Alignement.RIGHT)
			cache.X -= sequence.Count - 1;
		else if (alignement == Alignement.CENTER)
			cache.X -= (sequence.Count - 1) / 2;

		for (int i = 0; i < sequence.Count; i++)
		{
			font.Draw(sequence[i], cache.ToVector());
			cache.X += 1;
		}
	}
}