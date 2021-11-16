using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class Turret : Entity
    {
        private Screen.Screen _screen;

        private Matrix modelTrans;
        private Vector3 modelScale;
        private Vector3 modelPos;
        private Vector3 modelRot;

        private Model _model;
        private ModelMesh _gunMesh;
        
        private List<BoundingBoxFiltered> boxfes;
        public BoundingBoxFiltered boxf; // test!
        private const float boxfMinX = -1f, boxfMinY = -1f, boxfMinZ = -1f;
        private const float boxfMaxX = 1f, boxfMaxY = 1f, boxfMaxZ = 1f;
        private Vector3 min;
        private Vector3 max;
        
        private Vector3[] gunColor;
        private Vector3 currentGunColor;

        //private Random rnd = new Random();
        private int gunColorOrdered;
        private float timerGunColor;

        private bool canShoot = true;
        private float shootTimer;

        private AssetManager _assMan;

        public Turret(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            modelPos = position;
            _assMan = assMan;

            _model = _assMan.Turret01Model;
            _gunMesh = _model.Meshes[1];

            modelScale = Vector3.One;
            modelRot.Y = MathHelper.ToRadians(180);
            modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);

            boxfes = new List<BoundingBoxFiltered>();
            
            min = new Vector3(modelTrans.Translation.X + boxfMinX, modelTrans.Translation.Y + boxfMinY,modelTrans.Translation.Z + boxfMinZ);
            max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterEnemyTurret, BoxFilters.MaskEnemyTurret);
            
            boxfes.Add(boxf);

            // shader colors
            gunColor = new Vector3[4];
            gunColor[0] = ModelColors.PlayerRayColor1;
            gunColor[1] = ModelColors.PlayerRayColor2;
            gunColor[2] = ModelColors.PlayerRayColor3;
            gunColor[3] = ModelColors.PlayerRayColor4;
            currentGunColor = gunColor[0];
        }

        private void AiTick(float dt)
        {
            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            modelPos.Z += speedZ * dt;

            if (_screen.PlayerShip != null)
            {
                Vector3 direction = Vector3.Normalize(_screen.PlayerShip.ModelPos - modelPos);
                modelRot = direction;

                float distance = Vector3.Distance(modelPos, _screen.PlayerShip.ModelPos);
                if (distance < 150)
                {
                    if (canShoot)
                    {
                        if (direction.Z > 0)
                        {
                            // Don't shoot behind.
                            _screen._EntitiesToAdd.Add(new EnemyRay(_screen, _assMan, modelPos + modelRot, modelRot));
                            canShoot = false;
                        }
                    }
                    else
                    {
                        float shootCd = 0.5f;
                        shootTimer += dt;
                        if (shootTimer >= shootCd)
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
            
            if (modelPos.Z > 10)
                Destroy();

            // trans
            modelTrans = Matrix.CreateScale(modelScale) * Matrix.CreateWorld(modelPos, modelRot, modelTrans.Up);

            // bb
            min = new Vector3(modelTrans.Translation.X + boxfMinX, modelTrans.Translation.Y + boxfMinY,modelTrans.Translation.Z + boxfMinZ);
            max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
            
            // raycolor
            timerGunColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = timerGunColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                gunColorOrdered++;
                if (gunColorOrdered > gunColor.Length - 1) gunColorOrdered = 0;
                currentGunColor = gunColor[gunColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                timerGunColor = 0;
            }
            
            //Console.WriteLine("timer: " + timerRayColor);
            //Console.WriteLine(currentRayColor);
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterPlayerRay:
                    Destroy();
                    break;
                case BoxFilters.FilterPlayerShip:
                    Destroy();
                    break;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_model, modelTrans);
            
            _screen.DrawModelMeshUnlitWithColor(_gunMesh, modelTrans, currentGunColor);
        }
        
        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
            //_screen.DrawBoundingBox(boundingBoxes, modelTrans);
        }

        public override void Destroy()
        {
            base.Destroy();
            
            SoundEffectInstance sfx = _assMan.SfxEnemyHit.CreateInstance();
            sfx.Volume = GameSettings.GameSettings.SfxVolume;
            sfx.Play();
            
            _screen._EntitiesToAdd.Add(new Explosion01(_screen, _assMan, modelPos));
        }
    }
}