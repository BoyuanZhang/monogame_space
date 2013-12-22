using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.Sprites;

namespace SolarBattle.LevelMaps
{
    public class LevelOne
    {
        public const int backGroundOriginX = 0;
        public const int backGroundOriginY = 0;

        public const int asteroidCount = 30;

        private Texture2D m_asteroidTexture;
        private Texture2D m_mapTexture;
        private Vector2 m_mapPosition;

        private const int screenHeight = Main.screenHeight;
        private const int screenWidth = Main.screenWidth;

        private LinkedList<Asteroid> m_asteroids;

        private Rectangle m_levelRectangle;

        public LevelOne(Texture2D levelMap, Texture2D asteroidTexture)
        {
            m_mapTexture = levelMap;
            m_asteroidTexture = asteroidTexture;
            m_mapPosition = new Vector2(backGroundOriginX, backGroundOriginY);
            m_levelRectangle = new Rectangle((int)m_mapPosition.X, (int)m_mapPosition.Y, m_mapTexture.Width, m_mapTexture.Height);

            m_asteroids = new LinkedList<Asteroid>();
            InitializeAsteroids();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Basic drawing of the background texture
            spriteBatch.Draw(m_mapTexture, m_levelRectangle, Color.White);

            //Draw all asteroids
            foreach (Asteroid asteroid in m_asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
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

        public LinkedList<Asteroid> GetAsteroids()
        {
            return m_asteroids;
        }

        private void InitializeAsteroids()
        {
            var rand = new Random();

            for (int i = 0; i < asteroidCount; i++)
            {
                int positionX = rand.Next( m_mapTexture.Width - m_asteroidTexture.Width );
                int positionY = rand.Next( m_mapTexture.Height - m_asteroidTexture.Height );
                Asteroid asteroid = new Asteroid(m_asteroidTexture, new Vector2(positionX, positionY));

                m_asteroids.AddLast(asteroid);
            }
        }
    }
}
