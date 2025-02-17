using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Bullet.Crimbine;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.TerraGunTree
{
	public class Crimbine : ModItem, ITimerItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harvester");
			Tooltip.SetDefault("Converts regular bullets into bones\nRight-click to shoot a slow-moving bloody amalgam\nShooting the bloody amalgam creates an explosion of organs with different effects\n5 second cooldown");
		}

		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 58;
			Item.height = 32;
			Item.useTime = 9;
			Item.useAnimation = 9;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.useTurn = false;
			Item.shoot = ModContent.ProjectileType<CrimbineBone>();
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.shootSpeed = 10f;
			Item.autoReuse = true;
			Item.useAmmo = AmmoID.Bullet;
			Item.crit = 6;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool AltFunctionUse(Player player) => player.ItemTimer<Crimbine>() <= 0;

		public override void HoldItem(Player player)
		{
			if (player.ItemTimer<Crimbine>() == 1) 
			{
				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(SoundID.MaxMana);

				for (int index1 = 0; index1 < 5; ++index1) 
				{
					int index2 = Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0.0f, 0.0f, (int)byte.MaxValue, new Color(), (float)Main.rand.Next(20, 26) * 0.1f);
					Main.dust[index2].noLight = false;
					Main.dust[index2].noGravity = true;
					Main.dust[index2].velocity *= 0.5f;
				}
			}
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (player.altFunctionUse == 2)
			{
				player.SetItemTimer<Crimbine>(300);

				type = ModContent.ProjectileType<CrimbineAmalgam>();
				velocity /= 4;
			}
			else
			{
				Item.shootSpeed = 10f;
				float spread = 8 * 0.0174f;//45 degrees converted to radians
				float baseSpeed = (float)velocity.Length();
				double baseAngle = Math.Atan2(velocity.X, velocity.Y);
				double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
				velocity.X = baseSpeed * (float)Math.Sin(randomAngle);
				velocity.Y = baseSpeed * (float)Math.Cos(randomAngle);

				if (type == ProjectileID.Bullet)
					type = ModContent.ProjectileType<CrimbineBone>();
			}
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
				SoundEngine.PlaySound(SoundID.Item95);
			else
				SoundEngine.PlaySound(SoundID.Item11);

			return base.UseItem(player);
		}

		public int TimerCount() => 1;

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Boomstick);
			recipe.AddIngredient(ItemID.TheUndertaker);
			recipe.AddIngredient(ItemID.Handgun, 1);
			recipe.AddIngredient(ModContent.ItemType<CoilSet.CoilPistol>(), 1);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}