using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarBattle.Sprites
{
    public class Asteroid : Sprite
    {
        public Asteroid( Texture2D asteroidTexture, Vector2 position  )
            : base( asteroidTexture, position)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture, GeneralSpriteBox, Color.White);
        }

       // public Rectangle SpecificSpriteBox { get { return new Rectangle( )} }
    }
}
