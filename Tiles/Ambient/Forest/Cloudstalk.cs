using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Armor.BotanistSet;
using SpiritMod.Items.ByBiome.Forest.Placeable.Decorative;
using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Forest
{
	[TileTag(TileTags.HarvestableHerb)]
	public class Cloudstalk : ModTile, IHarvestableHerb
	{
		private const int FrameWidth = 18; // A constant for readability and to kick out those magic numbers
		private const float BloomWindSpeed = 14; //Constant for bloom speed, in mph

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.IgnoredInHouseScore[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
			TileID.Sets.SwaysInWindBasic[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Cloudstalk");
			AddMapEntry(new Color(178, 234, 234), name);

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Grass, TileID.HallowedGrass, TileID.JungleGrass, ModContent.TileType<BriarGrass>(), ModContent.TileType<Stargrass>(), TileID.Cloud, TileID.RainCloud, TileID.SnowCloud };
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.ClayPot, TileID.PlanterBox };
			TileObjectData.addTile(Type);

			HitSound = SoundID.Grass;
			DustType = DustID.Cloud;
		}

		public bool CanBeHarvested(int i, int j) => Main.tile[i, j].HasTile && GetStage(i, j) == PlantStage.Grown;

		public override bool CanPlace(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (!tile.HasTile)
				return true;

			int tileType = tile.TileType;
			if (tileType == Type)
			{
				PlantStage stage = GetStage(i, j);
				return stage == PlantStage.Grown;
			}
			else
			{
				if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
				{
					bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
					bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
					bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

					if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
					{
						WorldGen.KillTile(i, j);

						if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);

						return true;
					}
				}
				return false;
			}
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
			=> spriteEffects = (i % 2 == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = -2;

		public override bool Drop(int i, int j)
		{
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted)
				return false;

			Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
			Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

			int herbItemStack = 0;
			int seedItemStack = 0;

			if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth) // Increased yields with Staff of Regrowth, even when not fully grown
			{
				if (stage == PlantStage.Grown)
					(herbItemStack, seedItemStack) = (2, Main.rand.Next(2, 5));
				else if (stage == PlantStage.Growing)
					seedItemStack = Main.rand.Next(1, 3);
			}
			else if (stage == PlantStage.Grown)
				(herbItemStack, seedItemStack) = (1, Main.rand.Next(1, 4));
			else if (stage == PlantStage.Growing)
				herbItemStack = 1;

			if (nearestPlayer.GetModPlayer<BotanistPlayer>().active && stage != PlantStage.Planted)
			{
				seedItemStack += 2;
				herbItemStack++;
			}

			var source = new EntitySource_TileBreak(i, j);

			if (herbItemStack > 0)
				Item.NewItem(source, worldPosition, ModContent.ItemType<CloudstalkItem>(), herbItemStack);
			if (seedItemStack > 0)
				Item.NewItem(source, worldPosition, ModContent.ItemType<CloudstalkSeed>(), seedItemStack);
			return false;
		}

		public override bool IsTileSpelunkable(int i, int j) => GetStage(i, j) == PlantStage.Grown;

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted) //Grow only if just planted
			{
				tile.TileFrameX += FrameWidth;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			float windSpeed = System.Math.Abs(Main.windSpeedCurrent) * 50;
			if (windSpeed > BloomWindSpeed && GetStage(i, j) == PlantStage.Growing)
				SetStage(i, j, PlantStage.Grown);
			else if (windSpeed <= BloomWindSpeed && GetStage(i, j) == PlantStage.Grown)
				SetStage(i, j, PlantStage.Growing);
		}

		// A helper method to quickly get the current stage of the herb (assuming the tile at the coordinates is our herb)
		private static PlantStage GetStage(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			return (PlantStage)(tile.TileFrameX / FrameWidth);
		}

		// A helper method to quickly set the current stage of the herb (assuming the tile at the coordinates is our herb)
		private static void SetStage(int i, int j, PlantStage stage)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)(FrameWidth * (int)stage);
		}
	}
}