using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CollisionDetection
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CollisionDetection : Game
    {
        public const int ScreenWidth = 1920, ScreenHeight = 1080;
        public const float AspectRatio = (float)ScreenWidth / (float)ScreenHeight,
            TopBound = 5000f, BottomBound = -5000f,
            LeftBound = -5000f, RightBound = 5000f,
            FrontBound = 5000f, BackBound = -5000f;
        Random _random;
        public Random Random { get { return _random; } }

        const int NumberOfShips = 10;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        // Set the position of the camera in world space, for our view matrix.
        Vector3 _cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
        SpaceShip[] _spaceShips;

        SpriteFont _fpsFont;
        int _frameRate, _frameCount;
        TimeSpan _elapsedFrameTime = TimeSpan.Zero;

        public CollisionDetection()
        {
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            // TODO: uncomment this while demonstrating
            // running fullscreen does not work well with debug mode
            //if (!_graphics.IsFullScreen)
            //    _graphics.ToggleFullScreen();
            // Once we are using non-default resolution we need to apply changes
            _graphics.ApplyChanges();
            IsMouseVisible = true;
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _random = new Random();

            _spaceShips = new SpaceShip[NumberOfShips];
            for (int i = 0; i < NumberOfShips; i++)
                _spaceShips[i] = new SpaceShip(this);

            _fpsFont = Content.Load<SpriteFont>("Models\\Font");

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Each spaceship updates itself
            for (int i = 0; i < NumberOfShips; i++)
                _spaceShips[i].Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, _spaceShips);


            // From: http://blogs.msdn.com/b/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
            _elapsedFrameTime += gameTime.ElapsedGameTime;

            if (_elapsedFrameTime > TimeSpan.FromSeconds(1))
            {
                _elapsedFrameTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCount;
                _frameCount = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _frameCount++;
            string fps = "FPS: " + _frameRate;

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_fpsFont, fps, new Vector2(32, 32), Color.White);
            _spriteBatch.End();

            //Sprite Batch messes some settings up for 3D drawing, this resets these settings 
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Each Spaceship Draws itself
            for (int i = 0; i < NumberOfShips; i++)
                _spaceShips[i].Draw(_cameraPosition);

            base.Draw(gameTime);
        }
    }
}
