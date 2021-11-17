using System;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using FZtarOGL.Screen;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;

namespace FZtarOGL.Level
{
    public class Level01 : Level
    {
        private int _segmentDefaultLength = 200;
        private int _segmentNextLength = 200;
        private int _currentSegment;
        private bool _entitiesSpawned;

        private BasicEffect basicEffectPrimitives;

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

            BackgroundMusic = assMan.SongLevel1.CreateInstance();
            BackgroundMusic.IsLooped = true;
            BackgroundMusic.Volume = GameSettings.GameSettings.MusicVolume;
            BackgroundMusic.Play();

            basicEffectPrimitives = new BasicEffect(screen.GraphicsDevice);

            const int dotPosStartX = -10;
            const int dotPosY = 0;
            const int dotPosStartZ = 10;
            const int dotPosOffsetX = 4;
            const int dotPosOffsetZ = 10;
            var dotPosZ = dotPosStartZ;
            for (var z = 0; z < floorDots2D.GetLength(0); z++) // 0-5
            {
                var dotPosX = dotPosStartX;
                dotPosZ -= dotPosOffsetZ;
                for (var x = 0; x < floorDots2D.GetLength(1); x++) // 0-6
                {
                    //Console.WriteLine("x: " + dotPosX + " z: " + dotPosZ);

                    floorDots2D[z, x].X = dotPosX;
                    floorDots2D[z, x].Y = dotPosY;
                    floorDots2D[z, x].Z = dotPosZ;

                    dotPosX += dotPosOffsetX;
                }
            }

            screen.SetContinueLevel(ContinueLevel.Level01);
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

            floorDotsPos.Z += VirtualSpeedZ * dt;

            if (floorDotsPos.Z > 10) floorDotsPos.Z -= 10;

