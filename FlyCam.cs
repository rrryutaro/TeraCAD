using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace TeraCAD
{
    internal class FlyCam
    {
        internal static Vector2 FlyCamPosition = Vector2.Zero;
        internal static bool Enabled { get; set; } = false;

        public FlyCam()
        {
        }

        public void Update()
        {
            if (Enabled && !Main.blockInput)
            {
                //move camera with arrow keys
                float speed = 30f;

                if (Main.keyState.IsKeyDown(Keys.LeftAlt)) speed *= .3f;
                if (Main.keyState.IsKeyDown(Keys.LeftShift)) speed *= 1.5f;

                if (PlayerInput.Triggers.Current.KeyStatus["Left"]) FlyCamPosition.X -= speed;
                if (PlayerInput.Triggers.Current.KeyStatus["Right"]) FlyCamPosition.X += speed;
                if (PlayerInput.Triggers.Current.KeyStatus["Up"]) FlyCamPosition.Y -= speed;
                if (PlayerInput.Triggers.Current.KeyStatus["Down"]) FlyCamPosition.Y += speed;

                //Vector2 size = new Vector2(Main.screenWidth, Main.screenHeight);
                //Main.screenPosition = FlyCamPosition - size / 2;

                //float x = (Main.mouseX + Main.screenPosition.X) / 16f;
                //float y = (Main.mouseY + Main.screenPosition.Y) / 16f;
                Player player = Main.player[Main.myPlayer];

                //if player right clicks, move their character to that position.
                if (!Main.mapFullscreen && !player.mouseInterface && PlayerInput.Triggers.Current.KeyStatus["Jump"])
                {
                    Vector2 cursorPosition = new Vector2(Main.mouseX - player.width / 2, Main.mouseY - player.height);
                    Vector2 cursorWorldPosition = Main.screenPosition + cursorPosition;

                    int mapWidth = Main.maxTilesX * 16;
                    int mapHeight = Main.maxTilesY * 16;
                    if (cursorWorldPosition.X < 0) cursorWorldPosition.X = 0;
                    else if (cursorWorldPosition.X + player.width > mapWidth) cursorWorldPosition.X = mapWidth - player.width;
                    if (cursorWorldPosition.Y < 0) cursorWorldPosition.Y = 0;
                    else if (cursorWorldPosition.Y + player.height > mapHeight) cursorWorldPosition.Y = mapHeight - player.height;

                    player.position = cursorWorldPosition;
                    player.velocity = Vector2.Zero;
                    player.fallStart = (int)(player.position.Y / 16f);
                }
            }
            else
            {
                //Vector2 size = new Vector2(Main.screenWidth, Main.screenHeight);
                FlyCamPosition = Main.screenPosition;// + size / 2;
            }
        }
    }

    public class FlyCamModPlayer : ModPlayer
    {
        public override bool Autoload(ref string name) => true;

        public override void ModifyScreenPosition()
        {
            if (FlyCam.Enabled)
            {
                Main.screenPosition = FlyCam.FlyCamPosition;
            }
        }

        public override void SetControls()
        {
            if (FlyCam.Enabled)
            {
                player.controlDown = false;
                player.controlUp = false;
                player.controlLeft = false;
                player.controlRight = false;

                player.controlMount = false;
                player.controlHook = false;
                player.controlThrow = false;
                player.controlJump = false;
                player.controlSmart = false;
                player.controlTorch = false;
            }
        }
    }
}
