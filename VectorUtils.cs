using System;
using Microsoft.Xna.Framework;

namespace TeraCAD
{
    public static class VectorUtils
    {
        public static Vector2 NearPoint(Vector2 posP, Vector2 posA, Vector2 posB)
        {
            Vector2 AB, AP;
            AB.X = posB.X - posA.X;
            AB.Y = posB.Y - posA.Y;
            AP.X = posP.X - posA.X;
            AP.Y = posP.Y - posA.Y;
            double len = Math.Pow((AB.X * AB.X) + (AB.Y * AB.Y), 0.5);
            Vector2 nAB = new Vector2((float)(AB.X / len), (float)(AB.Y / len));
            double dist_AX = nAB.X * AP.X + nAB.Y * AP.Y;
            Vector2 result = new Vector2((float)(posA.X + (nAB.X * dist_AX)), (float)(posA.Y + (nAB.Y * dist_AX)));
            return result;
        }

		public static float GetDistancePointToLine(Vector2 posP, Vector2 posA, Vector2 posB)
		{
			float result = float.MaxValue;
			Vector2 AB, AP;
			AB.X = posB.X - posA.X;
			AB.Y = posB.Y - posA.Y;
			AP.X = posP.X - posA.X;
			AP.Y = posP.Y - posA.Y;

			float D = Math.Abs(AB.X * AP.Y - AB.Y * AP.X);
			float L = Vector2.Distance(posA, posB);
			result = D / L;

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

		public static float GetRadian(this Vector2 v1, Vector2 v2)
		{
			float result = (float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
			return result;
		}

		/// <summary>
		/// v1からv2の点の向きを返す
		/// </summary>
		/// <returns>1:Left 2:Top 3:Right 4:Bottom 0:unknown</returns>
		public static int GetDirection(this Vector2 v1, Vector2 v2)
		{
			int result = 0;
			float degree = v1.GetRadian(v2).ToDegree();
			if (135 <= degree || degree < -135)
				result = 1;
			else if (-135 <= degree && degree < -45)
				result = 2;
			else if (45 <= degree && degree < 135)
				result = 4;
			else if (-45 <= degree || degree < 45)
				result = 3;
			return result;
		}

		/// <summary>
		/// 点(v1)が線分(start - end)の左右どちらの向きか
		/// </summary>
		/// <returns>1:左 2:右 0:線上</returns>
		public static int GetDirection(this Vector2 v1, Vector2 start, Vector2 end)
		{
			int result = 0;
			int n = (int)(v1.X * (start.Y - end.Y) + start.X * (end.Y - v1.Y) + end.X * (v1.Y - start.Y));
			if (0 < n)
				result = 1;
			else if (n < 0)
				result = 2;
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

		public static float ToDegree(this float f)
		{
			float result = (float)(f * 180 / Math.PI);
			return result;
		}
	}
}
