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
    /// Solar battle is a game written in Monogame mainly for learning purposes with the Monogame API, and game / graphical programming in General
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

        //Screen management textures such as, Main menu.. Loading screen.. Game over etc...
        private Texture2D m_scrmngr_loadScreenText;
        private Texture2D m_scrmngr_gameOverScreen;

        public enum GameState
        {
            Menu = 1,
            Run = 2,
            Gameover = 3,
            Reset = 4,
            Pause = 5,
            Quit = 6,
            LoadScreen = 7
        }

        public static GameState gameState = GameState.Run;
        
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

        //Currently Main is the temporary Screen manager for game over / restart etc... We should use an actual screen manager object in the future
        //Reset manager
        public void ResetGame()
        {
            gameState = GameState.Reset;
        }

        //Gameover manager
        public void GameOver()
        {
            gameState = GameState.Gameover;
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
            
            //Load in player ship with bullet textures
            m_playerShip = new PlayerShip(Content.Load<Texture2D>("PlayerShip"), Content.Load<Texture2D>("Bullet"), Content.Load<Texture2D>("PlayerEnergy"));

            //Load in level one map with player, and required level textures, such as the asteroids, enemies, and enemy bullets
            //This is pretty hack, should probably call a LoadContent function within the level class, and initiate all required textures there.
            m_levelOne = new LevelOne(m_playerShip, Content.Load<Texture2D>("BasicBlueBackground"), Content.Load<Texture2D>("Asteroid"), Content.Load<Texture2D>("EnemyShip"), Content.Load<Texture2D>("EnemyBullet"));

            //Initialize player camera (focused on player)
            m_playerCamera = new PlayerCamera(GraphicsDevice.Viewport, 0.5f);

            //Load in mini map with level, camera, and required mini map textures such as... camera / enemies / allies
            //Mini-map needs handle to graphics device & spritebatch to initialize the map texture that will be drawn upon load
            m_miniMap = new Minimap(GraphicsDevice, spriteBatch, m_levelOne, m_playerCamera, Content.Load<Texture2D>("PlayerSphere"), Content.Load<Texture2D>("EnemySphere"));

            //Initialize collision engine, with ship, and level, and the minimap (to see if sprites should be displayed in the mini-map or not)
            m_collisionEngine = new Collision(this, m_levelOne, m_playerShip, m_miniMap);

            //Load in Screen management textures

            m_scrmngr_loadScreenText = Content.Load<Texture2D>("LoadingScreen");
            m_scrmngr_gameOverScreen = Content.Load<Texture2D>("GameOverScreen");
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

            if (gameState == GameState.Run)
            {
                // TODO: Add your update logic here
                base.Update(gameTime);
                m_playerShip.Update();
                m_playerCamera.Update(m_playerShip);
                m_levelOne.Update();
                m_miniMap.Update();
                //Check for collisions
                m_collisionEngine.HandleCollisions();
            }
            else if (gameState == GameState.Gameover)
            {
                //Press enter to restart game
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    gameState = GameState.Reset;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
            }
            else if (gameState == GameState.Reset)
            {
                m_playerShip.Reset();
                m_levelOne.Reset();
                m_miniMap.Reset(spriteBatch, GraphicsDevice);

                gameState = GameState.Run;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            if (gameState == GameState.Run)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, m_playerCamera.getTransform());
                GraphicsDevice.Clear(Color.Black);
                m_levelOne.Draw(spriteBatch);
                m_playerShip.Draw(spriteBatch);

                spriteBatch.End();

                //Second sprite batch is for objects that are independent of the camera view, such as menus or the mini map
                //This sprite batch allows for non pre-multiplied blending (Transparent textures)
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                m_miniMap.Draw(spriteBatch, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
                m_playerShip.DrawEnergy(spriteBatch);
                spriteBatch.End();
            }
            else if (gameState == GameState.Reset)
            {
                //Draw Loading Screen
                spriteBatch.Begin();
                spriteBatch.Draw(m_scrmngr_loadScreenText, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            else if (gameState == GameState.Gameover)
            {
                //Draw Game over Screen
                spriteBatch.Begin();
                spriteBatch.Draw(m_scrmngr_gameOverScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }


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
