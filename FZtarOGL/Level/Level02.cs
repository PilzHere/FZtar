using System.Numerics;
using System.Threading.Tasks.Dataflow;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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

            levelSong = assMan.songLevel2;
            MediaPlayer.Play(levelSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;

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
                            "Hey! I can see my\nhouse from here!", 3));
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
    }
}