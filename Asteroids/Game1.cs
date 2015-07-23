#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion
namespace Asteroids
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;



		class ShipClass 
		{
			public Texture2D Texture;
			public Vector2 Position;
			public Vector2 Velocity;

			public float Acceleration;
			public float Rotation;
			public float RotationDelta;

			public Vector2 Size;
			public Vector2 MaxLimit;
			public Vector2 MinLimit;


		}

		ShipClass Ship;

		class AsteroidClass 
		{
			public Vector2 Position;
			public Vector2 Velocity;
			public float Rotation;
			public float RotationDelta;

			public Vector2 Size;

			public Vector2 MaxLimit;
			public Vector2 MinLimit;
		}

		public Texture2D AsteroidTexture;
		AsteroidClass Asteroid;

		Random RandNum;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here

			RandNum = new Random();

			Asteroid = new AsteroidClass();
			Asteroid.Position = new Vector2(RandNum.Next(graphics.PreferredBackBufferWidth),
				RandNum.Next(graphics.PreferredBackBufferHeight));
			Asteroid.Velocity = new Vector2(RandNum.Next(-3, 3), RandNum.Next(-3, 3));
			Asteroid.RotationDelta = RandNum.Next(-100, 100);

			int RandSize = RandNum.Next(32, 256);
			Asteroid.Size = new Vector2(RandSize, RandSize);

			Asteroid.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Asteroid.Size.X + 100),
				graphics.PreferredBackBufferHeight + (Asteroid.Size.Y + 100));
			Asteroid.MinLimit = new Vector2(0 - (Asteroid.Size.X = 100), 0 - (Asteroid.Size.Y - 100));

			Ship = new ShipClass();



			Ship.Position = new Vector2(graphics.PreferredBackBufferWidth / 2,
									   graphics.PreferredBackBufferHeight / 2);
			Ship.Velocity = new Vector2(0, 0);
			Ship.Acceleration = 0;
			Ship.Rotation = 0;
			Ship.RotationDelta = 0;

			Ship.Size = new Vector2(32.0f, 32.0f);
			Ship.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Ship.Size.X / 2),
				graphics.PreferredBackBufferHeight + (Ship.Size.Y / 2));


			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			Ship.Texture = Content.Load<Texture2D>("ship");
			AsteroidTexture = Content.Load<Texture2D>("asteroid");
			//TODO: use this.Content to load your game content here 
		}
		

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}
			// TODO: Add your update logic here

			Asteroid.Rotation += Asteroid.RotationDelta;
			Asteroid.Position += Asteroid.Velocity;

			if (Asteroid.Position.X > Asteroid.MaxLimit.X)
			{
				Asteroid.Velocity.X *= -1;
			}

			if (Asteroid.Position.X < Asteroid.MinLimit.X)
			{
				Asteroid.Velocity.X *= -1;
			}

			Ship.Acceleration = 0;
			Ship.RotationDelta = 0;
			//------------------
			// INFO: Arrow Keys 
			//------------------
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				Ship.Acceleration = -0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				Ship.Acceleration = 0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				Ship.RotationDelta = -0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				Ship.RotationDelta = 0.05f;
			}

			//----------------
			// INFO: WASD Keys
			//----------------

			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				Ship.Acceleration = -0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				Ship.Acceleration = 0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				Ship.RotationDelta = -0.05f;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Ship.RotationDelta = 0.05f;
			}

			Ship.Rotation += Ship.RotationDelta;

			Matrix PlayerRotationMatrix = Matrix.CreateRotationZ(Ship.Rotation);

			Ship.Velocity += Vector2.Transform(new Vector2(0, Ship.Acceleration), PlayerRotationMatrix);

			Ship.Position += Ship.Velocity;

			if (Ship.Position.X > Ship.MaxLimit.X)
			{
				Ship.Position.X = Ship.MinLimit.X;
			}
			else if (Ship.Position.X < Ship.MinLimit.X)
			{
				Ship.Position.X = Ship.MaxLimit.X;
			}

			if (Ship.Position.Y > Ship.MaxLimit.Y)
			{
				Ship.Position.Y = Ship.MinLimit.Y;
			}
			else if (Ship.Position.Y < Ship.MinLimit.Y)
			{
				Ship.Position.Y = Ship.MaxLimit.Y;
			}
						
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);
		
			//TODO: Add your drawing code here

			spriteBatch.Begin();

			spriteBatch.Draw(Ship.Texture,
				Ship.Position,
				null,
				Color.White,
				Ship.Rotation,
				new Vector2(Ship.Texture.Width / 2, Ship.Texture.Height / 2),
				new Vector2(Ship.Size.X / Ship.Texture.Width, Ship.Size.Y / Ship.Texture.Height),
				SpriteEffects.None,
				 0);
			spriteBatch.Draw(AsteroidTexture,
				Asteroid.Position,
				null,
				Color.White,
				Asteroid.Rotation,
				new Vector2(AsteroidTexture.Width / 2, AsteroidTexture.Height / 2),
				new Vector2(Asteroid.Size.X / AsteroidTexture.Width, Ship.Size.Y / AsteroidTexture.Height),
				SpriteEffects.None,
				0);

			spriteBatch.End();
            
			base.Draw (gameTime);
		}
	}
}

