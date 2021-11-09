using System;
using System.Collections.Generic;
using System.Linq;
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
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using level = FZtarOGL.Level.Level;

namespace FZtarOGL.Screen
{
    public class PlayScreen : Screen
    {
        private GraphicsDevice _graphicsDevice;

        private GuiDebugManager _guiManager;

        private OrthographicCamera _camera;

        private BasicEffect basicEffectPrimitives;

        private Vector3[,] floorDots2D = new Vector3[7, 6]; // z, x
        private Vector3 floorDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f

        private PlayerShip _playerShip;

        private BitmapFont _font01_16;
        private BitmapFont _font02_08;

        private Texture2D _hudMessageAvatar;
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

            _hudMessageAvatar = assMan.AvatarDrInet01Tex;

            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, 256, 224);
            _camera = new OrthographicCamera(viewportAdapter);

            Cam3d1 = new PerspectiveCamera(_graphicsDevice, game.Window);
            Cam3d1.Position = new Vector3(0, 1, 0);
            cam3dPos = Cam3d1.Position;
            Cam3d1.LookAtDirection = Vector3.Forward;
            Cam3d1.MovementUnitsPerSecond = 10f;
            Cam3d1.farClipPlane = 200;
            Cam3d1.nearClipPlane = 0.05f;
            Cam3d1.fieldOfViewDegrees = 45;
            Cam3d1.RotationRadiansPerSecond = 100f;

            basicEffectPrimitives = new BasicEffect(_graphicsDevice);

            Entities.Add(new Cube(this, assMan, new Vector3(0, 0.5f, -200)));

            _playerShip = new PlayerShip(this, spriteBatch, assMan, new Vector3(0, 0.5f, -6), Cam3d1);
            Entities.Add(_playerShip);

            // Debug gui
            var debugGuiRendererGameStats = new MonoGameImGuiRenderer(game);
            debugGuiRendererGameStats.Initialize();
            _guiManager = new GuiDebugManager(game, this, debugGuiRendererGameStats);

            const int dotPosStartX = -10;
            const int dotPosY = 0;
            const int dotPosStartZ = 10;
            const int dotPosOffsetX = 4;
            const int dotPosOffsetZ = 10;
            var dotPosZ = dotPosStartZ;
            for (var z = 0; z < floorDots2D.GetLength(0); z++) // 0-5
            {
                var dotPosX = dotPosStartX;
                dotPosZ -= dotPosOffsetZ;
                for (var x = 0; x < floorDots2D.GetLength(1); x++) // 0-6
                {
                    Console.WriteLine("x: " + dotPosX + " z: " + dotPosZ);

                    floorDots2D[z, x].X = dotPosX;
                    floorDots2D[z, x].Y = dotPosY;
                    floorDots2D[z, x].Z = dotPosZ;

                    dotPosX += dotPosOffsetX;
                }
            }

            camNextZ = Cam3d1.Position.Z - dotsOneStep;

