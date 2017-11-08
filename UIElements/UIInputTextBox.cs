using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using ReLogic.OS;

namespace TeraCAD.UIElements
{
    class UIInputTextBox : UIElement
    {
        public static int startChatLine = 0;
        public static int showCount = 10;

		public int maxLength;
		public bool isNumberOnly;

        UITextPanel<string> textBox = new UITextPanel<string>("");
        private bool Focus;
		private string inputText = "";
		private const int keyRepeatIntervalDefault = 10;
		private int keyRepeatInterval;
		private int keyRepeatIntervalCount;
		private bool isKeyDown;

		private Texture2D backTexture = Main.textBackTexture;
		private Texture2D backTextureLeft;
		private Texture2D backTextureCenter;
		private Texture2D backTextureRight;
		private Color colorActive = Color.White;
		private Color colorInactive = Color.White * 0.4f;

		private Regex regNumber;

		public string Text
		{
			get { return inputText; }
			set { inputText = value; }
		}

		public UIInputTextBox(int width, int maxLength = 100, bool isNumberOnly = false)
		{
			this.maxLength = maxLength;
			this.isNumberOnly = isNumberOnly;
			if (isNumberOnly)
			{
				regNumber = new Regex(@"[^0-9]");
			}

			this.Width.Set(width + 20, 0f);
			this.Height.Set(backTexture.Height, 0f);

			backTextureLeft = backTexture.Offset(0, 0, 10, 32);
			backTextureRight = backTexture.Offset(backTexture.Width - 10, 0, 10, 32);
			backTextureCenter = backTexture.Offset(10, 0, width, 32);
		}

		public override void Click(UIMouseEvent evt)
        {
			base.Click(evt);
            Focus = true;
            Main.blockInput = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (Focus)
            {
                Main.hasFocus = true;
                Main.chatRelease = false;
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();

				string input = Main.GetInputText("");

				//数値のみの場合
				if (isNumberOnly)
				{
					input = regNumber.Replace(input, "");
					if (inputText.Length == 0 && input.Equals("0"))
						input = "";
				}

				//最大文字数
				inputText += input;
				if (maxLength < inputText.Length)
				{
					inputText = inputText.Substring(0, maxLength);
				}

				//1文字削除
				if (Main.keyState.IsKeyDown(Keys.Back))
				{
					if (!isKeyDown)
					{
						keyRepeatIntervalCount = keyRepeatInterval = keyRepeatIntervalDefault;
						isKeyDown = true;
					}
					if (keyRepeatInterval < ++keyRepeatIntervalCount)
					{
						if (0 < keyRepeatInterval)
							--keyRepeatInterval;
						keyRepeatIntervalCount = 0;

						if (0 < inputText.Length)
							inputText = inputText.Substring(0, inputText.Length - 1);
					}
				}
				else if (Main.keyState.IsKeyUp(Keys.Back))
				{
					isKeyDown = false;
				}

				//入力終了
				if (Main.inputTextEnter || Main.inputTextEscape)
                {
					InputEnd();
                }
            }

            base.Update(gameTime);
        }

		public void InputEnd()
		{
			Focus = false;
			isKeyDown = false;

			Main.hasFocus = false;
			Main.blockInput = false;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
        {
			CalculatedStyle dimensions = base.GetInnerDimensions();
			Vector2 pos = dimensions.Position();

			Color color;
			if (Focus)
				color = colorActive;
			else
				color = colorInactive;

			spriteBatch.Draw(backTextureLeft, pos, new Rectangle(0, 0, backTextureLeft.Width, backTextureLeft.Height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(backTextureCenter, pos.Offset(backTextureLeft.Width, 0), new Rectangle(0, 0, backTextureCenter.Width, backTextureCenter.Height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(backTextureRight, pos.Offset(backTextureLeft.Width + backTextureCenter.Width, 0), new Rectangle(0, 0, backTextureRight.Width, backTextureRight.Height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			pos = pos.Offset(10, 6);
			if (Focus)
			{
				string text = Main.chatText;
				List<TextSnippet> list = ChatManager.ParseMessage(text, Color.White);
				string compositionString = Platform.Current.Ime.CompositionString;
				if (compositionString != null && compositionString.Length > 0)
				{
					list.Add(new TextSnippet(compositionString, new Color(255, 240, 20), 1f));
				}
				TextSnippet[] array = list.ToArray();
				int num = -1;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, array, pos.Offset(Main.fontMouseText.MeasureString(inputText).X, 0), 0f, Vector2.Zero, Vector2.One, out num, -1f, 2f);
			}
			Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, inputText, pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
		}
    }
}