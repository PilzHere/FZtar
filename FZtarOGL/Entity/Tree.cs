using System;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class Tree : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelRot;
        private Vector3 _modelPos;

        private Model _model;
        //private Vector3 _modelColor;

        private List<BoundingBoxFiltered> boxfes;

        private Screen.Screen _screen;

        private AssetManager _assMan;

        private BoundingBoxFiltered boxf01;
        
        // Bodies
        private const float Boxf01MinX = -1f, Boxf01MinY = 0, Boxf01MinZ = -1f;
        private const float Boxf01MaxX = 1f, Boxf01MaxY = 2.84f, Boxf01MaxZ = 1f;

        private Vector3 _min01, _max01;

        public Tree(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _assMan = assMan;
            //_modelColor = modelColor;

            _model = _assMan.Tree01Model;

            _modelScale = Vector3.One;

            var rand = new Random();
            int maxRotY = 360;
            
            _modelRot = new Vector3(0,MathHelper.ToRadians(rand.Next(0, maxRotY)),0);
            _modelPos = position;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);
            
            boxfes = new List<BoundingBoxFiltered>();
            
            _min01 = new Vector3(_modelTrans.Translation.X + Boxf01MinX, _modelTrans.Translation.Y + Boxf01MinY,_modelTrans.Translation.Z + Boxf01MinZ);
            _max01 = new Vector3(_modelTrans.Translation.X + Boxf01MaxX, _modelTrans.Translation.Y + Boxf01MaxY,_modelTrans.Translation.Z + Boxf01MaxZ);
            
            boxf01 = new BoundingBoxFiltered(this, _min01, _max01, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            
            boxfes.Add(boxf01);
        }

        public override void Tick(float dt)
        {
            if (_modelPos.Z > 10)
            {
                Destroy();
            }

            float speedZ = _screen.CurrentLevel.VirtualSpeedZ;
            _modelPos.Z += speedZ * dt; // move forward

            // trans
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);
            
            _min01 = new Vector3(_modelTrans.Translation.X + Boxf01MinX, _modelTrans.Translation.Y + Boxf01MinY,_modelTrans.Translation.Z + Boxf01MinZ);
            _max01 = new Vector3(_modelTrans.Translation.X + Boxf01MaxX, _modelTrans.Translation.Y + Boxf01MaxY,_modelTrans.Translation.Z + Boxf01MaxZ);
            
            boxf01.Box = new BoundingBox(_min01, _max01);

            boxfes.Clear();
            boxfes.Add(boxf01);
            
            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
        }

        public override void OnCollision(int filter, float dt)
        {
            /*switch (filter)
            {
                case BoxFilters.FilterPlayerRay:
                    SoundEffectInstance sfx = _assMan.SfxRayHitObstacle.CreateInstance();
                    sfx.Volume = GameSettings.GameSettings.SfxVolume;
                    sfx.Play();                    
                    break;
            }*/
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_model, _modelTrans);
            //_screen.DrawModelWithColor(_model, _modelTrans, _modelColor);
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