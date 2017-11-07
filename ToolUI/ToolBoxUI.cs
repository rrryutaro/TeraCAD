using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TeraCAD.UIElements;

namespace TeraCAD
{
    class ToolBoxUI : UIModState
	{
		static internal ToolBoxUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        internal UIDragablePanel panelMain;
        internal UIPanel panelTool;
        internal UIGrid gridTool;
        internal UIHoverImageButton closeButton;
		internal UIImageListButton btnRangeRect;
		internal UIImageListButton btnFlyCam;
        internal UIImageListButton btnRange;
        internal UIImageListButton btnSnap;
        internal UIImageListButton btnPaint;

        internal bool updateNeeded;

        internal string caption = $"TeraCAD v{TeraCAD.instance.Version}";

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
                TeraCAD.instance.toolMain.visible = value;
			}
		}

		public ToolBoxUI(UserInterface ui) : base(ui)
		{
			instance = this;
		}

        public void InitializeUI()
        {
            RemoveAllChildren();

            panelMain = new UIDragablePanel(true, true, true);
            panelMain.caption = caption;
            panelMain.drawCaptionPosition = 0;
            panelMain.SetPadding(6);
            panelMain.Left.Set(240f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(196f, 0f);
			panelMain.MinWidth.Set(196f, 0f);
			panelMain.MaxWidth.Set(Main.screenWidth, 0f);
			panelMain.Height.Set(142, 0f);
			panelMain.MinHeight.Set(110, 0f);
			panelMain.MaxHeight.Set(Main.screenHeight, 0f);

			Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
			//closeButton.Top.Set(6f, 0f);
			closeButton.Top.Set(0f, 0f);
			panelMain.Append(closeButton);

			//float topPos = menuMargin + Main.fontMouseText.MeasureString(caption).Y;
			float topPos = 0;
			float leftPos = 0;

			//ボタン：レンジ矩形
			texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/rangeRectangle");
			btnRangeRect = new UIImageListButton(
				new List<Texture2D>() { texture, texture },
				new List<object>() { true, false },
				new List<string>() { "Display range rectangle: On", "Display range rectangle: Off" },
				1);
			btnRangeRect.OnClick += (a, b) =>
			{
				btnRangeRect.NextIamge();
				btnRangeRect.visibilityActive = btnRangeRect.visibilityInactive = btnRangeRect.GetValue<bool>() ? 1.0f : 0.4f;
			};
			leftPos += menuMargin;
			btnRangeRect.Left.Set(leftPos, 0f);
			btnRangeRect.Top.Set(topPos, 0f);
			panelMain.Append(btnRangeRect);

			//ボタン：フライカメラ
			btnFlyCam = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AngelWings].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AngelWings].Resize(menuIconSize),
                },
                new List<object>() { true, false },
                new List<string>() { "Fly cam: On", "Fly cam: Off" },
                1);
            btnFlyCam.OnClick += (a, b) =>
            {
                btnFlyCam.NextIamge();
                FlyCam.Enabled = btnFlyCam.GetValue<bool>();
                btnFlyCam.visibilityActive = btnFlyCam.visibilityInactive = btnFlyCam.GetValue<bool>() ? 1.0f : 0.4f;
            };
			leftPos += menuMargin + menuIconSize;
			btnFlyCam.Left.Set(leftPos, 0f);
            btnFlyCam.Top.Set(topPos, 0f);
            panelMain.Append(btnFlyCam);

            //ボタン：レンジ
            btnRange = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.Toolbelt].Resize(menuIconSize),
                    Main.itemTexture[ItemID.Toolbelt].Resize(menuIconSize),
                },
                new List<object>() { true, false },
                new List<string>() { "Infinity range: On", "Infinity range: Off" },
                1);
            btnRange.OnClick += (a, b) =>
            {
                btnRange.NextIamge();
                btnRange.visibilityActive = btnRange.visibilityInactive = btnRange.GetValue<bool>() ? 1.0f : 0.4f;
            };
            leftPos += menuMargin + menuIconSize;
            btnRange.Left.Set(leftPos, 0f);
            btnRange.Top.Set(topPos, 0f);
            panelMain.Append(btnRange);

            //ボタン：スナップ
            btnSnap = new UIImageListButton(
                (new ImageList(ModLoader.GetMod("TeraCAD").GetTexture("ToolUI/Snap"), 28, 28)).listTexture,
                new List<object>() {
                    SnapType.TopLeft,
                    SnapType.TopCenter,
                    SnapType.TopRight,
                    SnapType.LeftCenter,
                    SnapType.Center,
                    SnapType.RightCenter,
                    SnapType.BottomLeft,
                    SnapType.BottomCenter,
                    SnapType.BottomRight,
                },
                new List<string>() {
                    "Snap: TopLeft",
                    "Snap: TopCenter",
                    "Snap: TopRight",
                    "Snap: LeftCenter",
                    "Snap: Center",
                    "Snap: RightCenter",
                    "Snap: BottomLeft",
                    "Snap: BottomCenter",
                    "Snap: BottomRight",
                },
                0);
            btnSnap.OnClick += (a, b) =>
            {
                btnSnap.NextIamge();
                ToolBox.snapType = btnSnap.GetValue<SnapType>();
            };
            btnSnap.OnRightClick += (a, b) =>
            {
                btnSnap.PrevIamge();
                ToolBox.snapType = btnSnap.GetValue<SnapType>();
            };
            leftPos += menuMargin + menuIconSize;
            btnSnap.Left.Set(leftPos, 0f);
            btnSnap.Top.Set(topPos, 0f);
            panelMain.Append(btnSnap);

			//ボタン：コンフィグ
			var btnConfig = new UIImageListButton(
				new List<Texture2D>() {
					Main.itemTexture[ItemID.Cog].Resize(menuIconSize),
				},
				new List<object>() { 0 },
				new List<string>() { "Show Config" },
				0);
			btnConfig.OnClick += (a, b) => ConfigUI.instance.Show = !ConfigUI.instance.Show;
			leftPos += menuMargin + menuIconSize;
			btnConfig.Left.Set(leftPos, 0f);
			btnConfig.Top.Set(topPos, 0f);
			panelMain.Append(btnConfig);

			topPos += menuIconSize + menuMargin;

            //ツールボックス
            panelTool = new UIPanel();
            panelTool.SetPadding(6);
            panelTool.Top.Set(topPos, 0f);
            panelTool.Width.Set(0, 1f);
            panelTool.Height.Set(-52, 1f);
            panelMain.Append(panelTool);

            gridTool = new UIGrid();
            gridTool.Width.Set(-20f, 1f);
            gridTool.Height.Set(0, 1f);
            gridTool.ListPadding = 2f;
            panelTool.Append(gridTool);

            var scrollbar = new FixedUIScrollbar(userInterface);
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.Left.Set(-20, 1f);
            panelTool.Append(scrollbar);
            gridTool.SetScrollbar(scrollbar);

            //シェイプツール
            var imageList = new ImageList(ModLoader.GetMod("TeraCAD").GetTexture("ToolUI/ShapeTools"), 28, 28);

			//選択ツール
			var btnSelect = new UISlotTool(imageList[0], ToolType.Select, gridTool.Count, "Selecct");
			btnSelect.OnClick += (a, b) => ToolBox.Select(btnSelect);
			gridTool.Add(btnSelect);
			//直線ツール
			var btnLine = new UISlotTool(imageList[1], ToolType.Line, gridTool.Count, "Line");
			btnLine.OnClick += (a, b) => ToolBox.Select(btnLine);
			gridTool.Add(btnLine);
			//矩形ツール
			var btnRect = new UISlotTool(imageList[2], ToolType.Rect, gridTool.Count, "Recangle");
			btnRect.OnClick += (a, b) => ToolBox.Select(btnRect);
			gridTool.Add(btnRect);
			//円ツール
			var btnCircle = new UISlotTool(imageList[3], ToolType.Circle, gridTool.Count, "Circle");
			btnCircle.OnClick += (a, b) => ToolBox.Select(btnCircle);
			gridTool.Add(btnCircle);
			//楕円ツール
			//var btnEllipse = new UISlotTool(imageList[4], ToolType.Ellipse, gridTool.Count, "Ellipse");
			//btnEllipse.OnClick += (a, b) => ToolBox.Select(btnEllipse);
			//gridTool.Add(btnEllipse);
			//イメージツール
			var btnImage = new UISlotTool(Main.itemTexture[ItemID.TheCursedMan], ToolType.Image, gridTool.Count, "Image");
			btnImage.OnClick += (a, b) =>
			{
				ToolBox.Select(btnImage);
				ImageUI.instance.Show = btnImage.isSelect;
			};
			gridTool.Add(btnImage);
			//削除ツール
			var btnEraser = new UISlotTool(imageList[5], ToolType.Eraser, gridTool.Count, "Eraser");
			btnEraser.OnClick += (a, b) => ToolBox.Select(btnEraser);
			gridTool.Add(btnEraser);

			updateNeeded = true;
		}

		internal bool isDisplayRangeRectangle
		{
			get
			{
				return btnRangeRect.GetValue<bool>();
			}
		}

		internal void UpdateGrid()
		{
			if (!updateNeeded) { return; }
			updateNeeded = false;
        }

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
            UpdateGrid();
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

    public class UISlotTool : UISlot
    {
        public ToolType Tool;
        public UISlotTool(Texture2D texture, ToolType tool, int sortOrder, string tooltip) : base(texture, sortOrder, tooltip)
        {
            Tool = tool;
        }
    }
}
