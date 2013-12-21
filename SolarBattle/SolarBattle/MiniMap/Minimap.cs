using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.LevelMaps;
using SolarBattle.Sprites;
using SolarBattle.Camera;

namespace SolarBattle.MiniMap
{
    public class Minimap
    {
        //Positions of the minimap and center of the minimap, relative to the screen
        private const int miniMapWidth = 190;
        private const int miniMapHeight = 190;
        private const int miniMapCenterX = 95;
        private const int miniMapCenterY = 95;
        private const int mapOriginX = Main.screenWidth - miniMapWidth - 5;
        private const int mapOriginY = Main.screenHeight - miniMapHeight - 5;

        private PlayerCamera m_playerCamera;
        private LevelOne m_level;

        private Rectangle m_miniMapRectangle;
        private Texture2D m_cameraTexture;

        public Minimap(LevelOne levelMap, PlayerCamera playerCamera, Texture2D cameraTexture)
        {
            m_level = levelMap;
            m_playerCamera = playerCamera;
            m_miniMapRectangle = new Rectangle(mapOriginX, mapOriginY, miniMapWidth, miniMapHeight);
            m_cameraTexture = cameraTexture;

            initializeMapSprites();
        }

        //Initialize all sprites the minimap will use. If mini map initialization is not called, the mini map sprites will not be drawn.
        public void initializeMapSprites()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cameraCenter = m_playerCamera.getCameraFocus();
            Texture2D levelTexture = m_level.GetTexture();
            spriteBatch.Draw(levelTexture, m_miniMapRectangle,
                                new Rectangle((int)cameraCenter.X - levelTexture.Width / 2, (int)cameraCenter.Y - levelTexture.Width / 2, levelTexture.Width, levelTexture.Height), 
                                Color.GreenYellow * 0.5f);

            //draw player at center of the mini map
            spriteBatch.Draw(m_cameraTexture, new Rectangle( mapOriginX + miniMapCenterX -2, mapOriginY + miniMapCenterY - 2, 4, 4) ,Color.WhiteSmoke );
        }
    }
}
