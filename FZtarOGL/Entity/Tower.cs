using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class Tower : Entity
    {
        private Matrix ModelTrans;
        private Vector3 ModelScale;
        private Vector3 ModelRot;
        private Vector3 ModelPos;

        private Model _model;
        private Vector3 _modelColor;
        private ModelMesh _towerSignalMesh01, _towerSignalMesh02, _towerSignalMesh03, _towerSignalMesh04;

        private List<BoundingBoxFiltered> boxfes;

        private Screen.Screen _screen;

        private AssetManager _assMan;
        
        public BoundingBoxFiltered boxf01, boxf02, boxf03;
        
        // Bodies
        // tower
        private const float boxf01MinX = 1.5f, boxf01MinY = 0f, boxf01MinZ = 1.5f;
        private const float boxf01MaxX = 1.5f, boxf01MaxY = 15f, boxf01MaxZ = 1.5f;
        //base bottom
        private const float boxf02MinX = 3f, boxf02MinY = 0f, boxf02MinZ = 3f;
        private const float boxf02MaxX = 3f, boxf02MaxY = 1f, boxf02MaxZ = 3f;
        //base top
        private const float boxf03MinX = 2f, boxf03MinY = -1f, boxf03MinZ = -2f;
        private const float boxf03MaxX = 2f, boxf03MaxY = 2f, boxf03MaxZ = -2f;

        private Vector3 min01;
        private Vector3 max01;
        private Vector3 min02;
        private Vector3 max02;
        private Vector3 min03;
        private Vector3 max03;
        
        private Vector3[] towerSignalColors;
        private Vector3 currenttowerSignalColor;

        //private Random rnd = new Random();
        private int towerSignalColorOrdered;
        private float timerTowerSignalColor;

        private bool _useSignals;

        public Tower(Screen.Screen screen, AssetManager assMan, Vector3 position, Vector3 modelColor, bool useSignals)
        {
            _screen = screen;
            _assMan = assMan;
            _modelColor = modelColor;
            _useSignals = useSignals;

            _model = _assMan.TowerModel;
            _towerSignalMesh01 = _model.Meshes[1];
            _towerSignalMesh02 = _model.Meshes[2];
            _towerSignalMesh03 = _model.Meshes[3];
            _towerSignalMesh04 = _model.Meshes[4];

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
            
            min01 = new Vector3(ModelTrans.Translation.X - boxf01MinX, ModelTrans.Translation.Y - boxf01MinY,ModelTrans.Translation.Z - boxf01MinZ);
            max01 = new Vector3(ModelTrans.Translation.X + boxf01MaxX, ModelTrans.Translation.Y + boxf01MaxY,ModelTrans.Translation.Z + boxf01MaxZ);
            min02 = new Vector3(ModelTrans.Translation.X - boxf02MinX, ModelTrans.Translation.Y - boxf02MinY,ModelTrans.Translation.Z - boxf02MinZ);
            max02 = new Vector3(ModelTrans.Translation.X + boxf02MaxX, ModelTrans.Translation.Y + boxf02MaxY,ModelTrans.Translation.Z + boxf02MaxZ);
            min03 = new Vector3(ModelTrans.Translation.X - boxf03MinX, ModelTrans.Translation.Y - boxf03MinY,ModelTrans.Translation.Z - boxf03MinZ);
            max03 = new Vector3(ModelTrans.Translation.X + boxf03MaxX, ModelTrans.Translation.Y + boxf03MaxY,ModelTrans.Translation.Z + boxf03MaxZ);
            
            boxf01 = new BoundingBoxFiltered(this, min01, max01, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            boxf02 = new BoundingBoxFiltered(this, min02, max02, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            boxf03 = new BoundingBoxFiltered(this, min03, max03, BoxFilters.FilterObstacle, BoxFilters.MaskObstacle);
            
            boxfes.Add(boxf01);
            boxfes.Add(boxf02);
            boxfes.Add(boxf03);
            
            // shader colors
            towerSignalColors = new Vector3[6];
            towerSignalColors[0] = ModelColors.Red2;
            towerSignalColors[1] = ModelColors.Red1;
            towerSignalColors[2] = ModelColors.Orange;
            towerSignalColors[3] = ModelColors.Yellow;
            towerSignalColors[4] = ModelColors.Orange;
            towerSignalColors[5] = ModelColors.Red1;
            currenttowerSignalColor = towerSignalColors[0];
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
            
            min01 = new Vector3(ModelTrans.Translation.X - boxf01MinX, ModelTrans.Translation.Y - boxf01MinY,ModelTrans.Translation.Z - boxf01MinZ);
            max01 = new Vector3(ModelTrans.Translation.X + boxf01MaxX, ModelTrans.Translation.Y + boxf01MaxY,ModelTrans.Translation.Z + boxf01MaxZ);
            min02 = new Vector3(ModelTrans.Translation.X - boxf02MinX, ModelTrans.Translation.Y - boxf02MinY,ModelTrans.Translation.Z - boxf02MinZ);
            max02 = new Vector3(ModelTrans.Translation.X + boxf02MaxX, ModelTrans.Translation.Y + boxf02MaxY,ModelTrans.Translation.Z + boxf02MaxZ);
            min03 = new Vector3(ModelTrans.Translation.X - boxf03MinX, ModelTrans.Translation.Y - boxf03MinY,ModelTrans.Translation.Z - boxf03MinZ);
            max03 = new Vector3(ModelTrans.Translation.X + boxf03MaxX, ModelTrans.Translation.Y + boxf03MaxY,ModelTrans.Translation.Z + boxf03MaxZ);
            
            boxf01.Box = new BoundingBox(min01, max01);
            boxf02.Box = new BoundingBox(min02, max02);
            boxf03.Box = new BoundingBox(min03, max03);

            boxfes.Clear();
            boxfes.Add(boxf01);
            boxfes.Add(boxf02);
            boxfes.Add(boxf03);
            
            foreach (var boxf in boxfes)
            {
                _screen.BoundingBoxesFiltered.Add(boxf);
            }
            
            // thrustercolor
            timerTowerSignalColor += dt;
            const float timeRayColor = 0.025f;
            bool changeColor = timerTowerSignalColor >= timeRayColor;
            if (changeColor)
            {
                // ordered
                towerSignalColorOrdered++;
                if (towerSignalColorOrdered > towerSignalColors.Length - 1) towerSignalColorOrdered = 0;
                currenttowerSignalColor = towerSignalColors[towerSignalColorOrdered];

                // random
                //int rayColor = rnd.Next(0, rayColors.Length);
                //currentRayColor = rayColors[rayColor];

                timerTowerSignalColor = 0;
            }
        }

        public override void OnCollision(int filter, float dt)
        {
            switch (filter)
            {
                //case BoxFilters.FilterPlayerShip:
                    //Console.WriteLine("TOWER HIT");                    
                    //break;
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
            _screen.DrawModelWithColor(_model, ModelTrans, _modelColor);

            if (_useSignals)
            {
                _screen.DrawModelMeshUnlitWithColor(_towerSignalMesh01, ModelTrans, currenttowerSignalColor);
                _screen.DrawModelMeshUnlitWithColor(_towerSignalMesh02, ModelTrans, currenttowerSignalColor);
                _screen.DrawModelMeshUnlitWithColor(_towerSignalMesh03, ModelTrans, currenttowerSignalColor);
                _screen.DrawModelMeshUnlitWithColor(_towerSignalMesh04, ModelTrans, currenttowerSignalColor);
            }
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