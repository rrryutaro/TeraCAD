using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;

namespace TeraCAD.UIElements
{
    internal class UISliderColor : UIElement
    {
		private static Texture2D texture = Main.colorBarTexture;
		public bool isDown;
		private ColorSlidersSet colorSet;
		private int downIndex;

		public event Action<UISliderColor> OnChanged;

		public Color color
		{
			get
			{
				return colorSet.GetColor();
			}
			set
			{
				colorSet.SetHSL(value);
			}
		}

		public UISliderColor() : base()
		{
			colorSet = new ColorSlidersSet();
			colorSet.SetHSL(Color.Red);
			Width.Set(texture.Width, 0f);
			Height.Set(texture.Height * 6, 0f);
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			//base.MouseDown(evt);
			downIndex = 0;
			Vector2 pos = GetDimensions().Position();
			Rectangle rect1 = new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height * 2);
			pos.Y += texture.Height * 2;
			Rectangle rect2 = new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height * 2);
			pos.Y += texture.Height * 2;
			Rectangle rect3 = new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height * 2);
			if (rect1.Contains((int)evt.MousePosition.X, (int)evt.MousePosition.Y))
				downIndex = 1;
			else if (rect2.Contains((int)evt.MousePosition.X, (int)evt.MousePosition.Y))
				downIndex = 2;
			else if (rect3.Contains((int)evt.MousePosition.X, (int)evt.MousePosition.Y))
				downIndex = 3;

			if (0 < downIndex)
				isDown = true;
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			//base.MouseUp(evt);
			downIndex = 0;
			isDown = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (isDown)
			{
				var dim = GetDimensions().Position();
				var pos = Main.MouseScreen;
				float value = 0f;
				if (pos.X <= dim.X)
					value = 0f;
				else if (dim.X + texture.Width <= pos.X)
					value = 1f;
				else
					value = (float)Math.Round(1f - (texture.Width - (pos.X - dim.X)) / texture.Width, 2);

				Vector3 hSLVector = colorSet.GetHSLVector();
				switch (downIndex)
				{
					case 1:
						hSLVector.X = value;
						break;
					case 2:
						hSLVector.Y = value;
						break;
					case 3:
						hSLVector.Z = value;
						break;
				}
				colorSet.SetHSL(hSLVector);

				Changed();
			}
		}

		public virtual void Changed()
		{
			if (this.OnChanged != null)
			{
				this.OnChanged(this);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
			Vector3 hSLVector = colorSet.GetHSLVector();
			DelegateMethods.v3_1 = hSLVector;

			Vector2 pos = GetDimensions().Position().Offset(texture.Width, texture.Height);
			IngameOptions.valuePosition = pos;
			IngameOptions.DrawValueBar(spriteBatch, 1f, hSLVector.X, 0, new Utils.ColorLerpMethod(DelegateMethods.ColorLerp_HSL_H));

			pos = pos.Offset(0, texture.Height * 2);
			IngameOptions.valuePosition = pos;
			IngameOptions.DrawValueBar(spriteBatch, 1f, hSLVector.Y, 0, new Utils.ColorLerpMethod(DelegateMethods.ColorLerp_HSL_S));

			pos = pos.Offset(0, texture.Height * 2);
			IngameOptions.valuePosition = pos;
			IngameOptions.DrawValueBar(spriteBatch, 1f, hSLVector.Z, 0, new Utils.ColorLerpMethod(DelegateMethods.ColorLerp_HSL_L));

			colorSet.SetHSL(hSLVector);
		}
    }
}
