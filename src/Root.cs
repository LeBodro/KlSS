using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
	class Root : Game
	{
		public const int SCALE = 4;
		public const int SIZE_LENGTH = SCALE * GridPosition.CELL_SIZE * GridPosition.GRID_SIZE;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		AudioLibrary sounds;
		Texture2D atlasTex;
		SpriteSheet atlas;
		Player player;
		Command inputs;
		LevelLoader loader;
		Level playingLevel;
		LevelEditor editor;
		int currentLevel = 0;
		bool isInLevelEditMode = false;
		bool isTransitionning = false;

		public Root()
		{
			graphics = new GraphicsDeviceManager(this);

			graphics.PreferredBackBufferWidth = SIZE_LENGTH;
			graphics.PreferredBackBufferHeight = SIZE_LENGTH;
			graphics.PreferMultiSampling = false;

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
			atlasTex = Content.Load<Texture2D>("atlas" + GridPosition.CELL_SIZE);
			atlas = new SpriteSheet(spriteBatch, atlasTex, 3, 4);
			player = new Player(atlas, 10);

			Texture2D editorTex = Content.Load<Texture2D>("editorAtlas");
			SpriteSheet editorAtlas = new SpriteSheet(spriteBatch, editorTex, 4, 4);

			// Prepare level loading
			loader = new LevelLoader(atlas, Content);
			editor = new LevelEditor(atlas, editorAtlas);
			editor.Player = player;
			LoadLevel();

			// Player inputs
			inputs = new Command();
			inputs.Map(Keys.Left, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.LEFT));
			inputs.Map(Keys.Right, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.RIGHT));
			inputs.Map(Keys.Up, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.UP));
			inputs.Map(Keys.Down, Command.Event.JUST_DOWN, () => MovePlayer(Sprite.Direction.DOWN));

			// System inputs
			inputs.Map(Keys.Space, Command.Event.JUST_DOWN, LoadLevel);
			inputs.Map(Keys.PageUp, Command.Event.JUST_DOWN, LoadNextLevel);
			inputs.Map(Keys.PageDown, Command.Event.JUST_DOWN, LoadPreviousLevel);
			inputs.Map(Keys.Tab, Command.Event.JUST_DOWN, ToggleLevelEditor);
		}

		void MovePlayer(Sprite.Direction direction)
		{
			player.Move(direction);
			playingLevel.UpdateStep();
		}

		void ProgressJob()
		{
			System.Threading.Thread.Sleep(1000);
			LoadNextLevel();
			isTransitionning = false;
		}

		void ProgressToNextLevel()
		{
			isTransitionning = true;
			new System.Threading.Thread(ProgressJob).Start();
		}

		void LoadNextLevel()
		{
			if (isInLevelEditMode)
				currentLevel = (currentLevel + 1) % (LevelLoader.LevelCount + 1);
			else
				currentLevel = (currentLevel + 1) % LevelLoader.LevelCount;
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
			if (isInLevelEditMode)
				playingLevel.OnDone += LoadLevel;
			else
				playingLevel.OnDone += ProgressToNextLevel;
			player.Reset();
			player.MoveTo(playingLevel.StartingPlayerPosition);
			player.CurrentLevel = playingLevel;
			editor.Target = playingLevel;
			editor.LevelId = currentLevel;
		}

		void ToggleLevelEditor()
		{
			isInLevelEditMode = !isInLevelEditMode;
			if (isInLevelEditMode)
			{
				playingLevel.OnDone -= ProgressToNextLevel;
				playingLevel.OnDone += LoadLevel;
			}
			else
			{
				playingLevel.OnDone -= LoadLevel;
				playingLevel.OnDone += ProgressToNextLevel;
			}
			graphics.PreferredBackBufferWidth += (isInLevelEditMode ? 1 : -1) * GridPosition.CELL_SIZE * 3 * SCALE;
			graphics.ApplyChanges();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// No game inputs taken during transitions
			if (!isTransitionning)
			{
				inputs.Update();

				if (isInLevelEditMode)
					editor.Update();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			playingLevel.Draw();
			player.Draw();

			if (isInLevelEditMode)
				editor.Draw();

			base.Draw(gameTime);
		}
	}
}
