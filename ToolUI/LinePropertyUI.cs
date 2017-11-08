using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TeraCAD.UIElements;
using TeraCAD.Tools;

namespace TeraCAD
{
	enum LinePropertyTarget
	{
		Shapes,
		Cursor,
		CursorSnap
	};

	class LinePropertyUI : UIModState
	{
		static internal LinePropertyUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

		static internal LinePropertyTarget linePropertyTarget;

		internal UIDragablePanel panelMain;
        internal UIHoverImageButton closeButton;
		internal UISlider sliderTransmittance;
		internal UISlider sliderWidth;
		internal UISliderColor sliderColor;

		internal bool updateNeeded;

        internal string caption = $"";

		private UIText[] labelConfig;

		private int[] lineWidth = { 1, 4, 4 };
		private Color[] lineColor = { Color.Red, Color.Blue, Color.Green };
		private float[] lineTransmittance = { 0f, 0f, 0f };

		private Color labelColorSelected = Color.White * 0.8f;
		private Color labelColorActive = Color.White;
		private Color labelColorInactive = Color.White * 0.3f;

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
				ToolLineProperty.instance.visible = value;
			}
		}

		public LinePropertyUI(UserInterface ui) : base(ui)
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

			Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

            float topPos = 0;
            float leftPos = menuMargin;

			//キャプション
			var label = new UIText("Line Property");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Line Property").Y;

			labelConfig = new UIText[3];
			//設定対象：図形
			labelConfig[0] = new UIText("Shapes");
			labelConfig[0].TextColor = labelColorSelected;
			labelConfig[0].Left.Set(leftPos, 0f);
			labelConfig[0].Top.Set(topPos, 0f);
			labelConfig[0].OnMouseOver += Label_OnMouseOver;
			labelConfig[0].OnMouseOut += Label_OnMouseOut;
			labelConfig[0].OnClick += (a, b) => ChangeConfig(LinePropertyTarget.Shapes);
			panelMain.Append(labelConfig[0]);
			leftPos += Main.fontMouseText.MeasureString("Shapes").X + menuMargin;

			//設定対象：カーソル
			labelConfig[1] = new UIText("Cursor");
			labelConfig[1].TextColor = labelColorInactive;
			labelConfig[1].Left.Set(leftPos, 0f);
			labelConfig[1].Top.Set(topPos, 0f);
			labelConfig[1].OnMouseOver += Label_OnMouseOver;
			labelConfig[1].OnMouseOut += Label_OnMouseOut;
			labelConfig[1].OnClick += (a, b) => ChangeConfig(LinePropertyTarget.Cursor);
			panelMain.Append(labelConfig[1]);
			leftPos += Main.fontMouseText.MeasureString("Cursor").X + menuMargin;

			//設定対象：カーソルスナップ
			labelConfig[2] = new UIText("Snap");
			labelConfig[2].TextColor = labelColorInactive;
			labelConfig[2].Left.Set(leftPos, 0f);
			labelConfig[2].Top.Set(topPos, 0f);
			labelConfig[2].OnMouseOver += Label_OnMouseOver;
			labelConfig[2].OnMouseOut += Label_OnMouseOut;
			labelConfig[2].OnClick += (a, b) => ChangeConfig(LinePropertyTarget.CursorSnap);
			panelMain.Append(labelConfig[2]);
			leftPos += Main.fontMouseText.MeasureString("Snap").X + menuMargin;

			leftPos = menuMargin;
			topPos += Main.fontMouseText.MeasureString("Shapes").Y * 2;

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
			sliderWidth.OnChanged += SliderWidth_OnChanged;
			panelMain.Append(sliderWidth);
			topPos += sliderWidth.Height.Pixels;

			//線色
			label = new UIText("Line color:");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Line color:").Y;

			sliderColor = new UISliderColor();
			sliderColor.Left.Set(leftPos, 0f);
			sliderColor.Top.Set(topPos, 0f);
			sliderColor.OnChanged += SliderColor_OnChanged;
			panelMain.Append(sliderColor);
			topPos += sliderColor.Height.Pixels;

			//透過率
			label = new UIText("Color transmittance:");
			label.Left.Set(leftPos, 0f);
			label.Top.Set(topPos, 0f);
			panelMain.Append(label);
			panelMain.AddDragTarget(label);
			topPos += Main.fontMouseText.MeasureString("Color transmittance:").Y;

			sliderTransmittance = new UISlider();
			sliderTransmittance.Left.Set(leftPos, 0f);
			sliderTransmittance.Top.Set(topPos, 0f);
			sliderTransmittance.OnChanged += SliderTransmittance_OnChanged;
			panelMain.Append(sliderTransmittance);
			topPos += sliderTransmittance.Height.Pixels * 2;

			panelMain.MinHeight.Set(topPos, 0f);
			panelMain.Height.Set(topPos, 0f);

			updateNeeded = true;
		}

		private void SliderWidth_OnChanged(UISlider obj)
		{
			lineWidth[(int)linePropertyTarget] = sliderWidth.ValueInt;

			int width = GetLineWidth(linePropertyTarget);
			switch (linePropertyTarget)
			{
				case LinePropertyTarget.Shapes:
					if (ToolShape.shape != null)
						ToolShape.shape.width = width;
					break;
				case LinePropertyTarget.Cursor:
					if (ToolShape.cursorShape != null)
						ToolShape.cursorShape.width = width;
					break;
				case LinePropertyTarget.CursorSnap:
					if (ToolShape.cursorSnapShape != null)
						ToolShape.cursorSnapShape.width = width;
					break;
			}
		}

		private void SliderColor_OnChanged(UISliderColor obj)
		{
			lineColor[(int)linePropertyTarget] = sliderColor.color;

			SetConfigTargetColor();
		}

		private void SliderTransmittance_OnChanged(UISlider obj)
		{
			lineTransmittance[(int)linePropertyTarget] = sliderTransmittance.Value;

			SetConfigTargetColor();
		}

		private void SetConfigTargetColor()
		{
			Color color = GetColor(linePropertyTarget);
			switch (linePropertyTarget)
			{
				case LinePropertyTarget.Shapes:
					if (ToolShape.shape != null)
						ToolShape.shape.color = color;
					break;
				case LinePropertyTarget.Cursor:
					if (ToolShape.cursorShape != null)
						ToolShape.cursorShape.color = color;
					break;
				case LinePropertyTarget.CursorSnap:
					if (ToolShape.cursorSnapShape != null)
						ToolShape.cursorSnapShape.color = color;
					break;
			}
		}

		private void Label_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target is UIText)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				(evt.Target as UIText).TextColor = labelColorActive;
			}
		}

		private void Label_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target is UIText)
			{
				if (evt.Target == labelConfig[(int)linePropertyTarget])
					(evt.Target as UIText).TextColor = labelColorSelected;
				else
					(evt.Target as UIText).TextColor = labelColorInactive;
			}
		}

		private void ChangeConfig(LinePropertyTarget target)
		{
			if (linePropertyTarget != target)
			{
				int index = (int)linePropertyTarget;
				lineWidth[index] = sliderWidth.ValueInt;
				lineColor[index] = sliderColor.color;
				lineTransmittance[index] = sliderTransmittance.Value;
				labelConfig[index].TextColor = labelColorInactive;

				index = (int)target;
				sliderWidth.ValueInt = lineWidth[index];
				sliderColor.color = lineColor[index];
				sliderTransmittance.Value = lineTransmittance[index];
				labelConfig[index].TextColor = labelColorSelected;

				linePropertyTarget = target;
			}
		}

		public int GetLineWidth(LinePropertyTarget target)
		{
			int result = lineWidth[(int)target];
			return result;
		}

		public Color GetColor(LinePropertyTarget target)
		{
			Color result = lineColor[(int)target] * (1f - lineTransmittance[(int)target]);
			return result;
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
				color.A = (byte)(255 * (1f - sliderTransmittance.Value));
				Tool.tooltip = $"Line Color: {color.ToString()}";
			}
			else if (labelConfig[0].IsMouseHovering)
			{
				Tool.tooltip = "Shapes config";
			}
			else if (labelConfig[1].IsMouseHovering)
			{
				Tool.tooltip = "TeraCAD cursor config";
			}
			else if (labelConfig[2].IsMouseHovering)
			{
				Tool.tooltip = "TeraCAD cursor snap config";
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
