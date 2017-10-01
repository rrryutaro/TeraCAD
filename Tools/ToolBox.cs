using System;
using Microsoft.Xna.Framework;
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
        Stamp,
        Dropper
    }

    public static class ToolTypeUtils
    {
        public static bool isShapeTool(this ToolType type)
        {
            bool result =
                type == ToolType.Line ||
                type == ToolType.Rect ||
                type == ToolType.Ellipse;
            return result;
        }
    }

    class ToolBox : Tool
	{
        public static ToolBox instance;
        private ToolBoxUI ui;

        public FlyCam flyCam;
        private StampTool toolStamp;
        private ToolShape toolShape;

        public static ToolType SelectedTool { get; set; }
        public static UISlotTool SelectedSlot { get; set; }
        public static SnapType snapType;

        public static bool FlyCam { get{ return ToolBox.instance.ui.btnFlyCam != null && ToolBox.instance.ui.btnFlyCam.GetValue<bool>(); } }
        public static bool InfinityRange { get { return ToolBox.instance.ui.btnRange != null && ToolBox.instance.ui.btnRange.GetValue<bool>(); } }

        public ToolBox() : base(typeof(ToolBoxUI))
		{
            instance = this;
            ui = uistate as ToolBoxUI;
            flyCam = new FlyCam();

            toolStamp = new StampTool();
            toolShape = new ToolShape();

            SelectedTool = ToolType.None;
        }

        public static void Select(UISlotTool slot)
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

            //スタンプツール
            StampTool.instance.visible = SelectedTool == ToolType.Dropper || SelectedTool == ToolType.Stamp;
        }

        internal override void UIUpdate()
        {
            try
            {
                base.UIUpdate();
                flyCam.Update();

                if (toolStamp.visible)
                {
                    toolStamp.UIUpdate();
                }
                if (SelectedTool.isShapeTool())
                {
                    toolShape.Update();
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
                if (toolStamp.visible)
                {
                    toolStamp.UIDraw();
                }
                if (SelectedTool.isShapeTool())
                {
                    toolShape.Draw();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        internal override void TooltipDraw()
        {
            base.TooltipDraw();
            if (toolStamp.visible)
            {
                toolStamp.TooltipDraw();
            }
        }

        internal static bool ContainsPoint(Vector2 point)
        {
            bool result =
                ToolBoxUI.instance.panelMain.ContainsPoint(point) ||
                ToolBoxUI.instance.panelMain.isDragOrResize ||
                StampUI.instance.panelMain.ContainsPoint(point) ||
                StampUI.instance.panelMain.isDragOrResize;

            return result;
        }
    }
}
