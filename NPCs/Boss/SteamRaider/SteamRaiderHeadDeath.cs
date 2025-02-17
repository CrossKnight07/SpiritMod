﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.BossLoot.StarplateDrops;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.SteamRaider
{
	public class SteamRaiderHeadDeath : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starplate Voyager");

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults()
		{
			NPC.width = 64; //324
			NPC.height = 56; //216
			NPC.boss = true;
			NPC.damage = 0;
			NPC.defense = 12;
			Music = MusicLoader.GetMusicSlot(Mod,"Sounds/Music/null");
			NPC.noTileCollide = true;
			NPC.dontTakeDamage = true;
			NPC.lifeMax = 65;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.value = 160f;
			NPC.knockBackResist = .16f;
			NPC.noGravity = true;
			NPC.dontCountMe = true;
		}

		int timeLeft = 200;
		float alphaCounter;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			float sineAdd = alphaCounter + 2;
			Main.spriteBatch.Draw(TextureAssets.Extra[49].Value, (NPC.Center - Main.screenPosition) - new Vector2(-2, 8), null, new Color((int)(7.5f * sineAdd), (int)(16.5f * sineAdd), (int)(18f * sineAdd), 0), 0f, new Vector2(50, 50), 0.25f * (sineAdd + .65f), SpriteEffects.None, 0f);
			return true;
		}

		public override void AI()
		{
			alphaCounter += 0.025f;
			NPC.alpha = 255 - timeLeft;
			if (timeLeft == 200)
				NPC.rotation = 3.14f;

			NPC.rotation += Main.rand.Next(-20, 20) / 100f;

			Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)));

			if (timeLeft < 50)
				Dust.NewDustPerfect(NPC.Center, 226, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)));

			timeLeft--;

			if (timeLeft <= 0)
			{
				if (!Main.expertMode)
				{
					NPC.DropItem(ModContent.ItemType<CosmiliteShard>(), 6, 10, NPC.GetSource_FromAI());
					NPC.DropItem(ModContent.ItemType<StarplateMask>(), 1f / 7, NPC.GetSource_FromAI());
					NPC.DropItem(ModContent.ItemType<Trophy3>(), 1f / 10, NPC.GetSource_FromAI());
				}

				if (Main.netMode != NetmodeID.Server)
				{
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Starplate1").Type, 1f);
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Starplate2").Type, 1f);
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Starplate3").Type, 1f);
					SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
				}

				for (int i = 0; i < 90; i++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, Main.rand.Next(-25, 25), Main.rand.Next(-13, 13));

				NPC.position.X = NPC.position.X + (NPC.width / 2);
				NPC.position.Y = NPC.position.Y + (NPC.height / 2);
				NPC.width = 30;
				NPC.height = 30;
				NPC.position.X = NPC.position.X - (NPC.width / 2);
				NPC.position.Y = NPC.position.Y - (NPC.height / 2);

				NPC.life = 0;
                NPC.active = false;
			}
		}

		//public override void ModifyNPCLoot(NPCLoot npcLoot)
		//{
		//	npcLoot.AddMasterModeDropOnAllPlayers<StarplatePetItem>();
		//	npcLoot.AddBossBag<SteamRaiderBag>();

		//	LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
		//	notExpertRule.AddCommon<StarMap>();
		//	notExpertRule.AddCommon<StarplateMask>(7);
		//	notExpertRule.AddCommon<Trophy3>(10);
		//	notExpertRule.AddCommon<CosmiliteShard>(1, 6, 10);

		//	npcLoot.Add(notExpertRule);
		//}
	}
}