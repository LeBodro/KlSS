using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
	class Root : Game
	{
		public const int LEVEL_COUNT = 6;
		public const int SCALE = 3;
		public const int SIZE_LENGTH = SCALE * GridPosition.CELL_SIZE * GridPosition.GRID_SIZE;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		AudioLibrary sounds;
		Texture2D playerTex;
		SpriteSheet playerSheet;
		Player player;
		Command inputs;
		LevelLoader loader;
		Level playingLevel;
		int currentLevel = 0;

		public Root()
		{
			graphics = new GraphicsDeviceManager(this);

			// Window
			graphics.PreferredBackBufferWidth = SIZE_LENGTH;
			graphics.PreferredBackBufferHeight = SIZE_LENGTH;
			graphics.PreferMultiSampling = false;

			// Fullscreen Window (Max Resolution)
			//graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			//graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

			// Fullscreen (Max Resolution)
			//graphics.IsFullScreen = true;

			graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Log.Clear();
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();

			Log.Print("Terminal Active\n");
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load sounds
			sounds = AudioLibrary.Instance;
			sounds.Add("Destroy", Content.Load<SoundEffect>("destroy"));
			sounds.Add("Win", Content.Load<SoundEffect>("win"));
			sounds.Add("Step", Content.Load<SoundEffect>("step"));

			// Preparing player
			playerTex = Content.Load<Texture2D>("player12");
			playerSheet = new SpriteSheet(spriteBatch, playerTex, 2, 2);
			player = new Player(playerSheet, 0);

			// Prepare level loading
			loader = new LevelLoader(spriteBatch, Content);
			LoadLevel();

			// Prepare inputs
			inputs = new Command();
			inputs.Map(Keys.Left, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.LEFT));
			inputs.Map(Keys.Right, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.RIGHT));
			inputs.Map(Keys.Up, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.UP));
			inputs.Map(Keys.Down, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.DOWN));

			inputs.Map(Keys.Space, Command.Event.JUST_DOWN, LoadLevel);
			inputs.Map(Keys.PageUp, Command.Event.JUST_DOWN, LoadNextLevel);
			inputs.Map(Keys.PageDown, Command.Event.JUST_DOWN, LoadPreviousLevel);
		}

		void MovePlayer(Sprite.Direction direction)
		{
			player.Move(direction);
			playingLevel.UpdateStep();
		}

		void LoadNextLevel()
		{
			currentLevel = (currentLevel + 1) % LEVEL_COUNT;
			LoadLevel();
		}

		void LoadPreviousLevel()
		{
			currentLevel = MathHelper.Max(0, currentLevel - 1);
			LoadLevel();
		}

		void LoadLevel()
		{
			playingLevel = loader.Load(currentLevel);
			playingLevel.OnDone += LoadNextLevel;
			player.Reset();
			player.MoveTo(playingLevel.StartingPlayerPosition);
			player.CurrentLevel = playingLevel;
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			inputs.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			playingLevel.Draw();
			player.Draw();

			base.Draw(gameTime);
		}
	}
}
