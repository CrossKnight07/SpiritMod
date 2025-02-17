using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Biomes;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Armor;
using SpiritMod.Items.Weapon.Thrown;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static SpiritMod.NPCUtils;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent.Bestiary;
using SpiritMod.Items.Weapon.Thrown.PlagueVial;

namespace SpiritMod.NPCs.Town
{
	[AutoloadHead]
	public class Rogue : ModNPC
	{
		public override string Texture => "SpiritMod/NPCs/Town/Rogue";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bandit");
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;
			NPCID.Sets.DangerDetectRange[NPC.type] = 1500;
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 16;
			NPCID.Sets.AttackAverageChance[NPC.type] = 30;

			NPC.Happiness
				.SetBiomeAffection<BriarSurfaceBiome>(AffectionLevel.Like).SetBiomeAffection<BriarUndergroundBiome>(AffectionLevel.Like)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Love)
				.SetNPCAffection<Adventurer>(AffectionLevel.Like)
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Hate);
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.Guide);
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.aiStyle = 7;
			NPC.damage = 30;
			NPC.defense = 30;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("A dashing rogue with a past shrouded in mystery. Whether he can be trusted or not is unclear, but he would gladly teach you the tricks of the trade regardless."),
			});
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server) {
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit1").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit2").Type);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit3").Type);
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) => Main.player.Any(x => x.active) && !NPC.AnyNPCs(NPCType<Rogue>()) && !NPC.AnyNPCs(NPCType<BoundRogue>());

		public override List<string> SetNPCNameList() => new() { "Zane", "Carlos", "Tycho", "Damien", "Shane", "Daryl", "Shepard", "Sly" };

		public override string GetChat()
		{
			List<string> dialogue = new List<string>
			{
				"Here to peruse my wares? They're quite sharp.",
				"Trust me- the remains of those bosses you kill don't go to waste.",
				"The world is filled with opportunity! Now go kill some things.",
				"This mask is getting musky...",
				"Look at that handsome devil! Oh, it's just a mirror.",
				"Here to satisfy all your murdering needs!",
				"Nice day we're having here! Now, who do you want dead?",
			};

			int wizard = NPC.FindFirstNPC(NPCID.Wizard);
			if (wizard >= 0) {
				dialogue.Add($"Tell {Main.npc[wizard].GivenName} to stop asking me where I got the charms. He doesn't need to know that. He would die of shock.");
			}

			int merchant = NPC.FindFirstNPC(NPCID.Merchant);
			if (merchant >= 0) {
				dialogue.Add($"Why is {Main.npc[merchant].GivenName} so intent on selling shurikens? That's totally my thing.");
			}
			return Main.rand.Next(dialogue);
		}

		public override void SetChatButtons(ref string button, ref string button2) => button = Language.GetTextValue("LegacyInterface.28");

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton) {
				shop = true;
			}
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			AddItem(ref shop, ref nextSlot, ItemID.Shuriken);
			AddItem(ref shop, ref nextSlot, ItemType<RogueHood>());
			AddItem(ref shop, ref nextSlot, ItemType<RoguePlate>());
			AddItem(ref shop, ref nextSlot, ItemType<RoguePants>());
            AddItem(ref shop, ref nextSlot, ItemType<RogueCrest>());

			if (!WorldGen.crimson)
            	AddItem(ref shop, ref nextSlot, ItemType<EoWDagger>(), check: NPC.downedBoss2);
			else
				AddItem(ref shop, ref nextSlot, ItemType<BoCShuriken>(), check: NPC.downedBoss2);

			AddItem(ref shop, ref nextSlot, ItemType<SkeletronHand>(), check: NPC.downedBoss3);
			AddItem(ref shop, ref nextSlot, ItemType<PlagueVial>(), check: Main.hardMode);
			AddItem(ref shop, ref nextSlot, ItemType<ShurikenLauncher>());
			AddItem(ref shop, ref nextSlot, ItemType<SwiftRune>());
			AddItem(ref shop, ref nextSlot, ItemType<AssassinMagazine>());
			AddItem(ref shop, ref nextSlot, ItemType<TargetCan>());
			AddItem(ref shop, ref nextSlot, ItemType<TargetBottle>());
			AddItem(ref shop, ref nextSlot, ItemType<Items.Placeable.Furniture.TreasureChest>());
            AddItem(ref shop, ref nextSlot, ItemType<Items.Armor.Masks.PsychoMask>());
        }

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 10;
			knockback = 3f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 5;
			randExtraCooldown = 5;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileType<Projectiles.Thrown.Kunai_Throwing>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 13f;
			randomOffset = 2f;
		}

		private float animCounter;
		public override void FindFrame(int frameHeight)
		{
			if (!NPC.IsABestiaryIconDummy)
				return;

			animCounter += 0.25f;
			if (animCounter >= 16)
				animCounter = 2;
			else if (animCounter < 2)
				animCounter = 2;

			int frame = (int)animCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override ITownNPCProfile TownNPCProfile() => new RogueProfile();
	}

	public class RogueProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.altTexture == 1 && !(npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn))
				return Request<Texture2D>("SpiritMod/NPCs/Town/Rogue_Alt_1");

			return Request<Texture2D>("SpiritMod/NPCs/Town/Rogue");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("SpiritMod/NPCs/Town/Rogue_Head");
	}
}
