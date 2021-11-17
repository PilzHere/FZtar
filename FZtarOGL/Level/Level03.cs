using System;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace FZtarOGL.Level
{
    public class Level03 : Level
    {
        private int _segmentDefaultLength = 200;
        private int _segmentNextLength = 200;
        private int _currentSegment;
        private bool _entitiesSpawned;

        private BasicEffect basicEffectPrimitives;

        private Random rand = new Random();

        private float lightSpeedSpeed;

        public Level03(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch) : base(screen, assMan,
            spriteBatch)
        {
            isLocatedInSpace = true;

            BackgroundTexture = AssMan.Bg04;
            FogColor1 = new Vector3(0 / 255f, 5 / 255f, 6 / 255f); // bg color horizon
            FogStart1 = 150;
            FogEnd1 = 200;

            const float backgroundPosOffsetX = 128;
            const float backgroundPosOffsetY = 11 - 2; // was 11
            BackgroundPos = new Vector2(backgroundPosOffsetX, backgroundPosOffsetY);
            BackgroundPosInt = new Vector2((int)BackgroundPos.X, (int)BackgroundPos.Y);
            BackgroundOrigin = new Vector2(256, 256);

            BackgroundMusic = assMan.SongLevel3.CreateInstance();
            BackgroundMusic.IsLooped = true;
            BackgroundMusic.Volume = GameSettings.GameSettings.MusicVolume;
            BackgroundMusic.Play();

            basicEffectPrimitives = new BasicEffect(screen.GraphicsDevice);

            for (var z = 0; z < spaceDots3D.GetLength(0); z++) // 0-5
            {
                for (var y = 0; y < spaceDots3D.GetLength(1); y++) // 0-6
                {
                    for (var x = 0; x < spaceDots3D.GetLength(2); x++) // 0-6
                    {
                        var dot = spaceDots3D[z, x, y];
                        dot.X = rand.Next(-5, 6);
                        dot.Y = rand.Next(-5, 6);
                        dot.Z = rand.Next(-10, 1);
                    }
                }
            }

            LevelVirtualSpeedZ *= 1.33f;
            
            screen.SetContinueLevel(ContinueLevel.Level03);
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

            for (var z = 0; z < spaceDots3D.GetLength(0); z++) // 0-5
            {
                for (var y = 0; y < spaceDots3D.GetLength(1); y++) // 0-6
                {
                    for (var x = 0; x < spaceDots3D.GetLength(2); x++) // 0-6
                    {
                        var dot = spaceDots3D[z, y, x];
                        if (!levelIsFinished) spaceDots3D[z, y, x].Z += VirtualSpeedZ * dt;
                        else
                        {
                            //if (lightSpeedSpeed < 100) lightSpeedSpeed += 5 * dt;
                            if (_spaceDotsMiniOffset.Z < 400) _spaceDotsMiniOffset.Z += 0.5f * dt;
                        }

                        if (dot.Z >= 1)
                        {
                            spaceDots3D[z, y, x] =
                                new Vector3(rand.Next(-20, 21), rand.Next(-20, 21), rand.Next(-200, -9));
                        }
                    }
                }
            }

            // spawn entities
            if (!_entitiesSpawned)
            {
                switch (_currentSegment)
                {
                    case 0:
                        // TEST HERE
                        
                        
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "In space...\nNo one can hear\nyou scream.", 3));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Cold, dark...\nAll alone.\nI kinda like it!", 2));
                        // obstacles

                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(8, 5, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-8, 5, -300)));

                        break;
                    case 1:
                    {
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Woah!\nLet's watch out\nfor those\nmeteoroids!", 3));

                        // obstacles
                        var posZ = -198;
                        var amount = 200;
                        for (var i = 0; i < amount; i++)
                        {
                            posZ -= 1;
                            var posX = rand.Next(-5, 6);
                            var posY = rand.Next(-3, 7);
                            var randomDirX = rand.Next(11);
                            var randomDirY = rand.Next(11);

                            float dirX;
                            float dirY;
                            if (randomDirX % 2 == 0) dirX = rand.NextSingle();
                            else dirX = -rand.NextSingle();
                            if (randomDirY % 2 == 0) dirY = rand.NextSingle();
                            else dirY = -rand.NextSingle();

                            if (i % 5 == 0)
                                Screen._Entities.Add(new SpaceJunk02(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                            else
                                Screen._Entities.Add(new SpaceJunk01(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                        }

                        // pickus
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 7, -400)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 5, -450)));
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 3, -500)));
                    }
                        break;

                    case 2:

                        // obstacles


                        // pickups


                        break;
                    case 3:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Kamikaze miners!\nLet's show them\nwho's boss.", 3));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-10, 0, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 15, -230), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(3, -3, -260), true));

                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-10, 15, -290), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(3, -5, -320), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-7, 10, -350), true));

                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-7, 5, -380), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-3, 10, -410), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(5, -5, -440), true));

                        // obstacles


                        break;
                    case 4:

                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Oh no!\nMore meteoroids!", 3));
                        
                        // obstacles
                    {
                        var posZ = -198;
                        var amount = 200;
                        for (var i = 0; i < amount; i++)
                        {
                            posZ -= 1;
                            var posX = rand.Next(-5, 6);
                            var posY = rand.Next(-3, 7);
                            var randomDirX = rand.Next(11);
                            var randomDirY = rand.Next(11);

                            float dirX;
                            float dirY;
                            if (randomDirX % 2 == 0) dirX = rand.NextSingle();
                            else dirX = -rand.NextSingle();
                            if (randomDirY % 2 == 0) dirY = rand.NextSingle();
                            else dirY = -rand.NextSingle();

                            if (i % 5 == 0)
                                Screen._Entities.Add(new SpaceJunk02(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                            else
                                Screen._Entities.Add(new SpaceJunk01(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                        }
                    }

                        break;

                    case 5:
                        
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-8, 5, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(8, 5, -300)));

                        break;
                    case 6:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "An armada of\nkamikaze miners!\nLet's end this\nNOW!", 3));
                        
                    {
                        float posZ = -200-33;
                        for (int i = 0; i < 33; i++)
                        {
                            posZ -= 50;
                            int posX = rand.Next(-20, 21);
                            int posY = rand.Next(-10, 21);
                            Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(posX, posY, posZ), true));
                        }
                    }

                        break;
                    
                    case 7:
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "7", 3));*/
                        
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(5, 7, -230)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 4, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-5, 0, -260)));
                        
                        break;
                    
                    case 8:
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "8", 3));*/
                        
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(5, 7, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 4, -275)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-5, 0, -350)));
                        
                        break;
                    
                    case 9:
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "9", 3));*/
                        
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(5, 7, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 4, -270)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-5, 0, -340)));
                        
                        break;

                    case 10:

                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "10", 3));*/
                        
                        break;

                    case 11:
                        
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "11", 3));*/
                        
                        break;
                    
                    case 12:
                        
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "12", 3));*/
                        
                        break;
                    
                    case 13:
                        
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Not these rocks\nagain!?", 3));
                    {
                        var posZ = -198;
                        var amount = 200;
                        for (var i = 0; i < amount; i++)
                        {
                            posZ -= 1;
                            var posX = rand.Next(-5, 6);
                            var posY = rand.Next(-3, 7);
                            var randomDirX = rand.Next(11);
                            var randomDirY = rand.Next(11);

                            float dirX;
                            float dirY;
                            if (randomDirX % 2 == 0) dirX = rand.NextSingle();
                            else dirX = -rand.NextSingle();
                            if (randomDirY % 2 == 0) dirY = rand.NextSingle();
                            else dirY = -rand.NextSingle();

                            if (i % 5 == 0)
                                Screen._Entities.Add(new SpaceJunk02(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                            else
                                Screen._Entities.Add(new SpaceJunk01(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                        }
                    }
                        
                        break;
                    
                    case 14:

                    {
                        var posZ = -198;
                        var amount = 200;
                        for (var i = 0; i < amount; i++)
                        {
                            posZ -= 1;
                            var posX = rand.Next(-5, 6);
                            var posY = rand.Next(-3, 7);
                            var randomDirX = rand.Next(11);
                            var randomDirY = rand.Next(11);

                            float dirX;
                            float dirY;
                            if (randomDirX % 2 == 0) dirX = rand.NextSingle();
                            else dirX = -rand.NextSingle();
                            if (randomDirY % 2 == 0) dirY = rand.NextSingle();
                            else dirY = -rand.NextSingle();

                            if (i % 5 == 0)
                                Screen._Entities.Add(new SpaceJunk02(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                            else
                                Screen._Entities.Add(new SpaceJunk01(Screen, AssMan, new Vector3(posX, posY, posZ),
                                    new Vector3(dirX, dirY, 1)));
                        }
                    }
                        
                        break;
                    
                    case 15:
                    
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "I'm receiving a\nsignal from...\nTHE MOON!", 3));
                        
                        break;
                    
                    case 16:

                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "The moon...\nThat must be\nwhere the thieves\nare hiding\nTo infinity and\nbeyond!", 3));
                        
                        break;
                    
                    case 17:

                        levelIsFinished = true;
                        
                        break;
                }
            }

            _entitiesSpawned = true;

            // background
            BackgroundPosInt.X = (int)BackgroundPos.X;
            BackgroundPosInt.Y = (int)BackgroundPos.Y;

            BackgroundPosOld = BackgroundPos;
        }

        private Vector3[,,] spaceDots3D = new Vector3[7, 6, 6]; // z, y, x

        private Vector3 _spaceDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f
        //private Vector3 spaceDotsPos = Vector3.Zero;

        public override void DrawGroundEffect(PerspectiveCamera cam, float dt)
        {
            // Draw dots on floor
            basicEffectPrimitives.View = cam.View;
            basicEffectPrimitives.Projection = cam.Projection;
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();

            for (var i = 0; i < spaceDots3D.GetLength(0); i++) // 0-5
            {
                for (var j = 0; j < spaceDots3D.GetLength(1); j++) // 0-6
                {
                    for (var k = 0; k < spaceDots3D.GetLength(2); k++) // 0-6
                    {
                        var vertices = new[]
                        {
                            new VertexPositionColor(spaceDots3D[i, j, k] /*+ spaceDotsPos*/, Color.White),
                            new VertexPositionColor(spaceDots3D[i, j, k] /*+ spaceDotsPos*/ + _spaceDotsMiniOffset,
                                Color.Gray)
                        };
                        Screen.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                    }
                }
            }
        }

        public override void DrawFloor()
        {
        }

        public override void DrawBackground(float dt, float rotation)
        {
            SpriteBatch.Draw(BackgroundTexture, BackgroundPosInt, null, Color.White, rotation, BackgroundOrigin,
                Vector2.One,
                SpriteEffects.None, 0);
        }

        public override void OnLevelFinished(float dt)
        {
        }

        public override void DrawOnLevelFinished(SpriteBatch sb, BitmapFont font, float dt)
        {
            String textGameFinished = "To be continued...";
            int posXGf = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                               SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                               font.GetStringRectangle(textGameFinished).Width / 2f);
            int posYGf = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                               SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                               font.GetStringRectangle(textGameFinished).Height / 2f);
            SpriteBatch.DrawString(font, textGameFinished, new Vector2(posXGf, posYGf - 16), Color.Red);

            String textGameFinished2 = "Press Escape to\nreturn to menu.";
            int posXGf2 = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                                SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                                font.GetStringRectangle(textGameFinished2).Width / 2f);
            int posYGf2 = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                                SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                                font.GetStringRectangle(textGameFinished2).Height / 2f);
            SpriteBatch.DrawString(font, textGameFinished2, new Vector2(posXGf2, posYGf2 + 16), Color.Red);
        }
    }
}