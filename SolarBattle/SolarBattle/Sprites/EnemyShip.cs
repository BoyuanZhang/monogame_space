using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.LevelMaps;
using SolarBattle.Utility;

namespace SolarBattle.Sprites
{
    public class EnemyShip : Sprite
    {
        private float m_rotation;
        private bool isAlive;

        private Texture2D m_bulletTexture;
        private Rectangle m_enemyMiniMapRectangle;
        private Rectangle m_evasionRectangle;

        private PlayerShip m_player;

        private int m_energy;
        private Vector2 m_velocity;
        private float m_shipSpeed;

        //HACK way of random movement around asteroids
        private int m_movementType;

        private LinkedList<Bullet> m_enemyBullets;

        public EnemyShip( Texture2D shipTexture, Vector2 spawnPosition, Texture2D bulletTexture, PlayerShip player ) : base( shipTexture, spawnPosition )
        {
            m_rotation = 1.5f * (float)Math.PI;
            isAlive = true;
            m_velocity = Vector2.Zero;

            m_bulletTexture = bulletTexture;
            m_enemyMiniMapRectangle = Rectangle.Empty;
            m_evasionRectangle = Rectangle.Empty;
            m_enemyBullets = new LinkedList<Bullet>();

            m_player = player;
            m_energy = 250;
            m_shipSpeed = 0.0f;

            //Very hack way of enforcing different movement around asteroids for different enemy ships (To avoid clutter)
            if (0.5 - LevelOne.worldRand.NextDouble() < 0)
                m_movementType = -1;
            else
                m_movementType = 1;
        }

