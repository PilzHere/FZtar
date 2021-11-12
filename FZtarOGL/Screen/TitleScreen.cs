using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Level;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace FZtarOGL.Screen
{
    public class TitleScreen : Screen
    {
        private GraphicsDevice _graphicsDevice;
        private OrthographicCamera _camera;
        
        private BitmapFont _font01_32, _font01_16, _font01_08;
        private BitmapFont _font02_32, _font02_16, _font02_08;
        
        private Texture2D _titleTex;
        
        public TitleScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) : base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _graphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            _graphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;

            _titleTex = AssMan.Title;
            
            _font01_32 = assMan.Font01_32;
            _font01_16 = assMan.Font01_16;
            _font01_08 = assMan.Font01_08;
            
            _font02_32 = assMan.Font02_32;
            _font02_16 = assMan.Font02_16;
            _font02_08 = assMan.Font02_08;
            
            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, 256, 224);
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
            
            // SET baxckgroudn level
            CurrentLevel = new LevelTitleScreen(this, assMan, spriteBatch);
        }

        public override void Input(GameTime gt, float dt)
        {
            
        }

        public override void Tick(GameTime gt, float dt)
        {
            
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

            Rectangle titleRect01 = new Rectangle(1, 18, 140, 32);
            Rectangle titleRect02 = new Rectangle(1, 51, 140, 32);
            SpriteBatch.Draw(_titleTex, new Vector2(256/2f - titleRect01.Width/2f + 140/4f, 224/2f - titleRect01.Height/2f - 32), titleRect01, Color.White);
            SpriteBatch.Draw(_titleTex, new Vector2(256/2f - titleRect02.Width/2f - 140/4f - 2, 224/2f - titleRect02.Height/2f - 32 - 32 - 4), titleRect02, Color.White);
            
            String choice01 = "CONTINUE GAME";
            SpriteBatch.DrawString(_font02_16, choice01, new Vector2(256/2f - _font02_16.MeasureString(choice01).Width/2f, 224/2f), Color.White);
            String choice02 = "START GAME";
            SpriteBatch.DrawString(_font02_16, choice02, new Vector2(256/2f - _font02_16.MeasureString(choice02).Width/2f, 224/2f + _font02_16.LineHeight), Color.White);
            String choice03 = "DISPLAY CONTROLS";
            SpriteBatch.DrawString(_font02_16, choice03, new Vector2(256/2f - _font02_16.MeasureString(choice03).Width/2f, 224/2f + _font02_16.LineHeight * 2), Color.White);
            String choice04 = "OPTIONS";
            SpriteBatch.DrawString(_font02_16, choice04, new Vector2(256/2f - _font02_16.MeasureString(choice04).Width/2f, 224/2f + _font02_16.LineHeight * 3), Color.White);
            String choice05 = "QUIT GAME";
            SpriteBatch.DrawString(_font02_16, choice05, new Vector2(256/2f - _font02_16.MeasureString(choice05).Width/2f, 224/2f + _font02_16.LineHeight * 4), Color.White);
            
            String credits01 = "Code by Christian \"PilzHere\" Pilz";
            SpriteBatch.DrawString(_font02_08, credits01, new Vector2(256/2f - _font02_08.MeasureString(credits01).Width/2f, 224 - _font02_08.LineHeight), Color.White);
            
            SpriteBatch.End();
        }

        public override void DrawDebugGUI(GameTime gt)
        {
            
        }

        public override void DrawModel(Model model, Matrix modelTransform)
        {
            
        }

        public override void DrawModelWithColor(Model model, Matrix modelTransform, Vector3 color)
        {
            
        }

        public override void DrawBoundingBox(List<BoundingBox> boundingBoxes, Matrix modelTransform)
        {
            
        }

        public override void DrawBoundingBoxFiltered(List<BoundingBoxFiltered> boundingBoxesFiltered, Matrix modelTransform)
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
    }
}