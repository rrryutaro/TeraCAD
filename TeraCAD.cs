using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using FKTModSettings;

namespace TeraCAD
{
	class TeraCAD : Mod
	{
        internal static TeraCAD instance;
        internal bool LoadedFKTModSettings = false;
        internal ModHotKey ToggleHotKeyMain;
        internal ToolBox toolMain;

        public TeraCAD()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            instance = this;

            if (!Main.dedServ)
            {
                ToggleHotKeyMain = RegisterHotKey("Toggle TeraCAD", "C");
                toolMain = new ToolBox();

                Config.LoadConfig();
                LoadedFKTModSettings = ModLoader.GetMod("FKTModSettings") != null;
                try
                {
                    if (LoadedFKTModSettings)
                    {
                        LoadModSettings();
                    }
                }
                catch { }
            }
        }

        public override void PreSaveAndQuit()
        {
            Config.SaveValues();
        }

        public override void PostUpdateInput()
        {
            try
            {
                if (LoadedFKTModSettings && !Main.gameMenu)
                {
                    UpdateModSettings();
                }
            }
            catch { }
        }

        private void LoadModSettings()
        {
            ModSetting setting = ModSettingsAPI.CreateModSettingConfig(this);
            setting.AddBool("isDrawShpes", "Display shpes", false);
			setting.AddBool("isDrawCursor", "Display TeraCAD cursor", false);
			setting.AddBool("isDrawCursorSnap", "Display TeraCAD cursor snap", false);
			setting.AddBool("isBorderCursorNone", "No border cursor when using TeraCAD tools", false);
		}

        private void UpdateModSettings()
        {
            ModSetting setting;
			if (ModSettingsAPI.TryGetModSetting(this, out setting))
			{
				setting.Get("isDrawShpes", ref Config.isDrawShpes);
				setting.Get("isDrawCursor", ref Config.isDrawCursor);
				setting.Get("isDrawCursorSnap", ref Config.isDrawCursorSnap);
				setting.Get("isBorderCursorNone", ref Config.isBorderCursorNone);
			}
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));

            layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                "TeraCAD: Tool",
                delegate
                {
                    toolMain.UIUpdate();
                    toolMain.UIDraw();
                    return true;
                },
                InterfaceScaleType.UI)
            );
			layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (layerIndex != -1)
            {
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "TeraCAD: Tooltip",
                    delegate
                    {
                        toolMain.TooltipDraw();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

	class TeraCADGrobalItem : GlobalItem
	{
		public override bool CanUseItem(Item item, Player player)
		{
			return ToolBox.SelectedTool == ToolType.None;
		}
	}
}
