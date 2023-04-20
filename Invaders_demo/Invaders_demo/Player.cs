using System.Numerics;
using Raylib_CsLo;

namespace Invaders_demo
{
	internal class Player
	{
		// private set: Anyone can access, but no one can replace
		public TransformComponent transform { get; private set; }
		public CollisionComponent collision;

		// Bullet timing
		double shootInterval = 0.3; // In seconds
		double lastShootTime;

		// TODO spriteRenderer
		Texture image;

		public Player(Vector2 startPos, Vector2 direction, float speed, int size, Texture image)
		{
			transform = new TransformComponent(startPos, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));

			this.image = image;

			lastShootTime = -shootInterval;
		}


		/// <summary>
		/// Updates player position, listens to keyboard 
		/// </summary>
		/// <returns>True when player wants to shoot, false normally</returns>
		public bool Update()
		{
			float deltaTime = Raylib.GetFrameTime();


			// TODO Change this to modify direction instead of position directly
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
			{
				transform.position.X -= transform.speed * deltaTime;
			}
			else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
			{
				transform.position.X += transform.speed * deltaTime;
			}

			bool shoot = false;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
			{
				// Player shoots
				double timeNow = Raylib.GetTime();
				double timeSinceLastShot = timeNow - lastShootTime;
				if (timeSinceLastShot >= shootInterval)
				{
					Console.WriteLine("Player shoots!");
					lastShootTime = timeNow;
					shoot = true;
				}
			}
			return shoot;
		}

		public void Draw()
		{
			// Expecting size and image to be squares
			float scaleX = collision.size.X / image.width;
			float scaleY = collision.size.Y / image.height;
			float scale = Math.Min(scaleX, scaleY);

			Raylib.DrawTextureEx(image, transform.position, 0.0f, scale, Raylib.WHITE);
			//Raylib.DrawTextureV(image, transform.position, Raylib.WHITE);

			Raylib.DrawRectangleLines((int)transform.position.X, (int)transform.position.Y, (int)collision.size.X, (int)collision.size.Y, Raylib.SKYBLUE);
		}

	}
}
