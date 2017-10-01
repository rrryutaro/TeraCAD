using Terraria;

namespace TeraCAD
{
    class StampTool : Tool
	{
        public static StampTool instance;
        private StampUI ui;

        private ToolDropper toolDropper;
        private ToolStamp toolStamp;

        public StampTool() : base(typeof(StampUI))
		{
            instance = this;
            ui = uistate as StampUI;
            toolDropper = new ToolDropper();
            toolStamp = new ToolStamp();
        }

        internal override void UIUpdate()
        {
            base.UIUpdate();

            if (ToolBox.SelectedTool == ToolType.Stamp)
            {
                toolStamp.Update();
            }
            else if (ToolBox.SelectedTool == ToolType.Dropper)
            {
                toolDropper.Update();
            }
        }

        internal override void UIDraw()
        {
            base.UIDraw();

            if (ToolBox.SelectedTool == ToolType.Stamp)
            {
                toolStamp.Draw();
            }
            else if (ToolBox.SelectedTool == ToolType.Dropper)
            {
                toolDropper.Draw();
            }
        }
    }
}
