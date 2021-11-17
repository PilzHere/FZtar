using Microsoft.Xna.Framework;

namespace FZtarOGL.Box
{
    public class BoundingBoxFiltered
    {
        private BoundingBox _box;
        private int _filter;
        private int _mask;
        private Entity.Entity _parent;

        public BoundingBox Box
        {
            get { return _box; }
            set { _box = value; }
        }

        public Entity.Entity Parent => _parent;

        public int Filter
        {
            get => _filter;
            set => _filter = value;
        }

        public int Mask
        {
            get => _mask;
            set => _mask = value;
        }

        public BoundingBoxFiltered(Entity.Entity parent, Vector3 min, Vector3 max, int filter, int mask)
        {
            _box = new BoundingBox(min, max);
            _filter = filter;
            _mask = mask;
            _parent = parent;
        }

        public BoundingBoxFiltered(Entity.Entity parent, BoundingBox box, int filter, int mask)
        {
            _box = box;
            _filter = filter;
            _mask = mask;
            _parent = parent;
        }

        public BoundingBoxFiltered(Entity.Entity parent, BoundingBox box)
        {
            _box = box;
            _parent = parent;
        }

        public BoundingBoxFiltered(Entity.Entity parent, Vector3 min, Vector3 max)
        {
            _box = new BoundingBox(min, max);
            _parent = parent;
        }
    }
}