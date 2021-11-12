using System;
using System.Linq;
using FZtarOGL.Asset;
using FZtarOGL.Profiler;
using FZtarOGL.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;
using gs = FZtarOGL.GameSettings.GameSettings;

namespace FZtarOGL
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private readonly string GameTitle;
        private uint _ticks, _frames, _lastUps, _lastFps; // Actual ups, fps.

        private float _deltaTime, _fps;
        private double _countTicksAndFramesTimer, _runTime;
        private int _smoothedFps; // Not really useful.

        private double _windowResizeTimer;

        private const int MaxWindowWidth = 7680, MaxWindowHeight = 4320; // Max 8k resolution.
        private const int MinWindowWidth = 256, MinWindowHeight = 224; // SNES resolution.
        private int windowScale = 4; // Only scales the window.
        private const int FboWidth = 256, FboHeight = 224;
        private int _windowedLastWidth, _windowedLastHeight;

        private RenderTarget2D _fbo;
        private Texture2D _fboTexture;

        private AssetManager _assMan;

        private ScreenManager _screenManager;

        public ScreenManager ScreenManager => _screenManager;

        public Game1()
        {
            GameTitle = "Frag Ztar";
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;

            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.HardwareModeSwitch = false; // Faster on my machine (Linux).
            _graphics.PreferredBackBufferWidth = MinWindowWidth * windowScale;
            _graphics.PreferredBackBufferHeight = MinWindowHeight * windowScale;
            const int desiredMaxFps = 60; // Desired fps. Not ACTUAL FPS.
            TargetElapsedTime = TimeSpan.FromSeconds(1d / desiredMaxFps); // Cap max FPS.

            _graphics.ApplyChanges();

            _windowedLastWidth = Window.ClientBounds.Width;
            _windowedLastHeight = Window.ClientBounds.Height;

            GameSettings.GameSettings.MusicVolume = 0; // change!

            base.Initialize();
            
            Window.Title = GameTitle;
        }
        
        private void UpdateWindowResize(GameTime gameTime)
        {
            _windowResizeTimer += gameTime.ElapsedGameTime.TotalSeconds;
            const double windowResizeTime = 0.2d;

            // Checking for window resize every frame is no good idea.
            if (_windowResizeTimer > windowResizeTime)
            {
                if (!_graphics.IsFullScreen)
                {
                    if (Window.ClientBounds.Width != _graphics.PreferredBackBufferWidth ||
                        Window.ClientBounds.Height != _graphics.PreferredBackBufferHeight)
                    {
                        _graphics.PreferredBackBufferWidth =
                            Math.Clamp(Window.ClientBounds.Width, MinWindowWidth, MaxWindowWidth);
                        _graphics.PreferredBackBufferHeight =
                            Math.Clamp(Window.ClientBounds.Height, MinWindowHeight, MaxWindowHeight);

                        _graphics.ApplyChanges();
                    }
                }

                _windowResizeTimer = 0;
            }
        }
        
        private void UpdateWindowFullscreen()
        {
            _graphics.ToggleFullScreen();

            if (_graphics.IsFullScreen)
            {
                _windowedLastWidth = _graphics.PreferredBackBufferWidth;
                _windowedLastHeight = _graphics.PreferredBackBufferHeight;

                _graphics.PreferredBackBufferWidth =
                    Math.Clamp(Window.ClientBounds.Width, MinWindowWidth, MaxWindowWidth);
                _graphics.PreferredBackBufferHeight =
                    Math.Clamp(Window.ClientBounds.Height, MinWindowHeight, MaxWindowHeight);
            }
            else
            {
                _graphics.PreferredBackBufferWidth = _windowedLastWidth;
                _graphics.PreferredBackBufferHeight = _windowedLastHeight;
            }

            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _assMan = new AssetManager(Content);
            _assMan.LoadAllAssets();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _fbo = new RenderTarget2D(_graphics.GraphicsDevice,
                FboWidth,
                FboHeight, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            _fboTexture = _fbo;

            _screenManager = new ScreenManager();
            _screenManager.AddScreen(new TitleScreen(this, _assMan, _spriteBatch, GraphicsDevice));
            //_screenManager.AddScreen(new PlayScreen(this, _assMan, _spriteBatch, GraphicsDevice));
        }
        
        private void UpdateRunTime(GameTime gameTime)
        {
            _runTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        private void CountTicksAndFramesPerSecond(GameTime gameTime)
        {
            _countTicksAndFramesTimer += gameTime.ElapsedGameTime.TotalSeconds;

            const double oneSecond = 1;
            if (_countTicksAndFramesTimer > oneSecond)
            {
                _lastUps = _ticks;
                _lastFps = _frames;

                _ticks = 0;
                _frames = 0;

                _countTicksAndFramesTimer--;
            }
        }
        
        private void CalculateDeltaTime(GameTime gameTime)
        {
            _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fps = 1 / _deltaTime;
            _smoothedFps = (int)Math.Round(_fps);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateWindowResize(gameTime);

            UpdateRunTime(gameTime);
            CountTicksAndFramesPerSecond(gameTime);
            CalculateDeltaTime(gameTime);
            
            _screenManager.Tick(); // On first tick: this is when the screen actually gets set.
            
            if (gs.DebugUpdateWindowTitle) UpdateWindowTitle();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.F))
            {
                UpdateWindowFullscreen();
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.F1))
            {
                gs.DebugRenderDebugGui = !gs.DebugRenderDebugGui;
            }
            
            _screenManager.GameScreens.First().Input(gameTime, _deltaTime);
            _screenManager.GameScreens.First().Tick(gameTime, _deltaTime);

            base.Update(gameTime);

            _ticks++;
        }

        private void UpdateWindowTitle()
        {
            var windowTitle = GameTitle + "    " + _lastFps + " FPS / " + _lastUps + " UPS " + _deltaTime +
                              " ms    Screens: " + _screenManager.GameScreens.Count + "    Ents: " +
                              _screenManager.GameScreens.First()._Entities.Count;
            Window.Title = windowTitle;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _graphics.GraphicsDevice.SetRenderTarget(_fbo); // render onto fbo
            _screenManager.GameScreens.First().Draw(_deltaTime);
            _screenManager.GameScreens.First().DrawGUI(_deltaTime);
            _graphics.GraphicsDevice.SetRenderTarget(null); // stop fbo rendering

            // Draw the FBO onto screen
            _fboTexture = _fbo;
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            var fboScaleWidth = (float)_graphics.PreferredBackBufferWidth / FboWidth;
            var fboScaleHeight = (float)_graphics.PreferredBackBufferHeight / FboHeight;
            _spriteBatch.Draw(_fboTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero,
                new Vector2(fboScaleWidth, fboScaleHeight),
                SpriteEffects.None, 0);
            _spriteBatch.End();

            // Update profiler stats
            if (gs.DebugRenderDebugGui)
            {
                ProfilerStats.DrawCalls = _graphics.GraphicsDevice.Metrics.DrawCount;
                ProfilerStats.ClearCalls = _graphics.GraphicsDevice.Metrics.ClearCount;
                ProfilerStats.TargetSwitches = _graphics.GraphicsDevice.Metrics.TargetCount;
                ProfilerStats.TextureSwitches = _graphics.GraphicsDevice.Metrics.TextureCount;
                ProfilerStats.FragmentShaderSwitches = _graphics.GraphicsDevice.Metrics.PixelShaderCount;
                ProfilerStats.VertexShaderSwitches = _graphics.GraphicsDevice.Metrics.VertexShaderCount;
                ProfilerStats.PrimitivesDrawn = _graphics.GraphicsDevice.Metrics.PrimitiveCount;
                ProfilerStats.SpritesDrawn = _graphics.GraphicsDevice.Metrics.SpriteCount;
                ProfilerStats.ModelsDrawn = _screenManager.GameScreens.First().ModelsDrawn;
                
                _screenManager.GameScreens.First().DrawDebugGUI(gameTime);
            }

            base.Draw(gameTime);

            _frames++;
        }
        
        protected override void OnExiting(object sender, EventArgs args)
        {
            MediaPlayer.Stop();
            
            _screenManager.RemoveAllScreens();

            _spriteBatch.Dispose();

            _graphics.Dispose();

            base.OnExiting(sender, args);
        }
        
        public uint LastUps => _lastUps;

        public uint LastFps => _lastFps;

        public float DeltaTime => _deltaTime;
    }
}