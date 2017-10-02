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
            panelMain.drawCaptionPosition = 1;
            panelMain.SetPadding(6);
            panelMain.Left.Set(240f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(160f, 0f);
			panelMain.MinWidth.Set(160f, 0f);
			panelMain.MaxWidth.Set(Main.screenWidth, 0f);
			panelMain.Height.Set(110, 0f);
			panelMain.MinHeight.Set(110, 0f);
			panelMain.MaxHeight.Set(Main.screenHeight, 0f);

			Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

            float topPos = menuMargin + Main.fontMouseText.MeasureString(caption).Y;
            float leftPos = 0;

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
            leftPos += menuMargin;
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
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatue7].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue8].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue9].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue4].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue5].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue6].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue1].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue2].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatue3].Resize(menuIconSize),
                },
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
            leftPos += menuMargin + menuIconSize;
            btnSnap.Left.Set(leftPos, 0f);
            btnSnap.Top.Set(topPos, 0f);
            panelMain.Append(btnSnap);

            topPos += menuIconSize + menuMargin;

            //ツールボックス
            panelTool = new UIPanel();
            panelTool.SetPadding(6);
            panelTool.Top.Set(topPos, 0f);
            panelTool.Width.Set(0, 1f);
            panelTool.Height.Set(- menuMargin - topPos, 1f);
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

            ////シェイプツール
            //texture = ModLoader.GetMod("TeraCAD").GetTexture("ToolUI/ShapeTools");
            //var btnToolLine = new UISlotTool(texture.Offset(0, 0, 28, 28), ToolType.Line);
            //var btnToolRect = new UISlotTool(texture.Offset(28, 0, 28, 28), ToolType.Rect);
            //var btnToolEllipse = new UISlotTool(texture.Offset(56, 0, 28, 28), ToolType.Ellipse);
            //btnToolLine.OnClick += (a, b) =>
            //{
            //    ToolBox.Select(btnToolLine);
            //};
            //btnToolRect.OnClick += (a, b) =>
            //{
            //    ToolBox.Select(btnToolRect);
            //};
            //btnToolEllipse.OnClick += (a, b) =>
            //{
            //    ToolBox.Select(btnToolEllipse);
            //};
            //gridTool.Add(btnToolLine);
            //gridTool.Add(btnToolRect);
            //gridTool.Add(btnToolEllipse);

            //スタンプツール、ドロッパーツール
            var slotStamp = new UISlotTool(Main.itemTexture[ItemID.Paintbrush].Resize(menuIconSize), ToolType.Stamp, "Currently, use is suspended due to license violation.");
            var slotDropper = new UISlotTool(Main.itemTexture[ItemID.EmptyDropper].Resize(menuIconSize), ToolType.Dropper, "Currently, use is suspended due to license violation.");
            slotStamp.OnClick += (a, b) =>
            {
                //ToolBox.Select(slotStamp);
            };
            slotDropper.OnClick += (a, b) =>
            {
                //ToolBox.Select(slotDropper);
            };
            gridTool.Add(slotStamp);
            gridTool.Add(slotDropper);

            updateNeeded = true;
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
        public UISlotTool(Texture2D texture, ToolType tool, string tooltip) : base (texture, tooltip)
        {
            Tool = tool;
        }

    }
}
