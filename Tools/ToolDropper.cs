using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Terraria;
using Terraria.ID;
using Newtonsoft.Json;

namespace TeraCAD
{
	internal class ToolDropper
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

        public void Update()
        {
            Player player = Main.LocalPlayer;
            player.showItemIcon = true;
            player.showItemIcon2 = ItemID.EmptyDropper;

            if (down && !Main.mouseLeft)
            {
                Copy();
                down = false;
            }

            if (!ToolBox.ContainsPoint(Main.MouseScreen))
            {
                down = Main.mouseLeft;
            }

            if (down)
            {
                Point point = Main.MouseWorld.ToTileCoordinates();
                if (startTileX == -1)
                {
                    startTileX = point.X;
                    startTileY = point.Y;
                }
                lastMouseTileX = point.X;
                lastMouseTileY = point.Y;
            }
            else
            {
                startTileX = -1;
                startTileY = -1;
            }
        }

        public static void Import()
        {
            try
            {
                foreach (var line in File.ReadAllLines($@"{Main.SavePath}\TeraCAD_Stamp.txt", Encoding.UTF8))
                {
                    Tile[,] tiles = JsonConvert.DeserializeObject<Tile[,]>(File.ReadAllText(line, Encoding.UTF8));
                    StampUI.instance.AddStamp(GetStampInfo(tiles));
                }
            }
            catch { }
        }

        public static void Export()
        {
            try
            {
                int index = 1;
                string path;
                List<string> list = new List<string>();
                foreach (var slot in StampUI.instance.gridStamp._items.Cast<UISlotStamp>())
                {
                    path = $@"{Main.SavePath}\TeraCAD_Stamp_{index++}.json";
                    list.Add(path);
                    File.WriteAllText(path, JsonConvert.SerializeObject(slot.stampInfo.Tiles), Encoding.UTF8);
                }
                if (0 < list.Count)
                    File.WriteAllLines($@"{Main.SavePath}\TeraCAD_Stamp.txt", list);
            }
            catch { }
        }

        public void Copy()
        {
            if (startTileX != -1 && startTileY != -1 && lastMouseTileX != -1 && lastMouseTileY != -1)
            {
                Vector2 upperLeft = new Vector2(Math.Min(startTileX, lastMouseTileX), Math.Min(startTileY, lastMouseTileY));
                Vector2 lowerRight = new Vector2(Math.Max(startTileX, lastMouseTileX), Math.Max(startTileY, lastMouseTileY));
                int minX = (int)upperLeft.X;
                int maxX = (int)lowerRight.X + 1;
                int minY = (int)upperLeft.Y;
                int maxY = (int)lowerRight.Y + 1;

                var tiles = new Tile[maxX - minX, maxY - minY];
                for (int i = 0; i < maxX - minX; i++)
                {
                    for (int j = 0; j < maxY - minY; j++)
                    {
                        tiles[i, j] = new Tile();
                    }
                }
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        if (WorldGen.InWorld(x, y))
                        {
                            Tile target = Framing.GetTileSafely(x, y);
                            tiles[x - minX, y - minY].CopyFrom(target);
                        }
                    }
                }

