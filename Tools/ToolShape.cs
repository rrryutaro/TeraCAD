using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TeraCAD.Shapes;
using Terraria.ModLoader;

namespace TeraCAD.Tools
{
    public class ToolShape
    {
        public static Shape shape;
        public static ShapeRect cursorShape;
        public static ShapeRect cursorSnapShape;
        public static List<Shape> hoverShapes = new List<Shape>();
        public static List<Shape> selectedShapes = new List<Shape>();
        public static Shape parallelCopyTargetShape;
        public static List<Shape> parallelCopyShapes = new List<Shape>();
        public static int step = 0;
        public List<Shape> list = new List<Shape>();

        public static void Clear()
        {
            step = 0;
            shape = null;
            hoverShapes.Clear();
            selectedShapes.Clear();
            parallelCopyTargetShape = null;
            parallelCopyShapes.Clear();
        }

        public void UpdateCursor()
        {
            if (cursorShape == null || cursorSnapShape == null)
            {
                cursorShape = new ShapeRect();
                cursorSnapShape = new ShapeRect();
                cursorShape.width = LinePropertyUI.instance.GetLineWidth(LinePropertyTarget.Cursor);
                cursorShape.color = LinePropertyUI.instance.GetColor(LinePropertyTarget.Cursor);
                cursorSnapShape.width = LinePropertyUI.instance.GetLineWidth(LinePropertyTarget.CursorSnap);
                cursorSnapShape.color = LinePropertyUI.instance.GetColor(LinePropertyTarget.CursorSnap);
            }

            if (ModContent.GetInstance<TeraCADConfig>().isDrawCursor)
            {
                cursorShape.pointStart = Main.MouseWorld.ToTileCoordinates().ToVector2() * 16;
                cursorShape.pointEnd = cursorShape.pointStart.Offset(ModUtils.tileSize, ModUtils.tileSize);
            }

            if (ModContent.GetInstance<TeraCADConfig>().isDrawCursorSnap)
            {
                cursorSnapShape.pointStart = Snap.GetSnapPoint(ToolBox.snapType).Offset(-2, -2);
                cursorSnapShape.pointEnd = cursorSnapShape.pointStart.Offset(4, 4);
            }
        }

