using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class Explosion01 : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelRot;
        private Vector3 _modelPos;

        private Model _model;

        private bool _scaleUp = true;

        private Screen.Screen _screen;

        public Explosion01(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;

            _model = assMan.Explosion01Model;

            _modelScale = new Vector3(0.25f, 0.25f, 0.25f);
            _modelRot = Vector3.Zero;
            _modelPos = position;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateRotationX(_modelRot.X) *
                          Matrix.CreateRotationY(_modelRot.Y) *
                          Matrix.CreateRotationZ(_modelRot.Z) *
                          Matrix.CreateTranslation(_modelPos);
        }

        public override void Tick(float dt)
        {
            // move forward
            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            _modelPos.Z += speedZ * dt;
            
            if (_modelPos.Z > 10) Destroy();

            float scaleSpeed = 2f;
            if (_scaleUp)
            {
                _modelScale.X += scaleSpeed * dt;
                _modelScale.Y += scaleSpeed * dt;
                _modelScale.Z += scaleSpeed * dt;

                if (_modelScale.X >= 1) _scaleUp = false;
            }
            else
            {
                _modelScale.X -= scaleSpeed * dt;
                _modelScale.Y -= scaleSpeed * dt;
                _modelScale.Z -= scaleSpeed * dt;

                if (_modelScale.X <= 0) Destroy();
            }

            // Look at cam.
            _modelTrans = Matrix.CreateBillboard(_modelPos, _screen.Cam3d.Position, _screen.Cam3d.Up,
                _screen.Cam3d.Forward);
            
            _modelTrans = Matrix.CreateScale(_modelScale) * Matrix.CreateRotationZ(-_screen.Cam3d.Up.X) *
                          Matrix.CreateTranslation(_modelPos);

            // Add transparent model(s) to render last.
            _screen.TransparentModels.Add(new TransparentModel(_model, _modelTrans));
        }

        public override void OnCollision(int filter, float dt)
        {
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
        }

        public override void DrawBoundingBox()
        {
        }
    }
}