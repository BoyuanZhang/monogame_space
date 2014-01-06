using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using SolarBattle.Sprites;
using SolarBattle.LevelMaps;
using SolarBattle.PartitionTree;
using SolarBattle.MiniMap;
using SolarBattle.Utility;

namespace SolarBattle.CollisionEngine
{
    public class Collision
    {
        protected LevelOne m_level;
        protected PlayerShip m_player;
        protected Minimap m_miniMap;
        protected Main m_gameManager;

        public Collision( Main gameManager, LevelOne level, PlayerShip player, Minimap miniMap)
        {
            m_gameManager = gameManager;
            m_level = level;
            m_player = player;

            m_miniMap = miniMap;
        }

        public void HandleCollisions()
        {
            PlayerShipCollisions();
            BulletCollisions();

            EnemyShipCollisions();

            //Check for sprite collisions against minimap rectangle, so the minimap will know whether to draw certain sprites or not
            MinimapCollisions();
        }

        private void PlayerShipCollisions()
        {
            PlayerShipBorderCollisions();
            PlayerShipAsteroidCollisions();
        }

        private void EnemyShipCollisions()
        {
            LinkedList<EnemyShip> enemyShips = m_level.GetEnemyShipList();

            foreach (EnemyShip enemyShip in enemyShips)
            {
                EnemyShipAsteroidCollisions( enemyShip);
                EnemyShipBorderCollisions(enemyShip);
            }

            //Hack way of preventing mass swarming, loop through enemy ships, make sure current ship is not colliding with its previous neighbour, then go to next, next ship
            if( enemyShips.Count > 1 && enemyShips.First.Next != null)
                PreventSwarmingEnemies(enemyShips.First.Next);
        }

        private void BulletCollisions()
        {
            //Check for bullet collisions of the player
            LinkedList<Bullet> bullets = m_player.Bullets;
            foreach (Bullet bullet in bullets)
            {
                BulletBorderCollisions(bullet);
                BulletAsteroidCollisions(bullet);
                BulletEnemyCollisions(bullet);
            }

            //Check for all enemy bullet collisions
            LinkedList<EnemyShip> enemyShips = m_level.GetEnemyShipList();

            foreach (EnemyShip enemyShip in enemyShips)
            {
                LinkedList<Bullet> bulletList = enemyShip.GetEnemyShipBullets();
                foreach (Bullet bullet in bulletList)
                {
                    BulletBorderCollisions(bullet);
                    BulletAsteroidCollisions(bullet);
                    BulletPlayerCollision(bullet);
                }
            }
        }

        private void PlayerShipBorderCollisions()
        {
            //Get the specific sprite box of the player ship for collision calculation
            Rectangle levelBorder = m_level.GetLevelRectangle();
            Rectangle playerSpriteBox = m_player.GeneralSpriteBox;
            //Check for Collisions with level border and player , and calculates re-adjust necessary and passes it to the ship's collision handler to 
            //re-position / other effects
            if (playerSpriteBox.Left < levelBorder.Left)
                m_player.ShipCollisionHandler(1, 0);
            else if (playerSpriteBox.Right > levelBorder.Right)
                m_player.ShipCollisionHandler(1, levelBorder.Right - (playerSpriteBox.Right - playerSpriteBox.Left));
            if (playerSpriteBox.Top < levelBorder.Top)
                m_player.ShipCollisionHandler(2, 0);
            else if (playerSpriteBox.Bottom >= levelBorder.Bottom)
                m_player.ShipCollisionHandler(2, levelBorder.Bottom - (playerSpriteBox.Bottom - playerSpriteBox.Top));
        }

        private void PlayerShipAsteroidCollisions()
        {
            List<LinkedList<Asteroid>> boundAsteroids = m_level.GetAsteroidPartitionTree().GetPartitionItems(m_player.GeneralSpriteBox);

            if (boundAsteroids.Count > 0)
            {
                foreach (LinkedList<Asteroid> asteroids in boundAsteroids)
                {
                    foreach (Asteroid asteroid in asteroids)
                    {
                        if (m_player.GeneralSpriteBox.Intersects(asteroid.GeneralSpriteBox))
                            if( PixelIntersects( m_player, asteroid ) )
                                m_player.ShipCollisionHandler(3, 0.0f);
                    }
                }
            }
        }

        private void EnemyShipAsteroidCollisions( EnemyShip enemyShip)
        {
            List<LinkedList<Asteroid>> boundAsteroids = m_level.GetAsteroidPartitionTree().GetPartitionItems(enemyShip.GeneralSpriteBox);

            if (boundAsteroids.Count > 0)
            {
                foreach (LinkedList<Asteroid> asteroids in boundAsteroids)
                {
                    foreach (Asteroid asteroid in asteroids)
                    {
                        //Check for asteroids within the vicinity of the enemy ship, and check for collisions against these asteroids.
                        //Also, if there is no collision against the enemy ship itself use the evasion prediction box of the enemy ship 
                        //to pre-emptively try to avoid collisions

                        //The evasion box is a simple technique used to evade asteroids, however it is not entirely accurate because it uses the 
                        //enemyship sprite box to find asteroids within vicinity, this will work for our purposes. However if you want more accurate
                        //prediction, we have to get all asteroids within partitions of the evasion box.
                        if (enemyShip.GeneralSpriteBox.Intersects(asteroid.GeneralSpriteBox))
                            enemyShip.ShipCollisionHandler(2);
                        else if (enemyShip.GetEvasionRectangle().Intersects(asteroid.GeneralSpriteBox))
                            enemyShip.ShipCollisionHandler(3);
                    }
                }
            }
        }

