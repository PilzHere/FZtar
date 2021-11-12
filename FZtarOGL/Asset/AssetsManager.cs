using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;

namespace FZtarOGL.Asset
{
    public class AssetManager
    {
        private ContentManager _contentManager;

        public AssetManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        // Textures location
        private const string _titleLoc = "textures/title";
        private const string _bg01Loc = "textures/bg01";
        private const string _bg02Loc = "textures/bg02";
        private const string _bg03Loc = "textures/bg03";
        private const string _bg04Loc = "textures/bg04";
        private const string _hudLoc = "textures/hud";
        private const string _avatarFrameLoc = "textures/avatarFrame";
        private const string _avatarFrameBgLoc = "textures/avatarFrameBg";
        private const string _avatarDrInet01Loc = "textures/drInet01";
        private const string _avatarDrInet02Loc = "textures/drInet02";
        private const string _avatarDrInet03Loc = "textures/drInet03";
        private const string _playerAimTexLoc = "textures/aim";

        // Textures
        public Texture2D Title;
        public Texture2D Bg01;
        public Texture2D Bg02;
        public Texture2D Bg03;
        public Texture2D Bg04;
        public Texture2D HudTex;
        public Texture2D AvatarFrameTex;
        public Texture2D AvatarFrameBgTex;
        public Texture2D AvatarDrInet01Tex;
        public Texture2D AvatarDrInet02Tex;
        public Texture2D AvatarDrInet03Tex;
        public Texture2D PlayerAimTex;

        // Models location
        private const string _modelFloorLoc = "models/floor";
        private const string _modelFloorCloudsLoc = "models/floorClouds";
        private const string _modelCubeLoc = "models/cube";
        private const string _modelPlayerShipLoc = "models/playerShip";
        private const string _modelTowerLoc = "models/tower01";
        private const string _modelRayLoc = "models/ray";
        private const string _modelRing01Loc = "models/ring01";
        private const string _modelRing02Loc = "models/ring02";
        private const string _modelBillboardHealth16Loc = "models/billboardPlaneHealth";
        private const string _modelBillboardFZ16Loc = "models/billboardPlaneFZ";
        private const string _modelHeich01Loc = "models/heich01";
        private const string _modelHeich02Loc = "models/heich02";
        private const string _modelHeich03Loc = "models/heich03";

        // Models
        public Model FloorModel;
        public Model FloorCloudsModel;
        public Model CubeModel;
        public Model PlayerShipModel;
        public Model TowerModel;
        public Model RayModel;
        public Model Ring01Model;
        public Model Ring02Model;
        public Model BillboardHealth16Model;
        public Model BillboardFZ16Model;
        public Model Heich01Model;
        public Model Heich02Model;
        public Model Heich03Model;

        // Font locations
        private const string _font01_08Loc = "fonts/font01_08";
        private const string _font01_16Loc = "fonts/font01_16";
        private const string _font01_32Loc = "fonts/font01_32";

        private const string _font02_08Loc = "fonts/font02_08";
        private const string _font02_16Loc = "fonts/font02_16";
        private const string _font02_32Loc = "fonts/font02_32";

        // Fonts
        public BitmapFont Font01_08;
        public BitmapFont Font01_16;
        public BitmapFont Font01_32;
        
        public BitmapFont Font02_08;
        public BitmapFont Font02_16;
        public BitmapFont Font02_32;

        // Music locations
        private const string musicLevel1Loc = "sound/music/level1"; // Credits to Juhani Junkala
        private const string musicLevel2Loc = "sound/music/level2"; // Credits to Juhani Junkala
        private const string musicLevel3Loc = "sound/music/level3"; // Credits to Juhani Junkala
        private const string musicTitleLoc = "sound/music/title"; // Credits to Juhani Junkala
        private const string musicEndingLoc = "sound/music/ending"; // Credits to Juhani Junkala

        // Music
        public Song songLevel1;
        public Song songLevel2;
        public Song songLevel3;
        public Song songTitle;
        public Song songEnding;

        public void LoadAllAssets()
        {
            Console.WriteLine("Beginning to load all assets.");

            // Load all assets here.
            LoadAllTextures();
            LoadAllModels();
            LoadAllFonts();
            LoadAllSounds();

            Console.WriteLine("All assets loaded.");
        }

