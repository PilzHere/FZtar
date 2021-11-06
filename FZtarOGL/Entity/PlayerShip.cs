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
    public class PlayerShip : Entity
    {
        public Matrix ModelTrans;
        public Vector3 ModelScale;
        public Vector3 ModelRot;
        public Vector3 ModelPos;

        private Model _model;

        private Texture2D _aimTex;

        private Vector3 _aimPos2d;
        private Vector2 _aimPos2dInt;
        private Vector3 _aimPos3d;

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

        public PlayerShip(Screen.Screen screen, SpriteBatch spriteBatch, AssetManager assMan, Vector3 position,
            PerspectiveCamera cam)
        {
            _screen = screen;
            _spriteBatch = spriteBatch;
            _cam = cam;
            _assMan = assMan;

            _aimTex = _assMan.LoadAsset<Texture2D>("textures/aim");
            _model = _assMan.LoadAsset<Model>("models/playerShip");

            ModelPos = position;
            ModelScale = new Vector3(1, 1, 1);
            ModelTrans = Matrix.CreateScale(ModelScale) *
                         Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);

            _viewport = new Viewport(0, 0, 256, 224);

            // Boundingboxes, one huge from all meshes min/max combined.
            boxfes = new List<BoundingBoxFiltered>();

            /*Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                meshTransform = Matrix.CreateScale(1); // Why do I need this?
                short filter = BoxFilters.filterPlayerShip;
                short mask = BoxFilters.maskPlayerShip;
                boxes.Add(BoundingBoxBuilder.BuildBoundingBoxFiltered(mesh, meshTransform, filter, mask));
            }*/
            
            // test
            Vector3 min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,ModelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,ModelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.filterPlayerShip, BoxFilters.maskPlayerShip);
            
            //BoundingSphere sphere = BoundingSphere.CreateFromPoints(_model.Meshes[0].BoundingSphere);
            //BoundingBox box = BoundingBox.CreateFromSphere(_model.Meshes[0].BoundingSphere);
            //boxf.Box = BoundingBox.CreateFromSphere(_model.Meshes[0].BoundingSphere);
            
            boxfes.Add(boxf);
        }

        private bool keyUpIsPressed;

        public void Input(GameTime gt, float dt)
        {
            if (KeyboardExtended.GetState().IsKeyUp(Keys.Up))
            {
                keyUpIsPressed = false;
            }

            if (!keyUpIsPressed && KeyboardExtended.GetState().IsKeyDown(Keys.Up))
            {
                _screen._Entities.Add(new PlayerRay(_screen, _assMan, ModelPos, ModelRot, this));
                keyUpIsPressed = true;
            }

            float rotXSpeedBoost = 1;
            
            if (KeyboardExtended.GetState().IsKeyDown(Keys.W))
            {
                if (ModelRot.X < 0) ModelRot.X += rotXSpeed * rotXSpeedBoost * dt;
                else ModelRot.X += rotXSpeed * dt;
                
                ModelPos.Y += speedY * dt;
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.S))
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
            }

            if (KeyboardExtended.GetState().IsKeyDown(Keys.D))
            {
                ModelPos.X += speedX * dt;

                if (ModelRot.Y > 0) ModelRot.Y -= rotYSpeed * rotYSpeedBoost * dt;
                else ModelRot.Y -= rotYSpeed * dt;
                //playerShipModelRot.Z -= playerShipRotZSpeed * dt;
            }
        }
        
        public override void Tick(float dt)
        {
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
            ModelTrans = Matrix.CreateRotationX(ModelRot.X) *
                         Matrix.CreateRotationY(ModelRot.Y) *
                         Matrix.CreateRotationZ(ModelRot.Z) *
                         Matrix.CreateTranslation(ModelPos);
            
            // bb
            Vector3 min = new Vector3(ModelTrans.Translation.X - boxfMinX, ModelTrans.Translation.Y - boxfMinY,ModelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(ModelTrans.Translation.X + boxfMaxX, ModelTrans.Translation.Y + boxfMaxY,ModelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
            
            //_screen.BoundingBoxesFilteredLists.Add(boxes);
            
            // aim
            const float camPlayerDist = 6;
            const float fovFar = float.MinValue + camPlayerDist;
            
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
            if (filter == BoxFilters.filterTower)
            {
                Console.WriteLine("PLAYERSHIP HIT");                
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
            
            //_screen.DrawBoundingBoxFiltered(boxes, ModelTrans);

            //Matrix mat = new Matrix();
            //mat = Matrix.CreateTranslation(ModelPos);
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
        }
    }
}