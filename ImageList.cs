using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace TeraCAD
{
    public class ImageList
    {
        public List<Texture2D> listTexture = new List<Texture2D>();
        public ImageList(Texture2D texture, int width, int height)
        {
            for (int y = 0; y < texture.Height / height; y++)
            {
                for (int x = 0; x < texture.Width / width; x++)
                {
                    listTexture.Add(texture.Offset(x * width, y * height, width, height));
                }
            }
        }

        public int Count
        {
            get
            {
                return listTexture.Count;
            }
        }

        public Texture2D this[int index]
        {
            get
            {
                return listTexture[index];
            }
        }

    }
}