                try
                {
                    StampUI.instance.AddStamp(GetStampInfo(tiles));
                }
                catch (Exception ex)
                {
                    Main.NewText("Dropper tool error.", Color.Red);
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
            SpriteBatch sb = Main.spriteBatch;
            if (down)
            {
                Vector2 upperLeft = new Vector2(Math.Min(startTileX, lastMouseTileX), Math.Min(startTileY, lastMouseTileY));
                Vector2 lowerRight = new Vector2(Math.Max(startTileX, lastMouseTileX) + 1, Math.Max(startTileY, lastMouseTileY) + 1);

                Vector2 upperLeftScreen = (upperLeft * 16f) - Main.screenPosition;
                Vector2 lowerRightScreen = (lowerRight * 16f) - Main.screenPosition;
                if (Main.LocalPlayer.gravDir == -1f)
                {
                    upperLeftScreen.Y = (float)Main.screenHeight - upperLeftScreen.Y;
                    lowerRightScreen.Y = (float)Main.screenHeight - lowerRightScreen.Y;

                    Utils.Swap(ref upperLeftScreen.Y, ref lowerRightScreen.Y);
                }

                Vector2 brushSize = lowerRight - upperLeft;
                Rectangle value = new Rectangle(0, 0, 1, 1);
                float r = 1f;
                float g = 0.9f;
                float b = 0.1f;
                float a = 1f;
                float scale = 0.6f;
                Color color = buffColor(Color.White, r, g, b, a);
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, 16f * brushSize, SpriteEffects.None, 0f);
                b = 0.3f;
                g = 0.95f;
                scale = (a = 1f);
                color = buffColor(Color.White, r, g, b, a);
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * 16f * brushSize.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * 16f * brushSize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
            }
        }

        public static StampInfo GetStampInfo(Tile[,] Tiles)
        {
            int maxTile = ModUtils.TextureMaxTile;

            StampInfo result = new StampInfo();
            result.Tiles = Tiles;

            int maxX = Tiles.GetLength(0);
            int maxY = Tiles.GetLength(1);
            int texMaxX = (int)Math.Ceiling((double)maxX / (double)maxTile);
            int texMaxY = (int)Math.Ceiling((double)maxY / (double)maxTile);
            int restX = maxX % maxTile == 0 ? maxTile : maxX % maxTile;
            int restY = maxY % maxTile == 0 ? maxTile : maxY % maxTile;

            result.Textures = new Texture2D[texMaxX, texMaxY];
            result.Width = maxX * 16;
            result.Height = maxY * 16;

            for (int x = 0; x < texMaxX; x++)
            {
                for (int y = 0; y < texMaxY; y++)
                {
                    Tile[,] tiles = new Tile[x < texMaxX - 1 ? maxTile : restX, y < texMaxY - 1 ? maxTile : restY];
                    for (int i = 0; i < tiles.GetLength(0); i++)
                    {
                        for (int j = 0; j < tiles.GetLength(1); j++)
                        {
                            tiles[i, j] = Tiles[x * maxTile + i, y * maxTile + j];
                        }
                    }
                    result.Textures[x, y] = TilesToTexture(tiles);
                }
            }

            return result;
        }

        /// <summary>
        /// タイルのテクスチャ内での矩形位置を取得する
        /// </summary>
        private static Rectangle GetTileRect(Tile tile, int halfIndex = 0)
        {
            Rectangle result;

            if (tile.halfBrick())
            {
                if (halfIndex == 0)
                    result = new Rectangle(tile.frameX, tile.frameY, 16, 4);
                else
                    result = new Rectangle(tile.frameX, tile.frameY + 12, 16, 4);
            }
            else if (tile.type == TileID.MinecartTrack)
                result = Minecart.GetSourceRect(tile.frameX, Main.tileFrame[tile.type]);
            else
                result = new Rectangle(tile.frameX, tile.frameY, 16, 16);

            if (result.X < 0)
                result.X = 0;
            if (result.Y < 0)
                result.Y = 0;

            return result;
        }

        private static Texture2D TilesToTexture(Tile[,] Tiles)
        {
            Texture2D result = null;
            try
            {
                int maxTile = ModUtils.TextureMaxTile;
                int maxX = Math.Min(Tiles.GetLength(0), maxTile);
                int maxY = Math.Min(Tiles.GetLength(1), maxTile);
                int width = maxX * 16;
                int height = maxY * 16;

                Color[] data = new Color[width * height];
                Color[] dataTile = new Color[256];
                Color[] dataWall = new Color[256];

                Texture2D textureTile;
                Texture2D textureWall;

                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        Tile tile = Tiles[x, y];

                        textureTile = null;
                        textureWall = null;
                        if (tile.active())
                        {
                            try
                            {
                                Main.instance.LoadTiles(tile.type);
                                if (canDrawColorTile(tile))
                                    textureTile = Main.tileAltTexture[tile.type, tile.color()];
                                else
                                    textureTile = Main.tileTexture[tile.type];

                                Rectangle rect = GetTileRect(tile);

                                if (textureTile.Width < rect.X + rect.Width)
                                {
                                    int width2 = textureTile.Width - rect.X;
                                    rect.Width = width2;

                                    Color[] d = new Color[16 * width2];

                                    textureTile.GetData<Color>(0, rect, d, 0, d.Length);

                                    for (int y2 = 0; y2 < 16; y2++)
                                    {
                                        for (int x2 = 0; x2 < 16; x2++)
                                        {
                                            if (x2 < width2)
                                                dataTile[y2 * 16 + x2] = d[y2 * width2 + x2];
                                            else
                                                dataTile[y2 * 16 + x2] = Color.Transparent;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tile.halfBrick())
                                        dataTile = GetHalfTile(tile, textureTile);
                                    else if (0 < tile.slope())
                                        dataTile = GetSlopeTile(tile, textureTile);
                                    else
                                        textureTile.GetData<Color>(0, rect, dataTile, 0, 256);
                                }
                            }
                            catch { }
                        }
                        if (0 < tile.wall)
                        {
                            Main.instance.LoadWall(tile.wall);
                            if (canDrawColorWall(tile) && tile.type < Main.wallAltTexture.GetLength(0) && Main.wallAltTexture[tile.type, tile.wallColor()] != null)
                                textureWall = Main.wallAltTexture[tile.type, tile.wallColor()];
                            else
                                textureWall = Main.wallTexture[tile.wall];
                            textureWall.GetData<Color>(0, new Rectangle(tile.wallFrameX() + 8 , tile.wallFrameY() + (Main.wallFrame[tile.wall] * 180) + 8, 16, 16), dataWall, 0, 256);
                        }

                        int w = x * 16;
                        if (tile.active() || 0 < tile.wall)
                        {
                            for (int y2 = 0; y2 < 16; y2++)
                            {
                                int h = (y * 16 * width) + (y2 * width);
                                for (int x2 = 0; x2 < 16; x2++)
                                {
                                    if (0 < tile.wall)
                                    {
                                        data[h + w + x2] = dataWall[y2 * 16 + x2];
                                    }
                                    if (tile.active() && dataTile[y2 * 16 + x2] != Color.Transparent)
                                        data[h + w + x2] = dataTile[y2 * 16 + x2];
                                }
                            }
                        }
                    }
                }

                result = new Texture2D(Main.graphics.GraphicsDevice, width, height);
                result.SetData<Color>(data);
            }
            catch (Exception ex)
            {
                int i = 0;
            }
            return result;
        }

        public static bool canDrawColorTile(Tile tile)
        {
            return tile != null && tile.color() > 0 && (int)tile.color() < Main.numTileColors && Main.tileAltTextureDrawn[(int)tile.type, (int)tile.color()] && Main.tileAltTextureInit[(int)tile.type, (int)tile.color()];
        }
        public static bool canDrawColorWall(Tile tile)
        {
            return tile != null && tile.wallColor() > 0 && Main.wallAltTextureDrawn[tile.wall, tile.wallColor()] && Main.wallAltTextureInit[tile.wall, tile.wallColor()];
        }

        public static Color[] GetHalfTile(Tile tile, Texture2D textureTile)
        {
            Color[] result = new Color[256];
            var data = new Color[64];
            //var r1 = new Rectangle((int)tile.frameX, tile.frameY + Main.tileFrame[tile.type] * 38, 16, 4);
            //var r2 = new Rectangle(144, 66 + Main.tileFrame[tile.type] * 38, 16, 4);

            var r1 = GetTileRect(tile, 0);
            var r2 = GetTileRect(tile, 1);

            textureTile.GetData<Color>(0, r1, data, 0, 64);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    result[(y + 8) * 16 + x] = data[y * 16 + x];
                }
            }

            textureTile.GetData<Color>(0, r2, data, 0, 64);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    result[(y + 12) * 16 + x] = data[y * 16 + x];
                }
            }

            return result;
        }
        public static Color[] GetSlopeTile(Tile tile, Texture2D textureTile)
        {
            Color[] result = new Color[256];
            var data = new Color[256];
            var rect = GetTileRect(tile);
            textureTile.GetData<Color>(0, rect, data, 0, 256);

            switch (tile.slope())
            {
                case 1:
                    for (int x = 0; x < 16; x += 2)
                    {
                        for (int y = 0; y < (14 - x); y += 2)
                        {
                            result[(y + 0 + x) * 16 + (x + 0)] = data[(y + 0) * 16 + (x + 0)];
                            result[(y + 0 + x) * 16 + (x + 1)] = data[(y + 0) * 16 + (x + 1)];
                            result[(y + 1 + x) * 16 + (x + 0)] = data[(y + 1) * 16 + (x + 0)];
                            result[(y + 1 + x) * 16 + (x + 1)] = data[(y + 1) * 16 + (x + 1)];
                        }
                        result[(14) * 16 + (x + 0)] = data[(14) * 16 + (x + 0)];
                        result[(14) * 16 + (x + 1)] = data[(14) * 16 + (x + 1)];
                        result[(15) * 16 + (x + 0)] = data[(15) * 16 + (x + 0)];
                        result[(15) * 16 + (x + 1)] = data[(15) * 16 + (x + 1)];
                    }
                    break;

                case 2:
                    for (int x = 0; x < 16; x += 2)
                    {
                        for (int y = 0; y < (x + 2); y += 2)
                        {
                            result[(y + 0 + (14 - x)) * 16 + (x + 0)] = data[(y + 0) * 16 + (x + 0)];
                            result[(y + 0 + (14 - x)) * 16 + (x + 1)] = data[(y + 0) * 16 + (x + 1)];
                            result[(y + 1 + (14 - x)) * 16 + (x + 0)] = data[(y + 1) * 16 + (x + 0)];
                            result[(y + 1 + (14 - x)) * 16 + (x + 1)] = data[(y + 1) * 16 + (x + 1)];
                        }
                        result[(14) * 16 + (x + 0)] = data[(14) * 16 + (x + 0)];
                        result[(14) * 16 + (x + 1)] = data[(14) * 16 + (x + 1)];
                        result[(15) * 16 + (x + 0)] = data[(15) * 16 + (x + 0)];
                        result[(15) * 16 + (x + 1)] = data[(15) * 16 + (x + 1)];
                    }
                    break;

                case 3:
                    for (int x = 0; x < 16; x += 2)
                    {
                        result[(0) * 16 + (x + 0)] = data[(0) * 16 + (x + 0)];
                        result[(0) * 16 + (x + 1)] = data[(0) * 16 + (x + 1)];
                        result[(1) * 16 + (x + 0)] = data[(1) * 16 + (x + 0)];
                        result[(1) * 16 + (x + 1)] = data[(1) * 16 + (x + 1)];
                        for (int y = 2; y < (16 - x); y += 2)
                        {
                            result[(y + 0) * 16 + (x + 0)] = data[(y + 0 + x) * 16 + (x + 0)];
                            result[(y + 0) * 16 + (x + 1)] = data[(y + 0 + x) * 16 + (x + 1)];
                            result[(y + 1) * 16 + (x + 0)] = data[(y + 1 + x) * 16 + (x + 0)];
                            result[(y + 1) * 16 + (x + 1)] = data[(y + 1 + x) * 16 + (x + 1)];
                        }
                    }
                    break;

                case 4:
                    for (int x = 0; x < 16; x += 2)
                    {
                        result[(0) * 16 + (x + 0)] = data[(0) * 16 + (x + 0)];
                        result[(0) * 16 + (x + 1)] = data[(0) * 16 + (x + 1)];
                        result[(1) * 16 + (x + 0)] = data[(1) * 16 + (x + 0)];
                        result[(1) * 16 + (x + 1)] = data[(1) * 16 + (x + 1)];
                        for (int y = 2; y < (x + 2); y += 2)
                        {
                            result[(y + 0) * 16 + (x + 0)] = data[(y + 0 + (14 - x)) * 16 + (x + 0)];
                            result[(y + 0) * 16 + (x + 1)] = data[(y + 0 + (14 - x)) * 16 + (x + 1)];
                            result[(y + 1) * 16 + (x + 0)] = data[(y + 1 + (14 - x)) * 16 + (x + 0)];
                            result[(y + 1) * 16 + (x + 1)] = data[(y + 1 + (14 - x)) * 16 + (x + 1)];
                        }
                    }
                    break;

                default:
                    result = data;
                    break;
            }
            return result;
        }
    }
}