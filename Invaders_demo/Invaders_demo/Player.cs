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

		public Player(Vector2 startPos, float speed, int size)
		{
			transform = new TransformComponent(startPos, new Vector2(0,0), speed);
			collision = new CollisionComponent(new Vector2(size, size));

			lastShootTime = -shootInterval;
		}


		/// <summary>
		/// Updates player position, listens to keyboard 
		/// </summary>
		/// <returns>True when player wants to shoot, false normally</returns>
		public bool Update()
		{
			float deltaTime = Raylib.GetFrameTime();
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
			{
				transform.position.X -= transform.speed * deltaTime;
			}
			else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
			{
				transform.position.X += transform.speed * deltaTime;
			}

			if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
			{
				// Player shoots
				double timeNow = Raylib.GetTime();
				double timeSinceLastShot = timeNow - lastShootTime;
				if (timeSinceLastShot >= shootInterval)
				{
					Console.WriteLine("Player shoots!");
					lastShootTime = timeNow;
					return true;
				}
			}
			return false;

		}

		public void Draw()
		{
			Raylib.DrawRectangleV(transform.position, collision.size, Raylib.SKYBLUE);
		}

	}
}
