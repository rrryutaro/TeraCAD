using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TeraCAD.Tools;
using Terraria.ModLoader;

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
        Image,
        Eraser,
        AllClear,
        ParallelCopy
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
                type == ToolType.Ellipse ||
                type == ToolType.Image ||
                type == ToolType.Eraser ||
                type == ToolType.AllClear ||
                type == ToolType.ParallelCopy;

            return result;
        }

        public static bool isSelect(this ToolType type)
        {
            bool result =
                type == ToolType.Select ||
                type == ToolType.Eraser ||
                type == ToolType.ParallelCopy;
            return result;
        }
    }

    class ToolBox : Tool
    {
        public static ToolBox instance;
        private ToolBoxUI ui;
        private ToolLineProperty toolLineProperty;
        private ToolSetting toolSetting;
        private ToolImage toolImage;

        public FlyCam flyCam;
        private ToolShape toolShape;

        public static ToolType SelectedTool { get; set; }
        public static UISlotTool SelectedSlot { get; set; }
        public static SnapType snapType;
        public static int snapDistance = 8;

        public static bool FlyCam { get { return ToolBox.instance.ui.btnFlyCam != null && ToolBox.instance.ui.btnFlyCam.GetValue<bool>(); } }
        public static bool InfinityRange { get { return ToolBox.instance.ui.btnRange != null && ToolBox.instance.ui.btnRange.GetValue<bool>(); } }

        private static Color? backupMouseColor;
        private static Color? backupMouseBorderColor;

        public static Color LineColor
        {
            get
            {
                return LinePropertyUI.instance.GetColor(LinePropertyTarget.Shapes);
            }
        }

        public static int LineWidth
        {
            get
            {
                return LinePropertyUI.instance.GetLineWidth(LinePropertyTarget.Shapes);
            }
        }

        public ToolBox() : base(typeof(ToolBoxUI))
        {
            instance = this;
            ui = uistate as ToolBoxUI;
            toolLineProperty = new ToolLineProperty();
            toolSetting = new ToolSetting();
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
            if (!result && ToolLineProperty.instance.visible)
            {
                result = LinePropertyUI.instance.panelMain.ContainsPoint(point);
            }
            if (!result && SelectedTool == ToolType.Image && ToolImage.instance.visible)
            {
                result = ImageUI.instance.panelMain.ContainsPoint(point);
            }
            return result;
        }

        internal static void Select(UISlotTool slot)
        {
            ToolShape.Clear();
            if (UISlotImage.SelectedImage != null)
            {
                UISlotImage.SelectedImage.isSelect = false;
                UISlotImage.SelectedImage = null;
            }

            if (slot == null)
            {
                if (SelectedSlot != null)
                {
                    SelectedSlot.isSelect = false;
                    SelectedSlot = null;
                }
                SelectedTool = ToolType.None;
            }
            else if (SelectedSlot == slot)
            {
                SelectedSlot.isSelect = false;
                SelectedSlot = null;
                SelectedTool = ToolType.None;
            }
            else
            {
                if (SelectedSlot != null)
                {
                    SelectedSlot.isSelect = false;
                    SelectedSlot = null;
                }
                SelectedTool = slot.Tool;
                SelectedSlot = slot;
                SelectedSlot.isSelect = true;
            }

            if (SelectedTool == ToolType.None)
            {
                if (backupMouseColor != null)
                {
                    Main.mouseColor = (Color)backupMouseColor;
                    Main.MouseBorderColor = (Color)backupMouseBorderColor;
                    backupMouseColor = null;
                    backupMouseBorderColor = null;
                }
            }
            else
            {
                if (backupMouseColor == null && ModContent.GetInstance<TeraCADConfig>().isBorderCursorNone && !SelectedTool.isSelect())
                {
                    backupMouseColor = Main.mouseColor;
                    backupMouseBorderColor = Main.MouseBorderColor;
                    Main.MouseBorderColor = Color.Transparent;
                }
                else if (SelectedTool.isSelect() && backupMouseColor != null)
                {
                    Main.mouseColor = (Color)backupMouseColor;
                    Main.MouseBorderColor = (Color)backupMouseBorderColor;
                    backupMouseColor = null;
                    backupMouseBorderColor = null;
                }
            }

            if (SelectedTool == ToolType.ParallelCopy)
            {
                SettingUI.instance.Show = true;
            }
            else
            {
                SettingUI.instance.Show = false;
            }
        }

        internal override void UIUpdate()
        {
            try
            {
                base.UIUpdate();
                flyCam.Update();
                if (toolLineProperty.visible)
                {
                    toolLineProperty.UIUpdate();
                }
                if (toolSetting.visible)
                {
                    toolSetting.UIUpdate();
                }
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
                if (visible || ModContent.GetInstance<TeraCADConfig>().isDrawShpes)
                {
                    toolShape.DrawShapes();
                }
                if (SelectedTool.isShapeTool())
                {
                    toolShape.Draw();
                }
                if (ui.isDisplayRangeRectangle && !InfinityRange)
                {
                    DrawRangeRectangle();
                }
                if (SelectedTool == ToolType.Image)
                {
                    toolImage.UIDraw();
                }
                if (toolSetting.visible)
                {
                    toolSetting.UIDraw();
                }
                if (toolLineProperty.visible)
                {
                    toolLineProperty.UIDraw();
                }
                base.UIDraw();
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
