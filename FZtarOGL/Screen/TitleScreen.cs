using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.GUI.Debug;
using FZtarOGL.Level;
using FZtarOGL.Utilities;
using ImGuiHandler.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;

namespace FZtarOGL.Screen
{
    public class TitleScreen : Screen
    {
        //private AssetManager assMan;

        private GraphicsDevice _graphicsDevice;
        private OrthographicCamera _camera;

        //private BitmapFont _font01_32, _font01_16, _font01_08;
        private BitmapFont _font02_32, _font02_16, _font02_08;
        //private BitmapFont _font03_32, _font03_16, _font03_08;

        private Texture2D _titleTex;
        private float _ztarPosX, _fragPosX, _fragPosXEnd, _ztarPosXEnd;
        private float _creditsPosX = GameConstants.SnesWidth + 32;

        private readonly int _rectBeginX = 1,
            _rect01BeginY = 18,
            _rect02BeginY = 51,
            _rectWidth = 140,
            _rectHeight = 32;

        private Rectangle titleRect01;
        private Rectangle titleRect02;

        private Menu _menu;

        private MenuChoice _menuChoice;

        //private bool _canContinue;
        private Color _continueColor;

        private String credits01 = "v" + GameConstants.VersionMajor + "." + GameConstants.VersionMinor +
                                   " Made by Christian \"PilzHere\" Pilz for FZ Game Jam 2021                Music and SFX by Juhani Junkala                PILZHERE.NET                Sourcecode: github.com/pilzhere/FZtar";

        private const String Choice01 = "START";
        private const String Choice02 = "CONTINUE";
        private const String Choice03 = "SHOW CONTROLS";
        private const String Choice04 = "OPTIONS";
        private const String Choice05 = "QUIT";

        private const String Controls01 = "W/S - MOVE UP/DOWN";
        private const String Controls02 = "A/D - MOVE LEFT/RIGHT";
        private const String Controls03 = "ARROW UP - SHOOT";
        private const String Controls04 = "ARROW LEFT - SHIELD";
        private const String Controls05 = "F - TOGGLE FULLSCREEN";
        private const String Controls06 = "F1 - SHOW DEBUG DATA";

        private const String Options01 = "ENTER - TOGGLE";
        private const String Options02 = "INVERT UP/DOWN:";
        private const String Options03 = "ON";
        private const String Options04 = "OFF";

        private const String ChoiceArrowLeft = ">";
        private const String ChoiceArrowRight = "<";
        private float _choiceArrowLeftX, _choiceArrowLeftY;
        private float _choiceArrowRightX, _choiceArrowRightY;

        public TitleScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) :
            base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _graphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            _graphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;

            _titleTex = AssMan.Title;
            titleRect01 = new Rectangle(_rectBeginX, _rect01BeginY, _rectWidth, _rectHeight);
            titleRect02 = new Rectangle(_rectBeginX, _rect02BeginY, _rectWidth, _rectHeight);

            _ztarPosX = 0 - titleRect01.Width / 2f;
            _fragPosXEnd = GameConstants.SnesWidth / 2f - titleRect01.Width / 2f + 140 / 4f;
            _fragPosX = GameConstants.SnesWidth + titleRect02.Width / 2f;
            _ztarPosXEnd = GameConstants.SnesWidth / 2f - titleRect02.Width / 2f - 140 / 4f - 2;

            /*_font01_32 = assMan.Font01_32;
            _font01_16 = assMan.Font01_16;
            _font01_08 = assMan.Font01_08;*/

            _font02_32 = assMan.Font02_32;
            _font02_16 = assMan.Font02_16;
            _font02_08 = assMan.Font02_08;

