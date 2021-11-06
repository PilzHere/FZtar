using System;
using FZtarOGL.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace FZtarOGL.Level
{
    public abstract class Level
    {
        protected Screen.Screen Screen;
        protected SpriteBatch SpriteBatch;
        protected AssetManager AssMan;
        
        public float VirtualSpeedZ = 33; // Everything moves with this!
        protected float VirtualSpeedZMax = 33;
        protected float VirtualSpeedBoostZMax = 66;

        protected float VirtualTravelDistance = 0;

        public float VirtualTravelDistance1 => VirtualTravelDistance;

        protected Texture2D BackgroundTexture;
        public Vector2 BackgroundPos;
        protected Vector2 BackgroundPosOld;
        protected Vector2 BackgroundPosInt;
        protected Vector2 BackgroundOrigin;
        protected Vector3 FogColor1;
        protected float FogStart1; // Z units from cam
        protected float FogEnd1; // // Z units from cam

        public Vector3 FogColor => FogColor1;
        public float FogStart => FogStart1;
        public float FogEnd => FogEnd1;

        public Level(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch)
        {
            Screen = screen;
            AssMan = assMan;
            SpriteBatch = spriteBatch;
        }

        public virtual void Tick(GameTime gt, float dt)
        {
            VirtualTravelDistance += VirtualSpeedZ * dt;
        }

        public abstract void DrawBackground(float dt, float rotation);

        public void KeepOldBackgroundPosition()
        {
            BackgroundPos = BackgroundPosOld;
        }
    }
}