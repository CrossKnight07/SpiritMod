﻿using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Pet;
using SpiritMod.Items.Sets.BloodcourtSet;
using SpiritMod.Projectiles.DonatorItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems
{
	public class DemonTail : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Demon Tail");
			Tooltip.SetDefault("Summons an eldritch abomination to follow you");
		}

		public override void SetDefaults()
		{
			Item.UseSound = SoundID.Item2;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.width = 22;
			Item.height = 32;
			Item.value = Item.sellPrice(0, 0, 54, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.noMelee = true;
			Item.buffType = ModContent.BuffType<LoomingPresence>();
			Item.shoot = ModContent.ProjectileType<DemonicBlob>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BlackLens);
			recipe.AddIngredient(ItemID.WaterCandle);
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}
