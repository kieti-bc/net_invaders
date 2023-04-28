using Raylib_CsLo;
using System.Numerics;

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
		int window_height = 720;

		Player player;
		List<Bullet> bullets;
		List<Enemy> enemies;

		double enemyShootInterval;
		double lastEnemyShootTime;
		float enemyBulletSpeed;
		float enemyBulletSize;
		float enemySpeed;
		float enemySpeedDown;
		float enemyMaxYLine;

		Texture playerImage;

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
			playerImage = Raylib.LoadTexture("data/images/playerShip2_green.png");

			ResetGame();
		}

		/// <summary>
		/// Resets everything back to starting position
		/// </summary>
		void ResetGame()
		{
			float playerSpeed = 120;
			int playerSize = 40;
			Vector2 playerStart = new Vector2(window_width / 2, window_height - playerSize * 2);

			player = new Player(playerStart, new Vector2(0, 0), playerSpeed, playerSize, playerImage);

			bullets = new List<Bullet>();

			enemies = new List<Enemy>();

			enemyShootInterval = 1.0f;
			lastEnemyShootTime = 5.0f; // Delays the first enemy shot
			enemyBulletSpeed = 60;
			enemyBulletSize = 10;
			enemySpeed = playerSpeed;
			enemySpeedDown = 10;
			enemyMaxYLine = window_height - playerSize * 4;

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

					Enemy enemy = new Enemy(enemyStart, new Vector2(1, 0), enemySpeed, playerSize, enemyScore);

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
						ScoreUpdate();	// Wait for enter, restart on input
						Raylib.BeginDrawing();
						Raylib.ClearBackground(Raylib.DARKGRAY);
						ScoreDraw(); // Draw text you win, with {score} points
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
			bool canGoDown = true; // Enemies can descent by default
			foreach (Enemy enemy in enemies)
			{
				if (enemy.active)
				{
					enemy.Update();

					bool enemyIn = IsInsideArea(enemy.transform, enemy.collision, 0, 0, window_width, window_height);

					if (enemyIn == false)
					{
						changeFormationDirection = true;
					}
					if (enemy.transform.position.Y > enemyMaxYLine)
					{
						canGoDown = false;
					}
				}
			}

			if (changeFormationDirection)
			{
				foreach (Enemy enemy in enemies)
				{
					enemy.transform.direction.X *= -1.0f;
					if (canGoDown)
					{
						enemy.transform.position.Y += enemySpeedDown;
					}
				}
			}

			// Check enemy shooting: has interval passed since last shoot
			double timeNow = Raylib.GetTime();
			if (timeNow - lastEnemyShootTime >= enemyShootInterval)
			{
				// Can shoot!
				Enemy shooter = FindBestEnemyShooter();
				if (shooter != null)
				{
					// Create enemy bullet below shooter
					// Travelling down the screen
					CreateBullet(shooter.transform.position 
						+ new Vector2(0, shooter.collision.size.Y),
						new Vector2(0, 1), enemyBulletSpeed, (int)enemyBulletSize);

					lastEnemyShootTime = timeNow;
				}
			}
		}

		Enemy FindBestEnemyShooter()
		{
			// is active?
			// closest to player on Y axis
			// closest to player on X axis, under treshold?

			Enemy best = null;
			// Start from worst possible
			float bestY = 0.0f;
			float bestXDifference = window_width;

			// start from last enemy
			for(int i = enemies.Count-1; i >= 0; i--)
			{
				Enemy test = enemies[i];
				if (test.active)
				{
					if(test.transform.position.Y >= bestY)
					{
						// Found better Y
						bestY = test.transform.position.Y;

						// Absolute value : itseisarvo
						float xDifference = Math.Abs(player.transform.position.X - test.transform.position.X);
						if (xDifference < bestXDifference && xDifference < 10)
						{
							bestXDifference = xDifference;
							best = test;
						}
					}
				}
			}

			return best;
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
			Rectangle playerRect = getRectangle(player.transform, player.collision);
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

					if (bullet.transform.direction.Y < 0)
					{
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
								state = GameState.ScoreScreen;
							}
							// Do not test the rest of bullets
							break;
						}
					}
					else
					{
						if (Raylib.CheckCollisionRecs(bulletRec, playerRect))
						{
							state = GameState.ScoreScreen;
						}
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

		bool IsInsideArea(TransformComponent transform, CollisionComponent collision,
			int left, int top, int right, int bottom)
		{
			float x = transform.position.X;
			float r = x + collision.size.X;

			float y = transform.position.Y;
			float b = y + collision.size.Y;

			if (x < left || y < top || r > right || b > bottom)
			{
				return false;
			}
			else
			{
				return true;
			}
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



		// Score screen
		void ScoreUpdate()
		{
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
			{
				ResetGame();
				state = GameState.Play;
			}
		}

		void ScoreDraw()
		{
			Raylib.DrawText($"Final score {scoreCounter}", 
				window_width /2 - 40, window_height/2 - 60, 20, Raylib.WHITE);

			Raylib.DrawText("Game over. Press Enter to play again", 
				window_width /2 - 40, window_height/2, 20, Raylib.WHITE);
		}
	}





}
