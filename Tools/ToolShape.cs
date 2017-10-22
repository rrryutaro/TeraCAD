using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TeraCAD.Shapes;

namespace TeraCAD.Tools
{
    public class ToolShape
    {
		public static Shape shape;
		public int step = 0;
        public List<Shape> list = new List<Shape>();

        public void Update()
        {
            if (ToolBox.ContainsPoint(Main.MouseScreen))
                return;

            if (Main.mouseRight)
            {
                step = 0;
                shape = null;
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
                        }
                    }
					else
					{
						if (ToolBox.SelectedTool == ToolType.Image)
						{
							(shape as ShapeImage).SetData();
						}
					}
                    break;

                case 1:
					if (ToolBox.SelectedTool == ToolType.Image)
					{
						if (!Main.mouseLeft)
							step = 0;
					}
					else
					{
						shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
						if (!Main.mouseLeft)
							step = 2;
					}
                    break;

                case 2:
                    shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                    if (Main.mouseLeft)
                    {
                        shape.pointEnd = Snap.GetSnapPoint(ToolBox.snapType);
                        list.Add(shape);
                        step = 3;
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
            if (ToolBox.SelectedTool == ToolType.Select)
            {
                foreach (var shape in GetNearShape(Main.MouseWorld, ToolBox.snapDistance))
                {
                    var shapeClone = shape.Clone();
                    shapeClone.width += 4;
                    shapeClone.color = Color.Yellow * 0.6f;
                    shapeClone.DrawSelf(Main.spriteBatch);
                }
            }
            if (shape != null)
            {
                shape.DrawSelf(Main.spriteBatch);
				Tool.tooltip = shape.GetTooltip();
            }
        }

		public void DrawShapes()
		{
			foreach (var x in list)
			{
				x.DrawSelf(Main.spriteBatch);
			}
		}
	}
}
