using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.World.Sepulchre
{
	public class SepulchreChandelier : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty; 
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Sepulchre Chandelier");
			AddMapEntry(new Color(179, 146, 107), name);

			AdjTiles = new int[] { TileID.Chandeliers };
			DustType = -1;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 1.0f;
			b = 0.4f;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<Sepulchre.SepulchreChandelierItem>());
		}
	}
	public class SepulchreChandelierItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sepulchre Chandelier");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;

			Item.maxStack = 999;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.createTile = ModContent.TileType<SepulchreChandelier>();
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<Items.Placeable.Tiles.SepulchreBrickTwoItem>(), 8);
			recipe.AddIngredient(ItemID.CursedTorch, 1);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}