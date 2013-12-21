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
        
        public Bullet(Texture2D bulletTexture, Vector2 bulletPosition, Vector2 shipVelocity, float rotation) : base(bulletTexture, bulletPosition, Vector2.Zero)
        {
            m_bulletVelocity = shipVelocity;
            m_alive = true;
            m_rotation = rotation;

            m_constantSpeed = 10.0f;
        }

        public override void Update()
        {
            m_position.X +=  m_constantSpeed * (float)Math.Cos(m_rotation) + m_bulletVelocity.X;
            m_position.Y +=  m_constantSpeed * (float)Math.Sin(m_rotation) + m_bulletVelocity.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, GeneralSpriteBox, Color.White);
        }

        public bool Alive { set { m_alive = value; } get { return m_alive; } }
    }
}
