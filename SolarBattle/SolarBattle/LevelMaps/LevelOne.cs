using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarBattle.LevelMaps
{
    public class LevelOne
    {
        public const int backGroundOriginX = 0;
        public const int backGroundOriginY = 0;

        private Texture2D m_mapTexture;
        private Vector2 m_mapPosition;

        private const int screenHeight = Main.screenHeight;
        private const int screenWidth = Main.screenWidth;

        private Rectangle m_levelRectangle;

        public LevelOne(Texture2D levelMap)
        {
            m_mapTexture = levelMap;
            m_mapPosition = new Vector2(backGroundOriginX, backGroundOriginY);
            m_levelRectangle = new Rectangle((int)m_mapPosition.X, (int)m_mapPosition.Y, m_mapTexture.Width, m_mapTexture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Basic drawing of the background texture
            spriteBatch.Draw(m_mapTexture, m_levelRectangle, Color.White);
        }

        public Rectangle GetLevelRectangle()
        {
            return m_levelRectangle;
        }

        //For use of the minimap
        public Texture2D GetTexture()
        {
            return m_mapTexture;
        }
    }
}
