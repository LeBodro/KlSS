using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class SpriteSheet
{
	SpriteBatch spriteBatch;
	Texture2D atlas;
	int rows;
	int columns;
	int cellWidth;
	int cellHeight;
	Rectangle cellRect;

	public SpriteSheet(SpriteBatch _spriteBatch, Texture2D _atlas, int _rows = 1, int _columns = 1)
	{
		spriteBatch = _spriteBatch;
		rows = _rows;
		columns = _columns;
		atlas = _atlas;
		cellWidth = atlas.Width / columns;
		cellHeight = atlas.Height / rows;
		cellRect = new Rectangle(0, 0, cellWidth, cellHeight);
	}

	public void Draw(int cellId, Vector2 pos)
	{
		cellRect.X = cellId % columns * cellWidth;
		cellRect.Y = cellId / rows * cellHeight;
		Begin();
		spriteBatch.Draw(atlas, pos, cellRect, Color.White);
		spriteBatch.End();
	}

	void Begin()
	{
		spriteBatch.Begin(
			SpriteSortMode.BackToFront,
			BlendState.NonPremultiplied,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone,
			null,
			Matrix.CreateScale(MonoGame.Root.Scale)
		);
	}
}