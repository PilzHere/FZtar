using System;

namespace FZtarOGL.Entity
{
    public abstract class Entity
    {
        protected Guid Id;
        protected bool ToDestroy;

        public Entity()
        {
            Id = Guid.NewGuid();
            
            Console.WriteLine("Entity added: " + Id);
        }

        public abstract void Tick(float dt);
        
        public abstract void OnCollision(int filter, float dt);

        public abstract void Draw2D(float dt);
        
        public abstract void Draw3D(float dt);
        
        public abstract void DrawBoundingBox();

        public virtual void Destroy()
        {
            ToDestroy = true;
        }

        public Guid _Id => Id;

        public bool _ToDestroy
        {
            get => ToDestroy;
            set => ToDestroy = value;
        }
    }
}