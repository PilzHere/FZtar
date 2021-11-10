using System;
using System.Collections;
using System.Collections.Generic;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;

namespace FZtarOGL.Level
{
    public abstract class Level
    {
        protected Screen.Screen Screen;
        protected SpriteBatch SpriteBatch;
        protected AssetManager AssMan;

        protected Song levelSong;
        protected bool isLocatedInSpace;

        public bool IsLocatedInSpace => isLocatedInSpace;

        private float VirtualSpeedZStd = 33;
        public float VirtualSpeedZ = 33; // Everything moves with this!
        protected float VirtualSpeedZMax = 33;
        protected float VirtualSpeedBoostZMax = 66;

        protected float VirtualTravelDistance;
        protected float CurrentTravelSpeed;

        public float CurrentTravelSpeed1 => CurrentTravelSpeed;

        public float VirtualTravelDistance1 => VirtualTravelDistance;

        protected Texture2D BackgroundTexture;
        public Vector2 BackgroundPos;
        public Vector2 BackgroundPosOld;
        protected Vector2 BackgroundPosInt;
        protected Vector2 BackgroundOrigin;
        protected Vector3 FogColor1;
        protected float FogStart1; // Z units from cam
        protected float FogEnd1; // Z units from cam

        protected bool displayMessage;
        protected Texture2D messageAvatar;
        protected float avatarScaleX;
        protected float avatarScaleY;
        protected String message;
        protected float messageCooldown = 0.5f;
        protected float messageTimer;

        protected List<Message.Message> Messages1;

        protected bool MoveEverythingForward;

        public bool MoveEverythingForward1
        {
            get => MoveEverythingForward;
            set => MoveEverythingForward = value;
        }

        public List<Message.Message> Messages => Messages1;

        public Vector3 FogColor => FogColor1;
        public float FogStart => FogStart1;
        public float FogEnd => FogEnd1;

        public Level(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch)
        {
            Screen = screen;
            AssMan = assMan;
            SpriteBatch = spriteBatch;

            Messages1 = new List<Message.Message>();

            MoveEverythingForward = true;
        }

        public virtual void Tick(GameTime gt, float dt)
        {
            if (MoveEverythingForward) VirtualSpeedZ = VirtualSpeedZStd;
            else VirtualSpeedZ = 0;
            
            VirtualTravelDistance += VirtualSpeedZ * dt;

            if (Messages.Count > 0)
            {
                messageTimer += dt;

                if (messageTimer >= messageCooldown)
                {
                    if (Messages1[0].Remove)
                    {
                        Messages1.RemoveAt(0);
                        messageTimer = 0;
                    }
                    else
                    {
                        Messages1[0].Tick(dt);
                    }
                }
            }

            //Console.WriteLine(messageTimer);
        }

        public void DrawMessage(SpriteBatch sb, float dt)
        {
            if (Messages.Count > 0)
            {
                if (messageTimer >= messageCooldown)
                {
                    Messages1[0].Draw(sb, dt);
                }
            }
        }

        public abstract void DrawGroundEffect(PerspectiveCamera cam, float dt);

        public abstract void DrawFloor();

        public abstract void DrawBackground(float dt, float rotation);

        public void KeepOldBackgroundPosition()
        {
            BackgroundPos = BackgroundPosOld;
        }

        public void KeepOldBackgroundPositionX()
        {
            BackgroundPos.X = BackgroundPosOld.X;
        }

        public void KeepOldBackgroundPositionY()
        {
            BackgroundPos.Y = BackgroundPosOld.Y;
        }
    }
}