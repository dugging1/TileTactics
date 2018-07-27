using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using TileTactics.Network;

namespace TileTactics {
	public enum GameState { MainMenu, Map }

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Main : Game {
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		public Camera2D camera;
		private RenderTarget2D rend;
		public Map map;
        public GUI gui;
		public InputHandler inputHandler = new InputHandler();
		private System.Windows.Forms.Form form;
		private bool wasMaximised;

		public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

		public const float Height = 1080.0f;
		public const float Width = 1920.0f;

		public bool isServer = false; //true = server; false = client
		public Server server;
		public Client client;

		public GameState gameState = GameState.MainMenu;

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
			graphics.SynchronizeWithVerticalRetrace = true;
			graphics.ApplyChanges();
			form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);


			//Window.AllowUserResizing = true;
			//Window.ClientSizeChanged += OnResize;
			IsMouseVisible = true;
			base.Initialize();
		}

		private float scale = 1;
		private void OnResize(object sender, EventArgs e) {
			scale = (float)Window.ClientBounds.Height/(float)GraphicsDevice.DisplayMode.Height;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			camera = new Camera2D(graphics.GraphicsDevice);
			camera.Zoom = 0.5f/(GraphicsDevice.DisplayMode.Height/Height);
			camera.Position = new Vector2(70*64/2);
			camera.MinimumZoom = 0.5f;
			camera.MaximumZoom = 1.2f;
			camera.Origin = new Vector2(0);

			if (isServer) {
				//TODO: Menu this
				//server = new Server("25.90.58.250", 25565, this);
			} else {
				//TODO: Menu this
				//client = new Client("25.90.58.250", 25565, this);
			}

			Textures.Add("Avatar", Content.Load<Texture2D>("Avatar"));
			Textures.Add("APBanner", Content.Load<Texture2D>("APBanner"));
			Textures.Add("Heart", Content.Load<Texture2D>("Heart"));
			Textures.Add("Tile", Content.Load<Texture2D>("tile"));
            Textures.Add("OffAvatar", Content.Load<Texture2D>("AvatarOff"));
            Textures.Add("UI", Content.Load<Texture2D>("UI"));
            Textures.Add("TileSelected", Content.Load<Texture2D>("tileselected"));
            Textures.Add("MMBackground", Content.Load<Texture2D>("MainMenu"));
            Textures.Add("MMAvatarSelecter", Content.Load<Texture2D>("AvatarSelecter"));
            Textures.Add("MMConnect", Content.Load<Texture2D>("ConnectOff"));
            Textures.Add("MMConnectHover", Content.Load<Texture2D>("ConnectHover"));
            Textures.Add("MMConnectDisabled", Content.Load<Texture2D>("ConnectDisabled"));
            Textures.Add("MMTextOff", Content.Load<Texture2D>("TextOff"));
            Textures.Add("MMTextOn", Content.Load<Texture2D>("TextOn"));

            Fonts.Add("Basic", Content.Load<SpriteFont>("SF"));
            Fonts.Add("UIFont", Content.Load<SpriteFont>("UIFont"));
            Fonts.Add("APFont", Content.Load<SpriteFont>("APText"));

            map = new Map();
            gui = new GUI(this);

			rend = new RenderTarget2D(GraphicsDevice, Convert.ToInt32(Width), Convert.ToInt32(Height));

            for (int i = 0; i < 20; i++)
            {
                map.AddRandomUnit("Test");
            }
            map.dailyAPBoost();
            map.setData(1, 2, new Unit("Test2"));
        }

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if(form.WindowState == System.Windows.Forms.FormWindowState.Maximized && !wasMaximised) {
				wasMaximised = true;
				OnResize(this, new EventArgs());
			}

			inputHandler.update();
			handleInput(gameTime);
			gui.update();

			if (isServer) { //TODO: uncomment when init is finished
				//server.update();
			} else {
				//client.update();
			}

			base.Update(gameTime);
		}

		private const float cameraSpeed = 0.5f;
		private void handleInput(GameTime dt) {
					#region Movement
			if (inputHandler.isKeyPressed(Keys.A)) {
				if(camera.BoundingRectangle.Left > 0) {
					camera.Position -= new Vector2(cameraSpeed*dt.ElapsedGameTime.Milliseconds, 0);
					if (camera.BoundingRectangle.Left < 0)
						camera.Position = new Vector2(0, camera.Position.Y);
				}
			}
			if (inputHandler.isKeyPressed(Keys.D)) {
				if (camera.Position.X+960/camera.Zoom <= 70*64) {
					camera.Position += new Vector2(cameraSpeed*dt.ElapsedGameTime.Milliseconds, 0);
					if (camera.Position.X+960/camera.Zoom > 70*64)
						camera.Position = new Vector2(70*64-960/camera.Zoom, camera.Position.Y);
				}
			}
			if (inputHandler.isKeyPressed(Keys.W)) {
				if (camera.BoundingRectangle.Top > 0) {
					camera.Position -= new Vector2(0, cameraSpeed*dt.ElapsedGameTime.Milliseconds);
					if (camera.BoundingRectangle.Top < 0)
						camera.Position = new Vector2(camera.Position.X, 0);
				}
			}
			if (inputHandler.isKeyPressed(Keys.S)) {
				if (camera.Position.Y+540/camera.Zoom <= 70*64) {
					camera.Position += new Vector2(0, cameraSpeed*dt.ElapsedGameTime.Milliseconds);
					if (camera.Position.Y+540/camera.Zoom > 70*64)
						camera.Position = new Vector2(camera.Position.X, 70*64-540/camera.Zoom);
				}
			}
			if (camera.Zoom + (inputHandler.deltaMWheelPos/1000.0f)/(GraphicsDevice.DisplayMode.Height/Height) < camera.MinimumZoom) {
				camera.Zoom = camera.MinimumZoom;
			}else if(camera.Zoom + (inputHandler.deltaMWheelPos/1000.0f)/(GraphicsDevice.DisplayMode.Height/Height) > camera.MaximumZoom) {
				camera.Zoom = camera.MaximumZoom;
			}else
				camera.Zoom += (inputHandler.deltaMWheelPos/1000.0f)/(GraphicsDevice.DisplayMode.Height/Height);
            #endregion

            #region SelectedTile
            if (gui.MainMenuOpen != true) {
                if (inputHandler.isMBtnPressed(0)) {
                    Vector2 mPos = inputHandler.MousePos;
                    Vector2 temp = camera.ScreenToWorld(mPos)/64;
                    map.TileSelected = new Vector2((int)Math.Floor((float)temp.X), (int)Math.Floor((float)temp.Y));
                    if (map.TileSelected.X <= 0 || map.TileSelected.X > 70 || map.TileSelected.Y <= 0 || map.TileSelected.Y > 70)
                        map.TileSelected = new Vector2(-1);
                }
            }
					#endregion
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
            gui.draw(spriteBatch, this);


            spriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin( transformMatrix: Matrix2D.CreateScale(1.0f/scale));
			spriteBatch.Draw(rend, new Vector2(0));
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
