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

        public Sprite(Texture2D spriteTexture, Vector2 spritePosition)
        {
            m_texture = spriteTexture;
            m_position = spritePosition;
            
            m_rotationOrigin = Vector2.Zero;
            m_rotationOrigin.X = m_texture.Width / 2;
            m_rotationOrigin.Y = m_texture.Height / 2;
        }

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        //Returns the center point of the sprite
        public Vector2 GetCenter()
        {
            return new Vector2(m_position.X + m_texture.Width / 2, m_position.Y + m_texture.Height / 2);
        }

        public Rectangle GeneralSpriteBox { get { return new Rectangle((int)m_position.X, (int)m_position.Y, m_texture.Width, m_texture.Height); } }
    }
}
