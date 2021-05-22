using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TeraCAD.UIElements;

namespace TeraCAD
{
    class SettingUI : UIModState
    {
        static internal SettingUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        static internal LinePropertyTarget linePropertyTarget;

        internal UIDragablePanel panelMain;
        internal UIHoverImageButton closeButton;
        internal UIText labelCaption;
        internal UIInputTextBox inputDistance;
        internal UIInputTextBox inputCount;

        internal bool updateNeeded;

        private bool show;
        public bool Show
        {
            get { return show; }
            set
            {
                if (value)
                {
                    Append(panelMain);
                }
                else
                {
                    RemoveChild(panelMain);
                }
                show = value;
                ToolSetting.instance.visible = value;
            }
        }

        public SettingUI(UserInterface ui) : base(ui)
        {
            instance = this;
        }

        public void InitializeUI()
        {
            RemoveAllChildren();

            linePropertyTarget = LinePropertyTarget.Shapes;

            panelMain = new UIDragablePanel(true, false, false);
            panelMain.SetPadding(6);
            panelMain.Left.Set(438f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(200f, 0f);
            panelMain.MinWidth.Set(200f, 0f);
            panelMain.MaxWidth.Set(Main.screenWidth, 0f);
            panelMain.Height.Set(300, 0f);
            panelMain.MinHeight.Set(300, 0f);
            panelMain.MaxHeight.Set(Main.screenHeight, 0f);
            panelMain.OnClick += (a, b) =>
            {
                inputDistance.InputEnd();
                inputCount.InputEnd();
            };

            Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
            closeButton = new UIHoverImageButton(texture, "Close");
            closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
            closeButton.Top.Set(6f, 0f);
            panelMain.Append(closeButton);

            float topPos = 0;
            float leftPos = menuMargin;
            Vector2 textSize;

            //キャプション
            labelCaption = new UIText("Parallel Copy Setting");
            labelCaption.Left.Set(leftPos, 0f);
            labelCaption.Top.Set(topPos, 0f);
            panelMain.Append(labelCaption);
            panelMain.AddDragTarget(labelCaption);
            textSize = Main.fontMouseText.MeasureString(labelCaption.Text);
            topPos += textSize.Y * 2;

            //距離：ラベル
            var label = new UIText("Distance (block):");
            label.Left.Set(leftPos, 0f);
            label.Top.Set(topPos, 0f);
            panelMain.Append(label);
            panelMain.AddDragTarget(label);
            textSize = Main.fontMouseText.MeasureString(label.Text);
            topPos += textSize.Y;
            //距離：インプット
            inputDistance = new UIInputTextBox((int)Main.fontMouseText.MeasureString("9999").X, 4, true);
            inputDistance.Text = "8";
            inputDistance.Left.Set(leftPos, 0f);
            inputDistance.Top.Set(topPos, 0f);
            inputDistance.OnClick += (a, b) => inputCount.InputEnd();
            panelMain.Append(inputDistance);
            topPos += inputDistance.Height.Pixels + textSize.Y;

            //カウント：ラベル
            label = new UIText("Copy count:");
            label.Left.Set(leftPos, 0f);
            label.Top.Set(topPos, 0f);
            panelMain.Append(label);
            panelMain.AddDragTarget(label);
            textSize = Main.fontMouseText.MeasureString(label.Text);
            topPos += textSize.Y;
            //カウント：インプット
            inputCount = new UIInputTextBox((int)Main.fontMouseText.MeasureString("99").X, 2, true);
            inputCount.Text = "10";
            inputCount.Left.Set(leftPos, 0f);
            inputCount.Top.Set(topPos, 0f);
            inputCount.OnClick += (a, b) => inputDistance.InputEnd();
            panelMain.Append(inputCount);
            topPos += inputCount.Height.Pixels + textSize.Y;

            panelMain.MinHeight.Set(topPos, 0f);
            panelMain.Height.Set(topPos, 0f);

            updateNeeded = true;
        }

        public int ParallelCopy_Distance
        {
            get
            {
                int result = 0;
                if (0 < inputDistance.Text.Length)
                    result = int.Parse(inputDistance.Text);
                return result;
            }
        }
        public int ParallelCopy_Count
        {
            get
            {
                int result = 0;
                if (0 < inputCount.Text.Length)
                    result = int.Parse(inputCount.Text);
                return result;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override TagCompound Save()
        {
            TagCompound result = base.Save();

            if (panelMain != null)
            {
                result.Add("position", panelMain.SavePositionJsonString());
            }
            return result;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            if (tag.ContainsKey("position"))
            {
                panelMain.LoadPositionJsonString(tag.GetString("position"));
            }
        }
    }
}
