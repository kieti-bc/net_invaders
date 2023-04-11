using Raylib_CsLo;
using System.Numerics;

namespace Invaders_demo
{
	internal class Invaders
	{
		int window_width = 640;
		int window_height = 420;

		Player player;
		List<Bullet> bullets;
		public void Run()
		{
			Init();
			GameLoop();
		}

		void Init()
		{
			// TODO luo ikkuna ja pelaaja ym.
			Raylib.InitWindow(window_width, window_height, "Space Invaders Demo");
			Raylib.SetTargetFPS(30);

			Vector2 playerStart = new Vector2( window_width / 2 , window_height - 80);
			float playerSpeed = 120;
			int playerSize = 40;
			player = new Player(playerStart, playerSpeed, playerSize);

			bullets = new List<Bullet>();
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

			if (playerShoots)
			{
				// TODO create new bullet from player
				Bullet bullet = new Bullet(player.transform.position, 
					new Vector2(0, -1), 
					300, 20);
				bullets.Add(bullet);
			}

			foreach(Bullet bullet in bullets)
			{
				bullet.Update();
			}

			KeepInsideArea(player.transform, player.collision,
				0, 0, window_width, window_height);
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
		void KeepInsideArea(TransformComponent transform, CollisionComponent collision,
			int left, int top, int right, int bottom)
		{
			float newX = Math.Clamp(transform.position.X, left, right - collision.size.X);

			float newY = Math.Clamp(transform.position.Y, top, bottom - collision.size.Y);

			transform.position.X = newX;
			transform.position.Y = newY;
		}

		void Draw()
		{
			player.Draw();

			foreach(Bullet bullet in bullets)
			{
				bullet.Draw();
			}
		}

	}
}
