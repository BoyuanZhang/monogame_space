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
        public const int mapWidth = 8000;
        public const int mapHeight = 8000;

        public const int asteroidCount =  65;
        public const int enemyCount = 50;

        public static Random worldRand = new Random();

        private Texture2D m_enemyShipTexture;
        private Texture2D m_enemyBulletTexture;
        private Texture2D m_asteroidTexture;
        private Texture2D m_mapTexture;
        private Vector2 m_mapPosition;

        private PlayerShip m_playerShip;

        //Partition tree for asteroids within this level.
        private SpritePartitionTree<Asteroid> m_asteroidPartitionTree;
        private LinkedList<Asteroid> m_asteroids;

        //Linked list of enemies for this level
        private LinkedList<EnemyShip> m_enemyShips;

        private Rectangle m_levelRectangle;

        //Level contains a specific number of asteroids (asteroidCount), and will contain the partition tree of the level objects (asteroids, enemies)
        //This is hack, when refactoring, add in a load content method within the level, and load in level required textures there
        public LevelOne( PlayerShip player, Texture2D levelMap, Texture2D asteroidTexture, Texture2D enemyShipTexture, Texture2D enemyBulletTexture)
        {
            //Set textures of objects the level requires
            m_mapTexture = levelMap;
            m_asteroidTexture = asteroidTexture;
            m_enemyShipTexture = enemyShipTexture;
            m_enemyBulletTexture = enemyBulletTexture;

            m_playerShip = player;
            //Set map position, and the entire map rectangle
            m_mapPosition = new Vector2(backGroundOriginX, backGroundOriginY);
            m_levelRectangle = new Rectangle((int)m_mapPosition.X, (int)m_mapPosition.Y, mapWidth, mapHeight);

            //Spawn asteroids onto map, and store asteroids in partition tree
            m_asteroidPartitionTree = new SpritePartitionTree<Asteroid>(m_levelRectangle);
           
            //Spawn asteroids onto map
            m_asteroids = new LinkedList<Asteroid>();
            InitializeAsteroids();

            //Spawn enemies onto map
            m_enemyShips = new LinkedList<EnemyShip>();
            InitializeEnemies();
        }

        //Update level (enemy ships within level )
        public void Update()
        {
            if( m_enemyShips.Count > 0 )
                UpdateEnemyShips(m_enemyShips.First);
        }

        public void UpdateEnemyShips( LinkedListNode< EnemyShip> enemyShip )
        {
            if (enemyShip.Next != null)
                UpdateEnemyShips(enemyShip.Next);

            if (enemyShip.Value.IsAlive == true)
                enemyShip.Value.Update();
            else if (enemyShip.Value.GetEnemyShipBullets().Count > 0)
                enemyShip.Value.Update();
            else
                m_enemyShips.Remove(enemyShip);
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
            
            //Draw all enemy ships
            foreach (EnemyShip enemyShip in m_enemyShips)
            {
                enemyShip.Draw(spriteBatch);
            }

            //Draw visualization of quadtree / comment out if you do not want to see the quadtrees
            //----------------------------------------------------------------------------------------------------------------------------------------------------------
          /*  List<Rectangle> quadRectangles = m_asteroidPartitionTree.GetQuadRectangles();
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
            }*/
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        //Draw the initial map to the mini-map render target once per load of mini map
        public void DrawMiniMap(SpriteBatch spriteBatch)
        {
            //Basic drawing of the background texture
            spriteBatch.Draw(m_mapTexture, m_levelRectangle, Color.White);

            //Draw all asteroids
            foreach (Asteroid asteroid in m_asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            m_asteroidPartitionTree.Clear();
            m_asteroids.Clear();
            
            //clear all enemy bullets before clearing enemies
            if( m_enemyShips.Count > 0 )
                ResetEnemyShips( m_enemyShips.First );   
       
            //reinitialize asteroids and enemies
            InitializeAsteroids();
            InitializeEnemies();
        }

        public SpritePartitionTree<Asteroid> GetAsteroidPartitionTree()
        {
            return m_asteroidPartitionTree;
        }

        public LinkedList<EnemyShip> GetEnemyShipList()
        {
            return m_enemyShips;
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
            for (int i = 0; i < asteroidCount; i++)
            {
                //150 hard-coded in so asteroids are not too close to borders
                int positionX =  worldRand.Next(150 ,mapWidth - (m_asteroidTexture.Width + 150));
                int positionY = worldRand.Next(150, mapHeight - (m_asteroidTexture.Height + 150));

                //Make sure not to spawn asteroids in the player spawn area
                while (positionX < 500 && positionY < 500)
                {
                    positionX = worldRand.Next(mapWidth - m_asteroidTexture.Width);
                    positionY = worldRand.Next(mapHeight - m_asteroidTexture.Height);
                }

                Asteroid asteroid = new Asteroid(m_asteroidTexture, new Vector2(positionX, positionY));

                m_asteroids.AddLast(asteroid);
                m_asteroidPartitionTree.Add(asteroid);
            }
        }

        private void InitializeEnemies()
        {
            for( int i = 0; i < enemyCount; i++)
            {
                //Parameters to initialize position, and flag to see if the current position is viable or not
                int positionX = 0;
                int positionY = 0;
                bool goodPosition = false;

                //Make sure not to spawn enemies on top of any asteroids
                while (!goodPosition)
                {
                    positionX = worldRand.Next(mapWidth - m_enemyShipTexture.Width);
                    positionY = worldRand.Next(mapHeight - m_enemyShipTexture.Height);

                    goodPosition = true;
                    //Make the rectangle a tiny bit bigger to avoid ships starting near asteroids
                    Rectangle enemyShipRect = new Rectangle(positionX-15, positionY-15, m_enemyShipTexture.Width+30, m_enemyShipTexture.Height+30);
                    List< LinkedList<Asteroid>> boundAsteroids = m_asteroidPartitionTree.GetPartitionItems(enemyShipRect);
                    //Make sure no enemies spawn near player at beginning
                    if (positionX < mapWidth/4 && positionY < mapHeight/4)
                    {
                        goodPosition = false;
                    }
                    else if (boundAsteroids.Count > 0)
                    {
                        //ok no enemies near spawn area, use partition tree with the enemy ship rectangle to detect
                        //if any collisions occur with the ship spawn area and nearby asteroids
                        foreach (LinkedList<Asteroid> asteroids in boundAsteroids)
                        {
                            if (!goodPosition)
                                break;
                            foreach (Asteroid asteroid in asteroids)
                            {
                                if (!goodPosition)
                                    break;
                                if (enemyShipRect.Intersects(asteroid.GeneralSpriteBox))
                                    goodPosition = false;
                            }
                        }
                    }
                }

                EnemyShip enemyShip = new EnemyShip(m_enemyShipTexture, new Vector2(positionX, positionY), m_enemyBulletTexture, m_playerShip);

                m_enemyShips.AddLast(enemyShip);
            }
        }

        private void ResetEnemyShips(LinkedListNode<EnemyShip> enemyShip)
        {
            if (enemyShip.Next != null)
                ResetEnemyShips(enemyShip.Next);

            enemyShip.Value.ClearBullets();
            m_enemyShips.Remove(enemyShip);
        }
    }
}
