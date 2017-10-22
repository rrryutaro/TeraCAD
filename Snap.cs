using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace TeraCAD
{
    public enum SnapType
    {
        TopLeft,
        TopCenter,
        TopRight,
        LeftCenter,
        Center,
        RightCenter,
        BottomLeft,
        BottomCenter,
        BottomRight,
    };

    public static class Snap
    {
        public static Vector2 GetSnapPoint(SnapType type, int width = 16, int height = 16)
        {
            Vector2 result;
            Vector2 point = Main.MouseWorld.ToTileCoordinates().ToVector2() * 16;
            SnapType snapType = type;
            if (Main.LocalPlayer.gravDir == -1f)
            {
                switch (snapType)
                {
                    case SnapType.TopLeft:
                        snapType = SnapType.BottomLeft;
                        break;
                    case SnapType.TopCenter:
                        snapType = SnapType.BottomCenter;
                        break;
                    case SnapType.TopRight:
                        snapType = SnapType.BottomRight;
                        break;
                    case SnapType.BottomLeft:
                        snapType = SnapType.TopLeft;
                        break;
                    case SnapType.BottomCenter:
                        snapType = SnapType.TopCenter;
                        break;
                    case SnapType.BottomRight:
                        snapType = SnapType.TopRight;
                        break;
                }
            }
            switch (snapType)
            {
                case SnapType.TopLeft:
                    break;
                case SnapType.TopCenter:
                    point = point.Offset(width / 2, 0);
                    break;
                case SnapType.TopRight:
                    point = point.Offset(width, 0);
                    break;
                case SnapType.LeftCenter:
                    point = point.Offset(0, height / 2);
                    break;
                case SnapType.Center:
                    point = point.Offset(width / 2, height / 2);
                    break;
                case SnapType.RightCenter:
                    point = point.Offset(width, height / 2);
                    break;
                case SnapType.BottomLeft:
                    point = point.Offset(0, height);
                    break;
                case SnapType.BottomCenter:
                    point = point.Offset(width / 2, height);
                    break;
                case SnapType.BottomRight:
                    point = point.Offset(height, height);
                    break;
            }
            if (Main.LocalPlayer.gravDir == -1f)
            {
                point.Y = (float)Main.screenHeight - point.Y;
                //point.Y -= height * 16;
            }
            result = point;
            return result;
        }
    }
}
