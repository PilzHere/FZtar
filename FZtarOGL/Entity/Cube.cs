using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class Cube : Entity
    {
        public Matrix ModelTrans;
        public Vector3 ModelScale;
        public Vector3 ModelRot;
        public Vector3 ModelPos;

        private Model _model;

        public Model Model => _model;
        private List<BoundingBox> boundingBoxes;

        private Screen.Screen _screen;

        private AssetManager _assMan;

        public Cube(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _assMan = assMan;

            _model = _assMan.CubeModel;

            ModelScale = Vector3.One;
            ModelRot = Vector3.Zero;
            ModelPos = position;
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);

            // Boundingboxes, one huge from all meshes min/max combined.
            boundingBoxes = new List<BoundingBox>();

            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                meshTransform = Matrix.CreateScale(1); // Why do I need this?
                boundingBoxes.Add(BoundingBoxBuilder.BuildBoundingBox(mesh, meshTransform));
            }
        }

        public override void Tick(float dt)
        {
            if (ModelPos.Z > 10)
            {
                ModelPos.Z = -200;
            }

            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            speedZ += 33; // faster than normal
            ModelPos.Z += speedZ * dt; // move forward

            ModelRot.X += 2.5f * dt;
            
            // trans
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);
        }

        public override void OnCollision(int filter, float dt)
        {
            
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_model, ModelTrans);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBox(boundingBoxes, ModelTrans);
        }

        public override void Destroy()
        {
        }
    }
}