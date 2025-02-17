﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.MoonlightSack
{
	public class Moonlight_Sack_Player : ModPlayer
	{
		public bool isEquipped;
		public int projectileTimer = 0;

		public override void ResetEffects() => isEquipped = false;

		public override void PostUpdate()
		{
			if (isEquipped)
			{
				projectileTimer++;
				for (int i = 0; i < Main.projectile.Length; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (Player.DistanceSQ(projectile.Center) <= 300 * 300 && projectile.owner == Player.whoAmI && projectile.active && projectile.minion && projectile.type != ModContent.ProjectileType<Moonlight_Sack_Lightning>())
					{		
						if (projectileTimer % 5 == 0)
						{
							Vector2 vel = Vector2.Normalize(projectile.Center - projectile.Center) * 8f;
							int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center.X, Player.Center.Y, vel.X, vel.Y, ModContent.ProjectileType<Moonlight_Sack_Lightning>(), (int)(Player.GetDamage(DamageClass.Summon).ApplyTo(12)), 1f, Player.whoAmI, 0.0f, 0.0f);
							Main.projectile[p].ai[0] = projectile.whoAmI;
						}
					}
				}
			}
		}

	}
}
