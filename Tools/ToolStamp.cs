using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text;
using System.Linq;
using Terraria;
using Terraria.ID;
using Newtonsoft.Json;

namespace TeraCAD
{
    public class StampInfo
    {
        public Tile[,] Tiles;
        public Texture2D[,] Textures;
        public int Width;
        public int Height;
    }

	internal class ToolStamp
    {
        private bool down;
        private bool up;

        private int startTileX = -1;
        private int startTileY = -1;
        private int lastMouseTileX = -1;
        private int lastMouseTileY = -1;
        private bool constrainToAxis;
        private int constrainedX;
        private int constrainedY;
        private int constrainedStartX;
        private int constrainedStartY;

        internal bool TransparentSelectionEnabled = false;

        public static StampInfo stampInfo;

        public void Update()
        {
            Player player = Main.LocalPlayer;
            player.showItemIcon = true;
            player.showItemIcon2 = ItemID.Paintbrush;

            if (down && !Main.mouseLeft)
            {
                down = false;
            }

            if (!ToolBox.ContainsPoint(Main.MouseScreen))
            {
                down = Main.mouseLeft;
            }
            if (down && stampInfo != null)
            {
                Stamp(stampInfo.Tiles);
            }
            else
            {
                startTileX = -1;
                startTileY = -1;
                constrainToAxis = false;
                constrainedX = -1;
                constrainedY = -1;
                constrainedStartX = -1;
                constrainedStartY = -1;
            }
        }

        public void Stamp(Tile[,] tilesA)
        {
            Tile[,] tiles = tilesA;
            if (StampUI.instance.isHorizontalReversal || StampUI.instance.isVerticalReversal)
            {
                bool isHR = StampUI.instance.isHorizontalReversal;
                bool isVR = StampUI.instance.isVerticalReversal;
            
                int maxX = tilesA.GetLength(0);
                int maxY = tilesA.GetLength(1);
            
                tiles = new Tile[maxX, maxY];
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        //tiles[isHR ? maxX - x - 1 : x, isVR ? maxY - y - 1 : y] = tilesA[x, y];
                        tiles[isHR ? maxX - x - 1 : x, isVR ? maxY - y - 1 : y] = tilesA[x, y];
                    }
                }
            }

            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            Vector2 brushsize = new Vector2(width, height);
            Vector2 evenOffset = Vector2.Zero;
            if (width % 2 == 0)
            {
                evenOffset.X = 1;
            }
            if (height % 2 == 0)
            {
                evenOffset.Y = 1;
            }
            Point point = (Main.MouseWorld + evenOffset * 8).ToTileCoordinates();
            point.X -= width / 2;
            point.Y -= height / 2;

            //スナップ
            Snap snap = new Snap();
            snap.type = ToolBox.snapType;
            snap.position = point.ToVector2();
            snap.width = width;
            snap.height = height;
            snap.isCorrection = false;
            point = snap.GetSnapPosition().ToPoint();

            if (startTileX == -1)
            {
                startTileX = point.X;
                startTileY = point.Y;
                lastMouseTileX = -1;
                lastMouseTileY = -1;
            }

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                constrainToAxis = true;
                if (constrainedStartX == -1 && constrainedStartY == -1)
                {
                    constrainedStartX = point.X;
                    constrainedStartY = point.Y;
                }

