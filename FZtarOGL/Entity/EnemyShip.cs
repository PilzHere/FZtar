using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace FZtarOGL.Entity
{
    public class EnemyShip : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale, _modelRot, _modelPos;

        private Model _model;
        private ModelMesh _thrusterMesh;

        private Vector3[] _thrusterColors;
        private Vector3 _currentThrusterColor;
        private int _thrusterColorOrdered;
        private float _timerThrusterColor;

        private int _hp, _maxHp = 1;
        //public int Hp => _hp;

        private List<BoundingBoxFiltered> _boxfes;

        //private const float playerShipMaxPosX = 9;
        private float speedX = 7; // 7

        //private const float playerShipRotZSpeed = 2;
        private float _rotXSpeed = 1;
        private float _rotYSpeed = 1;

        //private const float SpeedY = 5;
        //private float speedZ = 0; // 33

        private float _rotXMaxAngle = 0.25f;
        private float _rotZMaxAngle = 0.25f;
        private float _rotYMaxAngle = 0.35f;

        //public float SpeedX => speedX;
        //public float RotYSpeed => _rotYSpeed;
        //public float RotZMaxAngle => _rotZMaxAngle;
        //public float RotYMaxAngle => _rotYMaxAngle;

        private Screen.Screen _screen;

        private BoundingBoxFiltered _boxf;
        private const float BoxfMinX = -2, BoxfMinY = -1.5f, BoxfMinZ = -2;
        private const float BoxfMaxX = 2, BoxfMaxY = 1.5f, BoxfMaxZ = 2;
        private Vector3 _min;
        private Vector3 _max;

        private bool _gotHit;
        //private float gotHitTimer;
        //private bool toRenderShip = true;

        private AssetManager _assMan;

        private PlayerShip _targetShip;
        private Vector3 _aimPos;
        private float _shootTimer;
        private bool _canShoot;

        private bool _moveTowardsPlayer;
        private float _spawnY;

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

            _spawnY = _modelTrans.Translation.Y;

            _boxfes = new List<BoundingBoxFiltered>();
            _min = new Vector3(_modelTrans.Translation.X + BoxfMinX, _modelTrans.Translation.Y + BoxfMinY,
                _modelTrans.Translation.Z + BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,
                _modelTrans.Translation.Z + BoxfMaxZ);
            _boxf = new BoundingBoxFiltered(this, _min, _max, BoxFilters.FilterEnemyShip, BoxFilters.MaskEnemyShip);
            _boxfes.Add(_boxf);

            // thruster colors
            _thrusterColors = new Vector3[4];
            _thrusterColors[0] = ModelColors.PlayerThrusterColor1;
            _thrusterColors[1] = ModelColors.PlayerThrusterColor2;
            _thrusterColors[2] = ModelColors.PlayerThrusterColor3;
            _thrusterColors[3] = ModelColors.PlayerThrusterColor4;
            _currentThrusterColor = _thrusterColors[0];

            _hp = _maxHp;
        }

        private void AiTick(float dt)
        {
            var minY = _spawnY - 10;
            
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
                    if (_canShoot)
                    {
                        if (direction.Z > 0) // Don't shoot behind.
                        {
                            _screen._EntitiesToAdd.Add(new EnemyRay(_screen, _assMan, _modelPos, direction));
                            _canShoot = false;
                        }
                    }
                    else
                    {
                        var shootCoolDown = 0.5f;
                        _shootTimer += dt;
                        if (_shootTimer >= shootCoolDown)
                        {
                            _shootTimer = 0;
                            _canShoot = true;
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

            // trans
            /*_modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateRotationX(_modelRot.X) *
                          Matrix.CreateRotationY(_modelRot.Y) *
                          Matrix.CreateRotationZ(_modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);*/

            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateWorld(_modelPos, _modelRot, _modelTrans.Up);
                          

            // bb
            _min = new Vector3(_modelTrans.Translation.X + BoxfMinX, _modelTrans.Translation.Y + BoxfMinY,
                _modelTrans.Translation.Z + BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,
                _modelTrans.Translation.Z + BoxfMaxZ);
            _boxf.Box = new BoundingBox(_min, _max);

            _boxfes.Clear();
            _boxfes.Add(_boxf);

            foreach (var boxf in _boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

            // thrustercolor
            _timerThrusterColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = _timerThrusterColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                _thrusterColorOrdered++;
                if (_thrusterColorOrdered > _thrusterColors.Length - 1) _thrusterColorOrdered = 0;
                _currentThrusterColor = _thrusterColors[_thrusterColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                _timerThrusterColor = 0;
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
            _screen.DrawModelMeshUnlitWithColor(_thrusterMesh, _modelTrans, _currentThrusterColor);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(_boxfes, Matrix.Identity);
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