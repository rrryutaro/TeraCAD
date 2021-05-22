namespace TeraCAD
{
    class ToolLineProperty : Tool
    {
        public static ToolLineProperty instance;
        private LinePropertyUI ui;

        public ToolLineProperty() : base(typeof(LinePropertyUI))
        {
            instance = this;
            ui = uistate as LinePropertyUI;
        }
    }
}
