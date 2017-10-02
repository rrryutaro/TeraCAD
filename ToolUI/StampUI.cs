using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using TeraCAD.UIElements;

namespace TeraCAD
{
    class StampUI : UIModState
    {
        static internal StampUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        internal UIDragablePanel panelMain;
        internal UISplitterPanel panelSplitter;
        internal UIPanel panelStamp;
        internal UIPanel panelMaterial;
        internal UIGrid gridStamp;
        internal UIGrid gridMaterial;
        internal UIHoverImageButton btnClose;

        internal bool updateNeeded;
        internal string caption = $"Stamp Tool Count:??";

        public UIImageListButton btnHorizontalReversal;
        public UIImageListButton btnVerticalReversal;
        public bool isVerticalReversal { get { return btnVerticalReversal.GetValue<bool>(); } }
        public bool isHorizontalReversal { get { return btnHorizontalReversal.GetValue<bool>(); } }

        public StampUI(UserInterface ui) : base(ui)
        {
            instance = this;
        }

        public void InitializeUI()
        {
            RemoveAllChildren();

            //メインパネル
            panelMain = new UIDragablePanel(true, true, true);
            panelMain.caption = caption;
            panelMain.SetPadding(6);
            panelMain.Left.Set(400f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(800f, 0f);
            panelMain.MinWidth.Set(314f, 0f);
            panelMain.MaxWidth.Set(1393f, 0f);
            panelMain.Height.Set(400f, 0f);
            panelMain.MinHeight.Set(116f, 0f);
            panelMain.MaxHeight.Set(1000f, 0f);
            Append(panelMain);

            //スタンプパネル
            panelStamp = new UIPanel();
            panelStamp.SetPadding(6);
            panelStamp.MinWidth.Set(600, 0);
            gridStamp = new UIGrid();
            gridStamp.Width.Set(-20f, 1f);
            gridStamp.Height.Set(0, 1f);
            gridStamp.ListPadding = 2f;
            panelStamp.Append(gridStamp);
            var scrollbar = new FixedUIScrollbar(userInterface);
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.Left.Set(-20, 1f);
            panelStamp.Append(scrollbar);
            gridStamp.SetScrollbar(scrollbar);
            //素材パネル
            panelMaterial = new UIPanel();
            panelMaterial.SetPadding(6);
            panelMaterial.MinWidth.Set(100, 0);
            gridMaterial = new UIGrid();
            gridMaterial.Width.Set(-20f, 1f);
            gridMaterial.Height.Set(0, 1f);
            gridMaterial.ListPadding = 2f;
            panelMaterial.Append(gridMaterial);
            scrollbar = new FixedUIScrollbar(userInterface);
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.Left.Set(-20, 1f);
            panelMaterial.Append(scrollbar);
            gridMaterial.SetScrollbar(scrollbar);
            //スプリッターパネル
            panelSplitter = new UISplitterPanel(panelStamp, panelMaterial);
            panelSplitter.SetPadding(0);
            panelSplitter.Top.Pixels = menuIconSize + menuMargin * 2;
            panelSplitter.Width.Set(0, 1f);
            panelSplitter.Height.Set(-26 - menuIconSize, 1f);
            panelSplitter.Panel1Visible = true;
            panelMain.Append(panelSplitter);

            //閉じるボタン
            Texture2D texture = TeraCAD.instance.GetTexture("UIElements/closeButton");
            btnClose = new UIHoverImageButton(texture, "Close");
            btnClose.OnClick += (a, b) => StampTool.instance.visible = false;
            btnClose.Left.Set(-20f, 1f);
            btnClose.Top.Set(3f, 0f);
            panelMain.Append(btnClose);

            float topPos = 3;
            float leftPos = menuMargin;

            //ボタン：左右反転
            btnHorizontalReversal = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatueH].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatueH].Resize(menuIconSize),
                },
                new List<object>() { true, false },
                new List<string>() { "Horizontal reversal: On", "Horizontal reversal: Off" },
                1);
            btnHorizontalReversal.OnClick += (a, b) =>
            {
                btnHorizontalReversal.NextIamge();
                btnHorizontalReversal.visibilityActive = btnHorizontalReversal.visibilityInactive = btnHorizontalReversal.GetValue<bool>() ? 1.0f : 0.4f;
            };
            btnHorizontalReversal.Left.Set(leftPos, 0f);
            btnHorizontalReversal.Top.Set(topPos, 0f);
            btnHorizontalReversal.visibilityActive = 0.4f;
            panelMain.Append(btnHorizontalReversal);

            //ボタン：上下反転
            leftPos += menuIconSize + menuMargin;
            btnVerticalReversal = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatueV].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatueV].Resize(menuIconSize),
                },
                new List<object>() { true, false },
                new List<string>() { "Vertical reversal: On", "Vertical reversal: Off" },
                1);
            btnVerticalReversal.OnClick += (a, b) =>
            {
                btnVerticalReversal.NextIamge();
                btnVerticalReversal.visibilityActive = btnVerticalReversal.visibilityInactive = btnVerticalReversal.GetValue<bool>() ? 1.0f : 0.4f;
            };
            btnVerticalReversal.Left.Set(leftPos, 0f);
            btnVerticalReversal.Top.Set(topPos, 0f);
            btnVerticalReversal.visibilityActive = 0.4f;
            panelMain.Append(btnVerticalReversal);

            //ボタン：ゴミ箱
            leftPos += (menuIconSize + menuMargin) * 2;
            var btn = new UIImageListButton(
                new List<Texture2D>() { Main.trashTexture.Resize(menuIconSize) },
                new List<object>() { 0 },
                new List<string>() { "Delete" },
                0);
            btn.OnClick += (a, b) =>
            {
                if (UISlotStamp.CurrentSelect != null)
                {
                    gridStamp.Remove(UISlotStamp.CurrentSelect);
                    UISlotStamp.CurrentSelect = null;
                    ToolStamp.stampInfo = null;
                }
            };
            btn.Left.Set(leftPos, 0f);
            btn.Top.Set(topPos, 0f);
            panelMain.Append(btn);

            //ボタン：インポート
            leftPos += (menuIconSize + menuMargin) * 2;
            btn = new UIImageListButton(
                new List<Texture2D>() { Main.itemTexture[ItemID.AlphabetStatueI].Resize(menuIconSize) },
                new List<object>() { 0 },
                new List<string>() { "Import" },
                0);
            btn.OnClick += (a, b) =>
            {
                ToolDropper.Import();
            };
            btn.Left.Set(leftPos, 0f);
            btn.Top.Set(topPos, 0f);
            panelMain.Append(btn);

            //ボタン：インポート
            leftPos += (menuIconSize + menuMargin) * 1;
            btn = new UIImageListButton(
                new List<Texture2D>() { Main.itemTexture[ItemID.AlphabetStatueE].Resize(menuIconSize) },
                new List<object>() { 0 },
                new List<string>() { "Export" },
                0);
            btn.OnClick += (a, b) =>
            {
                ToolDropper.Export();
            };
            btn.Left.Set(leftPos, 0f);
            btn.Top.Set(topPos, 0f);
            panelMain.Append(btn);
        }

        internal void UpdateGrid()
        {
            if (!updateNeeded) { return; }
            updateNeeded = false;
            panelMain.caption = caption.Replace("??", $"{gridStamp.Count}");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateGrid();

            Main.instance.textBlinkerState = 0;
            Main.instance.textBlinkerCount = 0;

        }

        internal void AddStamp(StampInfo stampInfo)
        {
            var slot = new UISlotStamp(stampInfo, gridStamp.Count);
            gridStamp.Add(slot);
            updateNeeded = true;
        }
    }

    class UISlotStamp : UISlot
    {
        public static UISlotStamp CurrentSelect;
        public StampInfo stampInfo;
        public int sortOrder;

        public UISlotStamp(StampInfo stampInfo, int sortOrder) : base(stampInfo.Textures.Resize(100), string.Empty)
        {
            this.stampInfo = stampInfo;
            this.sortOrder = sortOrder;
        }
        public override void Click(UIMouseEvent evt)
        {
            if (evt.Target == this)
            {
                if (CurrentSelect != null)
                {
                    CurrentSelect.isSelect = false;
                }

                if (CurrentSelect == this)
                {
                    CurrentSelect = null;
                    ToolStamp.stampInfo = null;
                }
                else
                {
                    isSelect = true;
                    CurrentSelect = this;
                    ToolStamp.stampInfo = stampInfo;
                }
            }
        }
        public override int CompareTo(object obj)
        {
            int result = sortOrder < (obj as UISlotStamp).sortOrder ? 1 : -1;
            return result;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            effects = SpriteEffects.None;
            if (StampUI.instance.isHorizontalReversal)
                effects |= SpriteEffects.FlipHorizontally;
            if (StampUI.instance.isVerticalReversal)
                effects |= SpriteEffects.FlipVertically;

            base.DrawSelf(spriteBatch);
        }
    }
}