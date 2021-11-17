using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class House : Entity
    {
        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelRot;
        private Vector3 _modelPos;

        private Model _model;
        private Vector3 _modelColor;

        private List<BoundingBoxFiltered> _boxfes;

        private Screen.Screen _screen;

        private AssetManager _assMan;

        private BoundingBoxFiltered _boxf01;
        
        // Bodies
        private const float Boxf01MinX = -5f, Boxf01MinY = 0, Boxf01MinZ = -5f;
        private const float Boxf01MaxX = 5f, Boxf01MaxY = 5f, Boxf01MaxZ = 5f;

        private Vector3 _min01, _max01;

        public House(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 modelColor)
        {
            _screen = screen;
            _assMan = assMan;
            _modelColor = modelColor;

            _model = _assMan.House01Model;

            _modelScale = Vector3.One;

            _modelRot = Vector3.Zero;
            _modelPos = position;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);
            
            _boxfes = new List<BoundingBoxFiltered>();
            
            _min01 = new Vector3(_modelTrans.Translation.X + Boxf01MinX, _modelTrans.Translation.Y + Boxf01MinY,_modelTrans.Translation.Z + Boxf01MinZ);
            _max01 = new Vector3(_modelTrans.Translation.X + Boxf01MaxX, _modelTrans.Translation.Y + Boxf01MaxY,_modelTrans.Translation.Z + Boxf01MaxZ);
            
            _boxf01 = new BoundingBoxFiltered(this, _min01, _max01, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            
            _boxfes.Add(_boxf01);
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
            
            _boxf01.Box = new BoundingBox(_min01, _max01);

            _boxfes.Clear();
            _boxfes.Add(_boxf01);
            
            foreach (var boxf in _boxfes)
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
            //_screen.DrawModel(_model, _modelTrans);
            _screen.DrawModelWithColor(_model, _modelTrans, _modelColor);
        }

        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(_boxfes, Matrix.Identity);
        }
    }
}