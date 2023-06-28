using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.SteamRaider
{
	public class LaserBase : ModNPC
	{
		Vector2 direction9 = Vector2.Zero;
		//private bool shooting;
		//private bool inblock = true;
		Vector2 target = Vector2.Zero;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser Launcher");
			Main.npcFrameCount[NPC.type] = 1;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults()
		{
			NPC.width = 56;
			NPC.height = 46;
			NPC.damage = 0;
			NPC.defense = 12;
			NPC.noTileCollide = true;
			NPC.dontTakeDamage = true;
			NPC.lifeMax = 65;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.value = 160f;
			NPC.knockBackResist = .16f;
			NPC.noGravity = true;
			NPC.dontCountMe = true;
		}


		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame,
				 drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}
		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (NPC.alpha != 255)
				GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/Boss/SteamRaider/LaserBase_Glow").Value, screenPos);
		}

		public override bool PreAI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;
			if (NPC.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[1] = Main.rand.Next(160, 190);
				NPC.netUpdate = true;
			}

			if (++NPC.ai[0] >= NPC.ai[1])
			{
				SoundEngine.PlaySound(SoundID.Item110, NPC.Center);
				for (int i = 0; i < 40; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, -2f, 117, new Color(0, 255, 142), .6f);
					Main.dust[num].noGravity = true;
					Dust dust = Main.dust[num];
					dust.position.X += (Main.rand.Next(-50, 51) / 20) - 1.5f;
					dust.position.Y += (Main.rand.Next(-50, 51) / 20) - 1.5f;
					if (Main.dust[num].position != NPC.Center)
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 3f;
				}

				if (Main.expertMode)
					NPC.Transform(ModContent.NPCType<SuicideLaser>());
				else
					NPC.active = false;

				NPC.netUpdate = true;
			}
			else
			{
				NPC.velocity.X = 0;
				NPC.velocity.Y = 0;
			}

			if (NPC.ai[0] <= 75)
			{
				direction9 = player.Center - NPC.Center;
				direction9.Normalize();
			}
			if (NPC.ai[0] >= 60 && NPC.ai[0] <= 110 & NPC.ai[0] % 2 == 0)
			{
				{
					int dust = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.Electric);
					Main.dust[dust].velocity *= -1f;
					Main.dust[dust].scale *= .8f;
					Main.dust[dust].noGravity = true;
					Vector2 vector2_1 = new Vector2((float)Main.rand.Next(-80, 81), (float)Main.rand.Next(-80, 81));
					vector2_1.Normalize();
					Vector2 vector2_2 = vector2_1 * ((float)Main.rand.Next(50, 100) * 0.04f);
					Main.dust[dust].velocity = vector2_2;
					vector2_2.Normalize();
					Vector2 vector2_3 = vector2_2 * 34f;
					Main.dust[dust].position = NPC.Center - vector2_3;
				}
			}
			if (NPC.alpha != 255)
			{
				if (Main.rand.NextFloat() < 0.5f)
				{
					Vector2 position = new Vector2(NPC.Center.X - 10, NPC.Center.Y);
					Dust.NewDustPerfect(position, 226, new Vector2(0f, -6.421053f).RotatedBy(NPC.rotation), 0, new Color(255, 0, 0), 0.6578947f);
				}
				if (Main.rand.NextFloat() < 0.5f)
				{
					Vector2 position = new Vector2(NPC.Center.X + 10, NPC.Center.Y);
					Dust.NewDustPerfect(position, 226, new Vector2(0f, -6.421053f).RotatedBy(NPC.rotation), 0, new Color(255, 0, 0), 0.6578947f);
				}

				if (NPC.ai[0] == 110) //change to frame related later
				{
					SoundEngine.PlaySound(SoundID.NPCHit53, NPC.Center);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)direction9.X * 40, (float)direction9.Y * 40, ModContent.ProjectileType<StarLaser>(), NPCUtils.ToActualDamage(55, 1.5f, 2f), 1, Main.myPlayer);
				}
				if (NPC.ai[0] < 110 && NPC.ai[0] > 75 && NPC.ai[0] % 3 == 0)
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)direction9.X * 30, (float)direction9.Y * 30, ModContent.ProjectileType<StarLaserTrace>(), NPCUtils.ToActualDamage(27, 1.5f, 2f), 1, Main.myPlayer);
				NPC.rotation = direction9.ToRotation() - 1.57f;
			}
			return false;
		}
	}
}
