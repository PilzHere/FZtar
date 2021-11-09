using FZtarOGL.Asset;
using FZtarOGL.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FZtarOGL.Level
{
    public class Level01 : Level
    {
        private int _segmentDefaultLength = 200;
        private int _segmentNextLength = 200;
        private int _currentSegment;
        private bool _entitiesSpawned;

        public Level01(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch) : base(screen, assMan,
            spriteBatch)
        {
            BackgroundTexture = AssMan.Bg02;
            FogColor1 = new Vector3(112 / 255f, 160 / 255f, 160 / 255f); // bg color horizon
            FogStart1 = 150;
            FogEnd1 = 200;

            const float backgroundPosOffsetX = 128;
            const float backgroundPosOffsetY = 11 - 2; // was 11
            BackgroundPos = new Vector2(backgroundPosOffsetX, backgroundPosOffsetY);
            BackgroundPosInt = new Vector2((int)BackgroundPos.X, (int)BackgroundPos.Y);
            BackgroundOrigin = new Vector2(256, 256);

            levelSong = assMan.songAction;
            MediaPlayer.Play(levelSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;
        }

        public override void Tick(GameTime gt, float dt)
        {
            base.Tick(gt, dt);

            // check distance traveled
            if (VirtualTravelDistance > _segmentNextLength)
            {
                _currentSegment++;
                _entitiesSpawned = false;
                _segmentNextLength += _segmentDefaultLength;
            }

            // spawn entities
            if (!_entitiesSpawned)
            {
                switch (_currentSegment)
                {
                    case 0:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -200)));

                        break;
                    case 1:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200)));

                        // pickus
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -240)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -280)));

                        // messages
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.Font02_08,
                            "Hej alla!\nDr Inet har...\nHur gar det for\ner?", 3));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet02Tex, AssMan.Font02_08,
                            "Mindre an 10\ndagar kvar bara.\nMen kann ingen\nstress...\nMan ska ha roligt\npa ett jam!",
                            5));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.Font02_08,
                            "Lycka till i jamet!\nGO JAVA BOYS!", 3));

                        break;
                    case 2:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250)));

                        // pickups
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-3, 3, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 3, -240)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(3, 3, -280)));

                        break;
                    case 3:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-0, 0, -250)));

                        break;
                    case 4:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(7, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-7, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(3, 0, -250)));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-3, 0, -250)));

                        break;
                }
            }

            _entitiesSpawned = true;

            // background
            BackgroundPosInt.X = (int)BackgroundPos.X;
            BackgroundPosInt.Y = (int)BackgroundPos.Y;

            BackgroundPosOld = BackgroundPos;
        }

        public override void DrawBackground(float dt, float rotation)
        {
            SpriteBatch.Draw(BackgroundTexture, BackgroundPosInt, null, Color.White, rotation, BackgroundOrigin,
                Vector2.One,
                SpriteEffects.None, 0);
        }
    }
}