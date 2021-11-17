using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class EnemyRay : Entity
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

        public EnemyRay(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 rotation)
        {
            _screen = screen;
            _modelPos = position;
            _modelRot = rotation;

            _model = assMan.RayModel;

            _modelScale = new Vector3(1.5f, 1.5f, 1.5f);
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
            boxf = new BoundingBoxFiltered(this, _min, _max, BoxFilters.FilterEnemyRay, BoxFilters.MaskEnemyRay);
            
            boxfes.Add(boxf);

            // shader colors
            _rayColors = new Vector3[4];
            _rayColors[0] = ModelColors.EnemyRayColor1;
            _rayColors[1] = ModelColors.EnemyRayColor2;
            _rayColors[2] = ModelColors.EnemyRayColor3;
            _rayColors[3] = ModelColors.EnemyRayColor4;
            _currentRayColor = _rayColors[0];
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

            float speed = 65;
            _modelPos += _modelRot * speed * dt; // modelRot = direction from constructor.

            // trans
            /*modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);*/
            
            _modelTrans = Matrix.CreateScale(_modelScale) *
                          Matrix.CreateWorld(_modelPos, _modelRot, _modelTrans.Up);

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
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterObstacle:
                    Destroy();
                    break;
                case BoxFilters.FilterPlayerShip:
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
        }
    }
}