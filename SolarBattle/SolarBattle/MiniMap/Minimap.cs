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

        private PlayerCamera m_playerCamera;

        private Rectangle m_miniMapRectangle;
        private Rectangle m_playerRect;

        private Texture2D m_cameraTexture;

        private GraphicsDevice m_graphicsDevice;
        private RenderTarget2D m_mapRenderTarget;

        private float m_msTimeInterval;

        public Minimap(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, LevelOne levelMap, PlayerCamera playerCamera, Texture2D cameraTexture)
        {
            //Process minimap texture once, then when updating mini-map we only show the areas that need to be shown
            m_graphicsDevice = graphicsDevice;
            m_playerCamera = playerCamera;
            m_miniMapRectangle = new Rectangle(mapOriginX, mapOriginY, miniMapWidth, miniMapHeight);
            m_cameraTexture = cameraTexture;
            m_msTimeInterval = 0;

            initializeMapSprites();
            initializeRenderTarget(spriteBatch, levelMap);
        }

        //Initialize all sprites the minimap will use. If mini map initialization is not called, the mini map sprites will not be drawn.
        private void initializeMapSprites()
        {
            m_playerRect = new Rectangle(mapOriginX + (miniMapWidth/2)-2, mapOriginY + (miniMapHeight/2)-2, 4, 4);
        }

        //Draw the level to a 2D render target once, and use this map as the texture as the source rectangle to draw to the mini map
        private void initializeRenderTarget(SpriteBatch spriteBatch, LevelOne levelMap)
        {
            //4048 represents map height / width
            m_mapRenderTarget = new RenderTarget2D(m_graphicsDevice, 4048,  4048);
            m_graphicsDevice.SetRenderTarget(m_mapRenderTarget);

            spriteBatch.Begin();
            levelMap.Draw(spriteBatch);
            spriteBatch.End();

            m_graphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch, float elapsedMS)
        {
            m_msTimeInterval += elapsedMS;
            Vector2 cameraCenter = m_playerCamera.getCameraFocus();
            Rectangle cameraAreaRect = new Rectangle((int)cameraCenter.X - LevelOne.mapWidth / 2, (int)cameraCenter.Y - LevelOne.mapHeight / 2, LevelOne.mapWidth, LevelOne.mapHeight);

            spriteBatch.Draw((Texture2D)m_mapRenderTarget, m_miniMapRectangle, cameraAreaRect, Color.GreenYellow * 0.6f);

            //draw player at center of the mini map once every 500ms
            if (m_msTimeInterval >= 500)
            {
                spriteBatch.Draw(m_cameraTexture, m_playerRect, Color.White);
                m_msTimeInterval = 0;
            }
        }
    }
}
