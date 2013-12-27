using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.Sprites;
using SolarBattle.PartitionTree;

namespace SolarBattle.LevelMaps
{
    public class LevelOne
    {
        public const int backGroundOriginX = 0;
        public const int backGroundOriginY = 0;
        public const int mapWidth = 4048;
        public const int mapHeight = 4048;

        public const int asteroidCount = 30;

        private Texture2D m_asteroidTexture;
        private Texture2D m_mapTexture;
        private Vector2 m_mapPosition;

        //Partition tree for asteroids within this level.
        private SpritePartitionTree<Asteroid> m_asteroidPartitionTree;
        private LinkedList<Asteroid> m_asteroids;

        private Rectangle m_levelRectangle;

        //Level contains a specific number of asteroids (asteroidCount), and will contain the partition tree of the level objects (asteroids)
        public LevelOne(Texture2D levelMap, Texture2D asteroidTexture)
        {
            m_mapTexture = levelMap;
            m_asteroidTexture = asteroidTexture;
            m_mapPosition = new Vector2(backGroundOriginX, backGroundOriginY);
            m_levelRectangle = new Rectangle((int)m_mapPosition.X, (int)m_mapPosition.Y, mapWidth, mapHeight);

            m_asteroidPartitionTree = new SpritePartitionTree<Asteroid>(m_levelRectangle);
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

            //Draw visualization of quadtree / comment out if you do not want to see the quadtrees
            //----------------------------------------------------------------------------------------------------------------------------------------------------------
            List<Rectangle> quadRectangles = m_asteroidPartitionTree.GetQuadRectangles();
            int bw = 5;
            if (quadRectangles.Count > 0)
            {
                foreach (Rectangle rectangle in quadRectangles)
                {
                    spriteBatch.Draw(m_mapTexture, new Rectangle(rectangle.Left, rectangle.Top, bw, rectangle.Height), Color.Black); // Left
                    spriteBatch.Draw(m_mapTexture, new Rectangle(rectangle.Right, rectangle.Top, bw, rectangle.Height), Color.Black); // Right
                    spriteBatch.Draw(m_mapTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, bw), Color.Black); // Top
                    spriteBatch.Draw(m_mapTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, bw), Color.Black); // Bottom
                }
            }
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        public SpritePartitionTree<Asteroid> GetAsteroidPartitionTree()
        {
            return m_asteroidPartitionTree;
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

        //Load asteroid obstacles into level map
        //Randomly assign asteroid positions, then place them into quadtree
        private void InitializeAsteroids()
        {
            var rand = new Random();

            for (int i = 0; i < asteroidCount; i++)
            {
                int positionX = rand.Next(mapWidth - m_asteroidTexture.Width);
                int positionY = rand.Next( mapHeight - m_asteroidTexture.Height );
                Asteroid asteroid = new Asteroid(m_asteroidTexture, new Vector2(positionX, positionY));

                m_asteroids.AddLast(asteroid);
                m_asteroidPartitionTree.Add(asteroid);
            }
        }
    }
}
