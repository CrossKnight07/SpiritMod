﻿using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	public class OrichHoming : ModProjectile, ITrailProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Orichalcum Bloom");

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false; 
			Projectile.penetrate = 1;
			Projectile.timeLeft = 36;
			Projectile.extraUpdates = 1;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
		}

		public void DoTrailCreation(TrailManager tManager) => tManager.CreateTrail(Projectile, new GradientTrail(new Color(182, 66, 245), new Color(105, 42, 168)), new RoundCap(), new DefaultTrailPosition(), 20f, 150f);

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), 0.396f, 0.170588235f, 0.564705882f);
			bool flag25 = false;
			int jim = 1;
			float maxdist = 300f;
			for (int index1 = 0; index1 < 200; index1++) {
				if (Main.npc[index1].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1)) {
					float num23 = Main.npc[index1].position.X + (float)(Main.npc[index1].width / 2);
					float num24 = Main.npc[index1].position.Y + (float)(Main.npc[index1].height / 2);
					float num25 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num23) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num24);
					if (num25 < maxdist) {
						maxdist = Projectile.Distance(Main.npc[jim].Center);
						flag25 = true;
						jim = index1;
					}

				}
			}

			if (flag25) {

				Projectile.timeLeft++;
				float num1 = 6.5f;
				Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num2 = Main.npc[jim].Center.X - vector2.X;
				float num3 = Main.npc[jim].Center.Y - vector2.Y;
				float num4 = (float)Math.Sqrt((double)num2 * (double)num2 + (double)num3 * (double)num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;
				Projectile.velocity.X = (Projectile.velocity.X * (float)(num8 - 1) + num6) / (float)num8;
				Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num8 - 1) + num7) / (float)num8;
			}

			Projectile.ai[1] += 1f;
			if (Projectile.ai[1] >= 7200f) {
				Projectile.alpha += 5;
				if (Projectile.alpha > 255) {
					Projectile.alpha = 255;
					Projectile.Kill();
				}
			}

			Projectile.rotation += 0.3f;

			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] >= 10f) {
				Projectile.localAI[0] = 0f;
				int num416 = 0;
				int num417 = 0;
				float num418 = 0f;
				int num419 = Projectile.type;
				for (int num420 = 0; num420 < 1000; num420++) {
					if (Main.projectile[num420].active && Main.projectile[num420].owner == Projectile.owner && Main.projectile[num420].type == num419 && Main.projectile[num420].ai[1] < 3600f) {
						num416++;
						if (Main.projectile[num420].ai[1] > num418) {
							num417 = num420;
							num418 = Main.projectile[num420].ai[1];
						}
					}
					if (num416 > 8) {
						Main.projectile[num417].netUpdate = true;
						Main.projectile[num417].ai[1] = 36000f;
						return;
					}
				}
			}
		}
	}
}
