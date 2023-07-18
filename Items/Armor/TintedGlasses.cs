using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Armor.SummerVanity;

[AutoloadEquip(EquipType.Head)]
public class TintedGlasses : ModItem 
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Tinted Glasses");
        Tooltip.SetDefault("'They help sell the look, so it's best to keep them on even when it's dark.'");
		ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 20;
		Item.value = Item.buyPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}
}
