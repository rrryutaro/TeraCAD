using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using TeraCAD.UIElements;
using TeraCAD.Tools;

namespace TeraCAD
{
    class ToolImage : Tool
	{
        public static ToolImage instance;
        private ImageUI ui;

		private List<UISlotImage> drawImageList = new List<UISlotImage>();
		private int step;

		public ToolImage() : base(typeof(ImageUI))
		{
            instance = this;
			ui = uistate as ImageUI;
        }

		internal override void UIUpdate()
		{
			base.UIUpdate();

			if (UISlotImage.SelectedImage != null && Main.mouseLeft && step == 0 && !ToolBox.ContainsPoint(Main.MouseScreen))
			{
				step = 1;

				var slot = UISlotImage.SelectedImage.Clone();
				slot.position = Snap.GetSnapPoint(ToolBox.snapType, -slot.image.Width, -slot.image.Height);
				slot.mode = ui.ImagePositionMode;
				if (slot.mode == ImagePositionMode.Screen)
					slot.position -= Main.screenPosition;
				slot.transmittance = ImageUI.instance.Transmittance;
				drawImageList.Add(slot);
				
			}
			if (UISlotImage.SelectedImage != null && !Main.mouseLeft && step == 1)
				step = 0;
		}
	}
}
