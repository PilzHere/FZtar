using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace FZtarOGL.Entity
{
    public class EnemyShip : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale, _modelRot, _modelPos;

        public Vector3 ModelPos => _modelPos;

        private Model _model;
        private ModelMesh _thrusterMesh;

        private Vector3[] thrusterColors;
        private Vector3 currentThrusterColor;
        private int thrusterColorOrdered;
        private float timerThrusterColor;

        private int _hp, _maxHp = 1;
        public int Hp => _hp;

        private List<BoundingBoxFiltered> boxfes;

        //private const float playerShipMaxPosX = 9;
        private float speedX = 7; // 7

        //private const float playerShipRotZSpeed = 2;
        private float rotXSpeed = 1;
        private float rotYSpeed = 1;

        private const float speedY = 5;
        //private float speedZ = 0; // 33

        private float rotXMaxAngle = 0.25f;
        private float rotZMaxAngle = 0.25f;
        private float rotYMaxAngle = 0.35f;

        public float SpeedX => speedX;
        public float RotYSpeed => rotYSpeed;
        public float RotZMaxAngle => rotZMaxAngle;
        public float RotYMaxAngle => rotYMaxAngle;

        private Screen.Screen _screen;

        private BoundingBoxFiltered boxf;
        private const float boxfMinX = -2, boxfMinY = -1.5f, boxfMinZ = -2;
        private const float boxfMaxX = 2, boxfMaxY = 1.5f, boxfMaxZ = 2;
        private Vector3 min;
        private Vector3 max;

        private bool gotHit;
        //private float gotHitTimer;
        //private bool toRenderShip = true;

        private AssetManager _assMan;

        private PlayerShip _targetShip;
        private Vector3 _aimPos;
        private float shootTimer;
        private bool canShoot;

        private bool _moveTowardsPlayer = true;
        private float spawnY;

        public EnemyShip(Screen.Screen screen, AssetManager assMan, Vector3 position,
            bool moveTowardsPlayer)
        {
            _screen = screen;
            _targetShip = _screen.PlayerShip;
            _assMan = assMan;

            _moveTowardsPlayer = moveTowardsPlayer;

            _model = _assMan.EnemyShipModel01;
            _thrusterMesh = _model.Meshes[1];

            _modelPos = position + new Vector3(0, 10, 0);
            _modelScale = new Vector3(2, 4, 2);
            _modelRot = new Vector3(0, MathHelper.ToRadians(180), 0);
            //_modelRot = new Vector3(0, 0, 0);
            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateRotationX(_modelRot.X) *
                          Matrix.CreateRotationY(_modelRot.Y) *
                          Matrix.CreateRotationZ(_modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);

            spawnY = _modelTrans.Translation.Y;

            boxfes = new List<BoundingBoxFiltered>();
            min = new Vector3(_modelTrans.Translation.X + boxfMinX, _modelTrans.Translation.Y + boxfMinY,
                _modelTrans.Translation.Z + boxfMinZ);
            max = new Vector3(_modelTrans.Translation.X + boxfMaxX, _modelTrans.Translation.Y + boxfMaxY,
                _modelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterEnemyShip, BoxFilters.MaskEnemyShip);
            boxfes.Add(boxf);

            // thruster colors
            thrusterColors = new Vector3[4];
            thrusterColors[0] = ModelColors.PlayerThrusterColor1;
            thrusterColors[1] = ModelColors.PlayerThrusterColor2;
            thrusterColors[2] = ModelColors.PlayerThrusterColor3;
            thrusterColors[3] = ModelColors.PlayerThrusterColor4;
            currentThrusterColor = thrusterColors[0];

            _hp = _maxHp;
        }

        private void AiTick(float dt)
        {
            var minY = spawnY - 10;
            
            var speedZ = _screen.CurrentLevel.VirtualSpeedZ * 1.5f;

            Vector3 direction = new Vector3(0,0,1);
            
            if (_targetShip != null)
            {
                _aimPos = _targetShip.ModelPos;

                direction = Vector3.Normalize(_aimPos - _modelPos);

                if (_moveTowardsPlayer)
                {
                    if (_modelPos.Y > minY)
                    {
                        direction.Y -= 1.5f * dt;
                    }
                    /*else
                    {
                        _moveTowardsPlayer = false;
                    }*/

                    _modelRot = direction;
                }
                /*else
                {
                    Vector3 direction2 = Vector3.Normalize((direction + new Vector3(0, 0, 15)) - _modelPos);
                    direction2.Y += 5 * dt;
                    _modelRot = direction2;
                }*/
            }

            _modelPos += _modelRot * speedZ * dt;

            if (_targetShip != null)
            {
                var distanceToPlayerShip = Vector3.Distance(_modelPos, _aimPos);
                var maxDistToPlayerShip = 125f;
                
                if (distanceToPlayerShip >= 0 && distanceToPlayerShip <= maxDistToPlayerShip)
                {
                    if (canShoot)
                    {
                        if (direction.Z > 0) // Don't shoot behind.
                        {
                            _screen._EntitiesToAdd.Add(new EnemyRay(_screen, _assMan, _modelPos, direction));
                            canShoot = false;
                        }
                    }
                    else
                    {
                        var shootCoolDown = 0.5f;
                        shootTimer += dt;
                        if (shootTimer >= shootCoolDown)
                        {
                            shootTimer = 0;
                            canShoot = true;
                        }
                    }
                }
            }
        }

        public override void Tick(float dt)
        {
            AiTick(dt);

            if (_hp == 0)
            {
                Destroy();
            }

            if (gotHit)
            {
            }

            // trans
            /*_modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateRotationX(_modelRot.X) *
                          Matrix.CreateRotationY(_modelRot.Y) *
                          Matrix.CreateRotationZ(_modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);*/

            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateWorld(_modelPos, _modelRot, _modelTrans.Up);
                          

            // bb
            min = new Vector3(_modelTrans.Translation.X + boxfMinX, _modelTrans.Translation.Y + boxfMinY,
                _modelTrans.Translation.Z + boxfMinZ);
            max = new Vector3(_modelTrans.Translation.X + boxfMaxX, _modelTrans.Translation.Y + boxfMaxY,
                _modelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

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
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterObstacle:
                    TakeDamage(1);
                    break;
                case BoxFilters.FilterPlayerShip:
                    TakeDamage(1);
                    break;
                case BoxFilters.FilterPlayerRay:
                    TakeDamage(1);
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
        }

        public override void Draw2D(float dt)
        {
        }

        private float _gotHitFlashTime = 0.033f;
        private float _gotHitFlashTimer;

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_model, _modelTrans);
            _screen.DrawModelMeshUnlitWithColor(_thrusterMesh, _modelTrans, currentThrusterColor);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
        }

        public override void Destroy()
        {
            base.Destroy();
            
            SoundEffectInstance sfx = _assMan.SfxEnemyHit.CreateInstance();
            sfx.Volume = GameSettings.GameSettings.SfxVolume;
            sfx.Play();

            _screen._EntitiesToAdd.Add(new Explosion01(_screen, _assMan, _modelPos));
        }
    }
}