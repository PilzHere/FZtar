using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using FZtarOGL.GUI.Debug;
using FZtarOGL.Level;
using FZtarOGL.Utilities;
using ImGuiHandler.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using level = FZtarOGL.Level.Level;

namespace FZtarOGL.Screen
{
    public class PlayScreen : Screen
    {
        private GraphicsDevice _graphicsDevice;

        private GuiDebugManager _guiManager; // put into game isntead?!

        private OrthographicCamera _camera;

        private PlayerShip _playerShip;

        private BitmapFont _font01_16;
        private BitmapFont _font02_08;

        private Texture2D _hudTex;

        public PlayScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) :
            base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _graphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            _graphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;

            _hudTex = assMan.HudTex;

            _font01_16 = assMan.Font01_16;
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

            _playerShip = new PlayerShip(this, spriteBatch, assMan, new Vector3(0, 0.5f, -6), Cam3d1);
            Entities.Add(_playerShip);

            CurrentLevel = new Level01(this, assMan, spriteBatch);
            //CurrentLevel = new Level02(this, assMan, spriteBatch);
            //CurrentLevel = new Level03(this, assMan, spriteBatch);
        }

        public override void Input(GameTime gt, float dt)
        {
            //CAM
            //const float camSpeedZ = 33; // was 33
            const float camSpeedY = 5;
            const float camSpeedX = 7; // 7
            const float camRotMaxX = 0.1f;
            const float camRotXResetTolerance = 0.01f;
            const float camPosMaxX = 7.5f; // was 7.5f
            const float camPosMaxY = 6.5f;
            const float camPosMinY = 1;
            //const float camUpRotYMax = 0.995f;
            //const float camUpRotYSpeed = 0.2f;
            const float camRotRollSpeed = 20;
            const float camRotRollBackSpeed = 10;

            //const float maxCamDistanceToPlayer = 37.5f;

            _playerShip.Input(gt, dt);

            //const float camMaxRotNonLocalAngle = 0.05f;
            float nonLocalRotUpDown = 0;
            bool nonLocalRotChanged = false;
            float nonLocalRotBoost = 0.2f; // 0.15f
            float backgroundPosBoostY = 3.05f;
            float backgroundPosBoostX = 3.3f;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                nonLocalRotUpDown = -camSpeedY * nonLocalRotBoost * dt;
                Cam3d1.NonLocalRotateUpOrDown(gt, nonLocalRotUpDown);
                nonLocalRotChanged = true;

                _cam3dPos.Y += camSpeedY * dt;

                CurrentLevel.BackgroundPos.Y += camSpeedY * backgroundPosBoostY * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                nonLocalRotUpDown = camSpeedY * nonLocalRotBoost * dt;
                Cam3d1.NonLocalRotateUpOrDown(gt, nonLocalRotUpDown);
                nonLocalRotChanged = true;

                _cam3dPos.Y -= camSpeedY * dt;

                CurrentLevel.BackgroundPos.Y -= camSpeedY * backgroundPosBoostY * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                if (Cam3d1.Up.X >= -camRotMaxX) Cam3d1.RotateRollCounterClockwise(gt, camRotRollSpeed);
                CurrentLevel.BackgroundPos.X += camSpeedX * backgroundPosBoostX * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                if (Cam3d1.Up.X <= camRotMaxX) Cam3d1.RotateRollClockwise(gt, camRotRollSpeed);
                CurrentLevel.BackgroundPos.X -= camSpeedX * backgroundPosBoostX * dt;
            }

            // limit camera pos Y
            if (_cam3dPos.Y > camPosMaxY)
            {
                if (nonLocalRotChanged)
                    Cam3d1.NonLocalRotateUpOrDown(gt, -nonLocalRotUpDown);

                _cam3dPos.Y = camPosMaxY;
                CurrentLevel.BackgroundPos.Y = CurrentLevel.BackgroundPosOld.Y;
            }

            if (_cam3dPos.Y < camPosMinY)
            {
                if (nonLocalRotChanged)
                    Cam3d1.NonLocalRotateUpOrDown(gt, -nonLocalRotUpDown);

                _cam3dPos.Y = camPosMinY;
                CurrentLevel.BackgroundPos.Y = CurrentLevel.BackgroundPosOld.Y;
            }

            float maxDistXCamPlayerShip = 1.5f;
            float distanceXCamPlayerShip = _playerShip.ModelPos.X - _cam3dPos.X;
            if (distanceXCamPlayerShip < -maxDistXCamPlayerShip)
            {
                _cam3dPos.X -= camSpeedX * dt;
            }
            else if (distanceXCamPlayerShip > maxDistXCamPlayerShip)
            {
                _cam3dPos.X += camSpeedX * dt;
            }


            // Rotate back towards center
            if (!KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                if (Cam3d1.Up.X < 0)
                {
                    if (Cam3d1.Up.X <= -camRotXResetTolerance)
                    {
                        Cam3d1.RotateRollClockwise(gt, camRotRollBackSpeed);
                    }
                    else
                    {
                        Cam3d1.Up = Vector3.Up;
                    }
                }
            }

            if (!KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                if (Cam3d1.Up.X > 0)
                {
                    if (Cam3d1.Up.X >= camRotXResetTolerance)
                    {
                        Cam3d1.RotateRollCounterClockwise(gt, camRotRollBackSpeed);
                    }
                    else
                    {
                        Cam3d1.Up = Vector3.Up;
                    }
                }
            }

            if (_cam3dPos.X < -camPosMaxX)
            {
                _cam3dPos.X = -camPosMaxX;
                CurrentLevel.KeepOldBackgroundPositionX();
            }

            if (_cam3dPos.X > camPosMaxX)
            {
                _cam3dPos.X = camPosMaxX;
                CurrentLevel.KeepOldBackgroundPositionX();
            }

            Cam3d1.Position = _cam3dPos;

            // old code
            const float radius = 100;
            _newPosX += (float)Math.Cos(MathHelper.ToRadians(_acc)) * radius * dt;
            _newPosY += (float)Math.Sin(MathHelper.ToRadians(_acc)) * radius * dt;

            _acc++;
            const int resetAngle = 360;
            if (_acc >= resetAngle)
                _acc -= resetAngle;
            //old code end
        }

        public override void Tick(GameTime gt, float dt)
        {
            ResetCounters();

            transparentModels.Clear();

            RemoveDestroyedEntities();

            CheckCollisions(BoundingBoxesFiltered, dt);
            
            CurrentLevel.Tick(gt, dt);
            
            UpdateAllEntities(dt);

            OrderTransparentModelsByDistanceFromCam(Cam3d1);
        }

        // old code
        private float _newPosX, _newPosY;

        private int _acc;
        // old code end

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
                    
                    effect.DiffuseColor = color;

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

        public override void DrawModelUnlit(Model model, Matrix modelTransform)
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

                    effect.LightingEnabled = false;
                    effect.DiffuseColor = new Vector3(1, 1, 1);

                    for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                    {
                        effect.CurrentTechnique.Passes[i].Apply();
                    }
                }

                mesh.Draw();
            }

            _modelsDrawn++;
        }

        public override void DrawModelUnlitWithColor(Model model, Matrix modelTransform, Vector3 unlitColor)
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

                    effect.LightingEnabled = false;
                    effect.DiffuseColor = unlitColor;
                    effect.TextureEnabled = false;

                    for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                    {
                        effect.CurrentTechnique.Passes[i].Apply();
                    }
                }

                mesh.Draw();
            }

            _modelsDrawn++;
        }

        public override void DrawModelMeshUnlitWithColor(ModelMesh mesh, Matrix modelTransform, Vector3 unlitColor)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.World = modelTransform;
                effect.View = Cam3d1.View;
                effect.Projection = Cam3d1.Projection;

                effect.Alpha = 1;
                //effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                //effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                effect.LightingEnabled = false;
                effect.DiffuseColor = unlitColor;
                effect.TextureEnabled = false;

                for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                {
                    effect.CurrentTechnique.Passes[i].Apply();
                }
            }

            mesh.Draw();
        }

        public override void DrawBoundingBoxFiltered(List<BoundingBoxFiltered> boundingBoxesFiltered,
            Matrix modelTransform)
        {
            if (GameSettings.GameSettings.DebugRenderBoundingBoxes)
            {
                BoundingBoxesDrawEffect.World = modelTransform;
                BoundingBoxesDrawEffect.View = Cam3d1.View;
                BoundingBoxesDrawEffect.Projection = Cam3d1.Projection;

                // Use inside a drawing loop
                foreach (BoundingBoxFiltered box in boundingBoxesFiltered)
                {
                    Vector3[] corners = box.Box.GetCorners();
                    VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                    Color currentColor = Color.White;
                    switch (box.Filter)
                    {
                        case BoxFilters.FilterNothing:
                            currentColor = Color.Gray;
                            break;
                        case BoxFilters.FilterPlayerRay:
                            currentColor = Color.Red;
                            break;
                    }

                    // Assign the 8 box vertices
                    for (int i = 0; i < corners.Length; i++)
                    {
                        primitiveList[i] = new VertexPositionColor(corners[i], currentColor);
                    }

                    // Draw the box with a LineList
                    foreach (var pass in BoundingBoxesDrawEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.LineList, primitiveList, 0, 8,
                            bBoxIndices, 0, 12);
                    }
                }
            }
        }

        public override void DrawBoundingBox(List<BoundingBox> boundingBoxes, Matrix modelTransform)
        {
            if (GameSettings.GameSettings.DebugRenderBoundingBoxes)
            {
                // Use inside a drawing loop
                foreach (BoundingBox box in boundingBoxes)
                {
                    Vector3[] corners = box.GetCorners();
                    VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                    // Assign the 8 box vertices
                    for (int i = 0; i < corners.Length; i++)
                    {
                        primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
                    }

                    BoundingBoxesDrawEffect.World = modelTransform;
                    BoundingBoxesDrawEffect.View = Cam3d1.View;
                    BoundingBoxesDrawEffect.Projection = Cam3d1.Projection;

                    // Draw the box with a LineList
                    foreach (var pass in BoundingBoxesDrawEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserIndexedPrimitives(
                            PrimitiveType.LineList, primitiveList, 0, 8,
                            bBoxIndices, 0, 12);
                    }
                }
            }
        }


        public override void DrawGUI(float dt)
        {
            if (_playerShip != null)
            {
                SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise,
                    null, _camera.GetViewMatrix());

                // player aim
                _playerShip.DrawAim(dt);

                // game over
                if (_playerShip.Hp <= 0)
                {
                    String textGO = "GAME OVER";
                    int posXGO = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                                       SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                                       _font01_16.GetStringRectangle(textGO).Width / 2f);
                    int posYGO = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                                       SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                                       _font01_16.GetStringRectangle(textGO).Height / 2f);
                    SpriteBatch.DrawString(_font01_16, textGO, new Vector2(posXGO, posYGO), Color.Black);
                }

                // hp bg
                int posXLife = SpriteBatch.GraphicsDevice.Viewport.X;
                int posYLife = SpriteBatch.GraphicsDevice.Viewport.Y + SpriteBatch.GraphicsDevice.Viewport.Height;

                int widthLife = 1;
                int heightLife = 6;
                int xLife = 1;
                int yLife = 14;
                Rectangle rectLife = new Rectangle(xLife, yLife, widthLife, heightLife);
                int offsetXLife = 8;
                int offsetYLife = 8;
                int scaleX = (int)(100f / 100f * _playerShip.Hp);
                SpriteBatch.Draw(_hudTex, new Vector2(posXLife + offsetXLife, posYLife - heightLife - offsetYLife),
                    rectLife, Color.White, 0, Vector2.Zero, new Vector2(scaleX, 1),
                    SpriteEffects.None,
                    0);

                // hp
                int posX = SpriteBatch.GraphicsDevice.Viewport.X;
                int posY = SpriteBatch.GraphicsDevice.Viewport.Y + SpriteBatch.GraphicsDevice.Viewport.Height;

                int width = 110;
                int height = 13;
                int x = 1;
                int y = 1;
                Rectangle rect = new Rectangle(x, y, width, height);
                int offsetX = 4;
                int offsetY = 4;
                SpriteBatch.Draw(_hudTex, new Vector2(posX + offsetX, posY - height - offsetY), rect, Color.White, 0,
                    Vector2.Zero, Vector2.One,
                    SpriteEffects.None,
                    1);

                //boost background
                int rectWidthBoostBg = 58;
                int posXBoostBg = SpriteBatch.GraphicsDevice.Viewport.X + SpriteBatch.GraphicsDevice.Viewport.Width -
                                  rectWidthBoostBg;
                int posYBoostBg = SpriteBatch.GraphicsDevice.Viewport.Y + SpriteBatch.GraphicsDevice.Viewport.Height;

                int widthBoostBg = 110;
                int heightBoostBg = 13;
                int xBoostBg = 1;
                int yBoostBg = 1;
                Rectangle rectBoostBg = new Rectangle(xBoostBg, yBoostBg, widthBoostBg, heightBoostBg);
                int offsetXBoostBg = 4;
                int offsetYBoostBg = 4;
                SpriteBatch.Draw(_hudTex,
                    new Vector2(posXBoostBg - offsetXBoostBg, posYBoostBg - heightBoostBg - offsetYBoostBg),
                    rectBoostBg, Color.White, 0,
                    Vector2.Zero, Vector2.One,
                    SpriteEffects.None,
                    1);

                // boost
                int rectWidth = 58;
                int posXBoost = SpriteBatch.GraphicsDevice.Viewport.X + SpriteBatch.GraphicsDevice.Viewport.Width -
                                rectWidth;
                int posYBoost = SpriteBatch.GraphicsDevice.Viewport.Y + SpriteBatch.GraphicsDevice.Viewport.Height;

                int widthBoost = 1;
                int heightBoost = 6;
                int xBoost = 3;
                int yBoost = 14;
                Rectangle rectBoost = new Rectangle(xBoost, yBoost, widthBoost, heightBoost);
                int offsetXBoost = 0;
                int offsetYBoost = 8;
                int scaleXBoost = (int)(100f / 100f * _playerShip.Power);
                SpriteBatch.Draw(_hudTex, new Vector2(posXBoost - offsetXBoost, posYBoost - heightBoost - offsetYBoost),
                    rectBoost, Color.White, 0, Vector2.Zero, new Vector2(scaleXBoost, 1),
                    SpriteEffects.None,
                    0);

                // draw message if any
                CurrentLevel.DrawMessage(SpriteBatch, dt);

                SpriteBatch.End();
            }
        }

        public override void DrawDebugGUI(GameTime gt)
        {
            if (_guiManager == null)
            {
                // Debug gui
                var debugGuiRendererGameStats = new MonoGameImGuiRenderer(Game);
                debugGuiRendererGameStats.Initialize();
                _guiManager = new GuiDebugManager(Game, this, debugGuiRendererGameStats);
            }

            _guiManager.Tick();
            _guiManager.RenderElements(gt.ElapsedGameTime);
        }

        public override void Destroy()
        {
            // remove shit here

            base.Destroy();
        }
    }
}