        private void LoadAllSounds()
        {
            Console.WriteLine("Loading all sound assets...");

            LoadAllMusic();
            LoadAllSFX();
        }

        private void LoadAllSFX()
        {
            Console.WriteLine("Loading all sound effects assets...");
        }

        private void LoadAllMusic()
        {
            Console.WriteLine("Loading all music assets...");

            songLevel1 = _contentManager.Load<Song>(musicLevel1Loc);
            songLevel2 = _contentManager.Load<Song>(musicLevel2Loc);
            songLevel3 = _contentManager.Load<Song>(musicLevel3Loc);
            songTitle = _contentManager.Load<Song>(musicTitleLoc);
            songEnding = _contentManager.Load<Song>(musicEndingLoc);
        }

        private void LoadAllTextures()
        {
            Console.WriteLine("Loading all texture assets...");

            Title = _contentManager.Load<Texture2D>(_titleLoc);
            HudTex = _contentManager.Load<Texture2D>(_hudLoc);
            AvatarFrameTex = _contentManager.Load<Texture2D>(_avatarFrameLoc);
            AvatarFrameBgTex = _contentManager.Load<Texture2D>(_avatarFrameBgLoc);
            AvatarDrInet01Tex = _contentManager.Load<Texture2D>(_avatarDrInet01Loc);
            AvatarDrInet02Tex = _contentManager.Load<Texture2D>(_avatarDrInet02Loc);
            AvatarDrInet03Tex = _contentManager.Load<Texture2D>(_avatarDrInet03Loc);
            PlayerAimTex = _contentManager.Load<Texture2D>(_playerAimTexLoc);
            Bg01 = _contentManager.Load<Texture2D>(_bg01Loc);
            Bg02 = _contentManager.Load<Texture2D>(_bg02Loc);
            Bg03 = _contentManager.Load<Texture2D>(_bg03Loc);
            Bg04 = _contentManager.Load<Texture2D>(_bg04Loc);
        }

        private void LoadAllModels()
        {
            Console.WriteLine("Loading all model assets...");

            FloorModel = _contentManager.Load<Model>(_modelFloorLoc);
            FloorCloudsModel = _contentManager.Load<Model>(_modelFloorCloudsLoc);
            PlayerShipModel = _contentManager.Load<Model>(_modelPlayerShipLoc);
            TowerModel = _contentManager.Load<Model>(_modelTowerLoc);
            RayModel = _contentManager.Load<Model>(_modelRayLoc);
            CubeModel = _contentManager.Load<Model>(_modelCubeLoc);
            Ring01Model = _contentManager.Load<Model>(_modelRing01Loc);
            Ring02Model = _contentManager.Load<Model>(_modelRing02Loc);
            BillboardHealth16Model = _contentManager.Load<Model>(_modelBillboardHealth16Loc);
            BillboardFZ16Model = _contentManager.Load<Model>(_modelBillboardFZ16Loc);
            Heich01Model = _contentManager.Load<Model>(_modelHeich01Loc);
            Heich02Model = _contentManager.Load<Model>(_modelHeich02Loc);
            Heich03Model = _contentManager.Load<Model>(_modelHeich03Loc);
        }

        private void LoadAllFonts()
        {
            Console.WriteLine("Loading all font assets...");

            Font01_08 = _contentManager.Load<BitmapFont>(_font01_08Loc);
            Font01_16 = _contentManager.Load<BitmapFont>(_font01_16Loc);
            Font01_32 = _contentManager.Load<BitmapFont>(_font01_32Loc);
            
            Font02_08 = _contentManager.Load<BitmapFont>(_font02_08Loc);
            Font02_16 = _contentManager.Load<BitmapFont>(_font02_16Loc);
            Font02_32 = _contentManager.Load<BitmapFont>(_font02_32Loc);
        }

        /*public virtual T LoadAsset<T>(string location)
        {
            var asset = _contentManager.Load<T>(location);
            // BUG: Doesnt work.
            if (asset == null)
            {
                Console.WriteLine("OMFG ASSET NULL");
            }
            return _contentManager.Load<T>(location);
        }*/
    }
}