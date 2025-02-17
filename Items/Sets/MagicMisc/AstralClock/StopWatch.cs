using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using SpiritMod.Projectiles.Magic;
using SpiritMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.MagicMisc.AstralClock
{
	public class StopWatch : ModItem, ITimerItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Clock");
			Tooltip.SetDefault("Freezes time in a radius around the player \n60 second cooldown");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.mana = 100;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<Clock>();
			Item.shootSpeed = 0f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			MyPlayer modPlayer = player.GetSpiritPlayer();
			player.SetItemTimer<StopWatch>(3600);
			player.AddBuff(ModContent.BuffType<ClockBuff>(), 200);

			modPlayer.clockX = (int)position.X;
			modPlayer.clockY = (int)position.Y;

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override bool CanUseItem(Player player) => player.ItemTimer<StopWatch>() <= 0;

		public int TimerCount() => 1;
	}
}
