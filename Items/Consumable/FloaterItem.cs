using SpiritMod.NPCs.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class FloaterItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminous Floater");
			Tooltip.SetDefault("'A beacon at the beach'");
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 0, 3, 0);
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;
			Item.makeNPC = ModContent.NPCType<Floater1>();
		}
	}
}
