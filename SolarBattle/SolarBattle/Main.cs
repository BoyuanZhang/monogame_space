#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using SolarBattle.Sprites;
using SolarBattle.Camera;
using SolarBattle.LevelMaps;
using SolarBattle.MiniMap;
using SolarBattle.CollisionEngine;
#endregion

namespace SolarBattle
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    
    public class Main : Game
    {
        //Global debug variables ------------------------------------------------------------------------------------------

        //Global debug variables ------------------------------------------------------------------------------------------

        //Testing Variables -----------------------------------------------------------------------------------------------
        //time interval can be used for fps tracking through the console.
        static float g_timeInterval = 0;
        //Testing Variables -----------------------------------------------------------------------------------------------

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int screenWidth = 1300;
        public const int screenHeight = 800;

        private LevelOne m_levelOne;
        private PlayerShip m_playerShip;
        private PlayerCamera m_playerCamera;
        private Collision m_collisionEngine;
        private Minimap m_miniMap;
        
        public Main()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
          
            CenterWindow();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        public void CenterWindow()
        {
            //Bug in MonoGame does not allow us to set the position of the window through the code because we cannot get a handle to the OTKWindow
            //Workaround is the use reflection to access the private instance of the GameWindow within OpenTK, and then manually set the variables
            GameWindowHandler.SetPosition(this.Window, new Point(0, 0));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            
            //Load in level one map, with asteroids texture
            m_levelOne = new LevelOne(Content.Load<Texture2D>("BasicBlueBackground"), Content.Load<Texture2D>("Asteroid"));
            
            //Load in player ship with bullet textures
            m_playerShip = new PlayerShip(Content.Load<Texture2D>("PlayerShip"), Content.Load<Texture2D>("Bullet"));

            //Initialize player camera (focused on player)
            m_playerCamera = new PlayerCamera(GraphicsDevice.Viewport, 0.5f);

            //Initialize collision engine, with ship, and level
            m_collisionEngine = new Collision(m_levelOne, m_playerShip);

            //Load in mini map with level, camera, and required mini map textures such as... camera / enemies / allies
            //Mini-map needs handle to graphics device & spritebatch to initialize the map texture that will be drawn upon load
            m_miniMap = new Minimap(GraphicsDevice, spriteBatch, m_levelOne, m_playerCamera, Content.Load<Texture2D>("PlayerSphere"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
            m_playerShip.Update();
            m_playerCamera.Update(m_playerShip);
            //Check for collisions
            m_collisionEngine.HandleCollisions();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            // TODO: Add your drawing code here
            base.Draw(gameTime);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, m_playerCamera.getTransform());
            GraphicsDevice.Clear(Color.Black);
            m_levelOne.Draw(spriteBatch);
            m_playerShip.Draw(spriteBatch);

            spriteBatch.End();

            //Second sprite batch is for objects that are independent of the camera view, such as menus or the mini map
            //This sprite batch allows for non pre-multiplied blending (Transparent textures)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            m_miniMap.Draw(spriteBatch, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
            spriteBatch.End();


            //Uncomment for frame rate testing ----------------------------------------------------------------------------------
            double frameRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;

            g_timeInterval += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (g_timeInterval > 5)
            {
                System.Console.WriteLine(frameRate);
                g_timeInterval = 0;
            }
            //Uncomment for frame rate testing ----------------------------------------------------------------------------------
        }
    }
}
