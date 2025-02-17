using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Entities
{
	public class Visual_Projectile : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Purity Light");

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.hide = true;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			int index2 = Dust.NewDust(Projectile.Center, 8, 8, DustID.DungeonSpirit, 0.0f, 0.0f, 0, new Color(), 1f);
			Main.dust[index2].position = Projectile.Center;
			Main.dust[index2].velocity = Projectile.velocity;
			Main.dust[index2].noGravity = true;
			Main.dust[index2].scale = Projectile.scale;

			Player player = Main.player[Projectile.owner];
			float x = 0.15f;
			float y = 0.15f;

			Vector2 vector2_1 = Projectile.velocity + new Vector2((float)Math.Sign(player.Center.X - Projectile.Center.X), (float)Math.Sign(player.Center.Y - Projectile.Center.Y)) * new Vector2(x, y);
			Projectile.velocity = vector2_1;
			if ((double)Projectile.velocity.Length() > 4.0)
			{
				Vector2 vector2_2 = Projectile.velocity * (4f / Projectile.velocity.Length());
				Projectile.velocity = vector2_2;
			}
		}
	}
}