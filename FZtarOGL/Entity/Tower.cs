using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace FZtarOGL.Entity
{
    public class Tower : Entity
    {
        public Matrix ModelTrans;
        public Vector3 ModelScale;
        public Vector3 ModelRot;
        public Vector3 ModelPos;

        private Model _model;

        public Model Model => _model;
        public List<BoundingBoxFiltered> boxfes; // public for testing! cange to private

        private Screen.Screen _screen;

        private AssetManager _assMan;
        
        public BoundingBoxFiltered boxf; // test!
        float bbWidth = 4;
        float bbHeight = 10; // origo in bottom.
        float bbLength = 4;
        
        private const float boxfMinX = 1f, boxfMinY = 0f, boxfMinZ = 1f;
        private const float boxfMaxX = 1f, boxfMaxY = 10f, boxfMaxZ = 1f;

        public Tower(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _assMan = assMan;

            _model = _assMan.TowerModel;

            ModelScale = Vector3.One;
            ModelRot = Vector3.Zero;
            ModelPos = position;
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);

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
            
            // test
            float offsetX = bbWidth / 2f;
            float offsetY = bbHeight;
            float offsetZ = bbLength/2f;
            // Remember to switch Z with Y! because wrong in blender!
            Vector3 min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,ModelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,ModelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterTower, BoxFilters.MaskTower);
            
            boxfes.Add(boxf);
        }

        public override void Tick(float dt)
        {
            if (ModelPos.Z > 10)
            {
                //ModelPos.Z = -200;
                _ToDestroy = true;
            }

            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            ModelPos.Z += speedZ * dt; // move forward

            // trans
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);
            
            // test
            float offsetX = bbWidth / 2f;
            float offsetY = bbHeight;
            float offsetZ = bbLength/2f;
            // Remember to switch Z with Y!
            Vector3 min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,ModelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,ModelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);
            
            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterPlayerShip:
                    //Console.WriteLine("TOWER HIT");                    
                    break;
                case BoxFilters.FilterPlayerRay:
                    //ToDestroy = true;
                    break;
            }
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
            //_screen.DrawBoundingBox(boundingBoxes, ModelTrans);
            //_screen.DrawBoundingBox(boxes, ModelTrans);
            
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
        }

        public override void Destroy()
        {
        }
    }
}