using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace SpiritMod.Mechanics.AutoSell.Sell_NoValue
{
	public class Sell_NoValue : UIState
	{
		public static bool visible = false;

		public override void OnInitialize()
		{
			var buttonPlayTexture = SpiritModAutoSellTextures.sellNoValueButton;
			UIImageButton playButton = new UIImageButton(buttonPlayTexture);
			playButton.Left.Set(502, 0f);
			playButton.Top.Set(400, 0f);
			playButton.Width.Set(32, 0f);
			playButton.Height.Set(32, 0f);
			playButton.OnClick += new MouseEvent(PlayButtonClicked);

			Append(playButton);
		}

		private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			Player player = Main.LocalPlayer;
			if (!player.GetModPlayer<AutoSellPlayer>().sell_NoValue)
			{
				player.GetModPlayer<AutoSellPlayer>().sell_NoValue = true;
				SoundEngine.PlaySound(SoundID.MenuOpen);
			}
			else
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.GetModPlayer<AutoSellPlayer>().sell_NoValue = false;
			}
		}
	}
}