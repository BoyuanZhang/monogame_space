using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using SolarBattle.Sprites;

namespace SolarBattle.CollisionEngine
{
    public class Collision
    {
        protected Rectangle m_levelBorder;
        protected PlayerShip m_player;

        public Collision( Rectangle levelBorder, PlayerShip player)
        {
            m_levelBorder = levelBorder;
            m_player = player;
        }

        public void HandleCollisions()
        {
            LevelBorderCollisions();
        }

        //Collision engine will usually not access sprite positions directly, but for border collision
        private void LevelBorderCollisions()
        {
            //Get the specific sprite box of the player ship for collision calculation
            Rectangle playerSpriteBox = m_player.SpecificSpriteBox;
            //Check for Collisions with level border and player , and calculates re-adjust necessary and passes it to the ship's collision handler to 
            //re-position / other effects
            if (playerSpriteBox.Left < m_levelBorder.Left)
                m_player.ShipCollisionHandler(1, m_levelBorder.Left - (playerSpriteBox.Left - playerSpriteBox.Right) / 2);
            else if (playerSpriteBox.Right > m_levelBorder.Right)
                m_player.ShipCollisionHandler(1, m_levelBorder.Right - (playerSpriteBox.Right - playerSpriteBox.Left) / 2);
            if (playerSpriteBox.Top < m_levelBorder.Top)
                m_player.ShipCollisionHandler(2, m_levelBorder.Top + (playerSpriteBox.Bottom - playerSpriteBox.Top) / 2);
            else if (playerSpriteBox.Bottom > m_levelBorder.Bottom)
                m_player.ShipCollisionHandler(2, m_levelBorder.Bottom - (playerSpriteBox.Bottom - playerSpriteBox.Top) / 2);

            //Check for collision with level border and bullets, if there is collision, bullet is no longer live
            LinkedList<Bullet> bullets = m_player.Bullets;
            foreach (Bullet bullet in bullets)
            {
                Rectangle bulletSpriteBox = bullet.GeneralSpriteBox;
                if (bulletSpriteBox.Left <= m_levelBorder.Left)
                    bullet.Alive = false;
                else if (bulletSpriteBox.Right >= m_levelBorder.Right)
                    bullet.Alive = false;
                if (bulletSpriteBox.Top <= m_levelBorder.Top)
                    bullet.Alive = false;
                else if (bulletSpriteBox.Bottom >= m_levelBorder.Bottom)
                    bullet.Alive = false;
            }
        }
    }
}
