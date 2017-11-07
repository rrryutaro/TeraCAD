using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using ReLogic.OS;

namespace TeraCAD.UIElements
{
    class UITextBox : UIElement
    {
        public static int startChatLine = 0;
        public static int showCount = 10;

        UITextPanel<string> numberBox = new UITextPanel<string>("");
        private bool Focus;

        public override void Click(UIMouseEvent evt)
        {
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

                string input = Main.GetInputText(numberBox.Text);
                if (Main.inputText == Main.oldInputText)
                    return;

                if (input == "")
                {
                    numberBox.SetText("");
                    Main.PlaySound(Terraria.ID.SoundID.MenuTick);
                }

                if (Main.inputTextEnter || Main.inputTextEscape)
                {
                    Focus = false;
                    Main.hasFocus = false;
                    Main.blockInput = false;
                }
            }

            base.Update(gameTime);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            TextSnippet[] array = null;

            CalculatedStyle dimensions = base.GetInnerDimensions();


            //spriteBatch.Draw(Main.textBackTexture, dimensions.Position(), new Rectangle?(new Rectangle(0, 0, Main.textBackTexture.Width - 100, Main.textBackTexture.Height)), new Color(100, 100, 100, 100), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.textBackTexture, dimensions.Position(), new Rectangle?(new Rectangle(0, 0, Main.textBackTexture.Width, Main.textBackTexture.Height)), new Color(100, 100, 100, 100), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

            string text = Main.chatText;
            List<TextSnippet> list = ChatManager.ParseMessage(text, Microsoft.Xna.Framework.Color.White);
            string compositionString = Platform.Current.Ime.CompositionString;
            if (compositionString != null && compositionString.Length > 0)
            {
                list.Add(new TextSnippet(compositionString, new Microsoft.Xna.Framework.Color(255, 240, 20), 1f));
            }
            array = list.ToArray();

            int num2 = -1;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, array, dimensions.Position(), 0f, Vector2.Zero, Vector2.One, out num2, -1f, 2f);
        }
    }
}