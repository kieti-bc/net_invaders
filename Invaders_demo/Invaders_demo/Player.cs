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
		double shootInterval = 0.03; // In seconds
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
			bool inputRight = false;
			bool inputLeft = false;

			// INPUT

			// KEYBOARD CONTROLS
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
			{
				inputLeft = true;
			}
			else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
			{
				inputRight = true;
			}
			// MOUSE CONTROLS
			Vector2 mousePos = Raylib.GetMousePosition();
			Vector2 center = transform.position + collision.size / 2;

			/*
			 *    P---+    Mouse X
			 *    |   |
			 *    | c |
			 *    |   |
			 *    +---+
			 */

			if (center.X < mousePos.X)
			{

			}
			if (center.X > mousePos.X)
			{

			}
			/*

			// Follow mouse movement
			Vector2 toMouse = mousePos - center;
			float frameMove = transform.speed * deltaTime;
			if (toMouse.Length() > frameMove)
			{
				transform.direction = Vector2.Normalize(toMouse);
			}
			else
			{
				transform.direction = Vector2.Zero;
			}

			// Instant  movement ?!
			transform.position = mousePos;

			// Delta movement !!
			transform.position += Raylib.GetMouseDelta();

			Vector2 target = Raylib.GetMousePosition();
			if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
			{
				// Start moving to target
			}
			*/


			Vector2 moveDirection = new Vector2(0, 0);
			if (inputRight)
			{
				moveDirection.X += 1;
			}
			if (inputLeft)
			{
				moveDirection.X -= 1;
			}

			transform.direction = moveDirection;

			transform.position += transform.direction * transform.speed * deltaTime;


			bool shoot = false;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) ||
			Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
			{
				// Player shoots
				double timeNow = Raylib.GetTime();
				double timeSinceLastShot = timeNow - lastShootTime;
				if (timeSinceLastShot >= shootInterval)
				{
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
