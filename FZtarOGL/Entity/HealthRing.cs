using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class HealthRing : Entity
    {
        private Matrix ModelTransRing;
        private Vector3 ModelScaleRing;
        private Vector3 ModelRotRing;
        private Vector3 ModelPosRing;

        private Model _modelRing;

        private Matrix ModelTransBillboard;
        private Vector3 ModelScaleBillboard;
        private Vector3 ModelRotBillboard;
        private Vector3 ModelPosBillboard;

        private Model _modelBillboard;

        private Model ModelRing => _modelRing;
        private List<BoundingBoxFiltered> boxfes; // public for testing! cange to private

        private Screen.Screen _screen;

        private AssetManager _assMan;

        private BoundingBoxFiltered boxf;

        private const float boxfMinX = 0.75f, boxfMinY = 0.75f, boxfMinZ = 0.125f;
        private const float boxfMaxX = 0.75f, boxfMaxY = 0.75f, boxfMaxZ = 0.125f;

        private bool healthGiven;

        public HealthRing(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _assMan = assMan;

            _modelRing = _assMan.Ring01Model;
            _modelBillboard = _assMan.BillboardHealth16Model;

            ModelScaleRing = Vector3.One;
            ModelRotRing = Vector3.Zero;
            ModelPosRing = position;
            ModelTransRing = Matrix.CreateScale(ModelScaleRing) *
                             Matrix.CreateRotationX(ModelRotRing.X) *
                             Matrix.CreateRotationY(ModelRotRing.Y) *
                             Matrix.CreateRotationZ(ModelRotRing.Z) *
                             Matrix.CreateTranslation(ModelPosRing);

            // Boundingboxes, one huge from all meshes min/max combined.
            boxfes = new List<BoundingBoxFiltered>();

            /*Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                meshTransform = Matrix.CreateScale(1); // Why do I need this?
                boundingBoxes.Add(BoundingBoxBuilder.BuildBoundingBox(mesh, meshTransform));
            }*/


            Vector3 min = new Vector3(ModelTransRing.Translation.X - boxfMinX, ModelTransRing.Translation.Y - boxfMinY,
                ModelTransRing.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTransRing.Translation.X + boxfMaxX, ModelTransRing.Translation.Y + boxfMaxY,
                ModelTransRing.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterRingHealth, BoxFilters.MaskRingHealth);

            boxfes.Add(boxf);

            // billboard
            ModelScaleBillboard = Vector3.One;
            ModelRotBillboard = Vector3.Zero;
            ModelPosBillboard = position;
            ModelTransBillboard = Matrix.CreateScale(ModelScaleBillboard) *
                                  Matrix.CreateRotationX(ModelRotBillboard.X) *
                                  Matrix.CreateRotationY(ModelRotBillboard.Y) *
                                  Matrix.CreateRotationZ(ModelRotBillboard.Z) *
                                  Matrix.CreateTranslation(ModelPosBillboard);
        }

        public override void Tick(float dt)
        {
            if (ModelPosRing.Z > 10 || ModelPosBillboard.Z > 10)
            {
                //ModelPos.Z = -200;
                _ToDestroy = true;
            }

            // move forward
            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            ModelPosRing.Z += speedZ * dt;

            // rotate
            float rotSpeedZ = 2.5f;
            ModelRotRing.Z += rotSpeedZ * dt;

            // trans
            ModelTransRing = Matrix.CreateScale(ModelScaleRing) *
                             Matrix.CreateRotationX(ModelRotRing.X) *
                             Matrix.CreateRotationY(ModelRotRing.Y) *
                             Matrix.CreateRotationZ(ModelRotRing.Z) *
                             Matrix.CreateTranslation(ModelPosRing);

            // BILLBOARD
            // move forward
            ModelPosBillboard.Z += speedZ * dt;

            // trans
            // Look at cam.
            ModelTransBillboard = Matrix.CreateBillboard(ModelPosBillboard, _screen.Cam3d.Position, _screen.Cam3d.Up,
                _screen.Cam3d.Forward);

            // Rotate (stay steady even if camera rotates up/down) in Z axis.
            ModelTransBillboard = Matrix.CreateRotationZ(-_screen.Cam3d.Up.X) *
                                  Matrix.CreateTranslation(ModelPosBillboard);

            // box
            Vector3 min = new Vector3(ModelTransRing.Translation.X - boxfMinX, ModelTransRing.Translation.Y - boxfMinY,
                ModelTransRing.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTransRing.Translation.X + boxfMaxX, ModelTransRing.Translation.Y + boxfMaxY,
                ModelTransRing.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            if (healthGiven)
            {
                boxf.Filter = BoxFilters.FilterNothing;
            }

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }

            // Add transparent model(s) to render last.
            if (!healthGiven) _screen.TransparentModels.Add(new TransparentModel(_modelBillboard, ModelTransBillboard));
        }

        public override void OnCollision(int filter, float dt)
        {
            if (filter == BoxFilters.FilterPlayerShip)
            {
                if (!healthGiven) healthGiven = true;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_modelRing, ModelTransRing);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
        }

        public override void Destroy()
        {
        }
    }
}