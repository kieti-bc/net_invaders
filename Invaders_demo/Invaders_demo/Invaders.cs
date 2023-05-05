using Raylib_CsLo;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Invaders_demo
{
	internal class Invaders
	{
		enum GameState
		{
			Start,
			Play,
			ScoreScreen
		}
		GameState state;

		int window_width = 640;
		int window_height = 720;

		Player player;
		List<Bullet> bullets;
		List<Enemy> enemies;

		List<Vector2> startScreenStars;

		float playerBulletSpeed;

		int bulletSize;

		double enemyShootInterval;
		double lastEnemyShootTime;
		float enemyBulletSpeed;
		float enemySpeed;
		float enemySpeedDown;
		float enemyMaxYLine;

		Texture playerImage;
		List<Texture> enemyImages;
		Texture bulletImage;

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

			state = GameState.Start;

			// Player init
			playerImage = Raylib.LoadTexture("data/images/playerShip2_green.png");

			// Different enemy images for different rows
			enemyImages = new List<Texture>(4);
			enemyImages.Add(Raylib.LoadTexture("data/images/enemyBlack1.png"));
			enemyImages.Add(Raylib.LoadTexture("data/images/enemyBlue2.png"));
			enemyImages.Add(Raylib.LoadTexture("data/images/enemyGreen3.png"));
			enemyImages.Add(Raylib.LoadTexture("data/images/enemyRed4.png"));

			bulletImage = Raylib.LoadTexture("data/images/laserGreen14.png");

			Random random = new Random();

			startScreenStars = new List<Vector2>(20);
			for(int i = 0; i < startScreenStars.Capacity; i++)
			{
				startScreenStars.Add(new Vector2(random.Next(0, window_width), random.Next(-window_height, -1)));
			}
		}

		/// <summary>
		/// Resets everything back to starting position
		/// </summary>
		void ResetGame()
		{
			float playerSpeed = 220;
			int playerSize = 40;
			Vector2 playerStart = new Vector2(window_width / 2, window_height - playerSize * 2);

			player = new Player(playerStart, new Vector2(0, 0), playerSpeed, playerSize, playerImage);
			playerBulletSpeed = 320;

			bullets = new List<Bullet>();
			bulletSize = 16;

			enemies = new List<Enemy>();

			enemyShootInterval = 1.0f;
			lastEnemyShootTime = 5.0f; // Delays the first enemy shot
			enemyBulletSpeed = 60;
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

			int enemySize = playerSize;

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

					Enemy enemy = new Enemy(enemyStart, new Vector2(1, 0), enemySpeed, enemySize, enemyImages[row], enemyScore);

					enemies.Add(enemy);

					currentX += enemySize + enemyBetween; // Horizontal space between enemies
				}
				currentY += enemySize + enemyBetween; // Vertical space between enemies
			}

		}

		void GameLoop()
		{
			while (Raylib.WindowShouldClose() == false)
			{
				switch (state)
				{
					case GameState.Start:
						StartUpdate();
						Raylib.BeginDrawing();
						Raylib.ClearBackground(Raylib.BLACK);
						StartDraw();
						Raylib.EndDrawing();

						break;
					case GameState.Play:
						// UPDATE
						Update();
						// DRAW
						Raylib.BeginDrawing();
						Raylib.ClearBackground(Raylib.DARKBLUE);
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
				// Create bullet on center of player
				// Top left corner + half the size - half the bullet's size
				float x = player.transform.position.X + player.collision.size.X / 2 - bulletSize / 2;
				// just above the player;
				float y = player.transform.position.Y - bulletSize;
				Vector2 bPos = new Vector2(x, y);

				CreateBullet(bPos,
					new Vector2(0, -1),
					playerBulletSpeed, bulletSize);

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
					// Create bullet on center of enemy

					// Top left corner + half the size - half the bullet's size
					float x = shooter.transform.position.X + shooter.collision.size.X / 2 - bulletSize / 2;
					// Below shooter
					float y = shooter.transform.position.Y + shooter.collision.size.Y;
					Vector2 bPos = new Vector2(x, y);

					CreateBullet(bPos, new Vector2(0, 1), enemyBulletSpeed, (int)bulletSize);

					lastEnemyShootTime = timeNow;
				}
			}
		}

		/// <summary>
		/// Finds the enemy who is in the best position to shoot at the player
		/// </summary>
		/// <returns>The enemy who should shoot or null if no enemy is in good position</returns>
		Enemy FindBestEnemyShooter()
		{
			/* Pick an enemy who:
			 *  is active
			 *  is closest to player on Y axis
			 *  is within treshold to player on X axis.
			 *  
			 *  In a case where the first row has been partially
			 *  destroyed, the function should select from the rows
			 *  above if they have a good enemy
			 *   
			 *   in this situation the enemy O should be selected:
			 *   X = enemy
			 *   P = player
			 *   
			 *   X X X X
			 *   X O X X
			 *         X
			 *         
			 *     P
			 */

			Enemy best = null;

			// Start from worst possible values
			float bestY = 0.0f;
			float bestXDifference = window_width;

			// start from last enemy, since lowest row is last in the enemies list
			for(int i = enemies.Count-1; i >= 0; i--)
			{
				Enemy candidate = enemies[i];
				if (candidate.active)
				{
					// The enemy must be same or below the current best Y or not a valid shooter
					// has been found yet
					if (candidate.transform.position.Y >= bestY || best == null)
					{
						// Found better Y
						bestY = candidate.transform.position.Y;

						// Absolute value : itseisarvo
						float xDifference = Math.Abs(player.transform.position.X - candidate.transform.position.X);
						if (xDifference < bestXDifference && xDifference < bulletSize)
						{
							bestXDifference = xDifference;
							best = candidate;
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
							player.active = false;
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
				bullets.Add(new Bullet(pos, dir, speed, size, bulletImage, Raylib.RED));
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
			Raylib.DrawText($"Score: {scoreCounter}", 10, 10, 20, Raylib.WHITE);
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

		void DrawTextCentered(string text, int y, int fontSize, Color color)
		{
			int sw = Raylib.MeasureText(text, fontSize);

			Raylib.DrawText(text, window_width /2 - sw / 2
				, y, fontSize, color);
		}

		void StartUpdate()
		{
			
			for(int i = 0; i < startScreenStars.Count; i++)
			{
				Vector2 s = startScreenStars[i];
				s.Y = s.Y + 40 * Raylib.GetFrameTime();
				s.Y = s.Y % window_height;
				startScreenStars[i] = new Vector2(s.X, s.Y);
			}
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
			{
				ResetGame();
				state = GameState.Play;
			}
		}

		void StartDraw()
		{

			Raylib.DrawCircleGradient(window_width / 2, window_height / 2 -100, window_height * 3.0f, Raylib.BLACK, Raylib.BLUE);

			// Draw stars
			for(int i = 0; i < startScreenStars.Count; i++)
			{
				Raylib.DrawCircleGradient((int)startScreenStars[i].X, (int)startScreenStars[i].Y, 4.0f, Raylib.WHITE, Raylib.BLACK);
			}

			Color[] colors = { Raylib.LIGHTGRAY, Raylib.GRAY, Raylib.DARKGRAY, Raylib.GREEN };
			string gameName1 = "SPACE";
			string gameName2 = "INVADERS";
			int fontSize = 60;
			for (int i = 0; i < colors.Length; i++)
			{
				double change = Math.Sin(Raylib.GetTime() + i) * 4.0;
				double y = 120 + change * 14.0;

				DrawTextCentered(gameName1, (int)(y), fontSize, colors[i]);
				DrawTextCentered(gameName2, (int)(y + 60), fontSize, colors[i]);
			}

			string press = "Press ENTER";
			DrawTextCentered(press, window_height/2, 30, Raylib.YELLOW);
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
			// Center both lines of text usin Raylib.MeasureText

			string scoreText = $"Final score {scoreCounter}";


			string instructionText = "Game over. Press Enter to play again";
			if (player.active == true)
			{
				instructionText = "You Won! Press Enter to play again";
			}

			int fontSize = 20;
			int sw = Raylib.MeasureText(scoreText, fontSize);
			int iw = Raylib.MeasureText(instructionText, fontSize);

			Raylib.DrawText(scoreText, window_width /2 - sw / 2
				, window_height/2 - 60, fontSize, Raylib.WHITE);

			Raylib.DrawText(instructionText, window_width /2 - iw / 2, 
				window_height/2, fontSize, Raylib.WHITE);
		}
	}
}
