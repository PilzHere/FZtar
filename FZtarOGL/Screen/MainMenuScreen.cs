using System;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.GUI.Debug;
using ImGuiHandler.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace FZtarOGL.Screen
{
    public class MainMenuScreen : Screen
    {
        private GraphicsDevice _graphicsDevice;

        private GuiDebugManager _guiManager;

        private Texture2D bg01;
        private Texture2D bg02;
        private Sprite _spriteBg01;

        private OrthographicCamera _camera;
        private PerspectiveCamera _cam3d;

        private Model testModel;
        private Model floorModel;
        
        private Matrix modelTransform;
        private Vector3 modelPos;
        private Vector3 modelRot;

        private BasicEffect basicEffectPrimitives;
        private Vector3 startPoint = new Vector3(0, -3, 0);
        private Vector3 endPoint = new Vector3(0, -3, -100);

        private Vector3[,] floorDots2D = new Vector3[7, 6]; // z, x
        private Vector3 floorDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f

        public MainMenuScreen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) :
            base(game, assMan, spriteBatch)
        {
            _graphicsDevice = graphicsDevice;

            bg01 = assMan.LoadAsset<Texture2D>("textures/bg01");
            bg02 = assMan.LoadAsset<Texture2D>("textures/bg02");
            _spriteBg01 = new Sprite(bg01);

            testModel = assMan.LoadAsset<Model>("models/cubeFbx");
            floorModel = assMan.LoadAsset<Model>("models/floor");

            modelPos = new Vector3(0, 0 + .5f, -100);
            modelTransform = Matrix.CreateRotationX(modelRot.X) * Matrix.CreateRotationY(modelRot.Y) * Matrix.CreateRotationZ(modelRot.Z) * Matrix.CreateTranslation(modelPos);

            //Matrix.create
            var viewportAdapter = new BoxingViewportAdapter(game.Window, _graphicsDevice, 256, 224);
            _camera = new OrthographicCamera(viewportAdapter);
            
            _cam3d = new PerspectiveCamera(_graphicsDevice, game.Window);
            _cam3d.Position = new Vector3(0,1, 0);
            cam3dPos = _cam3d.Position;
            _cam3d.LookAtDirection = Vector3.Forward;
            _cam3d.MovementUnitsPerSecond = 10f;
            _cam3d.farClipPlane = 100;
            _cam3d.nearClipPlane = 0.05f;
            _cam3d.fieldOfViewDegrees = 45;
            _cam3d.RotationRadiansPerSecond = 100f;
            
            basicEffectPrimitives = new BasicEffect(_graphicsDevice);
            
                // Debug gui
            var debugGuiRendererGameStats = new MonoGameImGuiRenderer(game);
            debugGuiRendererGameStats.Initialize();
            _guiManager = new GuiDebugManager(game, this, debugGuiRendererGameStats);

            const int dotPosStartX = -10;
            const int dotPosY = 0;
            const int dotPosStartZ = 10;
            const int dotPosOffsetX = 4;
            const int dotPosOffsetZ = 10;
            int dotPosZ = dotPosStartZ;
            for (int z = 0; z < floorDots2D.GetLength(0); z++) // 0-5
            {
                int dotPosX = dotPosStartX;
                dotPosZ -= dotPosOffsetZ;
                for (int x = 0; x < floorDots2D.GetLength(1); x++) // 0-6
                {
                    Console.WriteLine("x: " + dotPosX + " z: " + dotPosZ);
                    
                    floorDots2D[z,x].X = dotPosX;
                    floorDots2D[z,x].Y = dotPosY;
                    floorDots2D[z,x].Z = dotPosZ;
                    
                    dotPosX += dotPosOffsetX;
                }
            }

            camNextZ = _cam3d.Position.Z - dotsOneStep;
        }
        
        private const float dotsOneStep = 10;
        private float camNextZ;

        private Vector3 cam3dPos;
        private Vector3 cam3dRot;
        private Vector3 cam3dLookAtTarget;

        private bool _upKeyReleased, _downKeyReleased, _leftKeyReleased, _rightKeyReleased;

        private Keys _newScreenKey = Keys.R;

        // Arrow keys are not checked in WasJustReleased/WasJustPressed!
        private Keys _moveTankUpKey = Keys.Up,
            _moveTankDownKey = Keys.Down,
            _moveTankLeftKey = Keys.Left,
            _moveTankRightKey = Keys.Right;

        public override void Input(GameTime gt, float dt)
        {
            if (KeyboardExtended.GetState().IsKeyDown(_newScreenKey))
            {
                //Game.ScreenManager.AddScreen(new PlayScreen(Game, AssMan, SpriteBatch,
                //    _graphicsDevice /*, DebugGuiRenderer*/));
            }

            // TANK
            /*if (KeyboardExtended.GetState().IsKeyUp(_moveTankUpKey))
            {
                _upKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankUpKey))
            {
                if (_upKeyReleased)
                {
                    
                    _upKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankDownKey))
            {
                _downKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankDownKey))
            {
                if (_downKeyReleased)
                {
                    
                    _downKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankLeftKey))
            {
                _leftKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankLeftKey))
            {
                if (_leftKeyReleased)
                {
                    
                    _leftKeyReleased = false;
                }
            }

            if (KeyboardExtended.GetState().IsKeyUp(_moveTankRightKey))
            {
                _rightKeyReleased = true;
            }

            if (KeyboardExtended.GetState().IsKeyDown(_moveTankRightKey))
            {
                if (_rightKeyReleased)
                {
                    
                    _rightKeyReleased = false;
                }
            }*/
            
            //CAM
            const float camSpeedZ = 33;
            const float camSpeedY = 5;
            const float camSpeedX = 10;
            const float camRotMaxX = 0.1f;
            const float camRotXResetTolerance = 0.01f;
            const float camPosMaxX = 7.5f;
            const float camPosMaxY = 6.5f;
            const float camPosMinY = 1;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                //cam3dPos.Z -= camSpeed * dt;
                cam3dPos.Y += camSpeedY * dt;
            }

            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
            {
                //cam3dPos.Z += camSpeed * dt;
                cam3dPos.Y -= camSpeedY * dt;
            }

            if (cam3dPos.Y > camPosMaxY)
                cam3dPos.Y = camPosMaxY;
            
            if (cam3dPos.Y < camPosMinY)
                cam3dPos.Y = camPosMinY;
            
            // always
            cam3dPos.Z -= camSpeedZ * dt;
            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                // BUG: No limit, does not set minimum rotation: just stops rotating.
                if (_cam3d.Up.X <= -camRotMaxX) Console.WriteLine("Cant rotate more!: " + _cam3d.Up.X);
                else _cam3d.RotateRollCounterClockwise(gt, 40f);
                
                cam3dPos.X -= camSpeedX * dt;
                bgPos.X += camSpeedZ * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                if (_cam3d.Up.X >= camRotMaxX) Console.WriteLine("Cant rotate more!: " + _cam3d.Up.X);
                else _cam3d.RotateRollClockwise(gt, 40f);
                
                cam3dPos.X += camSpeedX * dt;
                bgPos.X -= camSpeedZ * dt;
            }
            
            if (!KeyboardExtended.GetState().IsKeyDown(Keys.A) && _cam3d.Up.X < 0)
            {
                if (_cam3d.Up.X <= -camRotXResetTolerance)
                {
                    _cam3d.RotateRollClockwise(gt, 20f);
                }
                else
                {
                    _cam3d.Up = Vector3.Up;
                }
            } else if (!KeyboardExtended.GetState().IsKeyDown(Keys.D) && _cam3d.Up.X > 0)
            {
                if (_cam3d.Up.X >= camRotXResetTolerance)
                {
                    _cam3d.RotateRollCounterClockwise(gt, 20f);
                }
                else
                {
                    _cam3d.Up = Vector3.Up;
                }
            }
            
            //Console.WriteLine(_cam3d.Up);
            
            if (cam3dPos.X < -camPosMaxX)
            {
                cam3dPos.X = -camPosMaxX;
                bgPos = bgPosOld;
            }

            if (cam3dPos.X > camPosMaxX)
            {
                cam3dPos.X = camPosMaxX;
                bgPos = bgPosOld;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.Z))
            {
                cam3dPos.Y += camSpeedZ * dt;
            }
            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.X))
            {
                cam3dPos.Y -= camSpeedZ * dt;
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
                _cam3d.NonLocalRotateUpOrDown(gt, 1);
            }
            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.T))
            {
                //_cam3d.RotateUpOrDown(gt, -1);
                _cam3d.NonLocalRotateUpOrDown(gt, -1);
            }
            
            _cam3d.Position = cam3dPos;

            bgPosOld = bgPos;
            
            Console.WriteLine(_cam3d.Position.Y);
        }
        
        public override void Tick(GameTime gt, float dt)
        {
            RemoveDestroyedEntities();

            UpdateAllEntities(dt);
        }

        private float _newPosX, _newPosY;
        private int _acc;
        private Vector3 floorPos = new Vector3(0,0,-50);
        private Vector3 floorDotsPosOffset = Vector3.Zero;

        private Vector2 bgPos = new Vector2(128, 0 + 11);
        private Vector2 bgPosOld;
        
        public override void Draw(float dt)
        {
            _graphicsDevice.Clear(Color.HotPink);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,
                null, _camera.GetViewMatrix());

            // old code
            const float radius = 100;
            _newPosX += (float)Math.Cos(MathHelper.ToRadians(_acc)) * radius * dt;
            _newPosY += (float)Math.Sin(MathHelper.ToRadians(_acc)) * radius * dt;

            _acc++;
            const int resetAngle = 360;
            if (_acc >= resetAngle)
                _acc -= resetAngle;
            //old code end
            
            
            
            SpriteBatch.Draw(bg02, bgPos, null, Color.White, -_cam3d.Up.X, new Vector2(256, 256), Vector2.One, SpriteEffects.None, 0);

            DrawAllEntities(dt);

            SpriteBatch.End(); // TODO: Can be called in DrawGUI later?
            
            if (modelPos.Z > 0)
                modelPos.Z -= 100;
            
            modelPos.Z += 20 * dt;
            modelRot.X += 2.5f * dt;
            modelTransform = Matrix.CreateRotationX(modelRot.X) * Matrix.CreateRotationY(modelRot.Y) * Matrix.CreateRotationZ(modelRot.Z) * Matrix.CreateTranslation(modelPos);

            Vector3 newFloorPos = new Vector3(cam3dPos.X, 0, cam3dPos.Z + floorPos.Z);
            //DrawModel(floorModel, Matrix.CreateTranslation(newFloorPos));

            
            //Console.WriteLine(_cam3d.Position.Z + " | " + camNextZ);
            
            if (_cam3d.Position.Z < camNextZ)
            {
                floorDotsPosOffset.Z -= dotsOneStep;
                camNextZ = (int) _cam3d.Position.Z - dotsOneStep;
            }

            // Draw dots on floor
            basicEffectPrimitives.View = _cam3d.View;
            basicEffectPrimitives.Projection = _cam3d.Projection;
            basicEffectPrimitives.DiffuseColor = new Vector3(1, 1, 1);
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < floorDots2D.GetLength(0); i++) // 0-5
            {
                for (int j = 0; j < floorDots2D.GetLength(1); j++) // 0-6
                {
                    var vertices = new[] { new VertexPositionColor(floorDots2D[i,j] + floorDotsPosOffset, Color.White),  new VertexPositionColor(floorDots2D[i,j] + floorDotsMiniOffset + floorDotsPosOffset, Color.White) };
                    _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                }
            }
            
            // For debugging
            Vector3 startPoint = new Vector3(-10, 0, -60);
            Vector3 endPoint = new Vector3(10, 0, -60);
            basicEffectPrimitives.DiffuseColor = new Vector3(1, 0, 0);
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            var vertices2 = new[] { new VertexPositionColor(startPoint, Color.Red),  new VertexPositionColor(endPoint, Color.Red) };
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices2, 0, 1);
            
            DrawModel(testModel, modelTransform);
        }

        private void DrawModel(Model model, Matrix modelTransform)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = modelTransform;
                    effect.View = _cam3d.View;
                    effect.Projection = _cam3d.Projection;
                    //effect.EnableDefaultLighting();
                    //effect.PreferPerPixelLighting = true;
                    effect.Alpha = 1;
                    
                    for (var i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                    {
                        effect.CurrentTechnique.Passes[i].Apply();
                    }
                }

                mesh.Draw();
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