            // spawn entities
            if (!_entitiesSpawned)
            {
                switch (_currentSegment)
                {
                    case 0:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -200), ModelColors.DarkGray,
                            true, false));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -200), ModelColors.DarkGray,
                            true, false));

                        // trees
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(10,0,-100)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-10,0,-100)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(9,0,-110)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-9,0,-110)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8,0,-120)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-8,0,-120)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(7,0,-130)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-7,0,-130)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(6,0,-140)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-6,0,-140)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(5,0,-150)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-5,0,-150)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(4,0,-160)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-4,0,-160)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3,0,-170)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-3,0,-170)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8,0,-250)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-2,0,-233)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3,0,-215)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-6,0,-240)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(0,0,-270)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3,0,-350)));

                        // messages
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage ,AssMan.Font02_08,
                            "Our warehouse has\nbeen breached!\n25 thousand GPU's\nin thieves's hand...\nNo one but me\ncan save it!",
                            5));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Damn miners!\nThey will do\nanything for\nthose bits of\ncoins... I mean...\nBitcoin.",
                            5));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet02Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "It's a good thing\nI still have my\nspaceship close\nat hand for\nsituations like\nthis!",
                            5));
                        break;
                    case 1:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-4, 0, -200), ModelColors.DarkGray,
                            true, false));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(4, 0, -200), ModelColors.DarkGray,
                            true, false));

                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-8, 0, -250), ModelColors.DarkGray,
                            true, false));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-0, 0, -250), ModelColors.DarkGray,
                            true, false));

                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -300), ModelColors.DarkGray,
                            true, false));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(8, 0, -300), ModelColors.DarkGray,
                            true, false));

                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.DarkGray));
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.DarkGray));

                        // pickus
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 3, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-4, 5, -250)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(4, 7, -300)));
                        
                        //tree
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(7,0,-433)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-4,0,-410)));

                        // messages
                        /*Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.Font02_08,
                            "Our warehouse has\nbeen breached!\n25 thousand GPU's\nin thieves's hand...\nNo one but me\ncan save it!", 5));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet02Tex, AssMan.Font02_08,
                            "It's a good thing\nI still have my\nspaceship close\nat hand for\nsituations like\nthis!",
                            5));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.Font02_08,
                            "Lycka till i jamet!", 3));*/

                        break;
                    case 2:

                        // obstacles
                        Screen._Entities.Add(new HaitchLow(Screen, AssMan, new Vector3(0, 0, -200), ModelColors.Red1));
                        Screen._Entities.Add(new HaitchHigh(Screen, AssMan, new Vector3(0, 0, -300), ModelColors.Gold));

                        //Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(9, 0, -200)));

                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.DarkGray));*/

                        // pickups
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 1, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 7.5f, -300)));
                        /*Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(3, 3, -280)));*/

                        break;
                    case 3:
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 6.25f, -220)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(11, 6.25f, -240)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-7, 6.25f, -260)));
                        
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(14, 0, -230), ModelColors.DarkBlue));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(-12, 0, -260), ModelColors.Orange));
                        
                        // obstacles
                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -250), ModelColors.DarkGray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-0, 0, -250), ModelColors.DarkGray));*/
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(6.5f,0,-200)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-4.5f,0,-220)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3.5f,0,-275)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-1.5f,0,-280)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8.5f,0,-283)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(10.5f,0,-290)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-10.5f,0,-295)));

                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(13, 0, -375), ModelColors.LightBlue));
                        
                        break;
                    case 4:

                        // obstacles
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(0, 0, -200), ModelColors.Purple));

                        // pickups
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 7.5f, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 1, -200)));

                        break;
                    case 5:

                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Incoming enemy!\nA kamikaze miner!", 3));

                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 10, -200), true));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(2.5f,0,-200)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-1.5f,0,-230)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3.5f,0,-253)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(9.5f,0,-270)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-8.5f,0,-300)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-6.5f,0,-320)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-3.5f,0,-340)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8.5f,0,-370)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-5.3f,0,-400)));

                        break;
                    case 6:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "There's more of\nthem!", 3));

                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(10, 15, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-7, 10, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 6, -300), true));
                        
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(-14, 0, -200), ModelColors.Blue2));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(0, 0, -220), ModelColors.Gold));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(12, 0, -310), ModelColors.Gray));

                        break;
                    case 7:
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 5, -198)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 5, -198)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 5, -198)));
                        
                        int max = 50;
                        for (var i = 0; i < max; i++)
                        {
                            if (i == 0 || i == max - 1)
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-10, 0, -200 - 3 * i),
                                    ModelColors.Blue1, true, false));
                            else
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-10, 0, -200 - 3 * i),
                                    ModelColors.Blue1, false, false));
                        }

                        for (var i = 0; i < max; i++)
                        {
                            if (i == 0 || i == max - 1)
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -200 - 3 * i),
                                    ModelColors.DarkGray, true, false));
                            else
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -200 - 3 * i),
                                    ModelColors.DarkGray, false, false));
                        }

                        for (var i = 0; i < max; i++)
                        {
                            if (i == 0 || i == max - 1)
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(10, 0, -200 - 3 * i),
                                    ModelColors.Red1, true, false));
                            else
                                Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(10, 0, -200 - 3 * i),
                                    ModelColors.Red1, false, false));
                        }
                        
                        // Left side
                        Screen._Entities.Add(new HaitchLow(Screen, AssMan, new Vector3(-5, 10, -200 - 3 * 10), ModelColors.Gold));
                        Screen._Entities.Add(new HaitchHigh(Screen, AssMan, new Vector3(-5, 0, -200 - 3 * 10), ModelColors.Green1));
                        
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(-5, 1, -200 - 3 * 20), ModelColors.Lime));
                        
                        Screen._Entities.Add(new HaitchLow(Screen, AssMan, new Vector3(-5, 10, -200 - 3 * 30), ModelColors.Red1));
                        Screen._Entities.Add(new HaitchHigh(Screen, AssMan, new Vector3(-5, 0, -200 - 3 * 30), ModelColors.Orange));
                        
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(-5, 1, -200 - 3 * 40), ModelColors.Purple));
                        
                        // Right side
                        Screen._Entities.Add(new HaitchLow(Screen, AssMan, new Vector3(5, 10, -200 - 3 * 10), ModelColors.Red1));
                        Screen._Entities.Add(new HaitchHigh(Screen, AssMan, new Vector3(5, 0, -200 - 3 * 10), ModelColors.Gold));
                        
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(5, 1, -200 - 3 * 20), ModelColors.Red2));
                        
                        Screen._Entities.Add(new HaitchLow(Screen, AssMan, new Vector3(5, 10, -200 - 3 * 30), ModelColors.Yellow));
                        Screen._Entities.Add(new HaitchHigh(Screen, AssMan, new Vector3(5, 0, -200 - 3 * 30), ModelColors.Green2));
                        
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(5, 1, -200 - 3 * 40), ModelColors.LightBlue));
                        break;
                    case 8:
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(6.5f,0,-200)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-4.5f,0,-220)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3.5f,0,-275)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-1.5f,0,-280)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8.5f,0,-283)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(10.5f,0,-290)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-10.5f,0,-295)));
                        break;
                    
                    case 9:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "It's those turrets\nagain!", 3));
                        
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 6.25f, -213)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-8, 6.25f, -203)));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(14, 0, -210), ModelColors.DarkBlue));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(-12, 0, -210), ModelColors.Gold));
                        
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(4, 6.25f, -253)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 6.25f, -247)));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(0, 0, -250), ModelColors.Orange));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(-14, 0, -250), ModelColors.Purple));
                        
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 6.25f, -305)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 6.25f, -295)));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(14, 0, -300), ModelColors.Red1));
                        Screen._Entities.Add(new House(Screen, AssMan, new Vector3(-14, 0, -300), ModelColors.DarkGreen1));
                        break;
                    
                    case 10:
                        // obstacles
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(0, 0, -200), ModelColors.Purple));

                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(6.5f,0,-200)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-4.5f,0,-220)));
                        
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(3.5f,0,-275)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-1.5f,0,-280)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(8.5f,0,-283)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(10.5f,0,-290)));
                        Screen._Entities.Add(new Tree(Screen, AssMan, new Vector3(-10.5f,0,-295)));
                        
                        // pickups
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 7.5f, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 1, -200)));
                        break;
                    case 11:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet02Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "I'm tracking the\nthe direction of\nthe stolen goods.", 3));
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "It seems I have\nto increase\naltitude!\nOnwards to the\nskies!", 3));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -200), ModelColors.DarkGray,
                            true, false));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -200), ModelColors.DarkGray,
                            true, false));
                        break;
                    case 12:
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

        private Vector3[,] floorDots2D = new Vector3[7, 6]; // z, x
        private Vector3 floorDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f
        private Vector3 floorDotsPos = Vector3.Zero;

        public override void DrawGroundEffect(PerspectiveCamera cam, float dt)
        {
            // Draw dots on floor
            basicEffectPrimitives.View = cam.View;
            basicEffectPrimitives.Projection = cam.Projection;
            basicEffectPrimitives.VertexColorEnabled = true;
            basicEffectPrimitives.CurrentTechnique.Passes[0].Apply();
            for (var i = 0; i < floorDots2D.GetLength(0); i++) // 0-5
            {
                for (var j = 0; j < floorDots2D.GetLength(1); j++) // 0-6
                {
                    var vertices = new[]
                    {
                        new VertexPositionColor(floorDots2D[i, j] + floorDotsPos, Color.White),
                        new VertexPositionColor(floorDots2D[i, j] + floorDotsPos + floorDotsMiniOffset,
                            Color.Gray)
                    };
                    Screen.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
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

        private float _loadNextLevelTimer;
        
        public override void OnLevelFinished(float dt)
        {
            var nextLevelTime = 5;
            _loadNextLevelTimer += dt;
            if (_loadNextLevelTimer >= nextLevelTime) Screen.LoadLevel(2);
        }

        public override void DrawOnLevelFinished(SpriteBatch sb, BitmapFont font, float dt)
        {
            String textLevelFinished = "Up up... and away!";
            int posXLf = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                               SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                               font.GetStringRectangle(textLevelFinished).Width / 2f);
            int posYLf = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                               SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                               font.GetStringRectangle(textLevelFinished).Height / 2f);
            SpriteBatch.DrawString(font, textLevelFinished, new Vector2(posXLf, posYLf - 16), Color.Red);
            
            String textLevelFinished2 = "It continues in the skies.";
            int posXLf2 = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                                SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                                font.GetStringRectangle(textLevelFinished2).Width / 2f);
            int posYLf2 = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                                SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                                font.GetStringRectangle(textLevelFinished2).Height / 2f);
            SpriteBatch.DrawString(font, textLevelFinished2, new Vector2(posXLf2, posYLf2 + 16), Color.Red);
        }
    }
}