using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.Critters
{
	public class Floater1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminous Floater");
			Main.npcFrameCount[NPC.type] = 40;
			Main.npcCatchable[NPC.type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 22;
			NPC.damage = 0;
			NPC.catchItem = (short)ModContent.ItemType<FloaterItem>();
			NPC.defense = 0;
			NPC.dontCountMe = true;
			NPC.lifeMax = 5;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
			NPC.knockBackResist = .35f;
			NPC.aiStyle = 18;
			NPC.noGravity = true;
			NPC.npcSlots = 0;
			AIType = NPCID.PinkJellyfish;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Below the waves of the elusive glowing ocean, lies hundreds of small, glowing jellyfish, dancing among the luminescent tide."),
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) 
			=> GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, ModContent.Request<Texture2D>("SpiritMod/NPCs/Critters/Floater_Critter_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, screenPos);

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 30; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 2.5f * hitDirection, -2.5f, 0, Color.White, Main.rand.NextFloat(.2f, .8f));
		}
		public override bool PreAI()
		{
			Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .3f, .2f, .3f);
			return true;
		}
	}
}
