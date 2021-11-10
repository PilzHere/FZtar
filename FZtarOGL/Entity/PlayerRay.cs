using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class PlayerRay : Entity
    {
        private Screen.Screen _screen;

        private PlayerShip _playerShip;

        private Matrix modelTrans;
        private Vector3 modelScale;
        private Vector3 modelPos;
        private Vector3 modelRot;

        private Model _model;
        
        private List<BoundingBoxFiltered> boxfes;
        public BoundingBoxFiltered boxf; // test!
        private const float boxfMinX = 0.5f, boxfMinY = 0.125f, boxfMinZ = 1f;
        private const float boxfMaxX = 0.5f, boxfMaxY = 0.125f, boxfMaxZ = 1f;
        
        private Vector3[] rayColors;
        private Vector3 currentRayColor;

        //private Random rnd = new Random();
        private int rayColorOrdered;
        private float timerRayColor;

        public PlayerRay(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 rotation, PlayerShip ship)
        {
            _screen = screen;
            modelPos = position;
            //modelRot = rotation;
            modelRot = rotation;
            _playerShip = ship;

            _model = assMan.RayModel;

            modelScale = Vector3.One;
            const float rayOffsetFromShip = 1;
            modelPos.Z -= rayOffsetFromShip;
            modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);

            boxfes = new List<BoundingBoxFiltered>();
            
            Vector3 min = new Vector3(modelTrans.Translation.X - boxfMinX, modelTrans.Translation.Y - boxfMinY,modelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
            boxf = new BoundingBoxFiltered(this, min, max, BoxFilters.FilterPlayerRay, BoxFilters.MaskPlayerRay);
            
            boxfes.Add(boxf);

            // shader colors
            rayColors = new Vector3[4];
            rayColors[0] = new Vector3(255/255f, 255/255f, 255/255f);
            rayColors[1] = new Vector3(240/255f, 105/255f, 35/255f);
            rayColors[2] = new Vector3(255/255f, 170/255f, 50/255f);
            rayColors[3] = new Vector3(255/255f, 230/255f, 90/255f);
            currentRayColor = rayColors[0];
        }

        public override void Tick(float dt)
        {
            if (modelPos.Z < _playerShip.ModelPos.Z - 200)
                _ToDestroy = true;

            if (!_screen.CurrentLevel.IsLocatedInSpace)
            {
                if (modelPos.Y < 0)
                    _ToDestroy = true;                
            }

            float speed = 100;
            //modelPos.Z -= speedZ * dt;

            modelPos.X += modelTrans.Forward.X * speed * dt;
            modelPos.Y += modelTrans.Forward.Y * speed * dt;
            modelPos.Z += modelTrans.Forward.Z * speed * dt;

            // trans
            modelTrans = Matrix.CreateScale(modelScale) *
                         Matrix.CreateRotationX(modelRot.X) *
                         Matrix.CreateRotationY(modelRot.Y) *
                         Matrix.CreateRotationZ(modelRot.Z) *
                         Matrix.CreateTranslation(modelPos);

            // bb
            Vector3 min = new Vector3(modelTrans.Translation.X - boxfMinX, modelTrans.Translation.Y - boxfMinY,modelTrans.Translation.Z - boxfMinZ);
            Vector3 max = new Vector3(modelTrans.Translation.X + boxfMaxX, modelTrans.Translation.Y + boxfMaxY,modelTrans.Translation.Z + boxfMaxZ);
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
            
            //Console.WriteLine("timer: " + timerRayColor);
            //Console.WriteLine(currentRayColor);
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                case BoxFilters.FilterTower:
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
            //_screen.DrawBoundingBox(boundingBoxes, modelTrans);
        }
    }
}