                if (constrainedX == -1 && constrainedY == -1)
                {
                    if (constrainedStartX != point.X)
                    {
                        constrainedY = point.Y;
                    }
                    else if (constrainedStartY != point.Y)
                    {
                        constrainedX = point.X;
                    }
                }
                if (constrainedX != -1)
                {
                    point.X = constrainedX;
                }
                if (constrainedY != -1)
                {
                    point.Y = constrainedY;
                }
            }
            else
            {
                constrainToAxis = false;
                constrainedX = -1;
                constrainedY = -1;
                constrainedStartX = -1;
                constrainedStartY = -1;
            }

            if (lastMouseTileX != point.X || lastMouseTileY != point.Y)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (WorldGen.InWorld(x + point.X, y + point.Y) && tiles[x, y] != null)
                        {
                            Tile target = Framing.GetTileSafely(x + point.X, y + point.Y);
                            int cycledX = ((x + point.X - startTileX) % width + width) % width;
                            int cycledY = ((y + point.Y - startTileY) % height + height) % height;
                            if (TransparentSelectionEnabled)
                            {
                                if (tiles[cycledX, cycledY].active())
                                {
                                    target.CopyFrom(tiles[cycledX, cycledY]);
                                }
                            }
                            else
                            {
                                target.CopyFrom(tiles[cycledX, cycledY]);
                            }
                        }
                    }
                }
                if (Main.netMode == 1)
                {
                    NetMessage.SendTileSquare(-1, point.X + width / 2, point.Y + height / 2, Math.Max(width, height));
                }
            }
        }

        private static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
        }

        internal void Draw()
        {
            if (stampInfo == null)
                return;

            Tile[,] tiles = stampInfo.Tiles;

            int maxTilesX = tiles.GetLength(0);
            int maxTilesY = tiles.GetLength(1);
            Vector2 brushsize = new Vector2(maxTilesX, maxTilesY);
            Vector2 evenOffset = Vector2.Zero;
            if (maxTilesX % 2 == 0)
            {
                evenOffset.X = 1;
            }
            if (maxTilesY % 2 == 0)
            {
                evenOffset.Y = 1;
            }
            Point point = (Main.MouseWorld + evenOffset * 8).ToTileCoordinates();

            point.X -= maxTilesX / 2;
            point.Y -= maxTilesY / 2;
            if (constrainToAxis)
            {
                if (constrainedX != -1)
                {
                    point.X = constrainedX;
                }
                if (constrainedY != -1)
                {
                    point.Y = constrainedY;
                }
            }

            Vector2 vector = new Vector2(point.X, point.Y) * 16f;
            vector -= Main.screenPosition;
            if (Main.LocalPlayer.gravDir == -1f)
            {
                vector.Y = (float)Main.screenHeight - vector.Y;
                vector.Y -= maxTilesY * 16;
            }

            //スナップ
            Snap snap = new Snap();
            snap.type = ToolBox.snapType;
            snap.position = vector;
            snap.width = stampInfo.Width;
            snap.height = stampInfo.Height;
            vector = snap.GetSnapPosition();

            if (!down)
            {
                DrawPreview(Main.spriteBatch, stampInfo, vector);
            }

            Rectangle value = new Rectangle(0, 0, 1, 1);
            float r = 1f;
            if (!down) r = .25f;
            float g = 0.9f;
            float b = 0.1f;
            float a = 1f;
            //a = .2f;
            float scale = 0.6f;
            Color color = buffColor(Color.White, r, g, b, a);
            //Main.spriteBatch.Draw(Main.magicPixel, vector, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, 16f * brushsize, SpriteEffects.None, 0f);
            b = 0.3f;
            g = 0.95f;
            scale = (a = 1f);
            color = buffColor(Color.White, r, g, b, a);
            Main.spriteBatch.Draw(Main.magicPixel, vector + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushsize.Y), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, vector + Vector2.UnitX * 16f * brushsize.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushsize.Y), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, vector + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushsize.X, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, vector + Vector2.UnitY * 16f * brushsize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushsize.X, 2f), SpriteEffects.None, 0f);
        }

        public static void DrawPreview(SpriteBatch sb, StampInfo info, Vector2 position)
        {
            int maxX = info.Textures.GetLength(0);
            int maxY = info.Textures.GetLength(1);
            bool isHR = StampUI.instance.isHorizontalReversal;
            bool isVR = StampUI.instance.isVerticalReversal;

            Texture2D[,] textures = new Texture2D[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    textures[isHR ? maxX - x - 1 : x, isVR ? maxY - y - 1 : y] = info.Textures[x, y];
                }
            }

            SpriteEffects effects = SpriteEffects.None;
            if (isHR)
                effects |= SpriteEffects.FlipHorizontally;
            if (isVR)
                effects |= SpriteEffects.FlipVertically;

            for (int x = 0; x < info.Textures.GetLength(0); x++)
            {
                Vector2 pos = position;
                pos.X += x * ModUtils.TextureMaxTile * 16;

                for (int y = 0; y < info.Textures.GetLength(1); y++)
                {
                    //sb.Draw(info.Textures[x, y], pos, Color.White * 0.6f);
                    sb.Draw(textures[x, y], pos, null, Color.White * 0.6f, 0f, Vector2.Zero, 1f, effects, 0f);

                    pos.Y += ModUtils.TextureMaxTile * 16;
                }
            }
        }
    }
}