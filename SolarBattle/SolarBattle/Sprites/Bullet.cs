using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarBattle.Sprites
{
    public class Bullet : Sprite
    {
        private Vector2 m_bulletVelocity;
        private float m_constantSpeed;
        private float m_rotation;
        private bool m_alive;
        
        //Bullet takes on ships velocity and orientation and fires in that direction with the additional velocity
        public Bullet(Texture2D bulletTexture, Vector2 bulletPosition, Vector2 shipVelocity, float rotation) : base(bulletTexture, bulletPosition)
        {
            m_bulletVelocity = shipVelocity;
            m_alive = true;
            m_rotation = rotation;

            //Bullets base speed
            m_constantSpeed = 10.0f;
        }

        //Bullet velocity is effected by the ship velocity
        public override void Update()
        {
            m_position.X +=  m_constantSpeed * (float)Math.Cos(m_rotation) + m_bulletVelocity.X;
            m_position.Y +=  m_constantSpeed * (float)Math.Sin(m_rotation) + m_bulletVelocity.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, SpecificSpriteBox, Color.Black);
        }

        //When the bullet is not alive it will be removed from the ships internally maintained bullet list
        public bool Alive { set { m_alive = value; } get { return m_alive; } }


        //************ Remove this when you find a better bullet texture, the specific sprite box should not be larger than the general sprite box in parent class Sprite
        public Rectangle SpecificSpriteBox { get { return new Rectangle((int)m_position.X - m_texture.Width / 2, (int)m_position.Y - m_texture.Height / 2, m_texture.Width * 2, m_texture.Height * 2); } }
    }
}
