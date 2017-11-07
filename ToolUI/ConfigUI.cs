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
    class ConfigUI : UIModState
	{
		static internal ConfigUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        internal UIDragablePanel panelMain;
        internal UIHoverImageButton closeButton;
		internal UISlider sliderTransmittance;
		internal UISlider sliderWidth;
		internal UISliderColor sliderColor;

		internal bool updateNeeded;

        internal string caption = $"";

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
				ToolConfig.instance.visible = value;
			}
		}

		public ConfigUI(UserInterface ui) : base(ui)
		{
			instance = this;
		}

        public void InitializeUI()
        {
            RemoveAllChildren();

            panelMain = new UIDragablePanel(true, false, false);
            panelMain.SetPadding(6);
            panelMain.Left.Set(438f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(200f, 0f);
			panelMain.MinWidth.Set(200f, 0f);
			panelMain.MaxWidth.Set(Main.screenWidth, 0f);
			panelMain.Height.Set(250, 0f);
			panelMain.MinHeight.Set(250, 0f);
			panelMain.MaxHeight.Set(Main.screenHeight, 0f);

			Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

            float topPos = 0;
            float leftPos = menuMargin;

			//キャプション
			var label = new UIText("TeraCAD Config");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("TeraCAD Line Config").Y;

			//線幅
			label = new UIText("Line width:");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Line width:").Y;

			sliderWidth = new UISlider(1, 16);
			sliderWidth.Left.Set(leftPos, 0f);
			sliderWidth.Top.Set(topPos, 0f);
			panelMain.Append(sliderWidth);
			topPos += sliderWidth.Height.Pixels;

			//線色
			label = new UIText("Color:");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Color:").Y;

			sliderColor = new UISliderColor();
			sliderColor.Left.Set(leftPos, 0f);
			sliderColor.Top.Set(topPos, 0f);
			panelMain.Append(sliderColor);
			topPos += sliderColor.Height.Pixels;

			//透過率
			label = new UIText("Transmittance:");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Transmittance:").Y;

			sliderTransmittance = new UISlider();
			sliderTransmittance.Left.Set(leftPos, 0f);
			sliderTransmittance.Top.Set(topPos, 0f);
			panelMain.Append(sliderTransmittance);
			topPos += sliderTransmittance.Height.Pixels;

			updateNeeded = true;
		}

		public float Transmittance
		{
			get
			{
				return 1f - sliderTransmittance.Value;
			}
		}

		public int LineWidth
		{
			get
			{
				return sliderWidth.ValueInt;
			}
		}

		public Color Color
		{
			get
			{
				return sliderColor.color;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (sliderTransmittance.IsMouseHovering || sliderTransmittance.isDown)
			{
				Tool.tooltip = $"Transmittance: {sliderTransmittance.Value * 100}%";
			}
			else if (sliderWidth.IsMouseHovering || sliderWidth.isDown)
			{
				Tool.tooltip = $"Line Width: {sliderWidth.ValueInt}";
			}
			else if (sliderColor.IsMouseHovering || sliderColor.isDown)
			{
				Color color = sliderColor.color;
				color.A = (byte)(255 * Transmittance);
				Tool.tooltip = $"Line Color: {color.ToString()}";
			}
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
