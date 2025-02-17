using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Block;
using SpiritMod.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Spirit
{
	public class UnstableWisp : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unstable Wisp");
			Main.npcFrameCount[NPC.type] = 4;
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 32;
			NPC.lifeMax = 150;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath6;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiritUndergroundBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("A cluster of souls that was unable to find a vessel to inhabit. It is unable to hold itself together and any slight touch will cause it to explode, releasing the many clumped souls."),
			});
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;

			if (player.ZoneSpirit() && player.ZoneDirtLayerHeight && !spawnInfo.PlayerInTown && !spawnInfo.Invasion)
			{
				int[] spawnTiles = { ModContent.TileType<SpiritDirt>(), ModContent.TileType<SpiritStone>(), ModContent.TileType<Spiritsand>(), ModContent.TileType<SpiritIce>(), };
				return spawnTiles.Contains(spawnInfo.SpawnTileType) ? 2f : 0f;
			}
			return 0f;
		}

		public override bool PreAI()
		{
			bool inRange = false;
			Vector2 target = Vector2.Zero;
			float triggerRange = 280f;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && !Main.player[i].dead)
				{
					float playerX = Main.player[i].position.X + (float)(Main.player[i].width / 2);
					float playerY = Main.player[i].position.Y + (float)(Main.player[i].height / 2);
					float distOrth = Math.Abs(NPC.position.X + (float)(NPC.width / 2) - playerX) + Math.Abs(NPC.position.Y + (float)(NPC.height / 2) - playerY);
					if (distOrth < triggerRange)
					{
						if (Main.player[i].Hitbox.Intersects(NPC.Hitbox))
						{
							NPC.life = 0;
							NPC.HitEffect(0, 10.0);
							NPC.checkDead();
							NPC.active = false;
							return false;
						}
						triggerRange = distOrth;
						target = Main.player[i].Center;
						inRange = true;
					}
				}
			}
			if (inRange)
			{
				Vector2 delta = target - NPC.Center;
				delta.Normalize();
				delta *= 0.95f;
				NPC.velocity = (NPC.velocity * 10f + delta) * (1f / 11f);
				return false;
			}
			if (NPC.velocity.Length() > 0.2f)
				NPC.velocity *= 0.98f;

			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0.9f, 0.9f, 1.5f);
			return false;
		}

		public override bool CheckDead()
		{
			Vector2 center = NPC.Center;
			Projectile.NewProjectile(NPC.GetSource_Death(), center.X, center.Y, 0f, 0f, ModContent.ProjectileType<UnstableWisp_Explosion>(), 100, 0f, Main.myPlayer);
			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.1f;
			if (NPC.frameCounter >= Main.npcFrameCount[NPC.type])
				NPC.frameCounter -= Main.npcFrameCount[NPC.type];
			int num = (int)NPC.frameCounter;
			NPC.frame.Y = num * frameHeight;
			NPC.spriteDirection = NPC.direction;
		}
	}
}
