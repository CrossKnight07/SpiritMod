using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Bullet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Partystarter
{
	public class PartyStarter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Partystarter");
			Tooltip.SetDefault("'Let's get this party started!'\nConverts regular bullets into VIP party bullets");
		}

		public override void SetDefaults()
		{
			Item.damage = 70;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 84;
			Item.height = 28;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 12;
			Item.useTurn = false;
			Item.value = Item.buyPrice(0, 19, 99, 0);
			Item.rare = ItemRarityID.Pink;
			Item.crit = 10;
			Item.UseSound = SoundID.Item40;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<PartyStarterBullet>();
			Item.shootSpeed = 17f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 70f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (type == ProjectileID.Bullet)
				type = ModContent.ProjectileType<PartyStarterBullet>();
		}
	}
}