        private void EnemyShipBorderCollisions(EnemyShip enemyShip)
        {
            //Get the specific sprite box of the player ship for collision calculation
            Rectangle levelBorder = m_level.GetLevelRectangle();
            Rectangle enemyShipSpriteBox = enemyShip.GeneralSpriteBox;
            //Check for Collisions with level border and enemy ship , and calculates re-adjust necessary and passes it to the ship's collision handler to 
            //re-position / other effects
            if (enemyShipSpriteBox.Left < levelBorder.Left || enemyShipSpriteBox.Right > levelBorder.Right || 
                enemyShipSpriteBox.Top < levelBorder.Top || enemyShipSpriteBox.Bottom >= levelBorder.Bottom)
                enemyShip.ShipCollisionHandler(1);
        }

        private void PreventSwarmingEnemies(LinkedListNode<EnemyShip> enemyShip)
        {
            if (enemyShip.Value.GeneralSpriteBox.Intersects(enemyShip.Previous.Value.GeneralSpriteBox) || 
                enemyShip.Value.GetEvasionRectangle().Intersects( enemyShip.Previous.Value.GetEvasionRectangle()))
                    enemyShip.Value.ShipCollisionHandler(4);

            if (enemyShip.Next != null)
                PreventSwarmingEnemies(enemyShip.Next);
        }

        private void BulletBorderCollisions( Bullet bullet )
        {
            Rectangle levelBorder = m_level.GetLevelRectangle();

            Rectangle bulletSpriteBox = bullet.GeneralSpriteBox;
            if (bulletSpriteBox.Left <= levelBorder.Left || bulletSpriteBox.Right >= levelBorder.Right ||
                bulletSpriteBox.Top <= levelBorder.Top || bulletSpriteBox.Bottom >= levelBorder.Bottom)
                bullet.Alive = false;
        }

        //Find the objects that are within the bullets partition space and intersecting the partition space
        //Go through all these objects, if a collision is found then set the bullet to no longer be alive so it is removed from
        //the ships list of alive bullets
        private void BulletAsteroidCollisions(Bullet bullet)
        {
            List<LinkedList<Asteroid>> boundAsteroids = m_level.GetAsteroidPartitionTree().GetPartitionItems(bullet.GeneralSpriteBox);

            if (boundAsteroids.Count > 0)
            {
                foreach (LinkedList<Asteroid> asteroids in boundAsteroids)
                {
                    foreach (Asteroid asteroid in asteroids)
                    {
                        if (bullet.GeneralSpriteBox.Intersects(asteroid.GeneralSpriteBox))
                            bullet.Alive = false;
                    }
                }
            }
        }

        private void BulletEnemyCollisions( Bullet bullet )
        {
            LinkedList<EnemyShip> enemyShips = m_level.GetEnemyShipList();

            foreach (EnemyShip enemyShip in enemyShips)
            {
                if (bullet.GeneralSpriteBox.Intersects(enemyShip.GeneralSpriteBox) && bullet.Alive)
                {
                    if (PixelIntersects(bullet, enemyShip))
                    {
                        bullet.Alive = false;
                        enemyShip.IsAlive = false;
                    }
                }
            }
        }

        private void BulletPlayerCollision(Bullet bullet)
        {
            if (bullet.GeneralSpriteBox.Intersects(m_player.GeneralSpriteBox))
            {
                bullet.Alive = false;
                
                //Currently Main is the temporary Screen handler that will handle
                //Game over, Level restart etc... In the future we should use an actual screen handler
                m_gameManager.GameOver();
            }
        }

        private void MinimapCollisions()
        {
            //Check for sprite collisions within mini map
            MinimapEnemyShipCollisions();
        }

        private void MinimapEnemyShipCollisions()
        {
            LinkedList<EnemyShip> enemyShips = m_level.GetEnemyShipList();

            foreach (EnemyShip enemyShip in enemyShips)
            {
                if (RectangleUtility.ContainedWithin(enemyShip.GeneralSpriteBox, m_miniMap.GetMiniMapAreaRectangle()))
                {
                    if( enemyShip.IsAlive )
                        m_miniMap.SpriteCollisionHandler(enemyShip);
                }
            }
        }

        
        //Check for pixel collision between sprites
        private bool PixelIntersects(Sprite spriteA, Sprite spriteB)
        {
            Rectangle rectA = spriteA.GeneralSpriteBox;
            Rectangle rectB = spriteB.GeneralSpriteBox;

            Color[] colorDataA = new Color[spriteA.GetTexture().Width * spriteA.GetTexture().Height];
            spriteA.GetTexture().GetData(colorDataA);
            Color[] colorDataB = new Color[spriteB.GetTexture().Width * spriteB.GetTexture().Height];
            spriteB.GetTexture().GetData(colorDataB);

            int top = Math.Max(rectA.Top, rectB.Top);
            int bottom = Math.Min(rectA.Bottom, rectB.Bottom);
            int left = Math.Max(rectA.Left, rectB.Left);
            int right = Math.Min(rectA.Right, rectB.Right);

            for (int r = top; r < bottom; r++)
            {
                for (int c = left; c < right; c++)
                {
                    Color colorA = colorDataA[(c - rectA.Left) + (r - rectA.Top) * rectA.Width];
                    Color colorB = colorDataB[(c - rectB.Left) + (r - rectB.Top) * rectB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
