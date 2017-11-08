namespace TeraCAD
{
    class ToolSetting : Tool
	{
        public static ToolSetting instance;
        private SettingUI ui;

		public ToolSetting() : base(typeof(SettingUI))
		{
            instance = this;
			ui = uistate as SettingUI;
        }
	}
}