            CurrentLevel = new Level01(this, assMan, spriteBatch);
        }

        private const float dotsOneStep = 10;
        private float camNextZ;

        private Vector3 cam3dPos;

        private Keys _newScreenKey = Keys.R;

        public override void Input(GameTime gt, float dt)
        {
            if (KeyboardExtended.GetState().IsKeyDown(_newScreenKey))
            {
                //Game.ScreenManager.AddScreen(new PlayScreen(Game, AssMan, SpriteBatch,
                //    _graphicsDevice /*, DebugGuiRenderer*/));
            }

            //CAM
            const float camSpeedZ = 33; // was 33
            const float camSpeedY = 5;
            const float camSpeedX = 7; // 7
            const float camRotMaxX = 0.1f;
            const float camRotXResetTolerance = 0.01f;
            const float camPosMaxX = 7.5f; // was 7.5f
            const float camPosMaxY = 6.5f;
            const float camPosMinY = 1;
            const float camUpRotYMax = 0.995f;
            const float camUpRotYSpeed = 0.2f;
            const float camRotRollSpeed = 20;
            const float camRotRollBackSpeed = 10;

            const float maxCamDistanceToPlayer = 37.5f;

            _playerShip.Input(gt, dt);

            const float camMaxRotNonLocalAngle = 0.05f;
            float nonLocalRotUpDown = 0;
            bool nonLocalRotChanged = false;
            float nonLocalRotBoost = 0.2f; // 0.15f

            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                nonLocalRotUpDown = -camSpeedY * nonLocalRotBoost * dt;
                Cam3d1.NonLocalRotateUpOrDown(gt, nonLocalRotUpDown);
                nonLocalRotChanged = true;

                //cam3dPos.Z -= camSpeed * dt;
                cam3dPos.Y += camSpeedY * dt;

                CurrentLevel.BackgroundPos.Y += camSpeedY * 3.05f * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                nonLocalRotUpDown = camSpeedY * nonLocalRotBoost * dt;
                Cam3d1.NonLocalRotateUpOrDown(gt, nonLocalRotUpDown);
                nonLocalRotChanged = true;

                //cam3dPos.Z += camSpeed * dt;
                cam3dPos.Y -= camSpeedY * dt;

                CurrentLevel.BackgroundPos.Y -= camSpeedY * 3.05f * dt;
            }

            //cam3dPos.Z -= camSpeedZ * dt;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                // BUG: No limit, does not set minimum rotation: just stops rotating.
                if (Cam3d1.Up.X <= -camRotMaxX) Console.WriteLine("Cant rotate more!: " + Cam3d1.Up.X);
                else Cam3d1.RotateRollCounterClockwise(gt, camRotRollSpeed);

                //cam3dPos.X -= camSpeedX * dt;
                CurrentLevel.BackgroundPos.X += camSpeedX * 3.3f * dt;

                //playerShipModelRot.Z += playerShipRotZSpeed * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                // BUG: No limit, does not set minimum rotation: just stops rotating.
                if (Cam3d1.Up.X >= camRotMaxX) Console.WriteLine("Cant rotate more!: " + Cam3d1.Up.X);
                else Cam3d1.RotateRollClockwise(gt, camRotRollSpeed);

                //cam3dPos.X += camSpeedX * dt;
                CurrentLevel.BackgroundPos.X -= camSpeedX * 3.3f * dt;

                //playerShipModelRot.Z -= playerShipRotZSpeed * dt;
            }

            // limit camera pos Y
            if (cam3dPos.Y > camPosMaxY)
            {
                if (nonLocalRotChanged)
                    Cam3d1.NonLocalRotateUpOrDown(gt, -nonLocalRotUpDown);

                cam3dPos.Y = camPosMaxY;
                CurrentLevel.BackgroundPos.Y = CurrentLevel.BackgroundPosOld.Y;
            }

            if (cam3dPos.Y < camPosMinY)
            {
                if (nonLocalRotChanged)
                    Cam3d1.NonLocalRotateUpOrDown(gt, -nonLocalRotUpDown);

                cam3dPos.Y = camPosMinY;
                CurrentLevel.BackgroundPos.Y = CurrentLevel.BackgroundPosOld.Y;
            }

            //Console.WriteLine(Cam3d.Forward.Y);

            // BUG: Dont use this. This is what is causing the stuttering witch the cam/playership when moving the playership.
            /*var distance = Vector3.DistanceSquared(Cam3d.Position, _playerShip.ModelPos);
            
            if (distance > maxCamDistanceToPlayer)
            {
                if (_playerShip.ModelPos.X > cam3dPos.X) // right
                {
                    cam3dPos.X += camSpeedX * dt;
                    //bgPos.X -= camSpeedX * 3.3f * dt;
                }
                else if (_playerShip.ModelPos.X < cam3dPos.X) // left
                {
                    cam3dPos.X -= camSpeedX * dt;
                    //bgPos.X += camSpeedX * 3.3f * dt;
                }
            }*/

            float maxDistXCamPlayerShip = 1.5f;
            float distanceXCamPlayerShip = _playerShip.ModelPos.X - cam3dPos.X;
            if (distanceXCamPlayerShip < -maxDistXCamPlayerShip)
            {
                cam3dPos.X -= camSpeedX * dt;
                //bgPos.X -= camSpeedX * 3.3f * dt;
            }
            else if (distanceXCamPlayerShip > maxDistXCamPlayerShip)
            {
                cam3dPos.X += camSpeedX * dt;
                //bgPos.X += camSpeedX * 3.3f * dt;
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

            if (cam3dPos.X < -camPosMaxX)
            {
                cam3dPos.X = -camPosMaxX;
                CurrentLevel.KeepOldBackgroundPositionX();
            }

            if (cam3dPos.X > camPosMaxX)
            {
                cam3dPos.X = camPosMaxX;
                CurrentLevel.KeepOldBackgroundPositionX();
            }

            Cam3d1.Position = cam3dPos;

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

            CurrentLevel.Tick(gt, dt);

            UpdateAllEntities(dt);

            floorDotsPos.Z += CurrentLevel.VirtualSpeedZ * dt;

            if (floorDotsPos.Z > 10) floorDotsPos.Z = 0;

            CheckCollisions(BoundingBoxesFiltered, dt);

            OrderTransparentModelsByDistanceFromCam(Cam3d1);
            
            Console.WriteLine();
        }

        // old code
        private float _newPosX, _newPosY;

        private int _acc;
        // old code end

        private Vector3 floorPos = new Vector3(0, 0, -50);
        private Vector3 floorDotsPos = Vector3.Zero;

        public override void Draw(float dt)
        {
            _graphicsDevice.Clear(Color.HotPink);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise,
                null, _camera.GetViewMatrix());

            CurrentLevel.DrawBackground(dt, -Cam3d1.Up.X);

            Draw2DAllEntities(dt);

            SpriteBatch.End(); // TODO: Can be called in DrawGUI later?

            // For 3D we use the depth buffer.
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.BlendState = BlendState.AlphaBlend; // Needed?

            // Draw dots on floor
            basicEffectPrimitives.View = Cam3d1.View;
            basicEffectPrimitives.Projection = Cam3d1.Projection;
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            for (var i = 0; i < floorDots2D.GetLength(0); i++) // 0-5
            {
                for (var j = 0; j < floorDots2D.GetLength(1); j++) // 0-6
                {
                    var vertices = new[]
                    {
                        new VertexPositionColor(floorDots2D[i, j] + floorDotsPos, Color.White),
                        new VertexPositionColor(floorDots2D[i, j] + floorDotsPos + floorDotsMiniOffset,
                            Color.Gray)
                    };
                    _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                }
            }

            // For debugging - red line at end
            /*Vector3 startPoint = new Vector3(-10, 0, -200);
            Vector3 endPoint = new Vector3(10, 0, -200);
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            var vertices2 = new[]
                { new VertexPositionColor(startPoint, Color.Red), new VertexPositionColor(endPoint, Color.Red) };
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices2, 0, 1);*/

            Draw3DAllEntities(dt);
            
            foreach (var transModel in TransparentModels)
            {
                if (transModel.Lit) DrawModel(transModel.Model, transModel.ModelTrans);
                else DrawModelUnlit(transModel.Model, transModel.ModelTrans);
            }

            if (GameSettings.GameSettings.DebugRenderBoundingBoxes) DrawBoundingBoxes();
        }

        public override void DrawModel(Model model, Matrix modelTransform)
        {
            _graphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d1.View;
                    effect.Projection = Cam3d1.Projection;

                    effect.Alpha = 1;
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

        public override void DrawModelUnlit(Model model, Matrix modelTransform)
        {
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d1.View;
                    effect.Projection = Cam3d1.Projection;

                    effect.Alpha = 1;

                    effect.LightingEnabled = false;
                    effect.DiffuseColor = new Vector3(1, 0, 0);

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
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d1.View;
                    effect.Projection = Cam3d1.Projection;

                    effect.Alpha = 1;

                    effect.LightingEnabled = false;
                    effect.DiffuseColor = unlitColor;

                    for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                    {
                        effect.CurrentTechnique.Passes[i].Apply();
                    }
                }

                mesh.Draw();
            }
            
            _modelsDrawn++;
        }

        /*public override void DrawTransparentModels(Model model, Matrix modelTransform)
        {
            foreach (var transModel in TransparentModels)
            {
                DrawModel(model, modelTransform);
            }
        }*/

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
                    if (box.Filter == BoxFilters.FilterPlayerRay) currentColor = Color.Red;

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

                // test
                //SpriteBatch.DrawString(_font01_16, "Hello...", new Vector2(50, 50), Color.White);
                //SpriteBatch.DrawString(_font02_08, "...World!", new Vector2(50, 100), Color.White);

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