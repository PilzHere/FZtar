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

        private Matrix modelTrans;
        private Vector3 modelScale;
        private Vector3 modelPos;
        private Vector3 modelRot;

        private Model _model;
        
        private List<BoundingBoxFiltered> boxfes;
        private BoundingBoxFiltered boxf;
        private const float boxfMinX = 1f, boxfMinY = 1f, boxfMinZ = 1f;
        private const float boxfMaxX = 1f, boxfMaxY = 1f, boxfMaxZ = 1f;
        private Vector3 min;
        private Vector3 max;
        
        private Vector3[] rayColors;
        private Vector3 currentRayColor;

        //private Random rnd = new Random();
        private int rayColorOrdered;
        private float timerRayColor;

        public EnemyRay(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 rotation)
        {
            _screen = screen;
            modelPos = position;
            modelRot = rotation;

            _model = assMan.RayModel;

            modelScale = new Vector3(1.5f, 1.5f, 1.5f);
            const float rayOffsetFromShip = 1;
            modelPos.Z -= rayOffsetFromShip;
            modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);

            boxfes = new List<BoundingBoxFiltered>();
            
            min = new Vector3(modelTrans.Translation.X - boxfMinX, modelTrans.Translation.Y - boxfMinY,modelTrans.Translation.Z - boxfMinZ);
            max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterEnemyRay, BoxFilters.MaskEnemyRay);
            
            boxfes.Add(boxf);

            // shader colors
            rayColors = new Vector3[4];
            rayColors[0] = ModelColors.EnemyRayColor1;
            rayColors[1] = ModelColors.EnemyRayColor2;
            rayColors[2] = ModelColors.EnemyRayColor3;
            rayColors[3] = ModelColors.EnemyRayColor4;
            currentRayColor = rayColors[0];
        }

        public override void Tick(float dt)
        {
            if (modelPos.Z < -210 || modelPos.Z > 10)
                Destroy();

            if (!_screen.CurrentLevel.IsLocatedInSpace)
            {
                if (modelPos.Y < 0)
                    Destroy();                
            }

            float speed = 65;
            modelPos += modelRot * speed * dt; // modelRot = direction from constructor.

            // trans
            /*modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);*/
            
            modelTrans = Matrix.CreateScale(modelScale) *
                          Matrix.CreateWorld(modelPos, modelRot, modelTrans.Up);

            // bb
            min = new Vector3(modelTrans.Translation.X - boxfMinX, modelTrans.Translation.Y - boxfMinY,modelTrans.Translation.Z - boxfMinZ);
            max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
            boxf.Box = new BoundingBox(min, max);

            boxfes.Clear();
            boxfes.Add(boxf);

            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
            
            // raycolor
            timerRayColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = timerRayColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                rayColorOrdered++;
                if (rayColorOrdered > rayColors.Length - 1) rayColorOrdered = 0;
                currentRayColor = rayColors[rayColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                timerRayColor = 0;
            }
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterObstacle:
                    ToDestroy = true;
                    break;
                case BoxFilters.FilterPlayerShip:
                    ToDestroy = true;
                    break;
            }
        }

        public override void Draw2D(float dt)
        {
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModelUnlitWithColor(_model, modelTrans, currentRayColor);
        }
        
        public override void DrawBoundingBox()
        {
            _screen.DrawBoundingBoxFiltered(boxfes, Matrix.Identity);
        }
    }
}