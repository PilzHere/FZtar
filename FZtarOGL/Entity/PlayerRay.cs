using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class PlayerRay : Entity
    {
        private Screen.Screen _screen;

        private Matrix _modelTrans;
        private Vector3 _modelScale;
        private Vector3 _modelPos;
        private Vector3 _modelRot;

        private Model _model;
        
        private List<BoundingBoxFiltered> boxfes;
        private BoundingBoxFiltered boxf;
        private const float BoxfMinX = 1f, BoxfMinY = 1f, BoxfMinZ = 1f;
        private const float BoxfMaxX = 1f, BoxfMaxY = 1f, BoxfMaxZ = 1f;
        private Vector3 _min;
        private Vector3 _max;
        
        private Vector3[] _rayColors;
        private Vector3 _currentRayColor;

        //private Random rnd = new Random();
        private int _rayColorOrdered;
        private float _timerRayColor;

        public PlayerRay(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 rotation)
        {
            _screen = screen;
            _modelPos = position;
            //modelRot = rotation;
            _modelRot = rotation;

            _model = assMan.RayModel;

            _modelScale = Vector3.One;
            const float rayOffsetFromShip = 1;
            _modelPos.Z -= rayOffsetFromShip;
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);

            boxfes = new List<BoundingBoxFiltered>();
            
            _min = new Vector3(_modelTrans.Translation.X - BoxfMinX, _modelTrans.Translation.Y - BoxfMinY,_modelTrans.Translation.Z - BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,_modelTrans.Translation.Z + BoxfMaxZ);
            boxf = new BoundingBoxFiltered(this, _min, _max, BoxFilters.FilterPlayerRay, BoxFilters.MaskPlayerRay);
            
            boxfes.Add(boxf);

            // shader colors
            _rayColors = new Vector3[4];
            _rayColors[0] = ModelColors.PlayerRayColor1;
            _rayColors[1] = ModelColors.PlayerRayColor2;
            _rayColors[2] = ModelColors.PlayerRayColor3;
            _rayColors[3] = ModelColors.PlayerRayColor4;
            _currentRayColor = _rayColors[0];
            
            SoundEffectInstance sfx = assMan.SfxRayShot.CreateInstance();
            sfx.Volume = GameSettings.GameSettings.SfxVolume;
            sfx.Play();
        }

        public override void Tick(float dt)
        {
            if (_modelPos.Z < -210 || _modelPos.Z > 10)
                Destroy();

            if (!_screen.CurrentLevel.IsLocatedInSpace)
            {
                if (_modelPos.Y < 0)
                    Destroy();                
            }

            float speed = 100;
            //modelPos.Z -= speedZ * dt;

            _modelPos.X += _modelTrans.Forward.X * speed * dt;
            _modelPos.Y += _modelTrans.Forward.Y * speed * dt;
            _modelPos.Z += _modelTrans.Forward.Z * speed * dt;

            // trans
            _modelTrans = Matrix.CreateScale(_modelScale) *
                         Matrix.CreateRotationX(_modelRot.X) *
                         Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) *
                         Matrix.CreateTranslation(_modelPos);

            // bb
            _min = new Vector3(_modelTrans.Translation.X - BoxfMinX, _modelTrans.Translation.Y - BoxfMinY,_modelTrans.Translation.Z - BoxfMinZ);
            _max = new Vector3(_modelTrans.Translation.X + BoxfMaxX, _modelTrans.Translation.Y + BoxfMaxY,_modelTrans.Translation.Z + BoxfMaxZ);
            boxf.Box = new BoundingBox(_min, _max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
            
            // raycolor
            _timerRayColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = _timerRayColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                _rayColorOrdered++;
                if (_rayColorOrdered > _rayColors.Length - 1) _rayColorOrdered = 0;
                _currentRayColor = _rayColors[_rayColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                _timerRayColor = 0;
            }
            
            //Console.WriteLine("timer: " + timerRayColor);
            //Console.WriteLine(currentRayColor);
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
                case BoxFilters.FilterEnemyTurret:
                    Destroy();
                    break;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModelUnlitWithColor(_model, _modelTrans, _currentRayColor);
        }
        
        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
            //_screen.DrawBoundingBox(boundingBoxes, modelTrans);
        }
    }
}