using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SolarBattle
{
    public static class GameWindowHandler
    {
        public static void SetPosition(this GameWindow window, Point position)
        {
            //Get access to the game window
            OpenTK.GameWindow OTKWindow = GetForm(window);
            //Set the game window position
            if (OTKWindow != null)
            {
                OTKWindow.X = position.X;
                OTKWindow.Y = position.Y;
            }
        }

        private static OpenTK.GameWindow GetForm(this GameWindow gameWindow)
        {
            //Use reflection, to access the private window field within Monogame's OpenTKGameWindow
            Type type = typeof(OpenTKGameWindow);
            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
                return field.GetValue(gameWindow) as OpenTK.GameWindow;
            return null;
        }
    }
}
