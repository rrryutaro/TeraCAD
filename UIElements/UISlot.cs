using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using System;

namespace TeraCAD.UIElements
{
    public class UISlot : UIElement
    {
        public static Texture2D defaultBackgroundTexture = Main.inventoryBack9Texture;
        public Texture2D backgroundTexture = defaultBackgroundTexture;
        public static float scale = 1.0f;
        public Texture2D texture;
        public int sortOrder;
        public bool isSelect;
        public string tooltip;

        protected SpriteEffects effects = SpriteEffects.None;

        public UISlot(Texture2D texture, int sortOrder, string tooltip)
        {
            this.texture = texture;
            this.sortOrder = sortOrder;
            this.tooltip = tooltip;
            SetSlotSize();
        }

        private void SetSlotSize()
        {
            int size = Math.Max(texture.Width, texture.Height);
            this.Width.Set(size * scale, 0f);
            this.Height.Set(size * scale, 0f);
        }

        public override void Recalculate()
        {
            SetSlotSize();
            base.Recalculate();
        }

        public override int CompareTo(object obj)
        {
            int result = sortOrder < (obj as UISlot).sortOrder ? -1 : 1;
            return result;
        }

        public virtual UISlot Clone()
        {
            UISlot result = new UISlot(texture, sortOrder, tooltip);
            return result;
        }

        protected static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = base.GetInnerDimensions();
            Rectangle rectangle = dimensions.ToRectangle();

            Rectangle value = new Rectangle(0, 0, 1, 1);
            float r = 1f;
            if (isSelect)
                r = .25f;
            float g = 0.9f;
            float b = 0.1f;
            float a = 1f;
            //a = .2f;
            float scale2 = 0.6f;
            Color color = buffColor(Color.White, r, g, b, a);
            //Main.spriteBatch.Draw(Main.magicPixel, dimensions.Position, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero,  , SpriteEffects.None, 0f);

            if (isSelect || IsMouseHovering)
                spriteBatch.Draw(Main.magicPixel, dimensions.ToRectangle(), color * scale2);

            spriteBatch.Draw(texture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, effects, 0f);

            if (isSelect || IsMouseHovering)
            {
                b = 0.3f;
                g = 0.95f;
                scale2 = (a = 1f);
                color = buffColor(Color.White, r, g, b, a);
                //Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * 16f * brushSize.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * 16f * brushSize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
                spriteBatch.Draw(Main.magicPixel, new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, 2), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle((int)dimensions.X, (int)dimensions.Y, 2, (int)dimensions.Height), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle((int)dimensions.X + (int)dimensions.Width - 2, (int)dimensions.Y, 2, (int)dimensions.Height), color * scale2);
                spriteBatch.Draw(Main.magicPixel, new Rectangle((int)dimensions.X, (int)dimensions.Y + (int)dimensions.Height - 2, (int)dimensions.Width, 2), color * scale2);
            }
            if (IsMouseHovering)
                ToolBox.tooltip = tooltip;
        }
    }
}
