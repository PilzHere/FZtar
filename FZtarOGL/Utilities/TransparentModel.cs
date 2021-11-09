using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Utilities
{
    public class TransparentModel
    {
        private Model _model;
        private Matrix _modelTrans;
        private float _distanceFromCamFromCam;
        private bool _lit;

        public bool Lit => _lit;

        public Model Model => _model;
        public Matrix ModelTrans => _modelTrans;

        public float DistanceFromCam
        {
            get => _distanceFromCamFromCam;
            set => _distanceFromCamFromCam = value;
        }

        public TransparentModel(Model model, Matrix modelTrans)
        {
            _model = model;
            _modelTrans = modelTrans;

            _lit = true;
        }
        
        public TransparentModel(Model model, Matrix modelTrans, bool unlit)
        {
            _model = model;
            _modelTrans = modelTrans;

            _lit = !unlit;
        }
    }
}