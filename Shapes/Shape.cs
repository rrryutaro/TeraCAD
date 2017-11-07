using System;
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
        Ellipse,
        Arc,
		Image,
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
            color = ToolBox.LineColor * ToolBox.Transmittance;
            width = ToolBox.LineWidth;
        }
        public virtual void CopyFrom (Shape shape)
        {
            type = shape.type;
            pointStart = shape.pointStart;
            pointEnd = shape.pointEnd;
            color = shape.color;
            width = shape.width;
        }

        public virtual Shape Clone()
        {
            Shape result = new Shape();
            result.CopyFrom(this);
            return result;
        }

		public ShapeLine[] GetLines()
		{
			ShapeLine[] result =
			{
				new ShapeLine(pointStart, new Vector2(pointEnd.X, pointStart.Y)),
				new ShapeLine(pointStart, new Vector2(pointStart.X, pointEnd.Y)),
				new ShapeLine(new Vector2(pointEnd.X, pointStart.Y), pointEnd),
				new ShapeLine(new Vector2(pointStart.X, pointEnd.Y), pointEnd),
			};
			return result;
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
        public virtual bool isNear(Vector2 point, int distance)
        {
			bool result = VectorUtils.GetDistancePointToLine(point, pointStart, pointEnd) <= distance;
            return result;
        }
        public virtual void DrawSelf(SpriteBatch spriteBatch)
        {
        }
		public virtual void DrawSelfHover(SpriteBatch spriteBatch)
		{
			DrawSelf(spriteBatch);
		}
		public virtual string GetTooltip()
		{
			string result;
			var pos = (pointStart - pointEnd) / 16;
			result = $"{Math.Abs((int)pos.X)} x {Math.Abs((int)pos.Y)}";
			return result;
		}
    }

    public class ShapeLine : Shape
    {
        public ShapeLine() : base()
        {
            type = ShapeType.Line;
        }
        public ShapeLine(Vector2 start, Vector2 end) : base()
        {
            type = ShapeType.Line;
            pointStart = start;
            pointEnd = end;
        }
        public override Shape Clone()
        {
            ShapeLine result = new ShapeLine();
            result.CopyFrom(this);
            return result;
        }
		public override bool isNear(Vector2 point, int distance)
		{
			bool result = VectorUtils.GetDistancePointToLine(point, pointStart, pointEnd) <= distance;
			if (result)
			{
				if (isHorizontal)
				{
					result = Math.Min(pointStart.X, pointEnd.X) <= point.X && point.X <= Math.Max(pointStart.X, pointEnd.X);
				}
				else if (isVertical)
				{
					result = Math.Min(pointStart.Y, pointEnd.Y) <= point.Y && point.Y <= Math.Max(pointStart.Y, pointEnd.Y);
				}
				else
				{
					result = GetRect().Contains(point.ToPoint());
				}
			}
			return result;
		}
		public override void DrawSelf(SpriteBatch spriteBatch)
        {
			spriteBatch.DrawLine(pointStart, pointEnd, width, color);
		}
		public override void DrawSelfHover(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawLine(pointStart, pointEnd, width + 4, Color.Yellow);
		}
		public bool isHorizontal
		{
			get
			{
				return pointStart.Y == pointEnd.Y;
			}
		}
		public bool isVertical
		{
			get
			{
				return pointStart.X == pointEnd.X;
			}
		}
		public int TileX
		{
			get
			{
				return (int)Math.Abs(pointStart.X - pointEnd.X) / ModUtils.tileSize;
			}
		}
		public int TileY
		{
			get
			{
				return (int)Math.Abs(pointStart.Y - pointEnd.Y) / ModUtils.tileSize;
			}
		}
		public Tile[,] GetLineToTiles()
		{
			Tile[,] result;
			//
			if (isHorizontal)
			{
				int maxX = TileX;
				result = new Tile[maxX, 1];
				for (int x = 0; x < maxX; x++)
				{
					var tile = result[x, 0];
					tile.active(true);
					tile.type = 1;
				}
			}
			else if (isVertical)
			{
				int maxY = TileY;
				result = new Tile[1, maxY];
				for (int y = 0; y < maxY; y++)
				{
					var tile = result[0, y];
					tile.active(true);
					tile.type = 1;
				}
			}
			else
			{
				int maxX = TileX;
				int maxY = TileY;
				result = new Tile[maxX, maxY];
			}
			return result;
		}
	}

    class ShapeRect : Shape
    {
        public ShapeRect()
        {
            type = ShapeType.Rect;
        }
        public override Shape Clone()
        {
            ShapeRect result = new ShapeRect();
            result.CopyFrom(this);
            return result;
        }
        public override bool isNear(Vector2 point, int distance)
        {
            bool result = false;
            foreach (var line in GetLines())
            {
                result = line.isNear(point, distance);
                if (result)
                    break;
            }
            return result;
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
			//Utils.DrawRectangle(spriteBatch, pointStart, pointEnd, color, color, width);
			spriteBatch.DrawRect(pointStart, pointEnd, width, color);
		}
		public override void DrawSelfHover(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawRect(pointStart, pointEnd, width + 4, Color.Yellow);
		}
	}

	class ShapeCircle : Shape
	{
		public ShapeCircle() : base()
		{
			type = ShapeType.Circle;
		}
		public ShapeCircle(Vector2 start, Vector2 end) : base()
		{
			type = ShapeType.Circle;
			pointStart = start;
			pointEnd = end;
		}
		public override Shape Clone()
		{
			ShapeCircle result = new ShapeCircle();
			result.CopyFrom(this);
			return result;
		}
		public override string GetTooltip()
		{
			string result;
			var pos = (pointStart - pointEnd) / 16;
			result = $"{Math.Max(Math.Abs((int)pos.X), Math.Abs((int)pos.Y))}";
			return result;
		}
		public override bool isNear(Vector2 point, int distance)
		{
			bool result = Math.Abs(Vector2.Distance(pointStart, pointEnd) - Vector2.Distance(pointStart, point)) <= distance;
			return result;
		}
		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawCircle(pointStart, pointStart.ToDistance(pointEnd), width, color);
		}
		public override void DrawSelfHover(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawCircle(pointStart, pointStart.ToDistance(pointEnd), width + 4, Color.Yellow);
		}
	}

	class ShapeEllipse : Shape
	{
		public ShapeEllipse() : base()
		{
			type = ShapeType.Ellipse;
		}
		public ShapeEllipse(Vector2 start, Vector2 end) : base()
		{
			type = ShapeType.Ellipse;
			pointStart = start;
			pointEnd = end;
		}
		public override Shape Clone()
		{
			ShapeEllipse result = new ShapeEllipse();
			result.CopyFrom(this);
			return result;
		}
		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawEllipse(pointStart, pointEnd, color);
		}
	}

	class ShapeImage : Shape
	{
		public Texture2D image;
		public ImagePositionMode mode;
		public float transmittance;

		public ShapeImage() : base()
		{
			type = ShapeType.Image;
		}
		public ShapeImage(UISlotImage slot) : base()
		{
			type = ShapeType.Image;
			image = slot.image;
		}
		public override Shape Clone()
		{
			ShapeImage result = new ShapeImage();
			result.CopyFrom(this);
			return result;
		}
		public override void CopyFrom(Shape shape)
		{
			base.CopyFrom(shape);
			var shapeImage = shape as ShapeImage;
			image = shapeImage.image;
			mode = shapeImage.mode;
			transmittance = shapeImage.transmittance;
		}
		public override bool isNear(Vector2 point, int distance)
		{
			bool result =
				Math.Min(pointStart.X, pointEnd.X) <= point.X &&
				Math.Min(pointStart.Y, pointEnd.Y) <= point.Y &&
				point.X <= Math.Max(pointStart.X, pointEnd.X) &&
				point.Y <= Math.Max(pointStart.Y, pointEnd.Y);
			return result;
		}
		public void SetData()
		{
			pointStart = Snap.GetSnapPoint(ToolBox.snapType, -image.Width, -image.Height);
			pointEnd = pointStart.Offset(image.Width, image.Height);
			mode = ImageUI.instance.ImagePositionMode;
			transmittance = ImageUI.instance.Transmittance;
			if (mode == ImagePositionMode.Screen)
				pointStart -= Main.screenPosition;
		}
		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			var point = pointStart;
			if (mode == ImagePositionMode.World)
				point -= Main.screenPosition;
			Main.spriteBatch.Draw(image, point, Color.White * transmittance);
		}
		public override void DrawSelfHover(SpriteBatch spriteBatch)
		{
			Main.spriteBatch.Draw(Main.magicPixel, GetRect(), Color.Yellow * 0.6f);
			foreach (var line in GetLines())
			{
				line.width = 4;
				line.color = Color.Yellow;
				line.DrawSelf(spriteBatch);
			}
		}
	}
}
