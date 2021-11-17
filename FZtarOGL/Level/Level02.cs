using System;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace FZtarOGL.Level
{
    public class Level02 : Level
    {
        private int _segmentDefaultLength = 200;
        private int _segmentNextLength = 200;
        private int _currentSegment;
        private bool _entitiesSpawned;

        private BasicEffect basicEffectPrimitives;

        private Model floorModel01;
        private Model floorModel02;
        private Model floorModel03;
        private Matrix floorModelTrans01;
        private Matrix floorModelTrans02;
        private Matrix floorModelTrans03;
        private Vector3 floorModel01Pos;
        private Vector3 floorModel02Pos;
        private Vector3 floorModel03Pos;

        private Model floorModel04;
        private Model floorModel05;
        private Model floorModel06;
        private Matrix floorModelTrans04;
        private Matrix floorModelTrans05;
        private Matrix floorModelTrans06;
        private Vector3 floorModelsFutherScale;
        private Vector3 floorModel04Pos;
        private Vector3 floorModel05Pos;
        private Vector3 floorModel06Pos;
        private float rotationY;

        public Level02(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch) : base(screen, assMan,
            spriteBatch)
        {
            isLocatedInSpace = true;
            
            BackgroundTexture = AssMan.Bg03;
            FogColor1 = new Vector3(112 / 255f, 160 / 255f, 160 / 255f); // bg color horizon
            FogStart1 = 150;
            FogEnd1 = 200;

            // close clouds
            floorModel01 = AssMan.FloorCloudsModel;
            floorModel02 = AssMan.FloorCloudsModel;
            floorModel03 = AssMan.FloorCloudsModel;

            floorModel01Pos = new Vector3(0, 0, 0);
            floorModel02Pos = new Vector3(0, 0, -100);
            floorModel03Pos = new Vector3(0, 0, -200);
            floorModelTrans01 = Matrix.CreateScale(Vector3.One) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(0) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel01Pos);
            floorModelTrans02 = Matrix.CreateScale(Vector3.One) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(0) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel02Pos);
            floorModelTrans03 = Matrix.CreateScale(Vector3.One) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(0) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel03Pos);

            // further clouds
            floorModel04 = AssMan.FloorCloudsModel;
            floorModel05 = AssMan.FloorCloudsModel;
            floorModel06 = AssMan.FloorCloudsModel;

            floorModel04Pos = new Vector3(0, -5, 0);
            floorModel05Pos = new Vector3(0, -5, -200);
            floorModel06Pos = new Vector3(0, -5, -400);

            floorModelsFutherScale = new Vector3(2, 2, 2);
            rotationY = MathHelper.ToRadians(180);

            floorModelTrans04 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel04Pos);
            floorModelTrans05 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel05Pos);
            floorModelTrans06 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) *
                                Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(floorModel06Pos);

            const float backgroundPosOffsetX = 128;
            const float backgroundPosOffsetY = 11 - 2; // was 11
            BackgroundPos = new Vector2(backgroundPosOffsetX, backgroundPosOffsetY);
            BackgroundPosInt = new Vector2((int)BackgroundPos.X, (int)BackgroundPos.Y);
            BackgroundOrigin = new Vector2(256, 256);

            BackgroundMusic = assMan.SongLevel2.CreateInstance();
            BackgroundMusic.IsLooped = true;
            BackgroundMusic.Volume = GameSettings.GameSettings.MusicVolume;
            BackgroundMusic.Play();

            basicEffectPrimitives = new BasicEffect(screen.GraphicsDevice);

            const int dotPosStartX = -10;
            const int dotPosY = -3;
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

            LevelVirtualSpeedZ *= 1.15f;
            
            screen.SetContinueLevel(ContinueLevel.Level02);
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

            //floor models close
            float speedZBoost = 2;
            float closeModelsSpeed = VirtualSpeedZ * speedZBoost * dt;
            floorModel01Pos.Z += closeModelsSpeed;
            floorModel02Pos.Z += closeModelsSpeed;
            floorModel03Pos.Z += closeModelsSpeed;

            float maxDistZ = 50;
            float startZ = 300;
            if (floorModel01Pos.Z >= maxDistZ) floorModel01Pos.Z -= startZ;
            if (floorModel02Pos.Z >= maxDistZ) floorModel02Pos.Z -= startZ;
            if (floorModel03Pos.Z >= maxDistZ) floorModel03Pos.Z -= startZ;

            floorModelTrans01 = Matrix.CreateTranslation(floorModel01Pos);
            floorModelTrans02 = Matrix.CreateTranslation(floorModel02Pos);
            floorModelTrans03 = Matrix.CreateTranslation(floorModel03Pos);

            //floor models further
            float furtherModelsSpeed = VirtualSpeedZ * dt;
            floorModel04Pos.Z += furtherModelsSpeed;
            floorModel05Pos.Z += furtherModelsSpeed;
            floorModel06Pos.Z += furtherModelsSpeed;

            maxDistZ = 100;
            startZ = 600;
            if (floorModel04Pos.Z >= maxDistZ) floorModel04Pos.Z -= startZ;
            if (floorModel05Pos.Z >= maxDistZ) floorModel05Pos.Z -= startZ;
            if (floorModel06Pos.Z >= maxDistZ) floorModel06Pos.Z -= startZ;

            floorModelTrans04 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) *
                                Matrix.CreateTranslation(floorModel04Pos);
            floorModelTrans05 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) *
                                Matrix.CreateTranslation(floorModel05Pos);
            floorModelTrans06 = Matrix.CreateScale(floorModelsFutherScale) * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(rotationY) *
                                Matrix.CreateRotationZ(0) *
                                Matrix.CreateTranslation(floorModel06Pos);

            // spawn entities
            if (!_entitiesSpawned)
            {
                switch (_currentSegment)
                {
                    case 0:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -200), ModelColors.Gray, true, true));

                        // messages
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "This is the right\nway.\nWow!\nWhat a view...", 3));
                        // messages
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Hey! I can see my\nhouse from here!", 3));
                        break;
                    case 1:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet01Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Incoming enemies,\nkamikaze miners again!", 3));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(10, 15, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-7, 10, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 6, -300), true));
                        
                        // obstacles
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray, true, true));
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray, true, true));

                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 6, -200)));
                        
                        // pickus
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -200)));
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -240)));
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -280)));
                        break;
                    case 2:

                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(13, 15, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-13, 10, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(5, 6, -300), true));
                        
                        // obstacles
                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.Gray, true, true));*/

                        // pickups
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-5, 4, -200)));
                        

                        break;
                    case 3:

                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray, true, true));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(7, 0, -250), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-7, 0, -250), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -250), ModelColors.White, true, true));

                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(4, 5, -250)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-4, 5, -250)));
                        
                        break;
                    case 4:

                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 7, -197)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 7, -197)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 3, -197)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 3, -197)));
                        
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 7f, -247)));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 3f, -247)));
                        
                        // obstacles
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(10, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-10, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -250), ModelColors.Gray, true, true));

                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 4, -230)));
                        
                        break;
                    case 5:
                        // towers'n'haitch
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-4.5f, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(4.5f, 0, -200), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(0, 0, -200 -0.1f), ModelColors.Orange));
                        
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 7.5f, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 1.5f, -200)));
                        
                        // towers'n'haitch
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-4.5f, 0, -250), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(4.5f, 0, -250), ModelColors.Gray, true, true));
                        Screen._Entities.Add(new Haitch(Screen, AssMan, new Vector3(0, 0, -250 -0.1f), ModelColors.Red1));
                        
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 7.5f, -250)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 1.5f, -250)));
                        break;
                    case 6:
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-6f, 0, -200), ModelColors.Orange, true, true));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(6, 0, -200), ModelColors.LightBlue, true, true));
                        
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-9, 5f, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-9, 8, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(-9, 1f, -200)));
                        
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(9, 5f, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(9, 8, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(9, 1f, -200)));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-13, 10, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(13, 7, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-2, 15, -350), true));
                        
                        break;
                    case 7:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Who built these\ntowers anyway?!", 3));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -200), ModelColors.DarkGray, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 7f, -197)));
                        //Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 7f, -198)));
                        
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 5f, -225)));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0f, 0, -250), ModelColors.DarkGray, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 7f, -247)));
                        //Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(0, 7f, -248)));
                        break;
                    case 8:
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(10, 0, -200), ModelColors.Red1, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(10, 7f, -197)));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-10, 0, -200), ModelColors.Red1, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-10, 7f, -197)));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(7, 0, -250), ModelColors.Green1, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(7, 7f, -247)));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-7, 0, -230), ModelColors.Green1, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-7, 7f, -227)));
                        
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-0, 0, -275), ModelColors.Blue1, true, true));
                        Screen._Entities.Add(new Turret(Screen, AssMan, new Vector3(-0, 7f, -272)));
                        break;
                    case 9:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet02Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "It's quite...\nToo quiet...", 3));
                        
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 1, -200)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 4, -230)));
                        Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 7, -270)));
                        break;
                    case 10:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "Here they come!", 3));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-1, 15, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-4, 10, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(5, 6, -300), true));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 0, -225), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-10, 5, -275), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(3, 2, -350), true));
                        break;
                    case 11:
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-10, 15, -200), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(10, 10, -250), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(5, 6, -300), true));
                        
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(0, 0, -225), true));
                        Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(-10, 5, -275), true));
                        //Screen._Entities.Add(new EnemyShip(Screen, AssMan, new Vector3(3, 2, -350), true));
                        break;
                    case 12:
                        Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                            AssMan.AvatarDrInet03Tex, AssMan.SfxMessage, AssMan.Font02_08,
                            "This cant be!?\nI'm picking up signals\nfrom outer space!\nLet's go!", 3.5f));
                        
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
            /*basicEffectPrimitives.View = cam.View;
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
            }*/
        }

        public override void DrawFloor()
        {
            // further clouds
            Screen.DrawModelUnlit(floorModel04, floorModelTrans04);
            Screen.DrawModelUnlit(floorModel05, floorModelTrans05);
            Screen.DrawModelUnlit(floorModel06, floorModelTrans06);

            // close clouds
            Screen.DrawModelUnlit(floorModel01, floorModelTrans01);
            Screen.DrawModelUnlit(floorModel02, floorModelTrans02);
            Screen.DrawModelUnlit(floorModel03, floorModelTrans03);
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
            if (_loadNextLevelTimer >= nextLevelTime) Screen.LoadLevel(3);
        }

        public override void DrawOnLevelFinished(SpriteBatch sb, BitmapFont font, float dt)
        {
            String textLevelFinished = "The sky is the limit?";
            int posXLf = (int)(SpriteBatch.GraphicsDevice.Viewport.X +
                               SpriteBatch.GraphicsDevice.Viewport.Width / 2f -
                               font.GetStringRectangle(textLevelFinished).Width / 2f);
            int posYLf = (int)(SpriteBatch.GraphicsDevice.Viewport.Y +
                               SpriteBatch.GraphicsDevice.Viewport.Height / 2f -
                               font.GetStringRectangle(textLevelFinished).Height / 2f);
            SpriteBatch.DrawString(font, textLevelFinished, new Vector2(posXLf, posYLf - 16), Color.Red);
            
            String textLevelFinished2 = "It continues in\nouter space!";
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