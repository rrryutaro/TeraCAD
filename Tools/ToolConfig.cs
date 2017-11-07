namespace TeraCAD
{
    class ToolConfig : Tool
	{
        public static ToolConfig instance;
        private ConfigUI ui;

		public ToolConfig() : base(typeof(ConfigUI))
		{
            instance = this;
			ui = uistate as ConfigUI;
        }
	}
}
