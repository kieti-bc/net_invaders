using Raylib_CsLo;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Invaders_demo
{
	internal class Invaders
	{
		enum GameState
		{
			Play,
			ScoreScreen
		}
		GameState state;

		int window_width = 640;
		int window_height = 420;

		Player player;
		List<Bullet> bullets;
		List<Enemy> enemies;

		// how much score player has
		int scoreCounter = 0;

		public void Run()
		{
			Init();
			GameLoop();
		}

		void Init()
		{
			Raylib.InitWindow(window_width, window_height, "Space Invaders Demo");
			Raylib.SetTargetFPS(30);

			state = GameState.Play;

			// Player init
			Texture playerImage = Raylib.LoadTexture("data/images/playerShip2_green.png");


			float playerSpeed = 120;
			int playerSize = 40;
			Vector2 playerStart = new Vector2(window_width / 2, window_height - playerSize * 2);

			player = new Player(playerStart, new Vector2(0, 0), playerSpeed, playerSize, playerImage);

			bullets = new List<Bullet>();

			enemies = new List<Enemy>();

			/*  Formation is 
			 *  X X X X 
			 *  X X X X
			 */
			int rows = 4;
			int columns = 4;
			int startX = 0;
			int startY = playerSize;
			int currentX = startX;
			int currentY = startY;
			int enemyBetween = playerSize;

			int maxScore = 40;
			int minScore = 10;
			int currentScore = maxScore;

			for (int row = 0; row < rows; row++)
			{
				currentX = startX; // Reset at start of new row

				// Score decreases when goin down
				currentScore = maxScore - row * 10;
				if (currentScore < minScore)
				{
					currentScore = minScore;
				}
				for (int col = 0; col < columns; col++)
				{
					Vector2 enemyStart = new Vector2(currentX, currentY);
					int enemyScore = currentScore;

					Enemy enemy = new Enemy(enemyStart, new Vector2(1, 0), playerSpeed, playerSize, enemyScore);

					enemies.Add(enemy);

					currentX += playerSize + enemyBetween; // Horizontal space between enemies
				}
				currentY += playerSize + enemyBetween; // Vertical space between enemies
			}
		}

		void GameLoop()
		{
			while (Raylib.WindowShouldClose() == false)
			{
				switch (state)
				{
					case GameState.Play:
						// UPDATE
						Update();
						// DRAW
						Raylib.BeginDrawing();
						Raylib.ClearBackground(Raylib.YELLOW);
						Draw();
						Raylib.EndDrawing();
						break;

					case GameState.ScoreScreen:
						// ScoreUpdate();	// Wait for any input, restart on input
						Raylib.BeginDrawing();
						Raylib.ClearBackground(Raylib.YELLOW);
						// ScoreDraw(); // Draw text you win, with {score} points
						Raylib.EndDrawing();

						break;
				}
			}
		}
		void Update()
		{
			UpdatePlayer();
			UpdateEnemies();
			UpdateBullets();
			CheckCollisions(); // Between enemies and bullets
		}
		void UpdatePlayer()
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
		}
		void UpdateEnemies()
		{
			bool changeFormationDirection = false;
			foreach (Enemy enemy in enemies)
			{
				if (enemy.active)
				{
					enemy.Update();
					bool enemyOut = KeepInsideArea(enemy.transform, enemy.collision, 0, 0, window_width, window_height);
					if (enemyOut)
					{
						changeFormationDirection = true;
					}
				}
			}
			if (changeFormationDirection)
			{
				foreach (Enemy enemy in enemies)
				{
					enemy.transform.direction.X *= -1.0f;
				}
			}
		}
		void UpdateBullets()
		{
			foreach (Bullet bullet in bullets)
			{
				if (bullet.active)
				{
					bullet.Update();

					bool isOutside = KeepInsideArea(bullet.transform, bullet.collision, 0, 0, window_width, window_height);

					if (isOutside)
					{
						bullet.active = false;
					}
				}
			}
		}

		/// <summary>
		/// Test each enemy against each bullet for collision.
		/// On collision set both enemy and bullet as inactive.
		/// </summary>
		void CheckCollisions()
		{
			foreach (Enemy enemy in enemies)
			{
				if (enemy.active == false)
				{
					continue;
				}
				Rectangle enemyRec = getRectangle(enemy.transform, enemy.collision);

				foreach (Bullet bullet in bullets)
				{
					if (bullet.active == false)
					{
						continue;
					}
					Rectangle bulletRec = getRectangle(bullet.transform, bullet.collision);

					if (Raylib.CheckCollisionRecs(bulletRec, enemyRec))
					{
						// Enemy hit!
						Console.WriteLine($"Enemy Hit! Got {enemy.scoreValue} points!");
						scoreCounter += enemy.scoreValue;
						enemy.active = false;
						bullet.active = false;

						int enemiesLeft = CountAliveEnemies();
						if (enemiesLeft == 0)
						{
							// Win game
						}
						// Do not test the rest of bullets
						break;
					}
				}
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
				if (bullet.active == false)
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
				if (bullet.active)
				{
					bullet.Draw();
				}
			}

			foreach (Enemy enemy in enemies)
			{
				if (enemy.active)
				{
					enemy.Draw();
				}
			}

			// Draw score
			Raylib.DrawText(scoreCounter.ToString(), 10, 10, 16, Raylib.BLACK);
		}

		int CountAliveEnemies()
		{
			int alive = 0;
			foreach(Enemy enemy in enemies)
			{
				if(enemy.active)
				{
					alive++;
				}
			}
			return alive;
		}
	}
}
