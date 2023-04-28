using System.Numerics;
using Raylib_CsLo;

namespace Invaders_demo
{
	internal class Player
	{
		// private set: Anyone can access, but no one can replace
		public TransformComponent transform { get; private set; }
		public CollisionComponent collision;
		SpriteRendererComponent spriteRenderer;

		// Bullet timing
		double shootInterval = 0.3; // In seconds
		double lastShootTime;

		public bool active;

		public Player(Vector2 startPos, Vector2 direction, float speed, int size, Texture image)
		{
			transform = new TransformComponent(startPos, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
			spriteRenderer = new SpriteRendererComponent(image, Raylib.SKYBLUE, transform, collision);

			lastShootTime = -shootInterval;
			active = true;
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
			spriteRenderer.Draw();
		}

	}
}
