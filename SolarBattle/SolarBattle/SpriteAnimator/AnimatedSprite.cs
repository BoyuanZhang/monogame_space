using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarBattle
{
    public class AnimatedSprite
    {
        //Under construction ...
        //this class must inherit from class Sprite, and is used to animate sprite sheets
        public Texture2D m_texture { get; set; }
        public int m_rows { get; set; }
        public int m_columns { get; set; }

        private int m_currentFrame;
        private int m_totalFrames;

        private int m_individualTextureWidth;
        private int m_individualTextureHeight;

        public AnimatedSprite( Texture2D texture, int rows, int columns)
        {
            m_texture = texture;
            m_rows = rows;
            m_columns = columns;

            m_currentFrame = 0;
            m_totalFrames = rows * columns;

            m_individualTextureWidth = m_texture.Width / m_columns;
            m_individualTextureHeight = m_texture.Height / m_rows;
        }

        public void Update()
        {
            //Update to next frame in sprite sheet
            m_currentFrame++;

            if (m_currentFrame == m_totalFrames)
                m_currentFrame = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Rectangle sprite = GetCurrentSprite();
            Rectangle mapPosition = GetMapPosition(location);
            //Draws the current sprite, using current sprite in the sprite sheet as the source rectangle, and 
            //mapPosition as the position on the world map where it will be drawn
            spriteBatch.Begin();
            spriteBatch.Draw(m_texture, mapPosition, sprite, Color.White);
            spriteBatch.End();
        }

        private Rectangle GetCurrentSprite()
        {
            //Gets the current sprite within sprite sheet by looking at the row / column, and individual length / height of the sprite.
            int row = (int)Math.Floor((float)m_currentFrame / (float)m_columns);
            int column = m_currentFrame % m_columns;

            Rectangle sprite = new Rectangle(m_individualTextureWidth * column, m_individualTextureHeight * row, m_individualTextureWidth, m_individualTextureHeight);

            return sprite;
        }

        private Rectangle GetMapPosition(Vector2 location)
        {
            Rectangle mapPosition = new Rectangle((int)location.X, (int)location.Y, m_individualTextureWidth, m_individualTextureHeight);

            return mapPosition;
        }
    }
}
