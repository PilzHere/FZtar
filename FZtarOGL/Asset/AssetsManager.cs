using System;
using Microsoft.Xna.Framework.Audio;
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
        private const string TitleLoc = "textures/title";
        private const string Bg01Loc = "textures/bg01";
        private const string Bg02Loc = "textures/bg02";
        private const string Bg03Loc = "textures/bg03";
        private const string Bg04Loc = "textures/bg04";
        private const string HudLoc = "textures/hud";
        private const string AvatarFrameLoc = "textures/avatarFrame";
        private const string AvatarFrameBgLoc = "textures/avatarFrameBg";
        private const string AvatarDrInet01Loc = "textures/drInet01";
        private const string AvatarDrInet02Loc = "textures/drInet02";
        private const string AvatarDrInet03Loc = "textures/drInet03";
        private const string PlayerAimTexLoc = "textures/aim";

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
        private const string ModelFloorLoc = "models/floor";
        private const string ModelFloorCloudsLoc = "models/floorClouds";
        private const string ModelCubeLoc = "models/cube";
        private const string ModelPlayerShipLoc = "models/playerShip";
        private const string ModelEnemyShipLoc = "models/enemyShip01";
        private const string ModelTowerLoc = "models/tower01";
        private const string ModelRayLoc = "models/ray";
        private const string ModelRing01Loc = "models/ring01";
        private const string ModelRing02Loc = "models/ring02";
        private const string ModelBillboardHealth16Loc = "models/billboardPlaneHealth";
        private const string ModelBillboardFz16Loc = "models/billboardPlaneFZ";
        private const string ModelHeich01Loc = "models/heich01";
        private const string ModelHeich02Loc = "models/heich02";
        private const string ModelHeich03Loc = "models/heich03";
        private const string ModelSpaceJunk01Loc = "models/spacejunk01";
        private const string ModelSpaceJunk02Loc = "models/spacejunk02";
        private const string ModelExplosion01Loc = "models/explosion01";
        private const string ModelTurret01Loc = "models/turret01";

        // Models
        public Model FloorModel;
        public Model FloorCloudsModel;
        public Model CubeModel;
        public Model PlayerShipModel;
        public Model EnemyShipModel01;
        public Model TowerModel;
        public Model RayModel;
        public Model Ring01Model;
        public Model Ring02Model;
        public Model BillboardHealth16Model;
        public Model BillboardFZ16Model;
        public Model Heich01Model;
        public Model Heich02Model;
        public Model Heich03Model;
        public Model SpaceJunk01Model;
        public Model SpaceJunk02Model;
        public Model Explosion01Model;
        public Model Turret01Model;

        // Font locations
        private const string Font01_08Loc = "fonts/font01_08";
        private const string Font01_16Loc = "fonts/font01_16";
        private const string Font01_32Loc = "fonts/font01_32";

        private const string Font02_08Loc = "fonts/font02_08";
        private const string Font02_16Loc = "fonts/font02_16";
        private const string Font02_32Loc = "fonts/font02_32";

        private const string Font03_08Loc = "fonts/font03_08";
        private const string Font03_16Loc = "fonts/font03_16";
        private const string Font03_32Loc = "fonts/font03_32";

        // Fonts
        public BitmapFont Font01_08;
        public BitmapFont Font01_16;
        public BitmapFont Font01_32;

        public BitmapFont Font02_08;
        public BitmapFont Font02_16;
        public BitmapFont Font02_32;

        public BitmapFont Font03_08;
        public BitmapFont Font03_16;
        public BitmapFont Font03_32;

        // Music locations
        private const string MusicLevel1Loc = "sound/music/level1"; // Credits to Juhani Junkala
        private const string MusicLevel2Loc = "sound/music/level2"; // Credits to Juhani Junkala
        private const string MusicLevel3Loc = "sound/music/level3"; // Credits to Juhani Junkala
        private const string MusicTitleLoc = "sound/music/titleExt"; // Credits to Juhani Junkala
        private const string MusicEndingLoc = "sound/music/ending"; // Credits to Juhani Junkala

        // Music
        // BUG: Use SoundEffect class instead of Song class for music. Using Song causes music to not loop correctly =(
        public SoundEffect SongLevel1;
        public SoundEffect SongLevel2;
        public SoundEffect SongLevel3;
        public SoundEffect SongTitle;
        public SoundEffect SongEnding;

        // SFX Locations
        private const string SfxPopupLoc = "sound/sfx/menu/sfxPopup";
        private const string SfxPopdownLoc = "sound/sfx/menu/sfxPopdown";
        private const string SfxPointerLoc = "sound/sfx/menu/sfxPointer";
        private const string SfxConfirmLoc = "sound/sfx/menu/sfxConfirm";
        private const string SfxCancelLoc = "sound/sfx/menu/sfxCancel";

        private const string SfxRayShotLoc = "sound/sfx/action/sfxRayShot";
        private const string SfxRayHitObstacleLoc = "sound/sfx/action/sfxRayHitObstacle";
        private const string SfxShieldLoc = "sound/sfx/action/sfxShield";
        private const string SfxPlayerHitLoc = "sound/sfx/action/sfxPlayerHit";
        private const string SfxPlayerDeathLoc = "sound/sfx/action/sfxPlayerDeath";
        private const string SfxEnemyHitLoc = "sound/sfx/action/sfxEnemyHit";
        private const string SfxPowerUpLoc = "sound/sfx/action/sfxPowerUp";
        private const string SfxHealthUpLoc = "sound/sfx/action/sfxHealthUp";
        private const string SfxMessageLoc = "sound/sfx/action/sfxMessage";
        private const string SfxLowHealthLoc = "sound/sfx/action/sfxLowHealth";

        // SFX
        public SoundEffect SfxPopup;
        public SoundEffect SfxPopdown;
        public SoundEffect SfxPointer;
        public SoundEffect SfxConfirm;
        public SoundEffect SfxCancel;

        public SoundEffect SfxRayShot;
        public SoundEffect SfxRayHitObstacle;
        public SoundEffect SfxShield;
        public SoundEffect SfxPlayerHit;
        public SoundEffect SfxPlayerDeath;
        public SoundEffect SfxEnemyHit;
        public SoundEffect SfxPowerUp;
        public SoundEffect SfxHealthUp;
        public SoundEffect SfxMessage;
        public SoundEffect SfxLowHealth;

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
            LoadAllSfx();
        }

        private void LoadAllSfx()
        {
            Console.WriteLine("Loading all sound effects assets...");

            SfxPopup = _contentManager.Load<SoundEffect>(SfxPopupLoc);
            SfxPopdown = _contentManager.Load<SoundEffect>(SfxPopdownLoc);
            SfxPointer = _contentManager.Load<SoundEffect>(SfxPointerLoc);
            SfxConfirm = _contentManager.Load<SoundEffect>(SfxConfirmLoc);
            SfxCancel = _contentManager.Load<SoundEffect>(SfxCancelLoc);

            SfxRayShot = _contentManager.Load<SoundEffect>(SfxRayShotLoc);
            SfxRayHitObstacle = _contentManager.Load<SoundEffect>(SfxRayHitObstacleLoc);
            SfxShield = _contentManager.Load<SoundEffect>(SfxShieldLoc);
            SfxPlayerHit = _contentManager.Load<SoundEffect>(SfxPlayerHitLoc);
            SfxPlayerDeath = _contentManager.Load<SoundEffect>(SfxPlayerDeathLoc);
            SfxEnemyHit = _contentManager.Load<SoundEffect>(SfxEnemyHitLoc);
            SfxPowerUp = _contentManager.Load<SoundEffect>(SfxPowerUpLoc);
            SfxHealthUp = _contentManager.Load<SoundEffect>(SfxHealthUpLoc);
            SfxMessage = _contentManager.Load<SoundEffect>(SfxMessageLoc);
            SfxLowHealth = _contentManager.Load<SoundEffect>(SfxLowHealthLoc);
        }

        private void LoadAllMusic()
        {
            Console.WriteLine("Loading all music assets...");

            SongLevel1 = _contentManager.Load<SoundEffect>(MusicLevel1Loc);
            SongLevel2 = _contentManager.Load<SoundEffect>(MusicLevel2Loc);
            SongLevel3 = _contentManager.Load<SoundEffect>(MusicLevel3Loc);
            SongTitle = _contentManager.Load<SoundEffect>(MusicTitleLoc);
            SongEnding = _contentManager.Load<SoundEffect>(MusicEndingLoc);
        }

        private void LoadAllTextures()
        {
            Console.WriteLine("Loading all texture assets...");

            Title = _contentManager.Load<Texture2D>(TitleLoc);
            HudTex = _contentManager.Load<Texture2D>(HudLoc);
            AvatarFrameTex = _contentManager.Load<Texture2D>(AvatarFrameLoc);
            AvatarFrameBgTex = _contentManager.Load<Texture2D>(AvatarFrameBgLoc);
            AvatarDrInet01Tex = _contentManager.Load<Texture2D>(AvatarDrInet01Loc);
            AvatarDrInet02Tex = _contentManager.Load<Texture2D>(AvatarDrInet02Loc);
            AvatarDrInet03Tex = _contentManager.Load<Texture2D>(AvatarDrInet03Loc);
            PlayerAimTex = _contentManager.Load<Texture2D>(PlayerAimTexLoc);
            Bg01 = _contentManager.Load<Texture2D>(Bg01Loc);
            Bg02 = _contentManager.Load<Texture2D>(Bg02Loc);
            Bg03 = _contentManager.Load<Texture2D>(Bg03Loc);
            Bg04 = _contentManager.Load<Texture2D>(Bg04Loc);
        }

        private void LoadAllModels()
        {
            Console.WriteLine("Loading all model assets...");

            FloorModel = _contentManager.Load<Model>(ModelFloorLoc);
            FloorCloudsModel = _contentManager.Load<Model>(ModelFloorCloudsLoc);
            PlayerShipModel = _contentManager.Load<Model>(ModelPlayerShipLoc);
            EnemyShipModel01 = _contentManager.Load<Model>(ModelEnemyShipLoc);
            TowerModel = _contentManager.Load<Model>(ModelTowerLoc);
            RayModel = _contentManager.Load<Model>(ModelRayLoc);
            CubeModel = _contentManager.Load<Model>(ModelCubeLoc);
            Ring01Model = _contentManager.Load<Model>(ModelRing01Loc);
            Ring02Model = _contentManager.Load<Model>(ModelRing02Loc);
            BillboardHealth16Model = _contentManager.Load<Model>(ModelBillboardHealth16Loc);
            BillboardFZ16Model = _contentManager.Load<Model>(ModelBillboardFz16Loc);
            Heich01Model = _contentManager.Load<Model>(ModelHeich01Loc);
            Heich02Model = _contentManager.Load<Model>(ModelHeich02Loc);
            Heich03Model = _contentManager.Load<Model>(ModelHeich03Loc);
            SpaceJunk01Model = _contentManager.Load<Model>(ModelSpaceJunk01Loc);
            SpaceJunk02Model = _contentManager.Load<Model>(ModelSpaceJunk02Loc);
            Explosion01Model = _contentManager.Load<Model>(ModelExplosion01Loc);
            Turret01Model = _contentManager.Load<Model>(ModelTurret01Loc);
        }

        private void LoadAllFonts()
        {
            Console.WriteLine("Loading all font assets...");

            Font01_08 = _contentManager.Load<BitmapFont>(Font01_08Loc);
            Font01_16 = _contentManager.Load<BitmapFont>(Font01_16Loc);
            Font01_32 = _contentManager.Load<BitmapFont>(Font01_32Loc);

            Font02_08 = _contentManager.Load<BitmapFont>(Font02_08Loc);
            Font02_16 = _contentManager.Load<BitmapFont>(Font02_16Loc);
            Font02_32 = _contentManager.Load<BitmapFont>(Font02_32Loc);

            Font03_08 = _contentManager.Load<BitmapFont>(Font03_08Loc);
            Font03_16 = _contentManager.Load<BitmapFont>(Font03_16Loc);
            Font03_32 = _contentManager.Load<BitmapFont>(Font03_32Loc);
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