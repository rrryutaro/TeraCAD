using Terraria;
using Terraria.IO;

namespace TeraCAD
{
    public static class Config
    {
        private static string ConfigPath = $@"{Main.SavePath}\Mod Configs\TeraCAD.json";
        private static Preferences config;
        private static int version = 1;
        public static void LoadConfig()
        {
            config = new Preferences(ConfigPath);

            if (config.Load())
            {
                config.Get("version", ref version);
                config.Get("isDrawShpes", ref isDrawShpes);
            }
            else
            {
                SaveValues();
            }
        }

        internal static void SaveValues()
        {
            config.Put("version", version);
            config.Put("isDrawShpes", isDrawShpes);
            config.Save();
        }

        public static bool isDrawShpes = true;
	}
}