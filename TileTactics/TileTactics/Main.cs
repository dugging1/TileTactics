﻿using Microsoft.Xna.Framework;
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
			camera.Zoom = 1.0f/(GraphicsDevice.DisplayMode.Height/Height);

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

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			GraphicsDevice.SetRenderTarget(rend);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
			map.draw(spriteBatch, this);

			spriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin();
			spriteBatch.Draw(rend, new Vector2(0), scale: new Vector2(1.0f/camera.Zoom));
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}