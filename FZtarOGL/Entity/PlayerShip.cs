using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace FZtarOGL.Entity
{
    public class PlayerShip : Entity
    {
        public Matrix ModelTrans;
        public Vector3 ModelScale;
        public Vector3 ModelRot;
        public Vector3 ModelPos;

        private Model _model;
        private ModelMesh _thrusterMesh;

        private Vector3[] thrusterColors;
        private Vector3 currentThrusterColor;
        private int thrusterColorOrdered;
        private float timerThrusterColor;

        private Texture2D _aimTex;

        private Vector3 _aimPos2d;
        private Vector2 _aimPos2dInt;
        private Vector3 _aimPos3d;

        private int _maxHp = 50;
        private int _hp;
        private int _maxPower = 50;
        private int _power;

        public int Hp => _hp;
        public int Power => _power;
        public Model Model => _model;

        //private BoundingBox _box;
        private List<BoundingBoxFiltered> boxfes;

        //private const float playerShipMaxPosX = 9;
        private float speedX = 7; // 7

        //private const float playerShipRotZSpeed = 2;
        private float rotXSpeed = 1;
        private float rotYSpeed = 1;
        private const float speedY = 5;
        private float speedZ = 0; // 33

        private float rotXMaxAngle = 0.25f;
        private float rotZMaxAngle = 0.25f;
        private float rotYMaxAngle = 0.35f;

        public float SpeedX => speedX;
        public float RotYSpeed => rotYSpeed;
        public float RotZMaxAngle => rotZMaxAngle;
        public float RotYMaxAngle => rotYMaxAngle;

        private Screen.Screen _screen;
        private SpriteBatch _spriteBatch;
        private PerspectiveCamera _cam;

        private Viewport _viewport;

        private AssetManager _assMan;

        public BoundingBoxFiltered boxf; // test!
        private const float boxfMinX = 0.75f, boxfMinY = 0.15f, boxfMinZ = 0.5f;
        private const float boxfMaxX = 0.75f, boxfMaxY = 0.35f, boxfMaxZ = 0.5f;
        private Vector3 min;
        private Vector3 max;

        private bool gotHit;
        private float gotHitTimer;
        private bool toRenderShip = true;

        public PlayerShip(Screen.Screen screen, SpriteBatch spriteBatch, AssetManager assMan, Vector3 position,
            PerspectiveCamera cam)
        {
            _screen = screen;
            _spriteBatch = spriteBatch;
            _cam = cam;
            _assMan = assMan;

            _aimTex = assMan.PlayerAimTex;
            _model = assMan.PlayerShipModel;
            _thrusterMesh = _model.Meshes[1];

            ModelPos = position;
            ModelScale = Vector3.One;
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);

            _viewport = new Viewport(0, 0, 256, 224);

            _hp = _maxHp;
            _power = 0;

            boxfes = new List<BoundingBoxFiltered>();

            min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,
                ModelTrans.Translation.Z - boxfMinZ);
            max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,
                ModelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterPlayerShip, BoxFilters.MaskPlayerShip);

            boxfes.Add(boxf);

            // thruster colors
            thrusterColors = new Vector3[4];
            thrusterColors[0] = ModelColors.PlayerThrusterColor1;
            thrusterColors[1] = ModelColors.PlayerThrusterColor2;
            thrusterColors[2] = ModelColors.PlayerThrusterColor3;
            thrusterColors[3] = ModelColors.PlayerThrusterColor4;
            currentThrusterColor = thrusterColors[0];
        }

        private bool keyUpIsPressed;
        private bool keySpaceIsPressed;

        private float rotationGlobalY;

        private Keys keyMoveUp = Keys.W;
        private Keys keyMoveDown = Keys.S;

        public void Input(GameTime gt, float dt)
        {
            if (GameSettings.GameSettings.InvertVerticalMovement)
            {
                keyMoveUp = Keys.S;
                keyMoveDown = Keys.W;
            }
            else
            {
                keyMoveUp = Keys.W;
                keyMoveDown = Keys.S;
            }

            // TODO: For testing!
            if (KeyboardExtended.GetState().IsKeyUp(Keys.Space))
            {
                keySpaceIsPressed = false;
            }

            if (!keySpaceIsPressed && KeyboardExtended.GetState().IsKeyDown(Keys.Space))
            {
                _screen.CurrentLevel.MoveEverythingForward1 = !_screen.CurrentLevel.MoveEverythingForward1;
                keySpaceIsPressed = true;
            }
            // TEsting end

            if (KeyboardExtended.GetState().IsKeyUp(Keys.Up))
            {
                keyUpIsPressed = false;
            }

            if (!keyUpIsPressed && KeyboardExtended.GetState().IsKeyDown(Keys.Up))
            {
                _screen._Entities.Add(new PlayerRay(_screen, _assMan, ModelPos, ModelRot));
                keyUpIsPressed = true;
            }

            float rotXSpeedBoost = 1;

            if (KeyboardExtended.GetState().IsKeyDown(keyMoveUp))
            {
                if (ModelRot.X < 0) ModelRot.X += rotXSpeed * rotXSpeedBoost * dt;
                else ModelRot.X += rotXSpeed * dt;

                ModelPos.Y += speedY * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(keyMoveDown))
            {
                if (ModelRot.X > 0) ModelRot.X -= rotXSpeed * rotXSpeedBoost * dt;
                else ModelRot.X -= rotXSpeed * dt;

                ModelPos.Y -= speedY * dt;
            }

            const float rotYSpeedBoost = 1;

            if (KeyboardExtended.GetState().IsKeyDown(Keys.A))
            {
                ModelPos.X -= speedX * dt;

                if (ModelRot.Y < 0) ModelRot.Y += rotYSpeed * rotYSpeedBoost * dt;
                else ModelRot.Y += rotYSpeed * dt;
                //playerShipModelRot.Z += playerShipRotZSpeed * dt;

                rotationGlobalY -= 1 * dt;

                ModelRot.Z += 3 * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                ModelPos.X += speedX * dt;

                if (ModelRot.Y > 0) ModelRot.Y -= rotYSpeed * rotYSpeedBoost * dt;
                else ModelRot.Y -= rotYSpeed * dt;
                //playerShipModelRot.Z -= playerShipRotZSpeed * dt;

                rotationGlobalY += 1 * dt;

                ModelRot.Z -= 3 * dt;
            }
        }

        public override void Tick(float dt)
        {
            if (gotHit)
            {
                float gotHitCooldown = 1.33f;
                gotHitTimer += dt;
                if (gotHitTimer >= gotHitCooldown)
                {
                    gotHitTimer = 0;
                    toRenderShip = true;
                    gotHit = false;
                }
            }

            //speedZ = _screen.CurrentLevel.VirtualSpeedZ;

            //ModelPos.Z -= speedZ * dt; // move forward

            // Rotate back, 0 if in deadzone
            const float modelRotXDeadZone = 0.015f;
            const float modelRotZDeadZone = 0.015f;
            const float modelRotYDeadZone = 0.015f;
            const float rotBackSpeed = 0.5f;

            // Rotate back, 0 if in deadzone
            // X rot 
            if (ModelRot.X > 0)
            {
                if (ModelRot.X > modelRotXDeadZone) ModelRot.X -= rotBackSpeed * dt;
                else ModelRot.X = 0;
            }

            if (ModelRot.X < 0)
            {
                if (ModelRot.X < -modelRotXDeadZone) ModelRot.X += rotBackSpeed * dt;
                else ModelRot.X = 0;
            }

            // Z rot
            if (ModelRot.Z > 0)
            {
                if (ModelRot.Z > modelRotZDeadZone) ModelRot.Z -= rotBackSpeed * dt;
                else ModelRot.Z = 0;
            }

            if (ModelRot.Z < 0)
            {
                if (ModelRot.Z < -modelRotZDeadZone) ModelRot.Z += rotBackSpeed * dt;
                else ModelRot.Z = 0;
            }

            // Y rot 
            if (ModelRot.Y > 0)
            {
                if (ModelRot.Y > modelRotYDeadZone) ModelRot.Y -= rotBackSpeed * dt;
                else ModelRot.Y = 0;
            }

            if (ModelRot.Y < 0)
            {
                if (ModelRot.Y < -modelRotYDeadZone) ModelRot.Y += rotBackSpeed * dt;
                else ModelRot.Y = 0;
            }

            // Z rot
            if (ModelRot.Z > 0)
            {
                if (ModelRot.Z > modelRotZDeadZone) ModelRot.Z -= rotBackSpeed * dt;
                else ModelRot.Z = 0;
            }

            if (ModelRot.Z < 0)
            {
                if (ModelRot.Z < -modelRotZDeadZone) ModelRot.Z += rotBackSpeed * dt;
                else ModelRot.Z = 0;
            }

            // limit player ship pos y
            const float modelPosMaxY = 7.5f;
            const float modelPosMinY = 0.5f;

            if (ModelPos.Y < modelPosMinY) ModelPos.Y = modelPosMinY;
            if (ModelPos.Y > modelPosMaxY) ModelPos.Y = modelPosMaxY;

            // limit player ship pos x
            const float modelPosMaxX = 9;

            if (ModelPos.X < -modelPosMaxX) ModelPos.X = -modelPosMaxX;
            if (ModelPos.X > modelPosMaxX) ModelPos.X = modelPosMaxX;

            if (ModelRot.X < -rotXMaxAngle) ModelRot.X = -rotXMaxAngle;
            if (ModelRot.X > rotXMaxAngle) ModelRot.X = rotXMaxAngle;

            if (ModelRot.Y < -rotYMaxAngle) ModelRot.Y = -rotYMaxAngle;
            if (ModelRot.Y > rotYMaxAngle) ModelRot.Y = rotYMaxAngle;

            if (ModelRot.Z < -rotZMaxAngle) ModelRot.Z = -rotZMaxAngle;
            if (ModelRot.Z > rotZMaxAngle) ModelRot.Z = rotZMaxAngle;

            // trans
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);

            // bb
            min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,
                ModelTrans.Translation.Z - boxfMinZ);
            max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,
                ModelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

            //_screen.BoundingBoxesFilteredLists.Add(boxes);

            // thrustercolor
            timerThrusterColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = timerThrusterColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                thrusterColorOrdered++;
                if (thrusterColorOrdered > thrusterColors.Length - 1) thrusterColorOrdered = 0;
                currentThrusterColor = thrusterColors[thrusterColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                timerThrusterColor = 0;
            }

            // aim
            const float camPlayerDist = 6;
            //const float fovFar = float.MinValue + camPlayerDist;
            const float fovFar = -200 + camPlayerDist;

            _aimPos3d.X = ModelTrans.Forward.X * fovFar * dt;
            _aimPos3d.Y = ModelTrans.Forward.Y * fovFar * dt;
            _aimPos3d.Z = ModelTrans.Forward.Z * fovFar * dt;

            //_aimPos3d = ModelTrans.Translation + new Vector3(0, 0, fovFar);
            _aimPos2d = _viewport.Project(_aimPos3d, _cam.Projection, _cam.View, _cam.World);
            _aimPos2dInt.X = (int)_aimPos2d.X - _aimTex.Width / 2;
            _aimPos2dInt.Y = (int)_aimPos2d.Y - _aimTex.Height / 2;
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterObstacle:
                    if (!gotHit)
                    {
                        TakeDamage(10);
                        gotHit = true;
                    }

                    break;
                case BoxFilters.FilterRingHealth:
                    Heal(10);
                    break;
                case BoxFilters.FilterRingPower:
                    IncreasePower(10);
                    break;
                case BoxFilters.FilterEnemyShip:
                    if (!gotHit)
                    {
                        TakeDamage(10);
                        gotHit = true;
                    }

                    break;
                case BoxFilters.FilterEnemyRay:
                    if (!gotHit)
                    {
                        TakeDamage(10);
                        gotHit = true;
                    }

                    break;
            }
        }

        private void Heal(int amount)
        {
            if (_hp < _maxHp) _hp += amount;
            if (_hp > _maxHp) _hp = _maxHp;
        }

        private void TakeDamage(int amount)
        {
            if (_hp > 0) _hp -= amount;
            if (_hp < 0) _hp = 0;
            
            SoundEffectInstance sfx = _assMan.SfxPlayerHit.CreateInstance();
            sfx.Volume = GameSettings.GameSettings.SfxVolume;
            sfx.Play();
        }

        private void IncreasePower(int amount)
        {
            if (_power < _maxPower) _power += amount;
            if (_power > _maxPower) _power = _maxPower;
        }

        private void ReducePower(int amount)
        {
            if (_power > 0) _power -= amount;
            if (_power < 0) _power = 0;
        }

        public override void Draw2D(float dt)
        {
        }

        private float _gotHitFlashTime = 0.033f;
        private float _gotHitFlashTimer;

        public override void Draw3D(float dt)
        {
            if (gotHit)
            {
                _gotHitFlashTimer += dt;
                if (_gotHitFlashTimer >= _gotHitFlashTime)
                {
                    toRenderShip = !toRenderShip;
                    _gotHitFlashTimer = 0;
                }
            }

            if (toRenderShip)
            {
                _screen.DrawModel(_model, ModelTrans);

                _screen.DrawModelMeshUnlitWithColor(_thrusterMesh, ModelTrans, currentThrusterColor);
            }
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
        }

        public void DrawAim(float dt)
        {
            _spriteBatch.Draw(_aimTex, _aimPos2dInt, null, Color.White, 0, Vector2.Zero, Vector2.One,
                SpriteEffects.None,
                0);
        }

        public override void Destroy()
        {
            base.Destroy();
            
            ReducePower(_maxPower);
            
            SoundEffectInstance sfx = _assMan.SfxPlayerDeath.CreateInstance();
            sfx.Volume = GameSettings.GameSettings.SfxVolume;
            sfx.Play();
            
            _screen._EntitiesToAdd.Add(new Explosion01(_screen, _assMan, ModelPos));
        }
    }
}