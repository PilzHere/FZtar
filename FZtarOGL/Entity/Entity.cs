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

        public abstract void Draw(float dt);

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