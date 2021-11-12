using System;
using System.Collections.Generic;
using System.Linq;
using FZtarOGL.Asset;
using FZtarOGL.Box;
using FZtarOGL.Camera;
using FZtarOGL.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FZtarOGL.Screen
{
    public abstract class Screen
    {
        protected Game1 Game;
        protected AssetManager AssMan;
        protected SpriteBatch SpriteBatch;

        protected long _modelsDrawn;

        public long ModelsDrawn => _modelsDrawn;

        public readonly GraphicsDevice GraphicsDevice;

        protected PerspectiveCamera Cam3d1;
        protected Vector3 _cam3dPos;

        public PerspectiveCamera Cam3d => Cam3d1;

        protected List<Entity.Entity> Entities;

        public Level.Level CurrentLevel;

        protected BasicEffect BoundingBoxesDrawEffect;

        public List<BoundingBoxFiltered> BoundingBoxesFiltered;
        private int boxfCount;

        public int BoxfCount => boxfCount;

        protected List<TransparentModel> transparentModels;

        public List<TransparentModel> TransparentModels => transparentModels;

        public Screen(Game1 game, AssetManager assMan, SpriteBatch spriteBatch)
        {
            Game = game;
            AssMan = assMan;
            SpriteBatch = spriteBatch;

            GraphicsDevice = Game.GraphicsDevice;

            Entities = new List<Entity.Entity>();
            BoundingBoxesFiltered = new List<BoundingBoxFiltered>();
            transparentModels = new List<TransparentModel>();

            // put colliderflag and mask inside a new class containing a boundingBox.
            //BoundingBox b = new BoundingBox();
            //b.

            BoundingBoxesDrawEffect = new BasicEffect(game.GraphicsDevice);
            BoundingBoxesDrawEffect.LightingEnabled = false;
            BoundingBoxesDrawEffect.TextureEnabled = false;
            BoundingBoxesDrawEffect.VertexColorEnabled = true;
            BoundingBoxesDrawEffect.Alpha = 1;
        }

        protected void ResetCounters()
        {
            _modelsDrawn = 0;
        }
        
        public abstract void Input(GameTime gt, float dt);
        public abstract void Tick(GameTime gt, float dt);

        public void CheckCollisions(List<BoundingBoxFiltered> boxesFiltered, float dt)
        {
            if (boxesFiltered.Count > 0)
            {
                // 1. Add boxes first, Entitiy -> screen -> list.add.
                //Console.WriteLine("#boxes: " + boxesFiltered.Count);
                // 2. do something
                foreach (var currentBoxf in boxesFiltered)
                {
                    foreach (var otherBoxf in boxesFiltered)
                    {
                        bool isSameBox = currentBoxf == otherBoxf;
                        if (!isSameBox)
                        {
                            bool containsBit = (currentBoxf.Mask & otherBoxf.Filter) == otherBoxf.Filter;
                            if (containsBit)
                            {
                                if (currentBoxf.Box.Intersects(otherBoxf.Box))
                                {
                                    currentBoxf.Parent.OnCollision(otherBoxf.Filter, dt);
                                }
                            }
                        }
                    }
                }

                //clear
                boxfCount = boxesFiltered.Count;

                BoundingBoxesFiltered.Clear();
            }
        }

        protected void OrderTransparentModelsByDistanceFromCam(PerspectiveCamera cam)
        {
            if (transparentModels.Count > 0)
            {
                foreach (var transModel in transparentModels)
                {
                    transModel.DistanceFromCam = Vector3.DistanceSquared(cam.Position, transModel.ModelTrans.Translation);
                }

                transparentModels = transparentModels.OrderByDescending(o => o.DistanceFromCam).ToList();
            }
        }

        public abstract void Draw(float dt);

        public abstract void DrawGUI(float dt);

        public abstract void DrawDebugGUI(GameTime gt);

        protected void UpdateAllEntities(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.Tick(dt);
            }
        }

        protected void Draw2DAllEntities(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.Draw2D(dt);
            }
        }

        protected void Draw3DAllEntities(float dt)
        {
            foreach (var entity in Entities)
            {
                entity.Draw3D(dt);
            }
        }

        protected void DrawBoundingBoxes()
        {
            foreach (var entity in Entities)
            {
                entity.DrawBoundingBox();
            }
        }

        public abstract void DrawModel(Model model, Matrix modelTransform);
        
        public abstract void DrawModelWithColor(Model model, Matrix modelTransform, Vector3 color);

        // Initialize an array of indices for the box. 12 lines require 24 indices
        protected short[] bBoxIndices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        public abstract void DrawBoundingBox(List<BoundingBox> boundingBoxes, Matrix modelTransform);

        public abstract void DrawBoundingBoxFiltered(List<BoundingBoxFiltered> boundingBoxesFiltered,
            Matrix modelTransform);

        public abstract void DrawModelUnlit(Model model, Matrix modelTransform);

        public abstract void DrawModelUnlitWithColor(Model model, Matrix modelTransform, Vector3 unlitColor);
        
        public abstract void DrawModelMeshUnlitWithColor(ModelMesh mesh, Matrix modelTransform, Vector3 unlitColor);

        protected void RemoveAllEntities()
        {
            Entities.Clear();
        }

        protected void RemoveDestroyedEntities()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                var ent = Entities[i];
                if (ent._ToDestroy)
                {
                    var destroyedId = ent._Id;
                    Entities.RemoveAt(i);

                    Console.WriteLine("Destroyed entity: " + destroyedId);
                }
            }
        }

        public virtual void Destroy()
        {
            RemoveAllEntities();
        }

        public List<Entity.Entity> _Entities => Entities;
    }
}