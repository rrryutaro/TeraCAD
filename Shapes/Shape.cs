using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TeraCAD.Shapes
{
    public enum ShapeType
    {
        Line,
        Rect,
        Circle,
        Elipse,
        Arc,
    }

    public class Shape
    {
        public ShapeType type;
        public Vector2 pointStart;
        public Vector2 pointEnd;
        public Color color;
        public int width;

        public Shape()
        {
            color = Color.Black;
            width = 1;
        }

        public Rectangle GetRect()
        {
            int x = (int)Math.Min(pointStart.X, pointEnd.X);
            int y = (int)Math.Min(pointStart.Y, pointEnd.Y);
            int width = (int)Math.Max(pointStart.X, pointEnd.X) - x;
            int height = (int)Math.Max(pointStart.Y, pointEnd.Y) - y;

            Rectangle result = new Rectangle(x, y, width, height);
            return result;
        }

        public virtual void DrawSelf(SpriteBatch spriteBatch)
        {
        }
    }

    class ShapeLine : Shape
    {
        public ShapeLine()
        {
            type = ShapeType.Line;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawLine(spriteBatch, pointStart, pointEnd, color, color, width);
        }
    }

    class ShapeRect : Shape
    {
        public ShapeRect()
        {
            type = ShapeType.Rect;
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawRectangle(spriteBatch, pointStart, pointEnd, color, color, width);
        }
    }
}
