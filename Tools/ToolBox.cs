using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using TeraCAD.UIElements;
using TeraCAD.Tools;

namespace TeraCAD
{
    public enum ToolType
    {
        None,
        Select,
        Line,
        Rect,
        Circle,
        Ellipse,
        Arc,
        Image
    }

    public static class ToolTypeUtils
    {
        public static bool isShapeTool(this ToolType type)
        {
            bool result =
                type == ToolType.Select ||
                type == ToolType.Line ||
                type == ToolType.Rect ||
                type == ToolType.Circle ||
				type == ToolType.Image;
            return result;
        }
    }

    class ToolBox : Tool
	{
        public static ToolBox instance;
        private ToolBoxUI ui;
		private ToolImage toolImage;

        public FlyCam flyCam;
        private ToolShape toolShape;

        public static ToolType SelectedTool { get; set; }
        public static UISlotTool SelectedSlot { get; set; }
        public static SnapType snapType;
        public static int snapDistance = 8;

        public static bool FlyCam { get{ return ToolBox.instance.ui.btnFlyCam != null && ToolBox.instance.ui.btnFlyCam.GetValue<bool>(); } }
        public static bool InfinityRange { get { return ToolBox.instance.ui.btnRange != null && ToolBox.instance.ui.btnRange.GetValue<bool>(); } }

        public ToolBox() : base(typeof(ToolBoxUI))
		{
            instance = this;
			ui = uistate as ToolBoxUI;

			toolImage = new ToolImage();

			flyCam = new FlyCam();
            toolShape = new ToolShape();

            SelectedTool = ToolType.None;
        }

		internal static bool ContainsPoint(Vector2 point)
		{
			bool result = false;
			if (instance.visible)
			{
				result = instance.ui.panelMain.ContainsPoint(point);
			}
			if (!result && SelectedTool == ToolType.Image && ToolImage.instance.visible)
			{
				result = ImageUI.instance.panelMain.ContainsPoint(point);
			}
			return result;
		}

		internal static void Select(UISlotTool slot)
        {
            if (SelectedSlot == slot)
            {
                slot.isSelect = false;
                SelectedSlot = null;
                SelectedTool = ToolType.None;
            }
            else
            {
                if (SelectedSlot != null)
                {
                    SelectedSlot.isSelect = false;
                }
                SelectedTool = slot.Tool;
                SelectedSlot = slot;
                slot.isSelect = true;
            }
        }

        internal override void UIUpdate()
        {
            try
            {
                base.UIUpdate();
                flyCam.Update();

                if (SelectedTool.isShapeTool())
                {
                    toolShape.Update();
                }
				if (SelectedTool == ToolType.Image)
				{
					toolImage.UIUpdate();
				}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        internal override void UIDraw()
        {
            try
            {
                base.UIDraw();
				if (visible)
				{
					toolShape.DrawShapes();
				}
                if (SelectedTool.isShapeTool())
                {
                    toolShape.Draw();
                }
				if (SelectedTool == ToolType.Image)
				{
					toolImage.UIDraw();
				}
				if (ui.isDisplayRangeRectangle && !InfinityRange)
				{
					DrawRangeRectangle();
				}
			}
			catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

		private void DrawRangeRectangle()
		{
			try
			{
				Player player = Main.LocalPlayer;
				int rangeX = Player.tileRangeX - 1;
				int rangeY = Player.tileRangeY - 1;
				if (0 < player.selectedItem)
				{
					rangeX += player.inventory[player.selectedItem].tileBoost;
					rangeY += player.inventory[player.selectedItem].tileBoost;
				}
				Vector2 upperLeft = player.position.Offset(-rangeX * 16, -rangeY * 16).ToTileCoordinates().ToVector2();
				Vector2 lowerRight = player.position.Offset(player.width + (rangeX + 1) * 16, player.height + rangeY * 16).ToTileCoordinates().ToVector2();
				Vector2 upperLeftScreen = upperLeft * 16f;
				Vector2 lowerRightScreen = lowerRight * 16f;
				upperLeftScreen -= Main.screenPosition;
				lowerRightScreen -= Main.screenPosition;
				Vector2 brushSize = lowerRight - upperLeft;

				Rectangle value = new Rectangle(0, 0, 1, 1);
				Color color = Color.Yellow * 0.6f;
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * 16f * brushSize.X, new Microsoft.Xna.Framework.Rectangle?(value), color, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * 16f * brushSize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
			}
			catch { }
		}

		internal override void TooltipDraw()
        {
            base.TooltipDraw();
			if (SelectedTool == ToolType.Image)
			{
				toolImage.TooltipDraw();
			}
		}
    }
}
