using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

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
                // 旧設定ファイルの削除
                var oldConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "TeraCAD.json");
                if (File.Exists(oldConfigPath))
                {
                    File.Delete(oldConfigPath);
                }

                ToggleHotKeyMain = RegisterHotKey("Toggle TeraCAD", "C");
                toolMain = new ToolBox();
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
