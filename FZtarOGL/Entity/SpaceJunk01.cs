using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class SpaceJunk01 : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelRot;
        private Vector3 _modelPos;

        private Model _modelSpaceJunk;

        private Vector3 _direction;
        private bool _rotateLeft;

        private List<BoundingBoxFiltered> boxfes; // public for testing! cange to private

        private Screen.Screen _screen;

        private BoundingBoxFiltered boxf;
        private Vector3 _min;
        private Vector3 _max;
        private const float BoxfMinX = 0.75f * 2, BoxfMinY = 0.75f * 2, BoxfMinZ = 0.25f * 2;
        private const float BoxfMaxX = 0.75f * 2, BoxfMaxY = 0.75f * 2, BoxfMaxZ = 0.25f * 2;

        private AssetManager _assMan;

        public SpaceJunk01(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 direction)
        {
            _screen = screen;
            _direction = direction;
            _assMan = assMan;

            _modelSpaceJunk = assMan.SpaceJunk01Model;
            
            _modelScale = Vector3.One;
            _modelRot = Vector3.Zero;
            _modelPos = position;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateRotationX(_modelRot.X) *
                          Matrix.CreateRotationY(_modelRot.Y) *
                          Matrix.CreateRotationZ(_modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);

            boxfes = new List<BoundingBoxFiltered>();

            _min = new Vector3(_modelTrans.Translation.X - BoxfMinX, _modelTrans.Translation.Y - BoxfMinY,
                _modelTrans.Translation.Z - BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,
                _modelTrans.Translation.Z + BoxfMaxZ);
            boxf = new BoundingBoxFiltered(this, _min, _max, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);

            boxfes.Add(boxf);

            if (MathUtils.GetRandomNumberBetween0And1() == 1) _rotateLeft = true;
        }

        public override void Tick(float dt)
        {
            if (_modelPos.Z > 10)
            {
                _ToDestroy = true;
            }

            // move forward
            Vector3 direction = _modelPos - -_direction;
            Vector3.Normalize(direction);

            float speedDampening = 150f;
            float speedX = _screen.CurrentLevel.VirtualSpeedZ / speedDampening * 1.2f;
            float speedY = _screen.CurrentLevel.VirtualSpeedZ / speedDampening * 1.2f;
            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            _modelPos.X += direction.X * speedX * dt;
            _modelPos.Y += direction.Y * speedY * dt;
            _modelPos.Z += speedZ * dt;
            
            // Look at cam.
            _modelTrans = Matrix.CreateBillboard(_modelPos, _screen.Cam3d.Position, _screen.Cam3d.Up,
                _screen.Cam3d.Forward);

            // rotate
            float rotSpeedZ = 2.5f;
            if (_rotateLeft)_modelRot.Z -= rotSpeedZ * dt;
            else _modelRot.Z += rotSpeedZ * dt;

            _modelTrans = Matrix.CreateRotationZ(-_screen.Cam3d.Up.X + _modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);

            // box
            _min = new Vector3(_modelTrans.Translation.X - BoxfMinX, _modelTrans.Translation.Y - BoxfMinY,
                _modelTrans.Translation.Z - BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,
                _modelTrans.Translation.Z + BoxfMaxZ);
            boxf.Box = new BoundingBox(_min, _max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

            // Add transparent model(s) to render last.
            _screen.TransparentModels.Add(new TransparentModel(_modelSpaceJunk, _modelTrans));
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterObstacle:
                    Destroy();
                    break;
                case BoxFilters.FilterEnemyShip:
                    Destroy();
                    break;
                case BoxFilters.FilterEnemyRay:
                    Destroy();
                    break;
                case BoxFilters.FilterPlayerShip:
                    Destroy();
                    break;
                case BoxFilters.FilterPlayerRay:
                    Destroy();
                    break;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            
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
        }
    }
}