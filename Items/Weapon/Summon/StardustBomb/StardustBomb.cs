using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Particles;
using System;
using Terraria.DataStructures;

namespace SpiritMod.Items.Weapon.Summon.StardustBomb
{
	public class StardustBomb : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Supernova");
			Tooltip.SetDefault("Summons a collapsing star\nThe star can be targeted\nDeal summon damage to the star to release a powerful explosion");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.QueenSpiderStaff);
			Item.damage = 0;
			Item.mana = 12;
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 8, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.knockBack = 2.5f;
			Item.UseSound = SoundID.Item20;
			Item.DamageType = DamageClass.Summon;
			Item.shootSpeed = 10f;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.noUseGraphic = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.ai[0] == player.whoAmI && npc.type == ModContent.NPCType<StardustBombNPC>())
				{
					npc.active = false;
					npc.netUpdate = true;
				}
			}

			if (player == Main.LocalPlayer)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SpawnStardustBomb, 2);
					packet.Write((byte)player.whoAmI);
					packet.WriteVector2(velocity);
					packet.Send();
				}
				else
				{
					int npcindex = NPC.NewNPC(Item.GetSource_ItemUse(Item), (int)position.X, (int)position.Y + 100, ModContent.NPCType<StardustBombNPC>(), 0, player.whoAmI);
					Main.npc[npcindex].velocity = velocity;
					Main.npc[npcindex].netUpdate = true;
				}
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.FragmentStardust, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
	}

	internal class StardustBombNPC : ModNPC
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Supernova");
			Main.npcFrameCount[NPC.type] = 7;
        }

		int returnCounter;
		readonly int returnCounterMax = 200;

		int boomdamage;
		float shrinkCounter = 0.25f;
		bool shrinking;
        
		public override void SetDefaults()
        {
            NPC.width = 158;
            NPC.height = 197;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.lifeMax = 30000;
            NPC.damage = 0;
            NPC.defense = 0;
			NPC.HitSound = SoundID.NPCHit3;
            NPC.noTileCollide = true;
			NPC.noGravity = true;
            NPC.dontCountMe = true;
        }

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.25f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			//Player player = Main.player[(int)NPC.ai[0]];
			returnCounter++;
			if (returnCounter >= returnCounterMax)
			{
				if (Explode())
					NPC.active = false;
				else
					shrinking = true;

				NPC.netUpdate = true;
			}
			else if (returnCounter % 40 == 1)
			{
				float scale = MathHelper.Clamp(LifeQuoteant * 25f, 0, 1);
				DustHelper.DrawStar(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(100.0f)), 206, 5, scale, scale * 0.8f, scale * 1.5f);
			}

			NPC.velocity *= 0.97f;
			NPC.rotation += 0.03f;

			Lighting.AddLight(NPC.Center, Color.Cyan.R * 0.005f, Color.Cyan.G * 0.005f, Color.Cyan.B * 0.005f);

			if (++NPC.ai[1] == 20 && Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop, NPC.Center);
			if (shrinking)
			{
				shrinkCounter += 0.1f;
				NPC.scale = 0.75f + (float)Math.Sin(shrinkCounter);
				if (NPC.scale < 0.3f)
				{
					NPC.active = false;
					for (int j = 0; j < 14; j++)
					{
						int timeLeft = Main.rand.Next(20, 40);

						ParticleHandler.SpawnParticle(new StarParticle(NPC.Center, Main.rand.NextVector2Circular(10, 7), Color.Cyan, Main.rand.NextFloat(0.15f, 0.3f), timeLeft));
					}
				}
				else if (NPC.scale > 1)
					NPC.scale = ((NPC.scale - 1) / 2f) + 1;
			}
			else
				NPC.scale = MathHelper.Min(NPC.ai[1] / 15f, 1);
		}

		private bool Explode()
		{
			if (boomdamage == 0)
				return false;

			Player player = Main.player[(int)NPC.ai[0]];

			for (int i = 0; i < 2; i++)
			{
				for (int j = 1; j < 5; ++j)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						float randFloat = Main.rand.NextFloat(6.28f);
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center + (randFloat.ToRotationVector2() * 60), Main.rand.NextFloat(6.28f).ToRotationVector2() * 16, Mod.Find<ModGore>("StarbombGore" + j).Type, 1f);
					}

					if (i == 0)
					{
						float scale = Main.rand.NextFloat(1.0f, 2.5f);
						DustHelper.DrawStar(NPC.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(120.0f)), 206, 5, scale, scale * 0.8f, Math.Min(scale, 1.8f));
					}
				}
			}

			Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<StarShockwave>(), (int)player.GetDamage(DamageClass.Summon).ApplyTo(boomdamage * 0.5f), 0, player.whoAmI);
			
			if (Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Thunder"), NPC.Center);
			}

			SpiritMod.tremorTime = 15;
			return true;
		}

		public override bool? CanBeHitByItem(Player player, Item item) => false;

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (ProjectileID.Sets.MinionShot[projectile.type] || projectile.minion)
				return base.CanBeHitByProjectile(projectile);
			return false;
		}

		public override void HitEffect(int hitDirection, double damage) => boomdamage += (int)damage;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			float breakCounter = (float)MathHelper.Clamp(LifeQuoteant, 0, 1);

			SpiritMod.CircleNoise.Parameters["breakCounter"].SetValue(breakCounter * 5f);
			SpiritMod.CircleNoise.Parameters["rotation"].SetValue(0 - (NPC.rotation / 1.25f) + (breakCounter * 3.5f));
			SpiritMod.CircleNoise.Parameters["colorMod"].SetValue(Color.Silver.ToVector4());
			SpiritMod.CircleNoise.Parameters["noise"].SetValue(Mod.Assets.Request<Texture2D>("Textures/noise").Value);
			SpiritMod.CircleNoise.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), null, Color.White, 0f, new Vector2(50, 50), 0.64f + (breakCounter * 2f), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), null, Color.Cyan with { A = 0 }, 0 - (NPC.rotation / 2), new Vector2(50, 50), 0.45f * NPC.scale + (breakCounter * 0.75f), SpriteEffects.None, 0f);

			for (int i = 0; i < 2; i++)
			{
				Rectangle frame = new Rectangle(0, 0, 48, 52);
				Color drawCol = Color.White;

				if (i > 0)
				{
					frame.Y = 52;
					drawCol *= breakCounter * 12f;
				}

				Main.spriteBatch.Draw(
					Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Star").Value,
					NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY),
					frame,
					drawCol,
					0 - (NPC.rotation / 2) + (LifeQuoteant * 30f),
					frame.Size() / 2,
					NPC.scale,
					SpriteEffects.None, 0
				);
			}

            Main.spriteBatch.Draw(
                Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC").Value,
				NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY),
				NPC.frame,
				drawColor,
				NPC.rotation,
				NPC.frame.Size() / 2,
				NPC.scale,
				SpriteEffects.None, 0
			);
			return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			float num107 = 0f;

			SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
			Color color28 = color29;
            color28 = NPC.GetAlpha(color28);
            color28 *= 0.5f;

            for (int num103 = 0; num103 < 6; num103++)
            {
                Vector2 vector29 = NPC.Center + ((float)num103 / 4f * 6.28318548f + NPC.rotation).ToRotationVector2() * (2f * num107 + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Glow").Value, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }

            num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

            spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
            color28 = color29;
            color28 = NPC.GetAlpha(color28);
            color28 *= 1f - num107;

            for (int num103 = 0; num103 < 6; num103++)
            {
                Vector2 vector29 = NPC.Center + ((float)num103 / 4f * 6.28318548f + NPC.rotation).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Weapon/Summon/StardustBomb/StardustBombNPC_Glow").Value, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = Rectangle.Empty;

		private float LifeQuoteant => 1f - ((float)NPC.life / NPC.lifeMax);
	}
}
