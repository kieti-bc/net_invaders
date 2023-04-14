using Raylib_CsLo;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Invaders_demo
{
	internal class Invaders
	{
		int window_width = 640;
		int window_height = 420;

		Player player;
		List<Bullet> bullets;
		Enemy enemy;

		public void Run()
		{
			Init();
			GameLoop();
		}

		void Init()
		{
			Raylib.InitWindow(window_width, window_height, "Space Invaders Demo");
			Raylib.SetTargetFPS(30);

			float playerSpeed = 120;
			int playerSize = 40;
			Vector2 playerStart = new Vector2(window_width / 2, window_height - playerSize * 2);
			player = new Player(playerStart, new Vector2(0,0), playerSpeed, playerSize);

			bullets = new List<Bullet>();
			// enemies = new List<Enemy>();

			Vector2 enemyStart = new Vector2(window_width / 2, playerSize * 2);

			enemy = new Enemy(enemyStart, new Vector2(1, 0), playerSpeed, playerSize);
		}
		
		void GameLoop()
		{
			while (Raylib.WindowShouldClose() == false)
			{
				// UPDATE
				Update();
				// DRAW
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Raylib.YELLOW);
				Draw();
				Raylib.EndDrawing();
			}
		}
		void Update()
		{
			bool playerShoots = player.Update();
			KeepInsideArea(player.transform, player.collision,
				0, 0, window_width, window_height);
			if (playerShoots)
			{
				// Create bullet
				CreateBullet(player.transform.position, 
					new Vector2(0, -1), 
					300, 20);

				Console.WriteLine($"Bullet count: {bullets.Count}");
			}


			// TODO test collision against every enemy!

			Rectangle enemyRec = getRectangle(enemy.transform, enemy.collision);
			foreach(Bullet bullet in bullets)
			{
				if (bullet.isActive == false)
				{
					continue;
				}
				bullet.Update();

				bool isOutside = KeepInsideArea(bullet.transform, bullet.collision, 0, 0, window_width, window_height);

				if (isOutside)
				{
					bullet.isActive = false;
					continue;
				}

				if (enemy.active)
				{
					Rectangle bulletRec = getRectangle(bullet.transform, bullet.collision);
					if (Raylib.CheckCollisionRecs(bulletRec, enemyRec))
					{
						// Enemy hit!
						Console.WriteLine("Enemy Hit!");
						enemy.active = false;
					}
				}
			}

			enemy.Update();
			bool enemyOut = KeepInsideArea(enemy.transform, enemy.collision, 0, 0, window_width, window_height);
			if (enemyOut)
			{
				enemy.transform.direction.X *= -1.0f;
			}
		}

		/// <summary>
		/// Either reactivates existing bullet or creates a new one
		/// </summary>
		/// <param name="pos">Starting position</param>
		/// <param name="dir">Starting direction</param>
		/// <param name="speed">Speed in pixels</param>
		/// <param name="size">Size in pixels</param>
		void CreateBullet(Vector2 pos, Vector2 dir, float speed, int size)
		{
			bool found = false;
			foreach(Bullet bullet in bullets)
			{
				if (bullet.isActive == false)
				{
					// Reset this
					bullet.Reset(pos, dir, speed, size);
					found = true;
					break;
				}
			}
			// No inactive bullets found!
			if (found == false)
			{
				bullets.Add(new Bullet(pos, dir, speed, size));
			}
		}

		Rectangle getRectangle(TransformComponent t, CollisionComponent c)
		{
			Rectangle r = new Rectangle(t.position.X,
				t.position.Y, c.size.X, c.size.Y);
			return r;
		}

		/// <summary>
		/// Keeps the given transform inside the given area bounds
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="collision"></param>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <returns>True if went outside given area</returns>
		bool KeepInsideArea(TransformComponent transform, CollisionComponent collision,
			int left, int top, int right, int bottom)
		{
			float newX = Math.Clamp(transform.position.X, left, right - collision.size.X);
			float newY = Math.Clamp(transform.position.Y, top, bottom - collision.size.Y);

			bool xChange = newX != transform.position.X;
			bool yChange = newY != transform.position.Y;

			transform.position.X = newX;
			transform.position.Y = newY;

			return xChange || yChange;
		}

		void Draw()
		{
			player.Draw();

			foreach(Bullet bullet in bullets)
			{
				if (bullet.isActive)
				{
					bullet.Draw();
				}
			}

				enemy.Draw();
		}
	}
}
