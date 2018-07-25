using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace TileTactics {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Main : Game {
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Camera2D camera;
		private RenderTarget2D rend;
		private Map map;
		private InputHandler inputHandler = new InputHandler();

		public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();


		public const float Height = 1080.0f;
		public const float Width = 1920.0f;

		public Main() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height/2;
			graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width/2;
			//graphics.IsFullScreen = true;
			graphics.SynchronizeWithVerticalRetrace = true;
			graphics.ApplyChanges();

			Window.AllowUserResizing = true;
			IsMouseVisible = true;
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			camera = new Camera2D(graphics.GraphicsDevice);
			camera.Zoom = 0.5f/(GraphicsDevice.DisplayMode.Height/Height);

			Textures.Add("Avatar", Content.Load<Texture2D>("TempAva"));
			Textures.Add("APBanner", Content.Load<Texture2D>("APBanner"));
			Textures.Add("Heart", Content.Load<Texture2D>("Heart"));
			Textures.Add("Tile", Content.Load<Texture2D>("tile"));

			map = new Map();

			rend = new RenderTarget2D(GraphicsDevice, Convert.ToInt32(Width), Convert.ToInt32(Height));
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() {
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			inputHandler.update();
			handleInput(gameTime);

			base.Update(gameTime);
		}

		private const float cameraSpeed = 0.5f;
		private void handleInput(GameTime dt) {
			if (inputHandler.isKeyPressed(Keys.A)) {
				if(camera.Position.X > 0) {
					camera.Position -= new Vector2(cameraSpeed*dt.ElapsedGameTime.Milliseconds, 0);
					if (camera.Position.X < 0)
						camera.Position = new Vector2(0, camera.Position.Y);
				}
			}
			if (inputHandler.isKeyPressed(Keys.D)) {
				if (camera.Position.X <= 70*64-480) {
					camera.Position += new Vector2(cameraSpeed*dt.ElapsedGameTime.Milliseconds, 0);
					if (camera.Position.X > 70*64-480)
						camera.Position = new Vector2(70*64-480, camera.Position.Y);
				}
			}
			if (inputHandler.isKeyPressed(Keys.W)) {
				if (camera.Position.Y > 0) {
					camera.Position -= new Vector2(0, cameraSpeed*dt.ElapsedGameTime.Milliseconds);
					if (camera.Position.X < 0)
						camera.Position = new Vector2(camera.Position.X, 0);
				}
			}
			if (inputHandler.isKeyPressed(Keys.S)) {
				if (camera.Position.Y < 70*64-270) {
					camera.Position += new Vector2(0, cameraSpeed*dt.ElapsedGameTime.Milliseconds);
					if (camera.Position.X > 70*64-270)
						camera.Position = new Vector2(camera.Position.X, 70*64-270);
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SetRenderTarget(rend);
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
			map.draw(spriteBatch, this);

			spriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin();
			spriteBatch.Draw(rend, new Vector2(0));
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
