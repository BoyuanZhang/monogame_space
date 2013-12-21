using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolarBattle.Sprites
{
    public class PlayerShip : Sprite
    {
        private float m_speed;
        //Ship velocity calculates and contains orientation / direction / speed
        private Vector2 m_shipVelocity;
        private float m_acceleration;
        private float m_friction;
        private float m_rotation;
        private int m_energy;

        private Texture2D m_bulletTexture;
        //Used a linked-list, because for all updates, every bullet will be traversed always, so required a datastructure with O(1) removal / insertion
        public LinkedList< Bullet> m_bullets;

        public PlayerShip(Texture2D playerShipTexture, Texture2D bulletTexture): base(playerShipTexture, new Vector2(100, 100), Vector2.Zero)
        {
            m_rotation = 1.5f*(float)Math.PI;
            m_friction = 0.02f;
            m_acceleration = 0.0f;
            m_energy = 1000;
            m_speed = 0.0f;

            m_bulletTexture = bulletTexture;
            m_bullets = new LinkedList<Bullet>();
        }

        public override void Update()
        {
            //Update based on user input
            UpdateKeyboardInput();

            //Update ship bullets
            if (m_bullets.Count > 0)
                UpdateBullets(m_bullets.First);

            //Resetting angle
            if (m_rotation >= 2.0f * (float)Math.PI || m_rotation <= -2.0f * (float)Math.PI)
                m_rotation = 0;

            //If ship energy not full, increase ship energy
            if (m_energy < 100)
                m_energy += 1;

            //update ship position
            m_position += m_shipVelocity;
        }

        public void UpdateKeyboardInput()
        {
            bool isAccelerating = false;
            //Update according to player input
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                m_rotation -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                m_rotation += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                //If shift key is pressed, the ship is accelerating
                isAccelerating = true;
            }
            //If ship is accelerating it can reach an acceleration of 3.6 / -3.6, how ever if it is not accelerating but still moving it
            //the acceleration will tick down to 0
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (isAccelerating && m_acceleration < 4)
                    m_acceleration += 0.2f;
                else if (!isAccelerating)
                    Decelerate();

                if (m_speed < 4.0)
                    m_speed += 0.25f;

                m_shipVelocity.X = m_acceleration * (float)Math.Cos(m_rotation) + m_speed * (float)Math.Cos(m_rotation);
                m_shipVelocity.Y = m_acceleration * (float)Math.Sin(m_rotation) + m_speed * (float)Math.Sin(m_rotation);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (isAccelerating && m_acceleration > -4)
                    m_acceleration -= 0.2f;
                else if (!isAccelerating)
                    Decelerate();

                if (m_speed > -4.0)
                    m_speed -= 0.25f;

                m_shipVelocity.X = m_acceleration * (float)Math.Cos(m_rotation) + m_speed * (float)Math.Cos(m_rotation);
                m_shipVelocity.Y = m_acceleration * (float)Math.Sin(m_rotation) + m_speed * (float)Math.Sin(m_rotation);
            }
            else if (m_shipVelocity != Vector2.Zero)
            {
                //if ships not moving bring velocity closer to 0
                m_shipVelocity -= m_shipVelocity * m_friction;

                if (m_speed > 0)
                    m_speed -= 0.1f;
                else
                    m_speed += 0.1f;

                Decelerate();
            }

            //Shot fired, only allow one shot to be fired if ship has energy
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (m_energy >= 100)
                {
                    m_energy = 0;
                    m_bullets.AddLast(new Bullet(m_bulletTexture, m_position, m_shipVelocity, m_rotation));
                }
            }
        }

        public void Decelerate()
        {
            if (m_acceleration < 0)
                m_acceleration += 0.1f;
            else if ( m_acceleration > 0)
                 m_acceleration -= 0.1f;
        }

        //All bullets will be updated, if a bullet's alive value has been set to false, it will be removed from the linked list
        public void UpdateBullets(LinkedListNode<Bullet> bullet)
        {
            if (!bullet.Value.Alive)
            {
                m_bullets.Remove(bullet);                        
            }
            else
                bullet.Value.Update();

            if (bullet.Next != null)
                UpdateBullets(bullet.Next);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, m_position, null, Color.White, m_rotation,m_rotationOrigin, 1f, SpriteEffects.None, 0 );
            //draw bullets
            if (m_bullets.Count > 0)
                foreach (Bullet bullet in m_bullets)
                {
                    if (bullet.Alive != false)
                        bullet.Draw(spriteBatch);
                }
        }
        //Handle Collision detection with border, the collision engine is kind enough to do most the work and throw over the position to move to :) 
        //Collision types: Type 1 - X border collision, Type 2 - Y border collision
        public void ShipCollisionHandler(int collisionType, float newPosition)
        {
            switch (collisionType)
            {
                case 1:
                    m_position.X = newPosition;
                    if (m_acceleration > 0)
                        m_acceleration -= 0.3f;
                    else
                        m_acceleration += 0.3f;
                    if (m_speed > 0)
                        m_speed -= 0.3f;
                    else
                        m_speed += 0.3f;
                    break;
                case 2:
                    m_position.Y = newPosition;
                    if (m_acceleration > 0)
                        m_acceleration -= 0.3f;
                    else
                        m_acceleration += 0.3f;
                    if (m_speed > 0)
                        m_speed -= 0.3f;
                    else
                        m_speed += 0.3f;
                    break;
            }
        }
        //Returns the center point of the ship
        public Vector2 GetCenter()
        {
            return new Vector2(m_position.X + m_texture.Width / 2, m_position.Y + m_texture.Height / 2);
        }

        //Returns the bullets linked list of this ship
        public LinkedList<Bullet> Bullets { get { return m_bullets; } }

        //Returns Sprite box of ship
        public Rectangle SpecificSpriteBox { get { return new Rectangle((int)m_position.X - m_texture.Width / 2, (int)m_position.Y - m_texture.Height, m_texture.Width, m_texture.Height * 2); } }
    }
}
