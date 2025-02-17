using SpiritMod.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AcidSofaTile = SpiritMod.Tiles.Furniture.Acid.AcidSofaTile;

namespace SpiritMod.Items.Placeable.Furniture.Acid
{
	[Sacrifice(1)]
	public class AcidSofa : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrosive Sofa");
		}


		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = 200;

			Item.maxStack = 99;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;

			Item.createTile = ModContent.TileType<AcidSofaTile>();
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AcidBrick>(), 5);
			recipe.AddIngredient(ItemID.Silk, 2);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();
		}
	}
}