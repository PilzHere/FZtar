using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class HealthRing : Entity
    {
        private Matrix _modelTransRing;
        private Vector3 _modelScaleRing;
        private Vector3 _modelRotRing;
        private Vector3 _modelPosRing;

        private Model _modelRing;

        private Matrix _modelTransBillboard;
        private Vector3 _modelScaleBillboard;
        private Vector3 _modelRotBillboard;
        private Vector3 _modelPosBillboard;

        private Model _modelBillboard;

        private List<BoundingBoxFiltered> _boxfes; // public for testing! cange to private

        private Screen.Screen _screen;

        private AssetManager _assMan;

        private BoundingBoxFiltered _boxf;

        private const float BoxfMinX = 0.75f, BoxfMinY = 0.75f, BoxfMinZ = 0.125f;
        private const float BoxfMaxX = 0.75f, BoxfMaxY = 0.75f, BoxfMaxZ = 0.125f;

        private bool _healthGiven;

        public HealthRing(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _assMan = assMan;

            _modelRing = _assMan.Ring01Model;
            _modelBillboard = _assMan.BillboardHealth16Model;

            _modelScaleRing = Vector3.One;
            _modelRotRing = Vector3.Zero;
            _modelPosRing = position;
            _modelTransRing = Matrix.CreateScale(_modelScaleRing) *
                             Matrix.CreateRotationX(_modelRotRing.X) *
                             Matrix.CreateRotationY(_modelRotRing.Y) *
                             Matrix.CreateRotationZ(_modelRotRing.Z) *
                             Matrix.CreateTranslation(_modelPosRing);

            // Boundingboxes, one huge from all meshes min/max combined.
            _boxfes = new List<BoundingBoxFiltered>();

            /*Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                meshTransform = Matrix.CreateScale(1); // Why do I need this?
                boundingBoxes.Add(BoundingBoxBuilder.BuildBoundingBox(mesh, meshTransform));
            }*/


            Vector3 min = new Vector3(_modelTransRing.Translation.X - BoxfMinX, _modelTransRing.Translation.Y - BoxfMinY,
                _modelTransRing.Translation.Z - BoxfMinZ);
            Vector3 max = new Vector3(_modelTransRing.Translation.X + BoxfMaxX, _modelTransRing.Translation.Y + BoxfMaxY,
                _modelTransRing.Translation.Z + BoxfMaxZ);
            _boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterRingHealth, BoxFilters.MaskRingHealth);

            _boxfes.Add(_boxf);

            // billboard
            _modelScaleBillboard = Vector3.One;
            _modelRotBillboard = Vector3.Zero;
            _modelPosBillboard = position;
            _modelTransBillboard = Matrix.CreateScale(_modelScaleBillboard) *
                                  Matrix.CreateRotationX(_modelRotBillboard.X) *
                                  Matrix.CreateRotationY(_modelRotBillboard.Y) *
                                  Matrix.CreateRotationZ(_modelRotBillboard.Z) *
                                  Matrix.CreateTranslation(_modelPosBillboard);
        }

        public override void Tick(float dt)
        {
            if (_modelPosRing.Z > 10 || _modelPosBillboard.Z > 10)
            {
                Destroy();
            }

            // move forward
            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            _modelPosRing.Z += speedZ * dt;

            // rotate
            float rotSpeedZ = 2.5f;
            _modelRotRing.Z += rotSpeedZ * dt;

            // trans
            _modelTransRing = Matrix.CreateScale(_modelScaleRing) *
                             Matrix.CreateRotationX(_modelRotRing.X) *
                             Matrix.CreateRotationY(_modelRotRing.Y) *
                             Matrix.CreateRotationZ(_modelRotRing.Z) *
                             Matrix.CreateTranslation(_modelPosRing);

            // BILLBOARD
            // move forward
            _modelPosBillboard.Z += speedZ * dt;

            // trans
            // Look at cam.
            _modelTransBillboard = Matrix.CreateBillboard(_modelPosBillboard, _screen.Cam3d.Position, _screen.Cam3d.Up,
                _screen.Cam3d.Forward);

            // Rotate (stay steady even if camera rotates up/down) in Z axis.
            _modelTransBillboard = Matrix.CreateRotationZ(-_screen.Cam3d.Up.X) *
                                  Matrix.CreateTranslation(_modelPosBillboard);

            // box
            Vector3 min = new Vector3(_modelTransRing.Translation.X - BoxfMinX, _modelTransRing.Translation.Y - BoxfMinY,
                _modelTransRing.Translation.Z - BoxfMinZ);
            Vector3 max = new Vector3(_modelTransRing.Translation.X + BoxfMaxX, _modelTransRing.Translation.Y + BoxfMaxY,
                _modelTransRing.Translation.Z + BoxfMaxZ);
            _boxf.Box = new BoundingBox(min, max);

            if (_healthGiven)
            {
                _boxf.Filter = BoxFilters.FilterNothing;
            }

            _boxfes.Clear();
            _boxfes.Add(_boxf);

            foreach (var boxf in _boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

            // Add transparent model(s) to render last.
            if (!_healthGiven) _screen.TransparentModels.Add(new TransparentModel(_modelBillboard, _modelTransBillboard));
        }

        public override void OnCollision(int filter, float dt)
        {
            if (filter == BoxFilters.FilterPlayerShip)
            {
                if (!_healthGiven)
                {
                    SoundEffectInstance sfx = _assMan.SfxHealthUp.CreateInstance();
                    sfx.Volume = GameSettings.GameSettings.SfxVolume;
                    sfx.Play();

                    _healthGiven = true;
                }
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_modelRing, _modelTransRing);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(_boxfes, Matrix.Identity);
        }
    }
}