        public void Update()
        {
            UpdateCursor();

            if (ToolBox.ContainsPoint(Main.MouseScreen) || Main.gameMenu)
                return;

            if (ToolBox.SelectedTool == ToolType.Select || ToolBox.SelectedTool == ToolType.Eraser)
            {
                UpdateSelectTool();
                return;
            }
            if (ToolBox.SelectedTool == ToolType.ParallelCopy)
            {
                UpdateSelectTool();
            }

            if (Main.mouseRight)
            {
                step = 0;
                shape = null;
                parallelCopyTargetShape = null;
                parallelCopyShapes.Clear();
            }

            switch (step)
            {
                case 0:
                    if (Main.mouseLeft)
                    {
                        switch (ToolBox.SelectedTool)
                        {
                            case ToolType.Line:
                                shape = new ShapeLine();
                                shape.pointStart = shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                                step = 1;
                                break;

                            case ToolType.Rect:
                                shape = new ShapeRect();
                                shape.pointStart = shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                                step = 1;
                                break;

                            case ToolType.Circle:
                                shape = new ShapeCircle();
                                shape.pointStart = shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                                step = 1;
                                break;

                            case ToolType.Image:
                                if (UISlotImage.SelectedImage != null)
                                {
                                    list.Add(shape.Clone());
                                    step = 1;
                                }
                                break;

                            case ToolType.AllClear:
                                list.Clear();
                                break;

                            case ToolType.ParallelCopy:
                                if (0 < hoverShapes.Count)
                                {
                                    parallelCopyTargetShape = hoverShapes[hoverShapes.Count - 1];
                                    step = 1;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (ToolBox.SelectedTool == ToolType.Image)
                        {
                            (shape as ShapeImage)?.SetData();
                        }
                    }
                    break;

                case 1:
                    if (ToolBox.SelectedTool == ToolType.Image)
                    {
                        if (!Main.mouseLeft)
                            step = 0;
                    }
                    else if (ToolBox.SelectedTool == ToolType.ParallelCopy)
                    {
                        if (!Main.mouseLeft)
                            step = 2;
                    }
                    else
                    {
                        shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                        if (!Main.mouseLeft)
                            step = 2;
                    }
                    break;

                case 2:
                    if (ToolBox.SelectedTool == ToolType.ParallelCopy)
                    {
                        ParallelCopy();
                        if (Main.mouseLeft && 0 < parallelCopyShapes.Count)
                        {
                            list.AddRange(parallelCopyShapes);
                            parallelCopyTargetShape = null;
                            parallelCopyShapes.Clear();
                            step = 3;
                        }
                    }
                    else
                    {
                        shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                        if (Main.mouseLeft)
                        {
                            shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                            list.Add(shape);
                            step = 3;
                        }
                    }
                    break;

                case 3:
                    if (!Main.mouseLeft)
                    {
                        step = 0;
                    }
                    break;
            }
        }

        private void ParallelCopy()
        {
            parallelCopyShapes.Clear();
            if (parallelCopyTargetShape != null)
            {
                Vector2 pos = Main.MouseWorld;
                Vector2 offset = Vector2.Zero;
                switch (parallelCopyTargetShape.type)
                {
                    case ShapeType.Line:
                        ShapeLine line = parallelCopyTargetShape as ShapeLine;

                        int direction = pos.GetDirection(line.pointStart, line.pointEnd);
                        if (0 < direction)
                        {
                            float radian = 0;
                            if (direction == 1)
                                radian = (-90f).ToRadian();
                            else if (direction == 2)
                                radian = (90f).ToRadian();

                            float rotation = line.pointStart.GetRadian(line.pointEnd);
                            int distance = SettingUI.instance.ParallelCopy_Distance * ModUtils.tileSize;
                            var cloneLine = line.Clone();
                            for (int i = 1; i <= SettingUI.instance.ParallelCopy_Count; i++)
                            {
                                cloneLine.pointStart = cloneLine.pointStart.ToRotationVector(distance, rotation - radian, true);
                                cloneLine.pointEnd = cloneLine.pointEnd.ToRotationVector(distance, rotation - radian, true);
                                parallelCopyShapes.Add(cloneLine.Clone());
                            }
                        }
                        break;

                    case ShapeType.Rect:
                    case ShapeType.Circle:
                    case ShapeType.Image:
                        switch (parallelCopyTargetShape.GetRect().Center().GetDirection(pos))
                        {
                            case 1: offset.X = -SettingUI.instance.ParallelCopy_Distance * ModUtils.tileSize; break;
                            case 2: offset.Y = -SettingUI.instance.ParallelCopy_Distance * ModUtils.tileSize; break;
                            case 3: offset.X = SettingUI.instance.ParallelCopy_Distance * ModUtils.tileSize; break;
                            case 4: offset.Y = SettingUI.instance.ParallelCopy_Distance * ModUtils.tileSize; break;
                        }
                        var clone = parallelCopyTargetShape.Clone();
                        for (int i = 1; i <= SettingUI.instance.ParallelCopy_Count; i++)
                        {
                            clone.pointStart += offset;
                            clone.pointEnd += offset;
                            parallelCopyShapes.Add(clone.Clone());
                        }
                        break;
                }
            }
        }

        private void UpdateSelectTool()
        {
            hoverShapes.Clear();
            hoverShapes.AddRange(GetNearShape(Main.MouseWorld, ToolBox.snapDistance));
            if (ToolBox.SelectedTool == ToolType.Eraser)
            {
                if (Main.mouseLeft)
                {
                    foreach (var hoverShape in hoverShapes)
                    {
                        list.Remove(hoverShape);
                    }
                }
            }

            //foreach (var shape in GetNearShape(Main.MouseWorld, ToolBox.snapDistance))
            //{
            //	var shapeClone = shape.Clone();
            //	shapeClone.width += 4;
            //	shapeClone.color = Color.Yellow * 0.6f;
            //	hoverShapes.Add(shapeClone);
            //}
        }

        public List<Shape> GetNearShape(Vector2 point, int distance)
        {
            List<Shape> result = new List<Shape>();
            for (int i = list.Count - 1; 0 <= i; i--)
            {
                if (list[i].isNear(point, distance))
                    result.Add(list[i]);
            }
            return result;
        }

        public void Draw()
        {
            if (ToolBox.SelectedTool != ToolType.None && !ToolBox.SelectedTool.isSelect() && !ToolBox.ContainsPoint(Main.MouseScreen) && !Main.gameMenu)
            {
                if (ModContent.GetInstance<TeraCADConfig>().isDrawCursor)
                    cursorShape.DrawSelf(Main.spriteBatch);
                if (ModContent.GetInstance<TeraCADConfig>().isDrawCursorSnap)
                    cursorSnapShape.DrawSelf(Main.spriteBatch);
            }
            if (shape != null)
            {
                shape.DrawSelf(Main.spriteBatch);

                if (!ToolBox.ContainsPoint(Main.MouseScreen))
                    Tool.tooltip = shape.GetTooltip();
            }
        }

        public void DrawShapes()
        {
            if (ToolBox.SelectedTool == ToolType.Select || ToolBox.SelectedTool == ToolType.Eraser || (ToolBox.SelectedTool == ToolType.ParallelCopy && step == 0))
            {
                foreach (var hoverShape in hoverShapes)
                {
                    hoverShape.DrawSelfHover(Main.spriteBatch);
                }
            }
            if (ToolBox.SelectedTool == ToolType.ParallelCopy)
            {
                foreach (var copyShape in parallelCopyShapes)
                {
                    copyShape.DrawSelfHover(Main.spriteBatch);
                    copyShape.DrawSelf(Main.spriteBatch);
                }
            }
            if (ToolBox.SelectedTool == ToolType.AllClear)
            {
                foreach (var x in list)
                {
                    x.DrawSelfHover(Main.spriteBatch);
                }
            }
            if (parallelCopyTargetShape != null)
            {
                parallelCopyTargetShape.DrawSelfSelect(Main.spriteBatch);
            }
            foreach (var x in list)
            {
                x.DrawSelf(Main.spriteBatch);
            }
        }
    }
}
