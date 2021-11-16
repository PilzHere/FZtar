using FZtarOGL.Asset;
using FZtarOGL.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Entity
{
    public class StaticShip : Entity
    {
        private Screen.Screen _screen;
        
        private Model _airshipModel;
        private Vector3 _modelRot;
        private Vector3 _modelPos;
        private Matrix _modelTrans;
        
        public StaticShip(Screen.Screen screen, AssetManager assMan, Vector3 position)
        {
            _screen = screen;
            _airshipModel = assMan.PlayerShipModel;
            _modelPos = position;
            
            _modelRot.X = 23f;
            _modelRot.Y = 75f;
            _modelRot.Z = 240f;
            _modelTrans = Matrix.CreateScale(Vector3.One) * Matrix.CreateRotationX(_modelRot.X) * Matrix.CreateRotationY(_modelRot.Y) *
                           Matrix.CreateRotationZ(_modelRot.Z) * Matrix.CreateTranslation(_modelPos);
        }

        public override void Tick(float dt)
        {
            float speedRotX = 0.5f, speedRotY = 0.75f, speedRotZ = 0.25f;
            _modelRot.X += speedRotX * dt;
            _modelRot.Y -= speedRotY * dt;
            _modelRot.Z += speedRotZ * dt;
            
            _modelTrans = Matrix.CreateScale(Vector3.One) * Matrix.CreateRotationX(_modelRot.X) * Matrix.CreateRotationY(_modelRot.Y) *
                         Matrix.CreateRotationZ(_modelRot.Z) * Matrix.CreateTranslation(_modelPos);
        }

        public override void OnCollision(int filter, float dt)
        {
            
        }

        public override void Draw2D(float dt)
        {
            
        }

        public override void Draw3D(float dt)
        {
            _screen.DrawModel(_airshipModel, _modelTrans);
        }

        public override void DrawBoundingBox()
        {
            
        }
    }
}