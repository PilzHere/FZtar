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
    public class MainMenuScreen : Screen
    {
        private GraphicsDevice _graphicsDevice;

        private GuiDebugManager _guiManager;

        private Texture2D bg01;

        private OrthographicCamera _camera;

        private Model floorModel;

        private BasicEffect basicEffectPrimitives;
        private Vector3 startPoint = new Vector3(0, -3, 0);
        private Vector3 endPoint = new Vector3(0, -3, -200);

        private Vector3[,] floorDots2D = new Vector3[7, 6]; // z, x
        private Vector3 floorDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f

        private PlayerShip _playerShip;

        private BitmapFont _fontFat16;

        public MainMenuScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) :
            base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _graphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            _graphicsDevice.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;

            bg01 = assMan.LoadAsset<Texture2D>("textures/bg01");
            floorModel = assMan.LoadAsset<Model>("models/floor");
            
            // TODO: Ladda ner BMFont på Windows laptop
            // Ladda ner fonts
            // convertera fonts
            // spara
            // ladda med content.mgcb
            // ?
            // profit
            
            // Content.mgcb
            /*#begin fonts/font01_16.fnt
            /importer:BMFontImporter
            /processor:BMFontProcessor
            /build:fonts/font01_16.fnt

            #begin fonts/fat16.fnt
            /importer:BitmapFontImporter
            /processor:BitmapFontProcessor
            /build:fonts/fat16.fnt*/
            
            //_fontFat16 = assMan.LoadAsset<BitmapFont>("fonts/fat16");

            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, 256, 224);
            _camera = new OrthographicCamera(viewportAdapter);

            Cam3d = new PerspectiveCamera(_graphicsDevice, game.Window);
            Cam3d.Position = new Vector3(0, 1, 0);
            cam3dPos = Cam3d.Position;
            Cam3d.LookAtDirection = Vector3.Forward;
            Cam3d.MovementUnitsPerSecond = 10f;
            Cam3d.farClipPlane = 200;
            Cam3d.nearClipPlane = 0.05f;
            Cam3d.fieldOfViewDegrees = 45;
            Cam3d.RotationRadiansPerSecond = 100f;

            basicEffectPrimitives = new BasicEffect(_graphicsDevice);

            Entities.Add(new Cube(this, assMan, new Vector3(0, 0.5f, -200)));
            
            _playerShip = new PlayerShip(this, spriteBatch, assMan, new Vector3(0, 0.5f, -6), Cam3d);
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

            camNextZ = Cam3d.Position.Z - dotsOneStep;

            CurrentLevel = new Level01(this, assMan, spriteBatch);
        }

        private const float dotsOneStep = 10;
        private float camNextZ;

        private Vector3 cam3dPos;
        private Vector3 cam3dRot;
        private Vector3 cam3dLookAtTarget;

        private Vector3 oldCam3dUp;

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
            const float camPosMaxX = 7.5f;
            const float camPosMaxY = 6.5f;
            const float camPosMinY = 1;
            const float camUpRotYMax = 0.995f;
            const float camUpRotYSpeed = 0.2f;
            const float camRotRollSpeed = 20;
            const float camRotRollBackSpeed = 10;

            const float maxCamDistanceToPlayer = 37.5f;

            _playerShip.Input(gt, dt);

            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                //_cam3d.NonLocalRotateUpOrDown(gt, camUpRotYSpeed);

                //cam3dPos.Z -= camSpeed * dt;
                cam3dPos.Y += camSpeedY * dt;
            }


            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                //_cam3d.NonLocalRotateUpOrDown(gt, -camUpRotYSpeed);

                //cam3dPos.Z += camSpeed * dt;
                cam3dPos.Y -= camSpeedY * dt;
            }

            // limit camera pos Y
            if (cam3dPos.Y > camPosMaxY)
                cam3dPos.Y = camPosMaxY;

            if (cam3dPos.Y < camPosMinY)
                cam3dPos.Y = camPosMinY;

            //cam3dPos.Z -= camSpeedZ * dt;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                // BUG: No limit, does not set minimum rotation: just stops rotating.
                if (Cam3d.Up.X <= -camRotMaxX) Console.WriteLine("Cant rotate more!: " + Cam3d.Up.X);
                else Cam3d.RotateRollCounterClockwise(gt, camRotRollSpeed);

                //cam3dPos.X -= camSpeedX * dt;
                CurrentLevel.BackgroundPos.X += camSpeedX * 3.3f * dt;

                //playerShipModelRot.Z += playerShipRotZSpeed * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                if (Cam3d.Up.X >= camRotMaxX) Console.WriteLine("Cant rotate more!: " + Cam3d.Up.X);
                else Cam3d.RotateRollClockwise(gt, camRotRollSpeed);

                //cam3dPos.X += camSpeedX * dt;
                CurrentLevel.BackgroundPos.X -= camSpeedX * 3.3f * dt;

                //playerShipModelRot.Z -= playerShipRotZSpeed * dt;
            }

            // BUG: Dont use this. This is what is causing the stuttering witch the cam/playership when moving the playership.
            // BUG: Solution: Dont check distance as a vector, just compare playership X with cam X.
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

            float maxDistX = 1.5f;
            float distanceX = _playerShip.ModelPos.X - cam3dPos.X;
            if (distanceX < -maxDistX)
            {
                cam3dPos.X -= camSpeedX * dt;
                //bgPos.X -= camSpeedX * 3.3f * dt;
            }
            else if (distanceX > maxDistX)
            {
                cam3dPos.X += camSpeedX * dt;
                //bgPos.X += camSpeedX * 3.3f * dt;
            }
            
            

            // Rotate back towards center
            if (!KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                if (Cam3d.Up.X < 0)
                {
                    if (Cam3d.Up.X <= -camRotXResetTolerance)
                    {
                        Cam3d.RotateRollClockwise(gt, camRotRollBackSpeed);
                    }
                    else
                    {
                        Cam3d.Up = Vector3.Up;
                    }
                }
            }

            if (!KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                if (Cam3d.Up.X > 0)
                {
                    if (Cam3d.Up.X >= camRotXResetTolerance)
                    {
                        Cam3d.RotateRollCounterClockwise(gt, camRotRollBackSpeed);
                    }
                    else
                    {
                        Cam3d.Up = Vector3.Up;
                    }
                }
            }

            if (cam3dPos.X < -camPosMaxX)
            {
                cam3dPos.X = -camPosMaxX;
                CurrentLevel.KeepOldBackgroundPosition();
            }

            if (cam3dPos.X > camPosMaxX)
            {
                cam3dPos.X = camPosMaxX;
                CurrentLevel.KeepOldBackgroundPosition();
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Z))
            {
                //cam3dPos.Y += camSpeedZ * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.X))
            {
                //cam3dPos.Y -= camSpeedZ * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Q))
            {
                // BUG: No limit, does not set minimum rotation: just stops rotating.
                //if (_cam3d.Up.X <= -0.14f) Console.WriteLine("Cant rotate more!: " + _cam3d.Up.X);
                //else _cam3d.RotateRollCounterClockwise(gt);
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.E))
            {
                // BUG: No limit, does not set maximum rotation: just stops rotating.
                //if (_cam3d.Up.X >= 0.14f) Console.WriteLine("Cant rotate more!: " + _cam3d.Up.X);
                //else _cam3d.RotateRollClockwise(gt);
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.R))
            {
                //_cam3d.RotateUpOrDown(gt, 1);
                Cam3d.NonLocalRotateUpOrDown(gt, 1);
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.T))
            {
                //_cam3d.RotateUpOrDown(gt, -1);
                Cam3d.NonLocalRotateUpOrDown(gt, -1);
            }

            Cam3d.Position = cam3dPos;

            oldCam3dUp = Cam3d.Up;
            //bgPosOld = bgPos;

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
            RemoveDestroyedEntities();

            CurrentLevel.Tick(gt, dt);

            UpdateAllEntities(dt);

            floorDotsPos.Z += CurrentLevel.VirtualSpeedZ * dt;

            if (floorDotsPos.Z > 10) floorDotsPos.Z = 0;
            
            CheckCollisions(BoundingBoxesFiltered, dt);
            
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

            CurrentLevel.DrawBackground(dt, -Cam3d.Up.X);

            Draw2DAllEntities(dt);

            SpriteBatch.End(); // TODO: Can be called in DrawGUI later?

            // For 3D we use the depth buffer.
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.BlendState = BlendState.AlphaBlend; // Needed?

            // Draw dots on floor
            basicEffectPrimitives.View = Cam3d.View;
            basicEffectPrimitives.Projection = Cam3d.Projection;
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
            /*Vector3 startPoint = new Vector3(-10, 0, -60);
            Vector3 endPoint = new Vector3(10, 0, -60);
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            var vertices2 = new[] { new VertexPositionColor(startPoint, Color.Red),  new VertexPositionColor(endPoint, Color.Red) };
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices2, 0, 1);*/

            Draw3DAllEntities(dt);
            
            if (GameSettings.GameSettings.DebugRenderBoundingBoxes) DrawBoundingBoxes();

            if (_playerShip != null)
            {
                SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise,
                    null, _camera.GetViewMatrix());

                _playerShip.DrawAim(dt);

                SpriteBatch.End();
            }
        }

        public override void DrawModel(Model model, Matrix modelTransform)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d.View;
                    effect.Projection = Cam3d.Projection;

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
        }

        public override void DrawModelUnlit(Model model, Matrix modelTransform)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d.View;
                    effect.Projection = Cam3d.Projection;

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
        }

        public override void DrawModelUnlitWithColor(Model model, Matrix modelTransform, Vector3 unlitColor)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = Cam3d.View;
                    effect.Projection = Cam3d.Projection;

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
        }

        public override void DrawBoundingBoxFiltered(List<BoundingBoxFiltered> boundingBoxesFiltered, Matrix modelTransform)
        {
            if (GameSettings.GameSettings.DebugRenderBoundingBoxes)
            {
                BoundingBoxesDrawEffect.World = modelTransform;
                BoundingBoxesDrawEffect.View = Cam3d.View;
                BoundingBoxesDrawEffect.Projection = Cam3d.Projection;
                
                // Use inside a drawing loop
                foreach (BoundingBoxFiltered box in boundingBoxesFiltered)
                {
                    Vector3[] corners = box.Box.GetCorners();
                    VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                    Color currentColor = Color.White;
                    if (box.Filter == BoxFilters.filterPlayerRay) currentColor = Color.Red;
                    
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
                    BoundingBoxesDrawEffect.View = Cam3d.View;
                    BoundingBoxesDrawEffect.Projection = Cam3d.Projection;

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