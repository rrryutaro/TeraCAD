using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace TeraCAD.UIElements
{
    internal class UISlider : UIElement
    {
		public float Value { get; set; }
		public bool isDown;

		public UISlider() : base()
		{
			Width.Set(Main.colorBarTexture.Width, 0f);
			Height.Set(Main.colorBarTexture.Height, 0f);
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);
			isDown = true;
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);
			isDown = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (isDown)
			{
				var dim = GetDimensions().Position();
				var pos = Main.MouseScreen;
				if (pos.X <= dim.X)
					Value = 0f;
				else if (dim.X + Main.colorBarTexture.Width <= pos.X)
					Value = 1f;
				else
					Value = (float)Math.Round(1f - (Main.colorBarTexture.Width - (pos.X - dim.X)) / Main.colorBarTexture.Width, 2);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
			IngameOptions.valuePosition = GetDimensions().Position().Offset(Main.colorBarTexture.Width, Main.colorBarTexture.Height);
			IngameOptions.DrawValueBar(spriteBatch, 1f, Value);
        }
    }
}
