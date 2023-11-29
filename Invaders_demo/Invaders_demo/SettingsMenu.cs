using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invaders_demo
{
	internal class SettingsMenu
	{
		public event EventHandler BackButtonPressed;
		public void ShowMenu()
		{
			int window_width = Raylib.GetScreenWidth();
			int window_height = Raylib.GetScreenHeight();
			int button_width = 100;
			int button_height = 40;
			int center_x = window_width / 2 - button_width / 2;
			int center_y = window_height / 2 - button_height / 2;

			RayGui.GuiLabel(new Rectangle(center_x, center_y - button_height, button_width, button_height), "OPTIONS");

			if (RayGui.GuiButton(new Rectangle(center_x, center_y
				, button_width, button_height), "Back"))
			{
				BackButtonPressed.Invoke(this, EventArgs.Empty);
			}

		}
	}
}
