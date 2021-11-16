using System;
using FZtarOGL.Asset;
using FZtarOGL.Camera;
using FZtarOGL.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FZtarOGL.Level
{
    public class LevelTitleScreen : Level
    {
        private int _segmentDefaultLength = 200;
        private int _segmentNextLength = 200;
        private int _currentSegment;
        private bool _entitiesSpawned;

        private BasicEffect basicEffectPrimitives;

        private Random rand = new Random();

        public LevelTitleScreen(Screen.Screen screen, AssetManager assMan, SpriteBatch spriteBatch) : base(screen,
            assMan,
            spriteBatch)
        {
            isLocatedInSpace = true;

            BackgroundTexture = AssMan.Bg04;
            FogColor1 = new Vector3(0, 0, 0);
            FogStart1 = 150;
            FogEnd1 = 200;

            const float backgroundPosOffsetX = 128;
            const float backgroundPosOffsetY = 11 - 2; // was 11
            BackgroundPos = new Vector2(backgroundPosOffsetX, backgroundPosOffsetY);
            BackgroundPosInt = new Vector2((int)BackgroundPos.X, (int)BackgroundPos.Y);
            BackgroundOrigin = new Vector2(256, 256);

            BackgroundMusic = assMan.SongTitle.CreateInstance();
            BackgroundMusic.IsLooped = true;
            BackgroundMusic.Volume = GameSettings.GameSettings.MusicVolume;
            BackgroundMusic.Play();

            basicEffectPrimitives = new BasicEffect(screen.GraphicsDevice);

            Screen._Entities.Add(new StaticShip(Screen, AssMan, new Vector3(0, 1, -7)));
            
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
                        spaceDots3D[z, y, x].Z += VirtualSpeedZ * dt;

                        if (dot.Z >= 1)
                        {
                            spaceDots3D[z, y, x] = new Vector3(rand.Next(-20, 21), rand.Next(-20, 21), rand.Next(-200, -9));
                        }
                    }
                }
            }
            
            //floorDotsPos.Z += VirtualSpeedZ * dt;

            //if (floorDotsPos.Z > 10) floorDotsPos.Z -= 10;

            // spawn entities
            if (!_entitiesSpawned)
            {
                switch (_currentSegment)
                {
                    case 0:

                        // obstacles
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -200), ModelColors.Gray));

                        break;
                    case 1:

                        // obstacles
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray));
                        //Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray));

                        // pickus
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -200)));
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -240)));
                        //Screen._Entities.Add(new HealthRing(Screen, AssMan, new Vector3(0, 5, -280)));

                        // messages
                        //Messages1.Add(new Message.Message(AssMan.AvatarFrameTex, AssMan.AvatarFrameBgTex,
                        //    AssMan.AvatarDrInet01Tex, AssMan.Font02_08,
                        //    "In space...\nNo one can hear\nyou scream.", 4));
                        break;
                    case 2:

                        // obstacles
                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.Gray));*/

                        // pickups
                        /*Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(-3, 3, -200)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(0, 3, -240)));
                        Screen._Entities.Add(new PowerRing(Screen, AssMan, new Vector3(3, 3, -280)));*/

                        break;
                    case 3:

                        // obstacles
                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(0, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-0, 0, -250), ModelColors.Gray));*/

                        break;
                    case 4:

                        // obstacles
                        /*Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-9, 0, -200), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(7, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-7, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(5, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-5, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(3, 0, -250), ModelColors.Gray));
                        Screen._Entities.Add(new Tower(Screen, AssMan, new Vector3(-3, 0, -250), ModelColors.Gray));*/

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
        private Vector3 spaceDotsMiniOffset = new Vector3(0, 0, 0.33f); // x was 0.05f
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
                            new VertexPositionColor(spaceDots3D[i, j, k] /*+ spaceDotsPos*/ + spaceDotsMiniOffset,
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
    }
}