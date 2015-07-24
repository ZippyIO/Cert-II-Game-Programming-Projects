#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#endregion
namespace Asteroids
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

		int PlayerLives = 5;

		class ShipClass
		{

			public Vector2 Position;
			public Vector2 Velocity;
			public float Acceleration;
			public Vector2 Size;
			public Vector2 MaxLimit;
			public Vector2 MinLimit;

			public float Rotation;
			public float RotationDelta;

		}



		public Texture2D ShipTexture;
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
		List<AsteroidClass> MyAsteroids;
		const int NUM_ASTEROIDS = 20;

		class MissileClass 
		{
			public Vector2 Positon;
			public Vector2 Velocity;
			public float Rotation;

			public Vector2 Size;

			public Vector2 MaxLimit;
			public Vector2 MinLimit;
		}
		public Texture2D MissileTexture;
		List<MissileClass> MyMissiles;

		TimeSpan LastShot = new TimeSpan(0,0,0,0);

		TimeSpan ShotCoolDown = new TimeSpan(0,0,0,0,100);


		Random RandNum;
				
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
			RandNum = new Random();

			MyAsteroids = new List<AsteroidClass>();
			MyMissiles = new List<MissileClass>();

			Ship = new ShipClass();

            // Function initiating
            InitalizeShip();
			InitalizeAsteroid();

            base.Initialize();	
        }

		private void InitalizeShip() 
		{

			Ship.Position = new Vector2(graphics.PreferredBackBufferWidth / 2,
				graphics.PreferredBackBufferHeight / 2);
			Ship.Velocity = new Vector2(0, 0);
			Ship.Acceleration = 0;
			Ship.Rotation = 0;
			Ship.RotationDelta = 0;
		}

		private void InitalizeAsteroid() 
		{

			for (int i = 0; i < NUM_ASTEROIDS; ++i)
			{

				AsteroidClass Asteroid = new AsteroidClass();


				Asteroid.Position = new Vector2(RandNum.Next(graphics.PreferredBackBufferWidth),
					RandNum.Next(graphics.PreferredBackBufferHeight));
				Asteroid.Velocity = new Vector2(RandNum.Next(-3, 3), RandNum.Next(-3, 3));
				Asteroid.RotationDelta = RandNum.Next(-100, 100);

				int RandSize = RandNum.Next(32, 256);
				Asteroid.Size = new Vector2(RandSize, RandSize);

				Asteroid.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Asteroid.Size.X + 100),
					graphics.PreferredBackBufferHeight + (Asteroid.Size.Y + 100));
				Asteroid.MinLimit = new Vector2(0 - (Asteroid.Size.X - 100), 0 - (Asteroid.Size.Y - 100));

				MyAsteroids.Add(Asteroid);
			}

		}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loading Sprties
			ShipTexture = Content.Load<Texture2D>("ship");
			AsteroidTexture = Content.Load<Texture2D>("asteroid");
			MissileTexture = Content.Load<Texture2D>("bullet");
        }

		 protected override void Update(GameTime gameTime)
        {
            // Function initiating
            CheckInput(gameTime);
			CreateMissiles(gameTime);
			UpdateShip(gameTime);
			UpdateAsteroid(gameTime);
			UpdateMissile(gameTime);
			CheckCollisons();

            base.Update(gameTime);
        }


		protected void CheckInput(GameTime gameTime) 
		{
			Ship.Acceleration = 0;
			Ship.RotationDelta = 0;

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}


			// ARROW KEYS \\
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


			// WASD KEYS \\
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
		}

		protected void CreateMissiles(GameTime gameTime) 
		{

            // Key space fires missiles if last shot time is greater than cool down time
			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				TimeSpan TimeSinceLastShot = gameTime.TotalGameTime - LastShot;

				if (TimeSinceLastShot > ShotCoolDown)
				{
					MissileClass Missile = new MissileClass();

					Missile.Positon = Ship.Position;

					Missile.Rotation = Ship.Rotation;

					Matrix MissileRotationMatrix = Matrix.CreateRotationZ(Missile.Rotation);
					Missile.Velocity = new Vector2(0, -10);
					Missile.Velocity = Vector2.Transform(Missile.Velocity, MissileRotationMatrix);
					Missile.Velocity = Missile.Velocity + Ship.Velocity;

					Missile.Size = new Vector2(16, 16);

					Missile.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + 500,
						graphics.PreferredBackBufferHeight + 500);
					Missile.MinLimit = new Vector2(-500, -500);

					MyMissiles.Add(Missile);

					LastShot = gameTime.TotalGameTime;
				}
			}

		}

		protected void UpdateShip(GameTime gameTime) 
		{
			Ship.Rotation += Ship.RotationDelta;

			Matrix PlayerRotationMatrix = Matrix.CreateRotationZ(Ship.Rotation);

			Ship.Velocity += Vector2.Transform(new Vector2(0, Ship.Acceleration), PlayerRotationMatrix);

			Ship.Position += Ship.Velocity;

			Ship.Size = new Vector2(32.0f, 32.0f);
			Ship.MaxLimit = new Vector2(graphics.PreferredBackBufferWidth + (Ship.Size.X / 2),
				graphics.PreferredBackBufferHeight + (Ship.Size.Y / 2));
			Ship.MinLimit = new Vector2(0 - (Ship.Size.X / 2), 0 - (Ship.Size.Y / 2));

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
		}

		protected void UpdateAsteroid(GameTime gameTime) 
		{
			foreach (AsteroidClass Asteroid in MyAsteroids)
			{
				Asteroid.Rotation += Asteroid.RotationDelta;
				Asteroid.Position += Asteroid.Velocity;

				if (Asteroid.Position.X > Asteroid.MaxLimit.X)
				{
					Asteroid.Velocity.X *= -1;
				}
				else if (Asteroid.Position.X < Asteroid.MinLimit.X)
				{
					Asteroid.Velocity.X *= -1;
				}

				if (Asteroid.Position.Y > Asteroid.MaxLimit.Y)
				{
					Asteroid.Velocity.Y *= -1;
				}
				else if (Asteroid.Position.X < Asteroid.MinLimit.Y)
				{
					Asteroid.Velocity.Y *= -1;
				}
			}

		}

		protected void UpdateMissile(GameTime gameTime) 
		{
			foreach (MissileClass Missile in MyMissiles)
			{
				Missile.Rotation += Ship.RotationDelta;
				Missile.Positon += Missile.Velocity;

				if (Missile.Positon.X > Missile.MaxLimit.X)
				{
					Missile.Velocity.X *= 1;
				}
				else if (Missile.Positon.X < Missile.MinLimit.X) 
				{
					Missile.Velocity.X *= 1;
				}
			}
		}

		private bool CircleCollisionCheck(Vector2 Object1Pos, float Object1Radius, Vector2 Object2Pos, float Object2Radius) 
		{
			float DistanceBetweenObjects = (Object1Pos - Object2Pos).Length();
			float SumOfRadii = Object1Radius + Object2Radius;

			if (DistanceBetweenObjects < SumOfRadii)
			{
				return true;
			}
			return false;
		}
		private void CheckCollisons() 
		{
			List<AsteroidClass> AsteroidDeathRow = new List<AsteroidClass>();
			List<MissileClass> MissileDeathRow = new List<MissileClass>();

			foreach (AsteroidClass Asteroid in MyAsteroids)
			{
				bool PlayerCollisionCheck = CircleCollisionCheck(Ship.Position, Ship.Size.X / 2,
					Asteroid.Position, Asteroid.Size.X / 2);
				if (PlayerCollisionCheck)
				{
                    // ShipDie TODO: Invulnerability, Sprite flashing
                    ShipDie();
					AsteroidDeathRow.Add(Asteroid);
				}

				foreach (MissileClass Missile in MyMissiles)
				{
					bool MissileCollisionCheck = CircleCollisionCheck(Missile.Positon, Missile.Size.X / 2,
						Asteroid.Position, Asteroid.Size.X / 2);
					if (MissileCollisionCheck)
					{
						MissileDeathRow.Add(Missile);
						AsteroidDeathRow.Add(Asteroid);
					}
				}
			}

			foreach(AsteroidClass Asteroid in AsteroidDeathRow)
			{
				MyAsteroids.Remove(Asteroid);
			}

			foreach(MissileClass Missile in MissileDeathRow)
			{
				MyMissiles.Remove(Missile);
			}
		}

		public void ShipDie()
		{
			Ship.Position = new Vector2(graphics.PreferredBackBufferWidth / 2,
				graphics.PreferredBackBufferHeight / 2);
			Ship.Velocity = new Vector2(0, 0);
			Ship.Acceleration = 0;
			Ship.Rotation = 0;
			Ship.RotationDelta = 0;
		}

        protected override void Draw(GameTime gameTime)
        {
			graphics.GraphicsDevice.Clear(Color.Black);
		
            

			spriteBatch.Begin();

            // Draw functions
			DrawShip(gameTime);
			DrawAsteroid(gameTime);
			DrawMissile(gameTime);
			DrawLives();

			spriteBatch.End();
            
            base.Draw(gameTime);
        }

		protected void DrawShip(GameTime gameTime) 
		{
			spriteBatch.Draw(ShipTexture,
				Ship.Position,
				null,
				Color.White,
				Ship.Rotation,
				new Vector2(ShipTexture.Width / 2, ShipTexture.Height / 2),
				new Vector2(Ship.Size.X / ShipTexture.Width, Ship.Size.Y / ShipTexture.Height),
				SpriteEffects.None,
				0);
		}

		protected void DrawAsteroid(GameTime gameTime) 
		{
			foreach (AsteroidClass Asteroid in MyAsteroids)
			{
				spriteBatch.Draw(AsteroidTexture,
					Asteroid.Position,
					null,
					Color.White,
					Asteroid.Rotation,
					new Vector2(AsteroidTexture.Width / 2, AsteroidTexture.Height / 2),
					new Vector2(Asteroid.Size.X / AsteroidTexture.Width, Asteroid.Size.Y / AsteroidTexture.Height),
					SpriteEffects.None,
					0);
			}
		}

		protected void DrawMissile(GameTime gameTime) 
		{
			foreach (MissileClass Missile in MyMissiles)
			{
				spriteBatch.Draw(MissileTexture,
					Missile.Positon,
					null,
					Color.White,
					Missile.Rotation,
					new Vector2(MissileTexture.Width / 2, MissileTexture.Height / 2),
					new Vector2(Missile.Size.X / MissileTexture.Width, Missile.Size.Y / MissileTexture.Height),
					SpriteEffects.None,
					0);
			}
		}

		private void DrawLives() 
		{
			for (int i = 0; i < PlayerLives; ++i)
			{
				spriteBatch.Draw(ShipTexture,
					new Vector2(Ship.Size.X * (i + 1), Ship.Size.Y),
					null,
					Color.White,
					0,
					new Vector2(ShipTexture.Width / 2, ShipTexture.Height / 2),
					new Vector2(Ship.Size.X / ShipTexture.Width,
						Ship.Size.Y / ShipTexture.Height),
					SpriteEffects.None,
					0);
			}
		}
    }
}