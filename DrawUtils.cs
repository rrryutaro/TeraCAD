using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TeraCAD
{
    public static class DrawUtils
	{
		public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, int width, Color color)
		{
			float width2 = width / 2f;
			float rotation = start.ToRotation(end);
			Vector2 scale = new Vector2(start.ToDistance(end), width2);
			Vector2 start2 = start.ToRotationVector(width2, rotation - (90f).ToRadian(), true);
			sb.DrawLine(start2, rotation, scale, width2, color);
			sb.DrawLine(start, rotation, scale, width2, color);
			//sb.Draw(Main.magicPixel, pointStart - Main.screenPosition, new Rectangle(0, 0, 1, width), Color.Blue, pointStart.ToRotation(pointEnd), Vector2.Zero, new Vector2(pointStart.ToDistance(pointEnd), 1f), SpriteEffects.None, 0f);
		}
		public static void DrawLine(this SpriteBatch sb, Vector2 start, float rotation, Vector2 scale, float width, Color color)
		{
			sb.Draw(Main.magicPixel, start - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}
		public static void DrawRect(this SpriteBatch sb, Vector2 start, Vector2 end, int width, Color color)
		{
			float width2 = width / 2f;
			Vector2 pos1 = new Vector2(end.X, start.Y);
			Vector2 pos2 = new Vector2(start.X, end.Y);
			float rotation1 = start.ToRotation(pos1);
			float rotation2 = start.ToRotation(pos2);

			sb.DrawLine(start.ToRotationVector(-width2, rotation1, true), pos1.ToRotationVector(width2, rotation1, true), width, color);
			sb.DrawLine(start.ToRotationVector(-width2, rotation2, true), pos2.ToRotationVector(width2, rotation2, true), width, color);
			sb.DrawLine(pos1.ToRotationVector(-width2, rotation2, true), end.ToRotationVector(width2, rotation2, true), width, color);
			sb.DrawLine(pos2.ToRotationVector(-width2, rotation1, true), end.ToRotationVector(width2, rotation1, true), width, color);
		}
		public static void DrawCircle(this SpriteBatch sb, Vector2 center, float radius, int width, Color color, int split = 36)
		{
			float add = 360f / split;
			for (int i = 0; i < split; i++)
			{
				sb.DrawLine(center.ToRotationVector(radius, i * add), center.ToRotationVector(radius, (i + 1) * add), width, color);
			}
		}
		public static void DrawEllipse(this SpriteBatch sb, Vector2 start, Vector2 end, Color color, int split = 36)
		{
			//float width = Math.Abs(start.X - end.X);
			//float height = Math.Abs(start.Y - end.Y);
			//Vector2 center = start.Offset(width / 2, height / 2);
			//float add = 360f / split;
			//for (int i = 0; i < split; i++)
			//{
			//	//sb.DrawLine(center.ToRotationVector(radius, i * add), center.ToRotationVector(radius, (i + 1) * add), width, color);
			//}
		}
	}
}
