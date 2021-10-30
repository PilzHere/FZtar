using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FZtarOGL.Asset
{
    public class AssetManager
    {
        private ContentManager _contentManager;
        
        public AssetManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private const string Bg01Loc = "Textures/bg01";
        
        private const string Map01Loc = "Maps/map01";
        
        public void LoadBasicAssets()
        {
            // Load all assets here.
            //LoadBasicTextures();
            //LoadBasicMaps();
        }

        private void LoadBasicTextures()
        {
            _contentManager.Load<Texture2D>(Bg01Loc);
        }
        
        private void LoadBasicMaps()
        {
            //_contentManager.Load<TiledMap>(Map01Loc);
        }
        
        public virtual T LoadAsset<T>(string location)
        {
            var asset = _contentManager.Load<T>(location);
            if (asset == null)
            {
                Console.WriteLine("OMFG ASSET NULL");
            }
            return _contentManager.Load<T>(location);
        }
    }
}