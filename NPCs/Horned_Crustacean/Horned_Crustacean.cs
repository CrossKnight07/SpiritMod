using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using SpiritMod.Items.Consumable.Fish;
using SpiritMod.Items.Sets.ReefhunterSet;
using SpiritMod.Mechanics.QuestSystem;
using Terraria.ModLoader.Utilities;
using SpiritMod.Items.Weapon.Magic.LuminanceSeacone;
using Terraria.GameContent.Bestiary;
using System.Reflection;

namespace SpiritMod.NPCs.Horned_Crustacean
{
	public class Horned_Crustacean : ModNPC
	{
		public bool hasGottenColor = false;
		public int r = 0;
		public int g = 0;
		public int b = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminous Prowler");
			Main.npcFrameCount[NPC.type] = 10;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Position = new Vector2(-10, -10),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.aiStyle = -1;
			NPC.lifeMax = 40;
			NPC.defense = 5;
			NPC.value = 200f;
			NPC.knockBackResist = 0.9f;
			NPC.width = 20;
			NPC.height = 40;
			NPC.damage = 30;
			NPC.lavaImmune = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit31;
			NPC.dontTakeDamage = false;
			NPC.DeathSound = SoundID.NPCDeath32;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Despite its size, this sea dweller packs a punch! It uses its sharp teeth-like spikes to hunt for fish and keep away intruders."),
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(r);
			writer.Write(g);
			writer.Write(b);
			writer.Write(hasGottenColor);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			r = reader.ReadInt32();
			g = reader.ReadInt32();
			b = reader.ReadInt32();
			hasGottenColor = reader.ReadBoolean();
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];

			if (NPC.velocity.X < 0f)
				NPC.spriteDirection = -1;
			else if (NPC.velocity.X > 0f)
				NPC.spriteDirection = 1;

			if (Vector2.Distance(player.Center, NPC.Center) <= 45f)
				NPC.velocity.X = 0f;

			if (NPC.wet && !player.wet)
			{
				NPC.noGravity = true;
				NPC.aiStyle = 16;
				AIType = NPCID.Goldfish;
				NPC.TargetClosest(false);
			}
			else
			{
				NPC.noGravity = false;
				NPC.aiStyle = 0;
			}

			if (player.wet && NPC.wet)
				Movement();

			if (!hasGottenColor)
			{
				hasGottenColor = true;
				r = Main.rand.Next(1, 255);
				g = Main.rand.Next(1, 255);
				b = Main.rand.Next(1, 255);
			}

			Lighting.AddLight(NPC.Center, r * 0.002f, g * 0.002f, b * 0.002f);

