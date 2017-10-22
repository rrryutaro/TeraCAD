using System;
using System.Text;
using System.IO;
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
using TeraCAD.Tools;
using TeraCAD.Shapes;

namespace TeraCAD
{
    class ImageUI : UIModState
	{
		static internal ImageUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        internal UIDragablePanel panelMain;
        internal UIPanel panelGrid;
        internal UIGrid grid;
        internal UIHoverImageButton closeButton;
        internal UIImageListButton btnLoad;
		internal UIImageListButton btnPositionMode;
		internal UISlider slider;

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
				ToolImage.instance.visible = value;
			}
		}

		public ImageUI(UserInterface ui) : base(ui)
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
            panelMain.Left.Set(405f, 0f);
            panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(300f, 0f);
			panelMain.MinWidth.Set(300f, 0f);
			panelMain.MaxWidth.Set(Main.screenWidth, 0f);
			panelMain.Height.Set(210, 0f);
			panelMain.MinHeight.Set(210, 0f);
			panelMain.MaxHeight.Set(Main.screenHeight, 0f);

			Texture2D texture = ModLoader.GetMod("TeraCAD").GetTexture("UIElements/closeButton");
			closeButton = new UIHoverImageButton(texture, "Close");
			closeButton.OnClick += (a, b) => Show = false;
            closeButton.Left.Set(-20f, 1f);
			closeButton.Top.Set(6f, 0f);
			panelMain.Append(closeButton);

            float topPos = 0;
            float leftPos = menuMargin;

			//ボタン：ロード
			btnLoad = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatueL].Resize(menuIconSize)
                },
                new List<object>() { 0 },
                new List<string>() { "Load image" },
                0);
			var btn = btnLoad;
			btn.OnClick += (a, b) => LoadImage();
            leftPos += menuMargin;
			btn.Left.Set(leftPos, 0f);
			btn.Top.Set(topPos, 0f);
            panelMain.Append(btn);

			//ボタン：配置モード
			btnPositionMode = new UIImageListButton(
				new List<Texture2D>() {
					Main.itemTexture[ItemID.AlphabetStatueW].Resize(menuIconSize),
					Main.itemTexture[ItemID.AlphabetStatueS].Resize(menuIconSize)
				},
				new List<object>() { ImagePositionMode.World, ImagePositionMode.Screen },
				new List<string>() { "Image position: World", "Image position: Screen" },
				0);
			btn = btnPositionMode;
			btn.OnClick += (a, b) => btn.NextIamge();
			leftPos += menuIconSize + menuMargin;
			btn.Left.Set(leftPos, 0f);
			btn.Top.Set(topPos, 0f);
			panelMain.Append(btn);

			//ツールボックス
			panelGrid = new UIPanel();
			var panel = panelGrid;

			panel.SetPadding(6);
            panel.Top.Set(32, 0f);
            panel.Width.Set(0, 1f);
            panel.Height.Set(-40, 1f);
            panelMain.Append(panel);

            grid = new UIGrid();
            grid.Width.Set(-20f, 1f);
            grid.Height.Set(0, 1f);
            grid.ListPadding = 2f;
            panel.Append(grid);

            var scrollbar = new FixedUIScrollbar(userInterface);
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(0, 1f);
            scrollbar.Left.Set(-20, 1f);
            panel.Append(scrollbar);
            grid.SetScrollbar(scrollbar);

			slider = new UISlider();
			leftPos += menuIconSize + menuMargin;
			slider.Left.Set(leftPos, 0f);
			slider.Top.Set(topPos, 0f);
			panelMain.Append(slider);

            updateNeeded = true;
		}

		public ImagePositionMode ImagePositionMode
		{
			get
			{
				return btnPositionMode.GetValue<ImagePositionMode>();
			}
		}

		public float Transmittance
		{
			get
			{
				return 1f - slider.Value;
			}
		}

		private static string imageFilePath = $@"{Main.SavePath}\TeraCAD_Image.txt";
		private void LoadImage()
		{
			if (File.Exists(imageFilePath))
			{
				try
				{
					grid.Clear();
					foreach (var path in File.ReadAllLines(imageFilePath, Encoding.UTF8))
					{
						using (var fs = new FileStream(path, FileMode.Open))
						{
							var slot = new UISlotImage(Texture2D.FromStream(Main.graphics.GraphicsDevice, fs), grid.Count, fs.Name);
							grid._items.Add(slot);
							grid._innerList.Append(slot);
						}
					}
					grid.UpdateOrder();
					grid._innerList.Recalculate();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
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

			if (slider.IsMouseHovering || slider.isDown)
			{
				Tool.tooltip = $"Transmittance: {slider.Value * 100}%";
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

    public class UISlotImage : UISlot
    {
		public static UISlotImage SelectedImage;
		public Texture2D image;
		public Vector2 position;
		public ImagePositionMode mode;
		public float transmittance;
		public UISlotImage(Texture2D texture, int sortOrder, string tooltip) : base(texture.Resize(100), sortOrder, tooltip)
        {
			image = texture;
		}

		public new UISlotImage Clone()
		{
			UISlotImage result = new UISlotImage(image, sortOrder, tooltip);
			result.position = position;
			result.mode = mode;
			return result;
		}

		public override void Click(UIMouseEvent evt)
		{
			if (SelectedImage == this)
			{
				this.isSelect = false;
				SelectedImage = null;
				if (ToolBox.SelectedTool == ToolType.Image)
				{
					ToolShape.shape = null;
				}
			}
			else
			{
				if (SelectedImage != null)
					SelectedImage.isSelect = false;
				this.isSelect = true;
				SelectedImage = this;
				if (ToolBox.SelectedTool == ToolType.Image)
				{
					ToolShape.shape = new ShapeImage(this);
				}
			}
		}
	}

	public enum ImagePositionMode
	{
		World,
		Screen,
	}
}
