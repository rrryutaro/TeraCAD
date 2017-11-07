using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TeraCAD
{
    class TeraCADPlayer : ModPlayer
    {
        private TagCompound mainUIData;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TeraCAD.instance.ToggleHotKeyMain.JustPressed)
            {
                ToolBoxUI.instance.Show = !ToolBoxUI.instance.Show;
				if (!ToolBoxUI.instance.Show)
				{
					ToolBox.Select(null);
					ConfigUI.instance.Show = false;
					ImageUI.instance.Show = false;
				}
            }
        }

        //public override TagCompound Save()
        //{
        //    return new TagCompound
        //    {
        //        ["TeraCAD"] = ToolBoxUI.instance.Save(),
        //    };
        //}
        //
        //public override void Load(TagCompound tag)
        //{
        //    if (tag.ContainsKey("TeraCADUI"))
        //    {
        //        if (tag.Get<object>("TeraCADUI").GetType().Equals(typeof(TagCompound)))
        //        {
        //            mainUIData = tag.Get<TagCompound>("TeraCADUI");
        //        }
        //    }
        //}

        public override void OnEnterWorld(Player player)
        {
            ToolBoxUI.instance.InitializeUI();
			ConfigUI.instance.InitializeUI();
			ImageUI.instance.InitializeUI();

			//if (mainUIData != null)
            //{
            //    ToolBoxUI.instance.Load(mainUIData);
            //}
        }

        public override void ResetEffects()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (ToolBox.InfinityRange)
                {
                    Player.tileRangeX = int.MaxValue / 32 - 20;
                    Player.tileRangeY = int.MaxValue / 32 - 20;
                }
            }
        }
    }
}
