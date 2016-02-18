namespace NeonArkanoid.Utility
{
    internal class UtilStrings
    {
        //PATH TO THE ASSETS
        private static readonly string AssetPath    =   "../assets/";

        //SPRITES
        public static readonly string SpritesPlayer =   AssetPath + "sprite/player/";
        public static readonly string SpritesBack   =   AssetPath + "sprite/background/";
        public static readonly string SpritesPause  =   AssetPath + "sprite/ui/pause/";
        public static readonly string SpritesMenu   =   AssetPath + "sprite/ui/menu/";
        public static readonly string SpritesObject =   AssetPath + "sprite/object/";

        //SOUNDS & MUSIC
        public static readonly string SoundsMenu    =   AssetPath + "sounds/ui/menu/";
        public static readonly string SoundsLevel   =   AssetPath + "sounds/level/";

        //DEBUG RESOURCES
        public static readonly string SpritesDebug  =   AssetPath + "sprite/debug/";
    }
}