        public override void Update()
        {
            m_enemyMiniMapRectangle = new Rectangle((int)GetCenter().X - LevelOne.mapWidth / 4, (int)GetCenter().Y - LevelOne.mapHeight / 4, LevelOne.mapWidth/2, LevelOne.mapHeight/2);

            //Update enemy ship angle to look at user, if user is within the enemy ships radar
            //If ship is angled at player, and player is on the ships radar fire!
            if (IsAlive)
            {
                UpdateShipAngle();
                UpdateShipPosition();

                //Evasion step to possibly avoid obstacles not including bullets
                //Update the evasion rectangle, so the collision engine will inform us if the evasion rectangle came into contact with something so we can slightly adjust our course
                UpdateEvasionRectangle();
            }

            //Update ship bullets
            if( m_enemyBullets.Count > 0 )
                UpdateBullets(m_enemyBullets.First);

            //Update ship energy
            if (m_energy < 250)
                m_energy++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw enemy ship if alive
            if( IsAlive )
                spriteBatch.Draw(m_texture, GetCenter(), null, Color.White, m_rotation, m_rotationOrigin, 1f, SpriteEffects.None, 0);

            //TEST**** draw the enemy evasion rectangle
           /* if (isAlive)
                spriteBatch.Draw(m_bulletTexture, m_evasionRectangle, Color.Black*0.5f);*/

            //Draw all enemy ship bullets
            foreach (Bullet bullet in m_enemyBullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public bool IsAlive { set { isAlive = value; } get { return isAlive; } }

        //Similar to player ship collision handler (Probably should have just done virtual inheritance from an extended Sprite type )
        //Type 1 is border collision, type 2 is asteroid collision, type 3 is evasion box collision, type 4 is swarm collision, 5 is player bullet evasion box collision
        public void ShipCollisionHandler(int collisionType)
        {
            switch (collisionType)
            {
                case 1:
                    m_position -= m_velocity;
                    m_shipSpeed *= -1.5f;
                    break;
                case 2:
                    m_position -= m_velocity;
                    m_rotation += 0.07f * m_movementType;
                    break;
                case 3:
                    m_rotation += 0.07f * m_movementType;
                    break;
                case 4:
                    m_rotation += 0.05f * m_movementType;
                    break;
            }
        }

        public void ClearBullets()
        {
            if (m_enemyBullets.Count > 0)
                m_enemyBullets.Clear();
        }

        public LinkedList<Bullet> GetEnemyShipBullets()
        {
            return m_enemyBullets;
        }

        public Rectangle GetEvasionRectangle()
        {
            return m_evasionRectangle;
        }

        private void UpdateShipPosition()
        {
            m_velocity = Vector2.Zero;
            if (RectangleUtility.ContainedWithin(m_player.GeneralSpriteBox, m_enemyMiniMapRectangle))
            {
                if (Vector2.Subtract(m_player.GetCenter(), GetCenter()).Length() > 1100)
                {
                    if (m_shipSpeed < 3.0f)
                        m_shipSpeed += 0.15f;
                }
                else if (Vector2.Subtract(m_player.GetCenter(), GetCenter()).Length() < 900)
                {
                    if (m_shipSpeed > -3.0f)
                        m_shipSpeed -= 0.15f;
                }
                else
                {
                    //Circle around player if engagement distance reached.
                    Vector2 circlingVelocity = new Vector2(m_shipSpeed / 2 * (float)Math.Cos(m_rotation + Math.PI / 2), m_shipSpeed / 2 * (float)Math.Sin(m_rotation + Math.PI / 2));
                    m_velocity += circlingVelocity;
                }

                m_velocity.X = m_shipSpeed * (float)Math.Cos(m_rotation);
                m_velocity.Y = m_shipSpeed * (float)Math.Sin(m_rotation);
                m_position += m_velocity;
            }
            else
            {
                // Ok player ship is not within enemy radar. Move ships slowly towards reported position of player

                //Multiply the absolute angle of the ship, so as to continue along obstacles, in the direction of the player
                if (m_player.GetCenter().X < m_position.X)
                    m_velocity.X = -0.8f * (float)Math.Abs(Math.Cos(m_rotation));
                else
                    m_velocity.X = 0.8f * (float)Math.Abs(Math.Cos(m_rotation));

                if (m_player.GetCenter().Y < m_position.Y)
                    m_velocity.Y = -0.8f * (float)Math.Abs(Math.Sin(m_rotation));
                else
                    m_velocity.Y = 0.8f * (float)Math.Abs(Math.Sin(m_rotation));

                m_position += m_velocity;
            }
        }

        private void UpdateShipAngle()
        {
            //Reset angle
            if (m_rotation >= 2.0f * (float)Math.PI)
                m_rotation = 0;
            else if (m_rotation <= 0)
                m_rotation = 2.0f * (float)Math.PI;

            //For testing
            //m_enemyMiniMapRectangle = new Rectangle((int)GetCenter().X - 200, (int)GetCenter().Y - 200, 400, 400);

            if (RectangleUtility.ContainedWithin(m_player.GeneralSpriteBox, m_enemyMiniMapRectangle))
            {
                Vector2 vectorToPlayer = Vector2.Subtract(m_player.GetCenter(), GetCenter());
                float angleToPlayer = (float)Math.Atan2(vectorToPlayer.Y, vectorToPlayer.X);

                if (angleToPlayer <= 0)
                    angleToPlayer = 2.0f * (float)Math.PI + angleToPlayer;

                float difference = m_rotation - angleToPlayer;

                //Check for fastest rotation to face the player, and move in that rotation direction
                if (Math.Abs(difference) > 0.05)
                {
                    if (difference < 0 && Math.Abs(difference) > Math.PI)
                        m_rotation -= 0.03f;
                    else if (difference < 0 || Math.Abs(difference) > Math.PI)
                        m_rotation += 0.03f;
                    else
                        m_rotation -= 0.03f;
                }
                else
                {
                    //Ship is facing player... if ship has enough energy, fire!!
                    if (m_energy >= 250)
                    {
                        Vector2 bulletVelocity = Vector2.Multiply(m_velocity, 1.25f);
                        m_energy = 0;
                        m_enemyBullets.AddLast(new Bullet(m_bulletTexture, GetCenter(), bulletVelocity, m_rotation));
                    }
                }
            }
        }

        private void UpdateEvasionRectangle()
        {
            //Update the evasion rectangle of the ship according to ships current position and direction vector
            Vector2 directionVect = Vector2.Normalize(m_velocity);

            Vector2 rectPosition = new Vector2(m_position.X + 180 * (directionVect.X), m_position.Y + 180 * (directionVect.Y));

            m_evasionRectangle.X = (int)rectPosition.X;
            m_evasionRectangle.Y = (int)rectPosition.Y-m_texture.Height/2;
            m_evasionRectangle.Width = m_texture.Width*3;
            m_evasionRectangle.Height = m_texture.Height*3;
        }

        public void UpdateBullets(LinkedListNode<Bullet> bullet)
        {
            if (bullet.Next != null)
                UpdateBullets(bullet.Next);

            if (!bullet.Value.Alive)
            {
                m_enemyBullets.Remove(bullet);
            }
            else
                bullet.Value.Update();
        }
    }
}
