using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.LevelMaps;
using SolarBattle.Sprites;
using SolarBattle.Camera;

namespace SolarBattle.MiniMap
{
    public class Minimap
    {
        //Positions of the minimap and center of the minimap, relative to the screen
        private const int miniMapWidth = 190;
        private const int miniMapHeight = 190;

        private const int mapOriginX = Main.screenWidth - miniMapWidth - 5;
        private const int mapOriginY = Main.screenHeight - miniMapHeight - 5;

        //Since level width and height are the same, just use width in this case
        //However if the level width changes, the divisor must also be changed to compensate for appropriate scale
        private float miniMapScale = (LevelOne.mapWidth/2.0f) / ((float)miniMapWidth);

        private PlayerCamera m_playerCamera;

        private Rectangle m_miniMapRectangle;
        private Rectangle m_playerRect;
        private Rectangle m_miniMapAreaRect;

        private Texture2D m_cameraTexture;
        private Texture2D m_enemyTexture;

        private RenderTarget2D m_mapRenderTarget;

        private LevelOne m_levelMap;

        private List<Sprite> m_miniMapSprites;

        private float m_msTimeInterval;

        public Minimap(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, LevelOne levelMap, PlayerCamera playerCamera, Texture2D cameraTexture, Texture2D enemyTexture)
        {
            //Process minimap texture once, then when updating mini-map we only show the areas that need to be shown
            m_playerCamera = playerCamera;
            m_miniMapRectangle = new Rectangle(mapOriginX, mapOriginY, miniMapWidth, miniMapHeight);

            m_cameraTexture = cameraTexture;
            m_enemyTexture = enemyTexture;

            m_levelMap = levelMap;

            m_miniMapSprites = new List<Sprite>();

            m_msTimeInterval = 0;

            initializeMapRepresentation();
            initializeRenderTarget(spriteBatch, graphicsDevice);
        }

        //Initialize all sprites the minimap will use. If mini map initialization is not called, the mini map sprites will not be drawn.
        private void initializeMapRepresentation()
        {
            m_playerRect = new Rectangle(mapOriginX + (miniMapWidth / 2) - 2, mapOriginY + (miniMapHeight / 2) - 2, 4, 4);
        }

        //Update sprites on minimap, such as enemy ships
        public void Update()
        {
            //clear list of mini map sprite items
            m_miniMapSprites.Clear();

            //Update the minimap area rectangle (in case player has moved)
            Vector2 cameraCenter = m_playerCamera.getCameraFocus();

            m_miniMapAreaRect = new Rectangle((int)cameraCenter.X - LevelOne.mapWidth / 4, (int)cameraCenter.Y - LevelOne.mapHeight / 4, LevelOne.mapWidth/2, LevelOne.mapHeight/2);
        }

        public void Reset( SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (m_miniMapSprites.Count > 0)
                m_miniMapSprites.Clear();

            m_mapRenderTarget.Dispose();
            initializeRenderTarget(spriteBatch, graphicsDevice);
        }

        public void SpriteCollisionHandler(Sprite sprite)
        {
            //If a sprite is within the mini map area rectangle, add it to a list of sprites that will be drawn
            m_miniMapSprites.Add(sprite);
        }

        //Draw the level to a 2D render target once, and use this map as the texture as the source rectangle to draw to the mini map
        private void initializeRenderTarget(SpriteBatch spriteBatch, GraphicsDevice graphicsdevice)
        {
            m_mapRenderTarget = new RenderTarget2D(graphicsdevice, LevelOne.mapWidth, LevelOne.mapHeight);
            graphicsdevice.SetRenderTarget(m_mapRenderTarget);

            spriteBatch.Begin();
            m_levelMap.DrawMiniMap(spriteBatch);
            spriteBatch.End();

            graphicsdevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch, float elapsedMS)
        {
            m_msTimeInterval += elapsedMS;

            spriteBatch.Draw((Texture2D)m_mapRenderTarget, m_miniMapRectangle, m_miniMapAreaRect, Color.GreenYellow * 0.6f);

            //draw player at center of the mini map once every 500ms
            if (m_msTimeInterval >= 500)
            {
                spriteBatch.Draw(m_cameraTexture, m_playerRect, Color.White);
                m_msTimeInterval = 0;
            }

            //draw any other sprites that are contained within the mini map
            foreach (Sprite sprite in m_miniMapSprites)
            {
                Rectangle enemyRectangle = CalculateSpriteOnMiniMap(sprite.GetCenter(), m_playerCamera.getCameraFocus());

                spriteBatch.Draw(m_enemyTexture, enemyRectangle, Color.White);
            }
        }

        public Rectangle GetMiniMapAreaRectangle()
        {
            return m_miniMapAreaRect;
        }

        private Rectangle CalculateSpriteOnMiniMap(Vector2 spritePos, Vector2 playerPos)
        {
            //Find vector difference
            Vector2 spriteOffset = Vector2.Subtract(playerPos, spritePos);
            
            //Scale vector difference from area mini map represents to actual mini map
            spriteOffset = Vector2.Divide(spriteOffset, miniMapScale);

            //Translate from map to mini-map
            spriteOffset.X = m_playerRect.Left - spriteOffset.X;
            spriteOffset.Y = m_playerRect.Top - spriteOffset.Y;

            //return rectangle
            return new Rectangle((int)spriteOffset.X, (int)spriteOffset.Y, 4, 4);
        }
    }
}
