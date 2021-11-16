using Microsoft.Xna.Framework;

namespace FZtarOGL.Utilities
{
    public class ModelColors
    {
        private const float Max = 255;

        private const int Red1R = 0xd7;
        private const int Red1G = 0x2d;
        private const int Red1B = 0x2d;
        
        private const int Red2R = 0xaa;
        private const int Red2G = 0x14;
        private const int Red2B = 0x3c;
        
        private const int OrangeR = 0xf0;
        private const int OrangeG = 0x69;
        private const int OrangeB = 0x23;
        
        private const int GoldR = 0xff;
        private const int GoldG = 0xaa;
        private const int GoldB = 0x32;
        
        private const int YellowR = 0xff;
        private const int YellowG = 0xe6;
        private const int YellowB = 0x5a;
        
        private const int LimeR = 0xbe;
        private const int LimeG = 0xd7;
        private const int LimeB = 0x2d;
        
        private const int Green1R = 0x64;
        private const int Green1G = 0xa5;
        private const int Green1B = 0x1e;
        
        private const int Green2R = 0x23;
        private const int Green2G = 0x7d;
        private const int Green2B = 0x14;
        
        private const int DarkGreen1R = 0x0f;
        private const int DarkGreen1G = 0x55;
        private const int DarkGreen1B = 0x19;
        
        private const int DarkGreen2R = 0x0f;
        private const int DarkGreen2G = 0x32;
        private const int DarkGreen2B = 0x23;
        
        private const int PurpleR = 0x64;
        private const int PurpleG = 0x2d;
        private const int PurpleB = 0xb4;

        private const int DarkBlueR = 0x0f;
        private const int DarkBlueG = 0x37;
        private const int DarkBlueB = 0x9b;
        
        private const int Blue1R = 0x14;
        private const int Blue1G = 0x69;
        private const int Blue1B = 0xc3;
        
        private const int Blue2R = 0x14;
        private const int Blue2G = 0xa0;
        private const int Blue2B = 0xcd;
        
        private const int LightBlueR = 0x41;
        private const int LightBlueG = 0xd7;
        private const int LightBlueB = 0xd7;
        
        private const int WhiteR = 0xff;
        private const int WhiteG = 0xff;
        private const int WhiteB = 0xff;
        
        private const int GrayR = 0x78;
        private const int GrayG = 0x91;
        private const int GrayB = 0xa5;
        
        private const int DarkGrayR = 0x37;
        private const int DarkGrayG = 0x41;
        private const int DarkGrayB = 0x5a;

        private const int PlayerRayColor2R = 240;
        private const int PlayerRayColor2G = 105;
        private const int PlayerRayColor2B = 35;
        
        private const int PlayerRayColor3R = 255;
        private const int PlayerRayColor3G = 170;
        private const int PlayerRayColor3B = 50;
        
        private const int PlayerRayColor4R = 255;
        private const int PlayerRayColor4G = 230;
        private const int PlayerRayColor4B = 90;
        
        private const int PlayerThrusterColor2R = 240;
        private const int PlayerThrusterColor2G = 105;
        private const int PlayerThrusterColor2B = 35;
        
        private const int PlayerThrusterColor3R = 255;
        private const int PlayerThrusterColor3G = 170;
        private const int PlayerThrusterColor3B = 50;
        
        private const int PlayerThrusterColor4R = 255;
        private const int PlayerThrusterColor4G = 230;
        private const int PlayerThrusterColor4B = 90;
        
        public static Vector3 Red1 = new Vector3(Red1R / Max, Red1G / Max, Red1B / Max);
        public static Vector3 Red2 = new Vector3(Red2R / Max, Red2G / Max, Red2B / Max);
        public static Vector3 Orange = new Vector3(OrangeR / Max, OrangeG / Max, OrangeB / Max);
        public static Vector3 Gold = new Vector3(GoldR / Max, GoldG / Max, GoldB / Max);
        public static Vector3 Yellow = new Vector3(YellowR / Max, YellowG / Max, YellowB / Max);
        public static Vector3 Lime = new Vector3(LimeR / Max, LimeG / Max, LimeB / Max);
        public static Vector3 Green1 = new Vector3(Green1R / Max, Green1G / Max, Green1B / Max);
        public static Vector3 Green2 = new Vector3(Green2R / Max, Green2G / Max, Green2B / Max);
        public static Vector3 DarkGreen1 = new Vector3(DarkGreen1R / Max, DarkGreen1G / Max, DarkGreen1B / Max);
        public static Vector3 DarkGreen2 = new Vector3(DarkGreen2R / Max, DarkGreen2G / Max, DarkGreen2B / Max);
        public static Vector3 Purple = new Vector3(PurpleR / Max, PurpleG / Max, PurpleB / Max);
        public static Vector3 DarkBlue = new Vector3(DarkBlueR / Max, DarkBlueG / Max, DarkBlueB / Max);
        public static Vector3 Blue1 = new Vector3(Blue1R / Max, Blue1G / Max, Blue1B / Max);
        public static Vector3 Blue2 = new Vector3(Blue2R / Max, Blue2G / Max, Blue2B / Max);
        public static Vector3 LightBlue = new Vector3(LightBlueR / Max, LightBlueG / Max, LightBlueB / Max);
        public static Vector3 White = new Vector3(WhiteR / Max, WhiteG / Max, WhiteB / Max);
        public static Vector3 Gray = new Vector3(GrayR / Max, GrayG / Max, GrayB / Max);
        public static Vector3 DarkGray = new Vector3(DarkGrayR / Max, DarkGrayG / Max, DarkGrayB / Max);

        public static Vector3 PlayerRayColor1 = White;
        public static Vector3 PlayerRayColor2 = new Vector3(PlayerRayColor2R / Max, PlayerRayColor2G / Max, PlayerRayColor2B / Max);
        public static Vector3 PlayerRayColor3 = new Vector3(PlayerRayColor3R / Max, PlayerRayColor3G / Max, PlayerRayColor3B / Max);
        public static Vector3 PlayerRayColor4 = new Vector3(PlayerRayColor4R / Max, PlayerRayColor4G / Max, PlayerRayColor4B / Max);
        
        public static Vector3 EnemyRayColor1 = White;
        public static Vector3 EnemyRayColor2 = new Vector3(255 / Max, 126 / Max, 126 / Max);
        public static Vector3 EnemyRayColor3 = new Vector3(255 / Max, 0 / Max, 0 / Max);
        public static Vector3 EnemyRayColor4 = new Vector3(255 / Max, 126 / Max, 126 / Max);
        
        public static Vector3 PlayerThrusterColor1 = White;
        public static Vector3 PlayerThrusterColor2 = new Vector3(PlayerThrusterColor2R / Max, PlayerThrusterColor2G / Max, PlayerThrusterColor2B / Max);
        public static Vector3 PlayerThrusterColor3 = new Vector3(PlayerThrusterColor3R / Max, PlayerThrusterColor3G / Max, PlayerThrusterColor3B / Max);
        public static Vector3 PlayerThrusterColor4 = new Vector3(PlayerThrusterColor4R / Max, PlayerThrusterColor4G / Max, PlayerThrusterColor4B / Max);
    }
}