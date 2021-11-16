using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace FZtarOGL.Message
{
    public class Message
    {
        private float _displayTime;
        private float _time;
        private float waitTime = 0.25f;

        private bool scaleUpX = true;
        private bool scaleUpY;
        private bool scaleDownX;
        private bool scaleDownY;

        private bool remove;

        public bool Remove => remove;

        private Texture2D _avatarFrame;
        private Texture2D _avatar;
        private Texture2D _avatarFrameBg;
        private BitmapFont _font;
        private String _message;
        private bool displayMessage;

        private float avatarScaleX = 1/64f;
        private float avatarScaleY = 1/16f;

        private bool sfxPlayed;
        private SoundEffectInstance _sfx;
        
        public Message(Texture2D avatarFrame, Texture2D avatarFrameBg, Texture2D avatar, SoundEffect sfx, BitmapFont font, String message, float displayTime)
        {
            _avatar = avatar;
            _font = font;
            _message = message;
            _sfx = sfx.CreateInstance();
            _displayTime = displayTime;

            _avatarFrame = avatarFrame;
            _avatarFrameBg = avatarFrameBg;
            _time = 0;
        }

        public void Tick(float dt)
        {
            // scale up avatar
            int speedScaleX = 8;
            int speedScaleY = 6;
            
            if (scaleUpX)
            {
                if (!sfxPlayed)
                {
                    _sfx.Volume = GameSettings.GameSettings.SfxVolume;
                    _sfx.Play();

                    sfxPlayed = true;
                }
                
                avatarScaleX += dt * speedScaleX;
                if (avatarScaleX > 1)
                {
                    avatarScaleX = 1;
                    scaleUpY = true;
                }

                if (scaleUpY)
                {
                    avatarScaleY += dt * speedScaleY;
                    if (avatarScaleY > 1)
                    {
                        avatarScaleY = 1;
                        scaleUpX = false;
                        scaleUpY = false;
                    }
                }
            }

            // display message
            else if (!scaleUpX && !scaleUpY && !scaleDownX && !scaleDownY)
            {
                displayMessage = true;
                
                _time += dt;

                if (_time > _displayTime)
                {
                    scaleDownY = true;
                }
            }
            
            // scale down avatar
            else if (scaleDownY)
            {
                displayMessage = false;
                
                avatarScaleY -= dt * speedScaleY;
                if (avatarScaleY < 1/16f)
                {
                    avatarScaleY = 1/16f;
                    scaleDownX = true;
                }

                if (scaleDownX)
                {
                    avatarScaleX -= dt * speedScaleX;
                    if (avatarScaleX < 1/64f)
                    {
                        avatarScaleX = 1/64f;
                        scaleDownY = false;
                        scaleDownX = false;

                        remove = true;
                    }
                }
            }
            
            //Console.WriteLine("scaleDownX: " + avatarScaleX);
            //Console.WriteLine("scaleDownY: " + avatarScaleY);
        }

        public void Draw(SpriteBatch sb, float dt)
        {
            // avatar
            //Rectangle rectMsg = new Rectangle(0, 16, 32, 48);
            int avatarPosX = 64 + 16;
            int bottomEdge = 224;
            int messageHeight = 64 - 40;
            int messageOffsetY = 4;
            
            sb.Draw(_avatarFrameBg, new Vector2(avatarPosX, bottomEdge - messageHeight - messageOffsetY),
                null, Color.White, 0, new Vector2(_avatar.Width/2f, _avatar.Height/2f + 8), Vector2.One,
                SpriteEffects.None,
                1);
            
            sb.Draw(_avatar, new Vector2(avatarPosX, bottomEdge - messageHeight - messageOffsetY),
                null, Color.White, 0, new Vector2(_avatar.Width/2f, _avatar.Height/2f + 8), new Vector2(avatarScaleX, avatarScaleY),
                SpriteEffects.None,
                0.5f);
            
            sb.Draw(_avatarFrame, new Vector2(avatarPosX, bottomEdge - messageHeight - messageOffsetY),
                null, Color.White, 0, new Vector2(_avatar.Width/2f, _avatar.Height/2f + 8), Vector2.One,
                SpriteEffects.None,
                0);

            // message
            int avatarWidth = 16;
            int offsetX = 2;
            int bottom = 224;
            int textureHeight = 64;
            int offsetY = 12;
            
            if (displayMessage) sb.DrawString(_font, _message, new Vector2(avatarPosX + avatarWidth + offsetX , bottom - textureHeight + offsetY), Color.White);
        }
    }
}