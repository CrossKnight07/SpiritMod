using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Accessory;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.SporeWheezer
{
	public class SporeWheezer : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spore Wheezer");
			Main.npcFrameCount[NPC.type] = 12;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 60;
			NPC.damage = 16;
			NPC.defense = 7;
			NPC.lifeMax = 63;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath53;
			NPC.value = 560f;
			NPC.knockBackResist = .55f;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.SporeWheezerBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,
				new FlavorTextBestiaryInfoElement("Usually parasitic, the glowing mushrooms formed a symbiotic relationship with wheezers as they consciously spread spores through their breath."),
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => (spawnInfo.SpawnTileType == TileID.MushroomGrass) && spawnInfo.SpawnTileY > Main.rockLayer ? 0.09f : 0f;

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 11; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Harpy, hitDirection, -1f, 0, default, .61f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer2").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer3").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer4").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer4").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SporeWheezer4").Type, 1f);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon(ItemID.GlowingMushroom, 1, 3, 5);
			npcLoot.AddCommon<WheezerScale>(15);
			npcLoot.AddCommon(ItemID.DepthMeter, 80);
			npcLoot.AddCommon<Items.Sets.FlailsMisc.ClatterMace.ClatterMace>(60);
			npcLoot.AddCommon(ItemID.Compass, 80);
			npcLoot.AddCommon(ItemID.Rally, 200);
		}

		int frame = 0;
		int timer = 0;
		int shootTimer = 0;
		public override void AI()
		{
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.1f, 0.15f);
			NPC.spriteDirection = NPC.direction;
			Player target = Main.player[NPC.target];
			float distance = NPC.DistanceSQ(target.Center);

			if (distance < 200 * 200)
			{
				NPC.velocity = Vector2.Zero;
				if (NPC.velocity == Vector2.Zero)
				{
					NPC.aiStyle = 0;
					if (target.position.X > NPC.position.X)
						NPC.spriteDirection = 1;
					else
						NPC.spriteDirection = -1;
					NPC.velocity.Y = 12f;
				}
				float num395 = Main.mouseTextColor / 200f - 0.35f;
				num395 *= 0.2f;
				NPC.scale = num395 + 0.95f;
				shootTimer++;
				if (shootTimer >= 90)
				{
					SoundEngine.PlaySound(SoundID.Item95, NPC.Center);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
						direction.Normalize();
						direction.X *= 5f;
						direction.Y *= 5f;

						int amountOfProjectiles = Main.rand.Next(2, 4);
						bool expertMode = Main.expertMode;
						int damage = expertMode ? 8 : 13;
						for (int i = 0; i < amountOfProjectiles; ++i)
						{
							float A = (float)Main.rand.Next(-50, 50) * 0.02f;
							float B = (float)Main.rand.Next(-60, -40) * 0.1f;
							int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction * 12), NPC.Center.Y - 10, direction.X + A, direction.Y + B, ModContent.ProjectileType<WheezerSporeHostile>(), damage, 1, Main.myPlayer, 0, 0);
							for (int k = 0; k < 11; k++)
							{
								Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Harpy, direction.X + A, direction.Y + B, 0, default, .61f);
							}
							Main.projectile[p].hostile = true;
						}
					}
					shootTimer = 0;
				}
			}
			else
			{
				NPC.scale = 1f;
				shootTimer = 0;
				NPC.aiStyle = 3;
				AIType = NPCID.SnowFlinx;
			}
			if (shootTimer > 120)
			{
				shootTimer = 120;
			}
			if (shootTimer < 0)
			{
				shootTimer = 0;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, Mod.Assets.Request<Texture2D>("NPCs/SporeWheezer/SporeWheezer_Glow").Value, screenPos);
		
		public override void FindFrame(int frameHeight)
		{
			float distance = 200 * 200;

			if (!NPC.IsABestiaryIconDummy)
			{
				Player target = Main.player[NPC.target];
				distance = NPC.DistanceSQ(target.Center);
			}

			if (distance < 200 * 200)
			{
				timer++;
				if (timer == 4)
				{
					frame++;
					timer = 0;
				}

				if (frame >= 11)
					frame = 7;
			}
			else
			{
				timer++;
				if (timer == 4)
				{
					frame++;
					timer = 0;
				}

				if (frame >= 6)
					frame = 0;
			}
			NPC.frame.Y = frameHeight * frame;
		}
	}
}