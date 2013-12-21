using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarBattle.Sprites
{
    public class Sprite
    {
        //All sprite types derived from this parent class
        protected Texture2D m_texture;
        protected Vector2 m_rotationOrigin;
        protected Vector2 m_position;

        public Sprite(Texture2D spriteTexture, Vector2 spritePosition, Vector2 rotationOrigin)
        {
            m_texture = spriteTexture;
            m_position = spritePosition;
            
            m_rotationOrigin = rotationOrigin;
            m_rotationOrigin.X = m_texture.Width / 2;
            m_rotationOrigin.Y = m_texture.Height / 2;
        }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public Rectangle GeneralSpriteBox { get { return new Rectangle((int)m_position.X - m_texture.Width/2, (int)m_position.Y - m_texture.Height/2, m_texture.Width*2, m_texture.Height*2); } }
    }
}
