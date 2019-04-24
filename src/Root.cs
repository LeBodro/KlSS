using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
	class Root : Game
	{
		public const double LEVEL_TRANSITION_DELAY = 1;
		public const int SCALE = 4;
		public const int SIZE_LENGTH = SCALE * GridPosition.CELL_SIZE * GridPosition.GRID_SIZE;
		const string PROGRESSION = "{0}/{1}";

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
		double delayToNextLevel;
		bool isTransitionning = false;
		TextSprite progression;
		TextSprite score;
		TextSprite instruction;
		int currentHighScore;
		InGameUI bottomMenu = new InGameUI();

		public Root()
		{
			graphics = new GraphicsDeviceManager(this);

			graphics.PreferredBackBufferWidth = SIZE_LENGTH;
			graphics.PreferredBackBufferHeight = SIZE_LENGTH + GridPosition.SCALED_CELL_SIZE;
			graphics.PreferMultiSampling = false;

			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Log.Clear();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			SaveGame.Load();
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load sounds
			sounds = AudioLibrary.Instance;
			sounds.Add("Destroy", Content.Load<SoundEffect>("destroy"));
			sounds.Add("Win", Content.Load<SoundEffect>("win"));
			sounds.Add("Step", Content.Load<SoundEffect>("step"));
			sounds.Add("Stick", Content.Load<SoundEffect>("stick"));

			// Prepare spritesheets
			atlasTex = Content.Load<Texture2D>("atlas" + GridPosition.CELL_SIZE);
			atlas = new SpriteSheet(spriteBatch, atlasTex, 3, 4);
			Texture2D editorTex = Content.Load<Texture2D>("editorAtlas");
			SpriteSheet systemAtlas = new SpriteSheet(spriteBatch, editorTex, 8, 8);

			// Prepare player
			player = new Player(atlas, 10);

			// Prepare level loading
			loader = new LevelLoader(atlas, Content);
			editor = new LevelEditor(atlas, systemAtlas);
			editor.Player = player;
			editor.OnNewLevel += delegate { currentLevel = SaveGame.LevelCount; LoadLevel(); };

			// Prepare UI
			instruction = new TextSprite(systemAtlas, TextSprite.Alignement.CENTER, 7, 14);
			progression = new TextSprite(systemAtlas, TextSprite.Alignement.RIGHT, 15, 16);
			score = new TextSprite(systemAtlas, TextSprite.Alignement.LEFT, 0, 16);
			bottomMenu.AddButton(new Sprite(systemAtlas, 10), delegate { currentLevel = 0; LoadLevel(); });
			bottomMenu.AddButton(new Sprite(systemAtlas, 13), LoadPreviousLevel);
			bottomMenu.AddButton(new Sprite(systemAtlas, 14), LoadNextLevel);
			bottomMenu.AddButton(new Sprite(systemAtlas, 11), delegate { currentLevel = SaveGame.LevelCount - 1; LoadLevel(); });

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

			// Load title screen level
			LoadLevel();
		}

		void MovePlayer(Sprite.Direction direction)
		{
			player.Move(direction);
			playingLevel.UpdateStep();
		}

		void EndTransition()
		{
			LoadNextLevel();
			isTransitionning = false;
		}

		void ProgressToNextLevel()
		{
			SaveGame.KeepHighScore(currentLevel, Score.Total);
			isTransitionning = true;
			delayToNextLevel = LEVEL_TRANSITION_DELAY;
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
			int levelId = SaveGame.GetLevelId(currentLevel);
			playingLevel = loader.Load(levelId);
			instruction.SetText(playingLevel.Instruction);
			if (isInLevelEditMode)
				playingLevel.OnDone += LoadLevel;
			else
				playingLevel.OnDone += ProgressToNextLevel;
			player.Reset();
			player.MoveTo(playingLevel.StartingPlayerPosition);
			player.CurrentLevel = playingLevel;
			editor.Target = playingLevel;
			editor.LevelId = levelId;
			progression.SetText(string.Format(PROGRESSION, currentLevel + 1, LevelLoader.LevelCount));
			currentHighScore = SaveGame.GetScore(currentLevel);
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

			double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
			// No game inputs taken during transitions
			if (!isTransitionning)
			{
				inputs.Update(deltaTime);
				bottomMenu.Update();
				if (currentHighScore > 0)
					score.SetText(string.Format(PROGRESSION, Score.Total, currentHighScore));
				else
					score.SetText(Score.Total);

				if (isInLevelEditMode)
					editor.Update();
			}
			else
			{
				delayToNextLevel -= deltaTime;
				if (delayToNextLevel <= 0)
					EndTransition();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			playingLevel.Draw();
			player.Draw();

			// UI
			instruction.Draw();
			progression.Draw();
			score.Draw();
			bottomMenu.Draw();

			if (isInLevelEditMode)
				editor.Draw();

			base.Draw(gameTime);
		}
	}
}
