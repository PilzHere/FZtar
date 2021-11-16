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
    public class Haitch : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelRot;
        private Vector3 _modelPos;

        private Model _model;
        private Vector3 _modelColor;

        private List<BoundingBoxFiltered> boxfes;

        private Screen.Screen _screen;

        private AssetManager _assMan;

        private BoundingBoxFiltered boxf01, boxf02, boxf03;
        
        // Bodies
        // left
        private const float Boxf01MinX = -5f, Boxf01MinY = 0f, Boxf01MinZ = -1.5f;
        private const float Boxf01MaxX = -3f, Boxf01MaxY = 10f, Boxf01MaxZ = 1.5f;

        // right
        private const float Boxf02MinX = 3f, Boxf02MinY = 0f, Boxf02MinZ = -1.5f;
        private const float Boxf02MaxX = 5f, Boxf02MaxY = 10f, Boxf02MaxZ = 1.5f;
        
        // middle
        private const float Boxf03MinX = -3f, Boxf03MinY = 3.5f, Boxf03MinZ = -1.5f;
        private const float Boxf03MaxX = 3f, Boxf03MaxY = 5.5f, Boxf03MaxZ = 1.5f;

        private Vector3 _min01, _max01;
        private Vector3 _min02, _max02;
        private Vector3 _min03, _max03;

        public Haitch(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 modelColor)
        {
            _screen = screen;
            _assMan = assMan;
            _modelColor = modelColor;

            _model = _assMan.Heich01Model;

            _modelScale = Vector3.One;
            _modelRot = Vector3.Zero;
            _modelPos = position;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);
            
            boxfes = new List<BoundingBoxFiltered>();
            
            _min01 = new Vector3(_modelTrans.Translation.X + Boxf01MinX, _modelTrans.Translation.Y + Boxf01MinY,_modelTrans.Translation.Z + Boxf01MinZ);
            _max01 = new Vector3(_modelTrans.Translation.X + Boxf01MaxX, _modelTrans.Translation.Y + Boxf01MaxY,_modelTrans.Translation.Z + Boxf01MaxZ);
            _min02 = new Vector3(_modelTrans.Translation.X + Boxf02MinX, _modelTrans.Translation.Y + Boxf02MinY,_modelTrans.Translation.Z + Boxf02MinZ);
            _max02 = new Vector3(_modelTrans.Translation.X + Boxf02MaxX, _modelTrans.Translation.Y + Boxf02MaxY,_modelTrans.Translation.Z + Boxf02MaxZ);
            _min03 = new Vector3(_modelTrans.Translation.X + Boxf03MinX, _modelTrans.Translation.Y + Boxf03MinY,_modelTrans.Translation.Z + Boxf03MinZ);
            _max03 = new Vector3(_modelTrans.Translation.X + Boxf03MaxX, _modelTrans.Translation.Y + Boxf03MaxY,_modelTrans.Translation.Z + Boxf03MaxZ);
            
            boxf01 = new BoundingBoxFiltered(this, _min01, _max01, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            boxf02 = new BoundingBoxFiltered(this, _min02, _max02, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            boxf03 = new BoundingBoxFiltered(this, _min03, _max03, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            
            boxfes.Add(boxf01);
            boxfes.Add(boxf02);
            boxfes.Add(boxf03);
        }

        public override void Tick(float dt)
        {
            if (_modelPos.Z > 10)
            {
                //ModelPos.Z = -200;
                _ToDestroy = true;
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
            _min02 = new Vector3(_modelTrans.Translation.X + Boxf02MinX, _modelTrans.Translation.Y + Boxf02MinY,_modelTrans.Translation.Z + Boxf02MinZ);
            _max02 = new Vector3(_modelTrans.Translation.X + Boxf02MaxX, _modelTrans.Translation.Y + Boxf02MaxY,_modelTrans.Translation.Z + Boxf02MaxZ);
            _min03 = new Vector3(_modelTrans.Translation.X + Boxf03MinX, _modelTrans.Translation.Y + Boxf03MinY,_modelTrans.Translation.Z + Boxf03MinZ);
            _max03 = new Vector3(_modelTrans.Translation.X + Boxf03MaxX, _modelTrans.Translation.Y + Boxf03MaxY,_modelTrans.Translation.Z + Boxf03MaxZ);
            
            boxf01.Box = new BoundingBox(_min01, _max01);
            boxf02.Box = new BoundingBox(_min02, _max02);
            boxf03.Box = new BoundingBox(_min03, _max03);

            boxfes.Clear();
            boxfes.Add(boxf01);
            boxfes.Add(boxf02);
            boxfes.Add(boxf03);
            
            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterPlayerRay:
                    SoundEffectInstance sfx = _assMan.SfxRayHitObstacle.CreateInstance();
                    sfx.Volume = GameSettings.GameSettings.SfxVolume;
                    sfx.Play();                    
                    break;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModelWithColor(_model, _modelTrans, _modelColor);
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