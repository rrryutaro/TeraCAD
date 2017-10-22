using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TeraCAD
{
    public static class VectorUtils
    {
        public static Vector2 NearPoint(Vector2 posP, Vector2 posA, Vector2 posB)
        {
            Vector2 AB, AP;
            AB.X = posB.X - posA.X;
            AB.Y = posB.Y - posA.Y;
            AP.X = Main.MouseWorld.X - posA.X;
            AP.Y = Main.MouseWorld.Y - posA.Y;
            double len = Math.Pow((AB.X * AB.X) + (AB.Y * AB.Y), 0.5);
            Vector2 nAB = new Vector2((float)(AB.X / len), (float)(AB.Y / len));
            double dist_AX = nAB.X * AP.X + nAB.Y * AP.Y;
            Vector2 result = new Vector2((float)(posA.X + (nAB.X * dist_AX)), (float)(posA.Y + (nAB.Y * dist_AX)));
            return result;
        }

		public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
		{
			bool result = false;
			intersection = Vector2.Zero;

			var d = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);
			if (d != 0.0f)
			{
				var u = ((p3.X - p1.X) * (p4.Y - p3.Y) - (p3.Y - p1.Y) * (p4.X - p3.X)) / d;
				var v = ((p3.X - p1.X) * (p2.Y - p1.Y) - (p3.Y - p1.Y) * (p2.X - p1.X)) / d;
				if (!(u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f))
				{
					intersection.X = p1.X + u * (p2.X - p1.X);
					intersection.Y = p1.Y + u * (p2.Y - p1.Y);
					result = true;
				}
			}
			return result;
		}

		public static bool PointOnLine(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			bool result = false;
			var v1 = p2 - p1;
			var v2 = p3 - p1;
			var L1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
			var L2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
			result = v1.X * v2.X + v1.Y * v2.Y == L1 * L2 && L2 <= L1;
			return result;
		}

		public static float ToDistance(this Vector2 v1, Vector2 v2)
		{
			float result = Vector2.Distance(v1, v2);
			return result;
		}

		public static float ToRotation(this Vector2 v1, Vector2 v2)
		{
			float result;
			float distance = Vector2.Distance(v1, v2);
			Vector2 v3 = (v2 - v1) / distance;
			result = (float)Math.Atan2((double)v3.Y, (double)v3.X);
			return result;
		}

		public static Vector2 ToRotationVector(this Vector2 v, float angle)
		{
			Vector2 result;
			double radian = angle.ToRadian();
			result = new Vector2((float)(v.X * Math.Cos(radian) - v.Y * Math.Sin(radian)), (float)(v.X * Math.Sin(radian) + v.Y * Math.Cos(radian)));
			return result;
		}

		public static Vector2 ToRotationVector(this Vector2 v1, float distance, float angle, bool bRadian = false)
		{
			Vector2 result;
			double radian = bRadian ? angle : angle.ToRadian();
			result = v1 + new Vector2((float)Math.Cos(radian) * distance, (float)Math.Sin(radian) * distance);
			return result;
		}

		public static float ToRadian(this float f)
		{
			float result = (float)(f * Math.PI / 180);
			return result;
		}
	}
}
