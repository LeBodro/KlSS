public class Collectible : Sprite
{
	public enum Type
	{
		KEY = 0,
		SHIELD,
		SWORD,
		HEART,
	}

	public Type type { get; private set; }
	public bool IsCollected { get; set; }
	public bool HasMoved { get; set; }

	public Collectible(SpriteSheet sheet, Type _type) : base(sheet, (int)_type + 4)
	{
		type = _type;
	}

	public override void Move(Direction direction, int distance = 1)
	{
		base.Move(direction, distance);
		HasMoved = true;
	}
}