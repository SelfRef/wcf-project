using System;
using WCFReference.GameScenes.Base;
using Microsoft.Xna.Framework;
using WCFReference;

namespace GameWindow.GameScenes
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SceneManager : Game
    {
        public IConnection Connection { get; set; }
        public bool multi = false;

        private GraphicsDeviceManager graphics;
        private IGameScene cur;
        private bool applySettings;

        public SceneManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = false;
            Window.ClientSizeChanged += (s, e) =>
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                applySettings = true;
            }; 
        }

        public SceneManager(int width, int height) : this()
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            applySettings = false;
        }

        public SceneManager(int width, int height, IConnection connect) : this(width, height)
        {
            Connection = connect;
            multi = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cur = new MainScene(this);
            cur.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            cur.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            cur.Update(gameTime);
            base.Update(gameTime);
            if (applySettings && IsActive) graphics?.ApplyChanges();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            cur.Draw(gameTime);
            base.Draw(gameTime);
        }
        public void ExitGame() // Helping method to handle exceptions.
        {
            try
            {
                Exit();
            }
            catch (Exception)
            {

            }
        }
    }
}
