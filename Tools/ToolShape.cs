using System;
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
        public int step = 0;
        public List<Shape> list = new List<Shape>();
        public Shape shape;

        public void Update()
        {
            switch (step)
            {
                case 0:
                    if (Main.mouseLeft)
                    {
                        switch (ToolBox.SelectedTool)
                        {
                            case ToolType.Line:
                                shape = new ShapeLine();
                                shape.pointStart = Main.MouseWorld;
                                shape.pointEnd = Main.MouseWorld;
                                step = 1;
                                break;

                            case ToolType.Rect:
                                shape = new ShapeRect();
                                shape.pointStart = Main.MouseWorld;
                                shape.pointEnd = Main.MouseWorld;
                                step = 1;
                                break;
                        }
                    }
                    break;

                case 1:
                    shape.pointEnd = Main.MouseWorld;
                    if (!Main.mouseLeft)
                    {
                        step = 2;
                    }
                    break;

                case 2:
                    shape.pointEnd = Main.MouseWorld;
                    if (Main.mouseLeft)
                    {
                        shape.pointEnd = Main.MouseWorld;
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

        public void Draw()
        {
            if (shape != null)
            {
                shape.DrawSelf(Main.spriteBatch);
            }
            foreach (var x in list)
            {
                x.DrawSelf(Main.spriteBatch);
            }
        }
    }
}
