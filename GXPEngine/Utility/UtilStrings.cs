namespace NeonArkanoid
{
    internal class UtilStrings
    {
        private const string AssetPath = "../assets/"; //NEEDS TO BE CHANGED TO BE IN PROJECT ROOT

        //SPRITES
        public static readonly string SpritesPlayer = AssetPath + "sprites/player/";
        public static readonly string SpritesEnemy = AssetPath + "sprites/enemy/";
        public static readonly string SpritesUpgrades = AssetPath + "sprites/upgrade/";

        public static readonly string SpritesPause = AssetPath + "sprites/ui/pause/";
        public static readonly string SpritesMenu = AssetPath + "sprites/ui/menu/";

        //SOUNDS & MUSIC
        public static string SoundsMenu = AssetPath + "sounds/ui/menu/";
        //DEBUG RESOURCES

        //GENERAL VALUES
        public static readonly int TileSize = 32;
        public static readonly int TilesX = 32;
        public static readonly int TilesY = 24;
    }
}