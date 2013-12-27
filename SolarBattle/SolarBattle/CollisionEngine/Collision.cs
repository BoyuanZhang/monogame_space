using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using SolarBattle.Sprites;
using SolarBattle.LevelMaps;
using SolarBattle.PartitionTree;

namespace SolarBattle.CollisionEngine
{
    public class Collision
    {
        protected LevelOne m_level;
        protected PlayerShip m_player;

        public Collision( LevelOne level, PlayerShip player)
        {
            m_level = level;
            m_player = player;
        }

        public void HandleCollisions()
        {
            ShipCollisions();
            BulletCollisions();
        }

        private void ShipCollisions()
        {
            ShipBorderCollisions();
        }

        private void BulletCollisions()
        {
            LinkedList<Bullet> bullets = m_player.Bullets;
            foreach (Bullet bullet in bullets)
            {
                BulletBorderCollisions(bullet);
                BulletAsteroidCollisions(bullet);
            }
        }

        private void ShipBorderCollisions()
        {
            //Get the specific sprite box of the player ship for collision calculation
            Rectangle levelBorder = m_level.GetLevelRectangle();
            Rectangle playerSpriteBox = m_player.SpecificSpriteBox;
            //Check for Collisions with level border and player , and calculates re-adjust necessary and passes it to the ship's collision handler to 
            //re-position / other effects
            if (playerSpriteBox.Left < levelBorder.Left)
                m_player.ShipCollisionHandler(1, levelBorder.Left - (playerSpriteBox.Left - playerSpriteBox.Right) / 2);
            else if (playerSpriteBox.Right > levelBorder.Right)
                m_player.ShipCollisionHandler(1, levelBorder.Right - (playerSpriteBox.Right - playerSpriteBox.Left) / 2);
            if (playerSpriteBox.Top < levelBorder.Top)
                m_player.ShipCollisionHandler(2, levelBorder.Top + (playerSpriteBox.Bottom - playerSpriteBox.Top) / 2);
            else if (playerSpriteBox.Bottom >= levelBorder.Bottom)
                m_player.ShipCollisionHandler(2, levelBorder.Bottom - (playerSpriteBox.Bottom - playerSpriteBox.Top) / 2);
        }

        private void BulletBorderCollisions( Bullet bullet )
        {
            Rectangle levelBorder = m_level.GetLevelRectangle();

            Rectangle bulletSpriteBox = bullet.GeneralSpriteBox;
            if (bulletSpriteBox.Left <= levelBorder.Left)
                bullet.Alive = false;
            else if (bulletSpriteBox.Right >= levelBorder.Right)
                bullet.Alive = false;
            if (bulletSpriteBox.Top <= levelBorder.Top)
                bullet.Alive = false;
            else if (bulletSpriteBox.Bottom >= levelBorder.Bottom)
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
    }
}