			if (!player.wet)
			{
				for (int i = 0; i < Main.projectile.Length; i++)
				{
					Projectile type = Main.projectile[i];

					if (Vector2.Distance(type.Center, NPC.Center) <= 100f && Vector2.Distance(type.Center, NPC.Center) > 20f && type.friendly && type.position.X > NPC.position.X && NPC.wet && type.active)
					{
						Vector2 vector2 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
						float num2 = type.position.X + Main.rand.Next(-10, 10) + (float)(type.width / 2) - vector2.X;
						float num3 = type.position.Y + Main.rand.Next(-10, 10) + (float)(type.height / 2) - vector2.Y;
						float num4 = 8f / (float)Math.Sqrt(num2 * num2 + num3 * num3);
						NPC.velocity.X = num2 * num4 * -1 * (5f / 6);
						NPC.velocity.Y = num3 * num4 * -1 * (5f / 6);
						NPC.spriteDirection = -1;
						NPC.direction = -1;
					}
					else if (Vector2.Distance(type.Center, NPC.Center) <= 100f && Vector2.Distance(type.Center, NPC.Center) > 20f && type.friendly && type.position.X < NPC.position.X && NPC.wet && type.active)
					{
						Vector2 vector2 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
						float num2 = type.position.X + Main.rand.Next(-10, 10) + (float)(type.width / 2) - vector2.X;
						float num3 = type.position.Y + Main.rand.Next(-10, 10) + (float)(type.height / 2) - vector2.Y;
						float num4 = 8f / (float)Math.Sqrt(num2 * num2 + num3 * num3);
						NPC.velocity.X = num2 * num4 * -1 * (5f / 6);
						NPC.velocity.Y = num3 * num4 * -1 * (5f / 6);
						NPC.spriteDirection = 1;
						NPC.direction = 1;
					}
				}
			}
		}

		private void Movement()
		{
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			if (!NPC.noTileCollide)
			{
				if (NPC.collideX)
				{
					NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
					if (NPC.direction == -1 && NPC.velocity.X > 0 && NPC.velocity.X < 2)
						NPC.velocity.X = 2f;

					if (NPC.direction == 1 && NPC.velocity.X < 0 && NPC.velocity.X > -2)
						NPC.velocity.X = -2f;
				}

				if (NPC.collideY)
				{
					NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
					if (NPC.velocity.Y > 0 && NPC.velocity.Y < 1)
						NPC.velocity.Y = 1f;

					if (NPC.velocity.Y < 0 && NPC.velocity.Y > -1)
						NPC.velocity.Y = -1f;
				}
			}

			NPC.TargetClosest(true);

			if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
			{
				if (NPC.ai[1] > 0 && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
				{
					NPC.ai[1] = 0.0f;
					NPC.ai[0] = 0.0f;
					NPC.netUpdate = true;
				}
			}
			else if (NPC.ai[1] == 0)
				++NPC.ai[0];

			if (NPC.ai[0] >= 300)
			{
				NPC.ai[1] = 1f;
				NPC.ai[0] = 0.0f;
				NPC.netUpdate = true;
			}

			if (NPC.ai[1] == 0)
			{
				NPC.alpha = 0;
				NPC.noTileCollide = false;
			}
			else
			{
				NPC.alpha = 200;
				NPC.noTileCollide = true;
			}

			if (NPC.direction == -1 && NPC.velocity.X > -4 && NPC.position.X > Main.player[NPC.target].position.X + Main.player[NPC.target].width)
			{
				NPC.velocity.X -= 0.08f;

				if (NPC.velocity.X > 4)
					NPC.velocity.X -= 0.04f;
				else if (NPC.velocity.X > 0.0)
					NPC.velocity.X -= 0.2f;

				if (NPC.velocity.X < -4)
					NPC.velocity.X = -4f;
			}
			else if (NPC.direction == 1 && NPC.velocity.X < 4 && NPC.position.X + NPC.width < Main.player[NPC.target].position.X)
			{
				NPC.velocity.X += 0.08f;

				if (NPC.velocity.X < -4)
					NPC.velocity.X += 0.04f;
				else if (NPC.velocity.X < 0.0)
					NPC.velocity.X += 0.2f;

				if (NPC.velocity.X > 4)
					NPC.velocity.X = 4f;
			}
			if (NPC.directionY == -1 && NPC.velocity.Y > -4 && NPC.position.Y > Main.player[NPC.target].position.Y + Main.player[NPC.target].height)
			{
				NPC.velocity.Y -= 0.1f;

				if (NPC.velocity.Y > 4)
					NPC.velocity.Y -= 0.05f;
				else if (NPC.velocity.Y > 0.0)
					NPC.velocity.Y -= 0.15f;

				if (NPC.velocity.Y < -4)
					NPC.velocity.Y = -4f;
			}
			else if (NPC.directionY == 1 && NPC.velocity.Y < 4 && NPC.position.Y + NPC.height < Main.player[NPC.target].position.Y)
			{
				NPC.velocity.Y += 0.1f;

				if (NPC.velocity.Y < -4)
					NPC.velocity.Y += 0.05f;
				else if (NPC.velocity.Y < 0.0)
					NPC.velocity.Y += 0.15f;

				if (NPC.velocity.Y > 4)
					NPC.velocity.Y = 4f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<RawFish>(2);
			npcLoot.AddCommon<LuminanceSeacone>(25);
			npcLoot.AddCommon<IridescentScale>(1, 3, 5);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
				for (int index1 = 0; index1 < 13; ++index1)
				{
					int index2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, new Color(r, g, b), 2.5f);
					Main.dust[index2].noGravity = true;
					Main.dust[index2].fadeIn = 1f;
					Main.dust[index2].velocity *= 4f;
					Main.dust[index2].noLight = true;
				}
			}

			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, 2.5f * hitDirection, -2.5f, 0, new Color(r, g, b), 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, 2.5f * hitDirection, -2.5f, 0, new Color(r, g, b), 0.5f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.AncientLight, 2.5f * hitDirection, -2.5f, 0, new Color(r, g, b), 0.7f);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.OceanMonster.Chance * 0.08f;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = TextureAssets.Npc[Type].Value;
			Vector2 drawOrigin = new Vector2(texture.Width, texture.Height / Main.npcFrameCount[Type]) / 2;
			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effects, 0);

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D glow = Mod.Assets.Request<Texture2D>("NPCs/Horned_Crustacean/Horned_Crustacean_Glow").Value;
			Vector2 drawOrigin = new Vector2(glow.Width, glow.Height / Main.npcFrameCount[Type]) / 2;
			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Color color = new Color(r - NPC.alpha, byte.MaxValue - NPC.alpha, g - NPC.alpha, b - NPC.alpha);

			Main.spriteBatch.Draw(glow, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);

			float num = 0.25f + (NPC.GetAlpha(drawColor).ToVector3() - new Vector3(4f)).Length() * 0.25f;
			for (int i = 0; i < 4; i++)
				Main.spriteBatch.Draw(glow, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) + (NPC.velocity.RotatedBy(i * 47079637050629, new Vector2()) * num), NPC.frame, new Color(r, g, b, 0), NPC.rotation, drawOrigin, NPC.scale, effects, 0.0f);
		}

		public override void FindFrame(int frameHeight)
		{
			Player player = Main.player[NPC.target];

			NPC.frameCounter++;
			if (NPC.IsABestiaryIconDummy)
				NPC.frameCounter -= 0.2f;

			bool nearby = Vector2.Distance(player.Center, NPC.Center) <= 45f && NPC.velocity.X == 0f;

			if (nearby && !NPC.IsABestiaryIconDummy)
			{
				if (!NPC.IsABestiaryIconDummy && NPC.frameCounter == 24 && Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
				{
					player.Hurt(PlayerDeathReason.LegacyDefault(), (int)(NPC.damage * 1.5f), NPC.direction, false, false, false, -1);
					NPC.frame.Y = 9 * frameHeight;
				}

				NPC.velocity.X = 0f;

				if (NPC.frameCounter < 5)
					NPC.frame.Y = 5 * frameHeight;
				else if (NPC.frameCounter < 10)
					NPC.frame.Y = 6 * frameHeight;
				else if (NPC.frameCounter < 15)
					NPC.frame.Y = 7 * frameHeight;
				else if (NPC.frameCounter < 20)
					NPC.frame.Y = 8 * frameHeight;
				else if (NPC.frameCounter < 25)
					NPC.frame.Y = 9 * frameHeight;
				else
					NPC.frameCounter = 0;
			}
			else
			{
				if (NPC.frameCounter < 6)
					NPC.frame.Y = 0 * frameHeight;
				else if (NPC.frameCounter < 12)
					NPC.frame.Y = 1 * frameHeight;
				else if (NPC.frameCounter < 18)
					NPC.frame.Y = 2 * frameHeight;
				else if (NPC.frameCounter < 24)
					NPC.frame.Y = 3 * frameHeight;
				else if (NPC.frameCounter < 30)
					NPC.frame.Y = 4 * frameHeight;
				else
					NPC.frameCounter = 0;
			}
		}
	}
}