            /*_font03_32 = assMan.Font03_32;
            _font03_16 = assMan.Font03_16;
            _font03_08 = assMan.Font03_08;*/

            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, GameConstants.SnesWidth,
                GameConstants.SnesHeight);
            _camera = new OrthographicCamera(viewportAdapter);

            Cam3d1 = new PerspectiveCamera(_graphicsDevice, game.Window);
            Cam3d1.Position = new Vector3(0, 1, 0);
            _cam3dPos = Cam3d1.Position;
            Cam3d1.LookAtDirection = Vector3.Forward;
            Cam3d1.MovementUnitsPerSecond = 10f;
            Cam3d1.farClipPlane = 200;
            Cam3d1.nearClipPlane = 0.05f;
            Cam3d1.fieldOfViewDegrees = 45;
            Cam3d1.RotationRadiansPerSecond = 100f;

            CurrentLevel = new LevelTitleScreen(this, assMan, spriteBatch);

            _menu = Menu.Main;
            _menuChoice = MenuChoice.Start;
        }

        public override void Reset()
        {
            _menu = Menu.Main;
            _menuChoice = MenuChoice.Start;

            CurrentLevel.ResetMusic();
        }

        private bool _keyUp01IsUp, _keyDown01IsUp, _keyUp02IsUp, _keyDown02IsUp, _keyEnterIsUp, _keyEscIsUp;

        public override void Input(GameTime gt, float dt)
        {
            if (KeyboardExtended.GetState().IsKeyUp(Keys.Escape))
            {
                _keyEscIsUp = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Escape))
            {
                if (_keyEscIsUp)
                {
                    SoundEffectInstance sfxCancel = AssMan.SfxCancel.CreateInstance();
                    sfxCancel.Volume = GameSettings.GameSettings.SfxVolume;
                    sfxCancel.Play();
                    
                    switch (_menu)
                    {
                        case Menu.Main:
                            _menuChoice = MenuChoice.Quit;
                            break;
                        case Menu.ShowControls:
                            _menu = Menu.Main;
                            _menuChoice = MenuChoice.ShowControls;
                            break;
                        case Menu.Options:
                            _menu = Menu.Main;
                            _menuChoice = MenuChoice.Options;
                            break;
                    }

                    _keyEscIsUp = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(Keys.Enter))
            {
                _keyEnterIsUp = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Enter))
            {
                if (_keyEnterIsUp)
                {
                    switch (_menu)
                    {
                        case Menu.Main:
                            switch (_menuChoice)
                            {
                                case MenuChoice.Start:
                                    CurrentLevel.StopMusic();
                                    
                                    SoundEffectInstance sfxConfirm = AssMan.SfxConfirm.CreateInstance();
                                    sfxConfirm.Volume = GameSettings.GameSettings.SfxVolume;
                                    sfxConfirm.Play();
                                    
                                    var ps = new PlayScreen(Game, AssMan, SpriteBatch,
                                        _graphicsDevice);
                                    
                                    // TEST LEVELS HERE!
                                    ps.LoadLevel(1);
                                    
                                    Game.ScreenManager.AddScreen(ps);
                                    break;

                                case MenuChoice.Continue:
                                    switch (Game.ContinueLevel)
                                    {
                                        case ContinueLevel.Level01:
                                            CurrentLevel.StopMusic();
                                            
                                            SoundEffectInstance sfxConfirm01 = AssMan.SfxConfirm.CreateInstance();
                                            sfxConfirm01.Volume = GameSettings.GameSettings.SfxVolume;
                                            sfxConfirm01.Play();
                                            
                                            var ps01 = new PlayScreen(Game, AssMan, SpriteBatch,
                                                _graphicsDevice);
                                            ps01.LoadLevel(1);
                                            Game.ScreenManager.AddScreen(ps01);
                                            break;

                                        case ContinueLevel.Level02:
                                            CurrentLevel.StopMusic();
                                            
                                            SoundEffectInstance sfxConfirm02 = AssMan.SfxConfirm.CreateInstance();
                                            sfxConfirm02.Volume = GameSettings.GameSettings.SfxVolume;
                                            sfxConfirm02.Play();
                                            
                                            var ps02 = new PlayScreen(Game, AssMan, SpriteBatch,
                                                _graphicsDevice);
                                            ps02.LoadLevel(2);
                                            Game.ScreenManager.AddScreen(ps02);
                                            break;

                                        case ContinueLevel.Level03:
                                            CurrentLevel.StopMusic();
                                            
                                            SoundEffectInstance sfxConfirm03 = AssMan.SfxConfirm.CreateInstance();
                                            sfxConfirm03.Volume = GameSettings.GameSettings.SfxVolume;
                                            sfxConfirm03.Play();
                                            
                                            var ps03 = new PlayScreen(Game, AssMan, SpriteBatch,
                                                _graphicsDevice);
                                            ps03.LoadLevel(3);
                                            Game.ScreenManager.AddScreen(ps03);
                                            break;
                                        
                                        case ContinueLevel.None:
                                            SoundEffectInstance sfxCancelN = AssMan.SfxCancel.CreateInstance();
                                            sfxCancelN.Volume = GameSettings.GameSettings.SfxVolume;
                                            sfxCancelN.Play();
                                            break;
                                        
                                        default:
                                            SoundEffectInstance sfxCancelD = AssMan.SfxCancel.CreateInstance();
                                            sfxCancelD.Volume = GameSettings.GameSettings.SfxVolume;
                                            sfxCancelD.Play();
                                            break;
                                    }

                                    break;

                                case MenuChoice.ShowControls:
                                    SoundEffectInstance sfxConfirmSC = AssMan.SfxConfirm.CreateInstance();
                                    sfxConfirmSC.Volume = GameSettings.GameSettings.SfxVolume;
                                    sfxConfirmSC.Play();
                                    
                                    _menu = Menu.ShowControls;
                                    break;

                                case MenuChoice.Options:
                                    SoundEffectInstance sfxConfirmO = AssMan.SfxConfirm.CreateInstance();
                                    sfxConfirmO.Volume = GameSettings.GameSettings.SfxVolume;
                                    sfxConfirmO.Play();
                                    
                                    _menu = Menu.Options;
                                    break;

                                case MenuChoice.Quit:
                                    SoundEffectInstance sfxConfirmQ = AssMan.SfxConfirm.CreateInstance();
                                    sfxConfirmQ.Volume = GameSettings.GameSettings.SfxVolume;
                                    sfxConfirmQ.Play();
                                    
                                    Game.Exit();
                                    break;
                            }

                            break;
                        case Menu.ShowControls:
                            SoundEffectInstance sfxConfirmSC2 = AssMan.SfxConfirm.CreateInstance();
                            sfxConfirmSC2.Volume = GameSettings.GameSettings.SfxVolume;
                            sfxConfirmSC2.Play();
                            
                            _menu = Menu.Main;
                            _menuChoice = MenuChoice.ShowControls;
                            break;

                        case Menu.Options:
                            SoundEffectInstance sfxPointer = AssMan.SfxPointer.CreateInstance();
                            sfxPointer.Volume = GameSettings.GameSettings.SfxVolume;
                            sfxPointer.Play();
                            
                            GameSettings.GameSettings.InvertVerticalMovement =
                                !GameSettings.GameSettings.InvertVerticalMovement;
                            break;
                    }

                    _keyEnterIsUp = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(Keys.Up)) _keyUp01IsUp = true;
            if (KeyboardExtended.GetState().IsKeyUp(Keys.W)) _keyUp02IsUp = true;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Up))
            {
                if (_keyUp01IsUp)
                {
                    SoundEffectInstance sfxPointer = AssMan.SfxPointer.CreateInstance();
                    sfxPointer.Volume = GameSettings.GameSettings.SfxVolume;
                    sfxPointer.Play();
                    
                    _menuChoice--;
                    if (_menuChoice < MenuChoice.Start) _menuChoice = MenuChoice.Quit;

                    _keyUp01IsUp = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                if (_keyUp02IsUp)
                {
                    SoundEffectInstance sfxPointer = AssMan.SfxPointer.CreateInstance();
                    sfxPointer.Volume = GameSettings.GameSettings.SfxVolume;
                    sfxPointer.Play();
                    
                    _menuChoice--;
                    if (_menuChoice < MenuChoice.Start) _menuChoice = MenuChoice.Quit;

                    _keyUp02IsUp = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(Keys.Down)) _keyDown01IsUp = true;
            if (KeyboardExtended.GetState().IsKeyUp(Keys.S)) _keyDown02IsUp = true;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Down))
            {
                if (_keyDown01IsUp)
                {
                    SoundEffectInstance sfxPointer = AssMan.SfxPointer.CreateInstance();
                    sfxPointer.Volume = GameSettings.GameSettings.SfxVolume;
                    sfxPointer.Play();
                    
                    _menuChoice++;
                    if (_menuChoice > MenuChoice.Quit) _menuChoice = MenuChoice.Start;

                    _keyDown01IsUp = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                if (_keyDown02IsUp)
                {
                    SoundEffectInstance sfxPointer = AssMan.SfxPointer.CreateInstance();
                    sfxPointer.Volume = GameSettings.GameSettings.SfxVolume;
                    sfxPointer.Play();
                    
                    _menuChoice++;
                    if (_menuChoice > MenuChoice.Quit) _menuChoice = MenuChoice.Start;

                    _keyDown02IsUp = false;
                }
            }
        }

        public override void Tick(GameTime gt, float dt)
        {
            ResetCounters();

            transparentModels.Clear();

            RemoveDestroyedEntities();

            AddQueuedEntities();

            CheckCollisions(BoundingBoxesFiltered, dt);

            CurrentLevel.Tick(gt, dt);

            UpdateAllEntities(dt);

            OrderTransparentModelsByDistanceFromCam(Cam3d1);

            float titleSpeedX = 240;
            _ztarPosX += titleSpeedX * dt;
            _fragPosX -= titleSpeedX * 1.9f * dt;

            if (_ztarPosX > _fragPosXEnd) _ztarPosX = _fragPosXEnd;
            if (_fragPosX < _ztarPosXEnd) _fragPosX = _ztarPosXEnd;

            float creditsSpeedX = 60;
            _creditsPosX -= creditsSpeedX * dt;

            if (_creditsPosX < -_font02_16.MeasureString(credits01).Width) _creditsPosX = GameConstants.SnesWidth + 32;

            switch (_menu)
            {
                case Menu.Main:
                    switch (_menuChoice)
                    {
                        case MenuChoice.Start:
                            _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                                _font02_16.MeasureString(Choice01).Width / 2f -
                                                16;
                            _choiceArrowLeftY = GameConstants.SnesHeight / 2f - 16;
                            _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                                 _font02_16.MeasureString(Choice01).Width / 2f +
                                                 8;
                            _choiceArrowRightY = GameConstants.SnesHeight / 2f - 16;
                            break;
                        case MenuChoice.Continue:
                            _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                                _font02_16.MeasureString(Choice02).Width / 2f -
                                                16;
                            _choiceArrowLeftY = GameConstants.SnesHeight / 2f;
                            _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                                 _font02_16.MeasureString(Choice02).Width / 2f +
                                                 8;
                            _choiceArrowRightY = GameConstants.SnesHeight / 2f;
                            break;
                        case MenuChoice.ShowControls:
                            _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                                _font02_16.MeasureString(Choice03).Width / 2f -
                                                16;
                            _choiceArrowLeftY = GameConstants.SnesHeight / 2f + 16;
                            _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                                 _font02_16.MeasureString(Choice03).Width / 2f +
                                                 8;
                            _choiceArrowRightY = GameConstants.SnesHeight / 2f + 16;
                            break;
                        case MenuChoice.Options:
                            _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                                _font02_16.MeasureString(Choice04).Width / 2f -
                                                16;
                            _choiceArrowLeftY = GameConstants.SnesHeight / 2f + 16 * 2;
                            _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                                 _font02_16.MeasureString(Choice04).Width / 2f +
                                                 8;
                            _choiceArrowRightY = GameConstants.SnesHeight / 2f + 16 * 2;
                            break;
                        case MenuChoice.Quit:
                            _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                                _font02_16.MeasureString(Choice05).Width / 2f -
                                                16;
                            _choiceArrowLeftY = GameConstants.SnesHeight / 2f + 16 * 3;
                            _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                                 _font02_16.MeasureString(Choice05).Width / 2f +
                                                 8;
                            _choiceArrowRightY = GameConstants.SnesHeight / 2f + 16 * 3;
                            break;
                    }

                    break;
                case Menu.Options:
                    _choiceArrowLeftX = GameConstants.SnesWidth / 2f -
                                        _font02_16.MeasureString(Options04).Width / 2f -
                                        16;
                    _choiceArrowLeftY = GameConstants.SnesHeight / 2f + 16;
                    _choiceArrowRightX = GameConstants.SnesWidth / 2f +
                                         _font02_16.MeasureString(Options04).Width / 2f +
                                         8;
                    _choiceArrowRightY = GameConstants.SnesHeight / 2f + 16;
                    break;
            }
        }

        public override void Draw(float dt)
        {
            _graphicsDevice.Clear(Color.HotPink);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise,
                null, _camera.GetViewMatrix());

            CurrentLevel.DrawBackground(dt, -Cam3d1.Up.X);

            Draw2DAllEntities(dt);

            SpriteBatch.End();

            // For 3D we use the depth buffer.
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.BlendState = BlendState.AlphaBlend; // Needed?

            CurrentLevel.DrawGroundEffect(Cam3d1, dt);

            // For debugging - red line at end
            /*Vector3 startPoint = new Vector3(-10, 0, -200);
            Vector3 endPoint = new Vector3(10, 0, -200);
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            var vertices2 = new[]
                { new VertexPositionColor(startPoint, Color.Red), new VertexPositionColor(endPoint, Color.Red) };
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices2, 0, 1);*/

            Draw3DAllEntities(dt);

            CurrentLevel.DrawFloor();

            foreach (var transModel in TransparentModels)
            {
                if (transModel.Lit) DrawModel(transModel.Model, transModel.ModelTrans);
                else DrawModelUnlit(transModel.Model, transModel.ModelTrans);
            }

            if (GameSettings.GameSettings.DebugRenderBoundingBoxes) DrawBoundingBoxes();
        }

        public override void DrawGUI(float dt)
        {
            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise,
                null, _camera.GetViewMatrix());

            // title
            SpriteBatch.Draw(_titleTex,
                new Vector2(_ztarPosX,
                    GameConstants.SnesHeight / 2f - titleRect01.Height / 2f - 48), titleRect01, Color.White);
            SpriteBatch.Draw(_titleTex,
                new Vector2(_fragPosX,
                    GameConstants.SnesHeight / 2f - titleRect02.Height / 2f - 32 - 48 - 4), titleRect02, Color.White);

            float offsetY = 16;

            switch (_menu)
            {
                case Menu.Main:
                    // arrows
                    SpriteBatch.DrawString(_font02_16, ChoiceArrowLeft,
                        new Vector2(_choiceArrowLeftX, _choiceArrowLeftY),
                        Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, ChoiceArrowRight,
                        new Vector2(_choiceArrowRightX, _choiceArrowRightY),
                        Color.LightGray);

                    // choices
                    SpriteBatch.DrawString(_font02_16, Choice01,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Choice01).Width / 2f,
                            GameConstants.SnesHeight / 2f - offsetY), Color.LightGray);

                    if (GetContinueLevel() != ContinueLevel.None) _continueColor = Color.LightGray;
                    else _continueColor = Color.DimGray;

                    SpriteBatch.DrawString(_font02_16, Choice02,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Choice02).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight - offsetY), _continueColor);
                    SpriteBatch.DrawString(_font02_16, Choice03,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Choice03).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 2 - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Choice04,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Choice04).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 3 - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Choice05,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Choice05).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 4 - offsetY), Color.LightGray);
                    break;
                case Menu.ShowControls:
                    SpriteBatch.DrawString(_font02_16, Controls01,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls01).Width / 2f,
                            GameConstants.SnesHeight / 2f - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Controls02,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls02).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Controls03,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls03).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 2 - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Controls04,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls04).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 3 - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Controls05,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls05).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 4 - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Controls06,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Controls06).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 5 - offsetY), Color.LightGray);
                    break;
                case Menu.Options:
                    // arrows
                    SpriteBatch.DrawString(_font02_16, ChoiceArrowLeft,
                        new Vector2(_choiceArrowLeftX, _choiceArrowLeftY),
                        Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, ChoiceArrowRight,
                        new Vector2(_choiceArrowRightX, _choiceArrowRightY),
                        Color.LightGray);

                    SpriteBatch.DrawString(_font02_16, Options01,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Options01).Width / 2f,
                            GameConstants.SnesHeight / 2f - offsetY), Color.LightGray);
                    SpriteBatch.DrawString(_font02_16, Options02,
                        new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Options02).Width / 2f,
                            GameConstants.SnesHeight / 2f + _font02_16.LineHeight - offsetY), Color.LightGray);

                    if (GameSettings.GameSettings.InvertVerticalMovement)
                    {
                        SpriteBatch.DrawString(_font02_16, Options03,
                            new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Options03).Width / 2f,
                                GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 2 - offsetY), Color.LightGray);
                    }
                    else
                    {
                        SpriteBatch.DrawString(_font02_16, Options04,
                            new Vector2(GameConstants.SnesWidth / 2f - _font02_16.MeasureString(Options04).Width / 2f,
                                GameConstants.SnesHeight / 2f + _font02_16.LineHeight * 2 - offsetY), Color.LightGray);
                    }

                    break;
            }

            // credits
            SpriteBatch.DrawString(_font02_16, credits01,
                new Vector2((int)_creditsPosX, GameConstants.SnesHeight - _font02_16.LineHeight - 2), Color.Goldenrod);

            SpriteBatch.End();
        }

        public override void DrawDebugGUI(GameTime gt)
        {
            if (Game.GuiManager == null)
            {
                // Debug gui
                var debugGuiRendererGameStats = new MonoGameImGuiRenderer(Game);
                debugGuiRendererGameStats.Initialize();
                Game.GuiManager = new GuiDebugManager(Game, this, debugGuiRendererGameStats);
            }


            if (Game.GuiManager.Screen != this) Game.GuiManager.Screen = this;

            Game.GuiManager.Tick();
            Game.GuiManager.RenderElements(gt.ElapsedGameTime);
        }

        public override void DrawModel(Model model, Matrix modelTransform)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d1.View;
                    effect.Projection = Cam3d1.Projection;

                    effect.Alpha = 1;
                    //effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    //effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    effect.FogEnabled = true;
                    effect.FogColor = CurrentLevel.FogColor;
                    effect.FogStart = CurrentLevel.FogStart;
                    effect.FogEnd = CurrentLevel.FogEnd;

                    effect.EnableDefaultLighting();
                    //effect.AmbientLightColor = new Vector3(0,1,0);
                    //effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(-0.5f, -1, -1);
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    //effect.DirectionalLight1.Enabled = false;
                    //effect.DirectionalLight1.Direction = new Vector3(0,1, 0);
                    //effect.DirectionalLight2.Enabled = false;
                    //effect.DirectionalLight2.Direction = new Vector3(0,0, -1);
                    //effect.PreferPerPixelLighting = true;

                    for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                    {
                        effect.CurrentTechnique.Passes[i].Apply();
                    }
                }

                mesh.Draw();
            }

            _modelsDrawn++;
        }

        public override void DrawModelWithColor(Model model, Matrix modelTransform, Vector3 color)
        {
        }

        public override void DrawBoundingBox(List<BoundingBox> boundingBoxes, Matrix modelTransform)
        {
        }

        public override void DrawBoundingBoxFiltered(List<BoundingBoxFiltered> boundingBoxesFiltered,
            Matrix modelTransform)
        {
        }

        public override void DrawModelUnlit(Model model, Matrix modelTransform)
        {
        }

        public override void DrawModelUnlitWithColor(Model model, Matrix modelTransform, Vector3 unlitColor)
        {
        }

        public override void DrawModelMeshUnlitWithColor(ModelMesh mesh, Matrix modelTransform, Vector3 unlitColor)
        {
        }

        enum Menu
        {
            Main,
            ShowControls,
            Options
        }

        enum MenuChoice
        {
            Start,
            Continue,
            ShowControls,
            Options,
            Quit
        }
    }
}