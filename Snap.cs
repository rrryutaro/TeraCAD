using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


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

    public class Snap
    {
        public SnapType type;
        public Vector2 position;
        public int width;
        public int height;
        public bool isCorrection = true;
        int corrX;
        int corrY;

        public Vector2 GetSnapPosition()
        {
            Vector2 result = position;

            if (isCorrection)
            {
                corrX = (width / 2) % ModUtils.tileSize;
                corrY = (height / 2) % ModUtils.tileSize;
            } 

            switch (type)
            {
                case SnapType.TopLeft:
                    result = position.Offset((width / 2) - corrX, (height / 2) - corrY);
                    break;
                case SnapType.TopCenter:
                    result = position.Offset(0, (height / 2) - corrY);
                    break;
                case SnapType.TopRight:
                    result = position.Offset((-width / 2) + corrX, (height / 2) - corrY);
                    break;
                case SnapType.LeftCenter:
                    result = position.Offset((width / 2) - corrX, 0);
                    break;
                case SnapType.Center:
                    break;
                case SnapType.RightCenter:
                    result = position.Offset((-width / 2) + corrX, 0);
                    break;
                case SnapType.BottomLeft:
                    result = position.Offset((width / 2) - corrX, (-height / 2) + corrY);
                    break;
                case SnapType.BottomCenter:
                    result = position.Offset(0, (-height / 2) + corrY);
                    break;
                case SnapType.BottomRight:
                    result = position.Offset((-width / 2) + corrX, (-height / 2) + corrY);
                    break;
            }
            return result;
        }
    }
}
