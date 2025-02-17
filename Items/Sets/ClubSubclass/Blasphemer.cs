using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class Blasphemer : ClubItem
    {
		internal override int ChargeTime => 66;
		internal override float Acceleration => 16f;
		internal override int MinDamage => 65;
		internal override int MaxDamage => 170;
		internal override float MinKnockback => 8f;
		internal override float MaxKnockback => 12f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Blasphemer");
            Tooltip.SetDefault("Charged strikes create a fiery geyser");
            SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/ClubSubclass/Blasphemer_Glow");
        }

        public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 6;
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.BlasphemerProj>();
        }

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SlagSet.CarvedRock>(), 25);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.245f, .126f, .066f);
			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/ClubSubclass/Blasphemer_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
	}
}