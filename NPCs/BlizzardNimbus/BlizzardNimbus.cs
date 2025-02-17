﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.BlizzardNimbus
{
	public class BlizzardNimbus : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blizzard Nimbus");
			Main.npcFrameCount[NPC.type] = 4;
			NPCHelper.ImmuneTo(this, BuffID.Frostburn, BuffID.Confused, ModContent.BuffType<MageFreeze>(), ModContent.BuffType<CryoCrush>());
		}

		public override void SetDefaults()
		{
			NPC.damage = 48;
			NPC.width = 40;
			NPC.height = 54;
			NPC.defense = 18;
			NPC.lifeMax = 220;
			NPC.knockBackResist = 0.3f;
			NPC.noGravity = true;
			NPC.value = Item.buyPrice(0, 0, 4, 0);
			NPC.HitSound = SoundID.NPCHit30;
			NPC.DeathSound = SoundID.NPCDeath49;

			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.BlizzardNimbusBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Blizzard,
				new FlavorTextBestiaryInfoElement("Having frozen over, this nimbus angrily rains down shards of ice in an attempt to expel all of it. What it doesn't know is that it could just leave the cold environment."),
			});
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			float num1164 = 4f;
			float num1165 = 0.75f;
			Vector2 vector133 = NPC.Center;
			float num1166 = Main.player[NPC.target].Center.X - vector133.X;
			float num1167 = Main.player[NPC.target].Center.Y - vector133.Y - 200f;
			float num1168 = (float)Math.Sqrt((double)(num1166 * num1166 + num1167 * num1167));
			if (num1168 < 20f) {
				num1166 = NPC.velocity.X;
				num1167 = NPC.velocity.Y;
			}
			else {
				num1168 = num1164 / num1168;
				num1166 *= num1168;
				num1167 *= num1168;
			}
			if (NPC.velocity.X < num1166) {
				NPC.velocity.X = NPC.velocity.X + num1165;
				if (NPC.velocity.X < 0f && num1166 > 0f) {
					NPC.velocity.X = NPC.velocity.X + num1165 * 2f;
				}
			}
			else if (NPC.velocity.X > num1166) {
				NPC.velocity.X = NPC.velocity.X - num1165;
				if (NPC.velocity.X > 0f && num1166 < 0f) {
					NPC.velocity.X = NPC.velocity.X - num1165 * 2f;
				}
			}
			if (NPC.velocity.Y < num1167) {
				NPC.velocity.Y = NPC.velocity.Y + num1165;
				if (NPC.velocity.Y < 0f && num1167 > 0f) {
					NPC.velocity.Y = NPC.velocity.Y + num1165 * 2f;
				}
			}
			else if (NPC.velocity.Y > num1167) {
				NPC.velocity.Y = NPC.velocity.Y - num1165;
				if (NPC.velocity.Y > 0f && num1167 < 0f) {
					NPC.velocity.Y = NPC.velocity.Y - num1165 * 2f;
				}
			}
			if (NPC.position.X + (float)NPC.width > Main.player[NPC.target].position.X && NPC.position.X < Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width && NPC.position.Y + (float)NPC.height < Main.player[NPC.target].position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && Main.netMode != NetmodeID.MultiplayerClient) {
				NPC.ai[0] += 4f;
				if (NPC.ai[0] > 32f) {
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.ai[0] = 0f;
						int num1169 = (int)(NPC.position.X + 10f + (float)Main.rand.Next(NPC.width - 20));
						int num1170 = (int)(NPC.position.Y + (float)NPC.height + 4f);
						int num184 = 26;
						if (Main.expertMode)
						{
							num184 = 14;
						}
						Projectile.NewProjectile(NPC.GetSource_FromAI(), (float)num1169, (float)num1170, 0f, 5f, ProjectileID.FrostShard, num184, 0f, Main.myPlayer, 0f, 0f);
						return;
					}
				}
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
			NPC.damage = (int)(NPC.damage * 0.6f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hitDirection, -1f, 0, default, 1f);
			}
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 13);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 12);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 11);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.FrostStaff, 50));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceSickle, 178));
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneOverworldHeight && Main.raining && Main.hardMode ? 0.4f : 0f;
	}
}