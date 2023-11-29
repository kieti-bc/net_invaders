using Raylib_CsLo;
using System.Numerics;

namespace Invaders_demo
{
	internal class MainMenu
	{
		public event EventHandler StartButtonPressed;
		public event EventHandler SettingsButtonPressed;
		public event EventHandler QuitButtonPressed;

		List<Vector2> startScreenStars;
		public MainMenu()
		{
			Random random = new Random();
			int window_width = Raylib.GetScreenWidth();
			int window_height = Raylib.GetScreenHeight();
			startScreenStars = new List<Vector2>(20);
			for(int i = 0; i < startScreenStars.Capacity; i++)
			{
				startScreenStars.Add(new Vector2(random.Next(0, window_width), random.Next(-window_height, -1)));
			}

		}
		public void Update()
		{
			int window_height = Raylib.GetScreenHeight();
			for(int i = 0; i < startScreenStars.Count; i++)
			{
				Vector2 s = startScreenStars[i];
				s.Y = s.Y + 40 * Raylib.GetFrameTime();
				s.Y = s.Y % window_height;
				startScreenStars[i] = new Vector2(s.X, s.Y);
			}

		}
		void DrawTextCentered(string text, int y, int fontSize, Color color)
		{
			int window_width = Raylib.GetScreenWidth();
			int sw = Raylib.MeasureText(text, fontSize);

			Raylib.DrawText(text, window_width /2 - sw / 2
				, y, fontSize, color);
		}
		public void ShowMenu()
		{
			int window_width = Raylib.GetScreenWidth();
			int window_height = Raylib.GetScreenHeight();

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

			int button_width = 100;
			int button_height = 40;
			int center_x = window_width / 2 - button_width / 2;
			int center_y = window_height / 2 - button_height / 2;
			if (RayGui.GuiButton(new Rectangle(center_x, center_y
				, button_width, button_height), "Start Game"))
			{
				StartButtonPressed.Invoke(this, EventArgs.Empty);
			}

			if (RayGui.GuiButton(new Rectangle(center_x, 
				center_y + button_height * 2,
				button_width, button_height), "Options"))
			{
				SettingsButtonPressed.Invoke(this, EventArgs.Empty);
			}

			if (RayGui.GuiButton(new Rectangle(center_x, 
				center_y + button_height * 4,
				button_width, button_height), "Quit"))
			{
				QuitButtonPressed.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
