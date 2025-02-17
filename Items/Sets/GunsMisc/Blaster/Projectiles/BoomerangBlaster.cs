using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class BoomerangBlaster : SubtypeProj
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 30;

		private int Style
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private bool returning = false;

		private Player Player => Main.player[Projectile.owner];

		public override string Texture => "SpiritMod/Items/Sets/GunsMisc/Blaster/Blaster";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Boomerang Blaster");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 700;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = false;
			AIType = -1;
		}

		public override void AI()
		{
			if (Player.itemAnimation > 2)
			{
				Player.heldProj = Projectile.whoAmI;

				float addRot = (float)(Player.itemAnimation / 4f);
				Projectile.Center = Player.Center + new Vector2(34, 6).RotatedBy(Projectile.velocity.ToRotation() - (addRot * Player.direction)) - Projectile.velocity;

				Player.itemRotation = MathHelper.WrapAngle(Player.AngleTo(Projectile.Center) + ((Player.direction < 0) ? MathHelper.Pi : 0));
				Projectile.rotation = Player.AngleTo(Projectile.Center);

				Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0) ? 1 : -1;
				return;
			}
			else
			{
				Player.itemTime = 2;
				Player.itemAnimation = 2;
			}

			if (++Counter > 8)
			{
				if (Counter > counterMax * 1.5f)
				{
					returning = true;

					float trackingSpeed = 8f;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Player.Center) * trackingSpeed, 0.1f);

					Projectile.rotation += 0.38f * Projectile.spriteDirection;

					if (Projectile.Hitbox.Intersects(Player.Hitbox))
						Projectile.active = false;
					else if (Projectile.Distance(Player.Center) < 80)
						Player.itemRotation = MathHelper.WrapAngle(Player.AngleTo(Projectile.Center) + ((Player.direction < 0) ? MathHelper.Pi : 0));
				}
				else
				{
					//Add simple curvature
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.04f);
					Projectile.rotation += (float)(((float)(Counter - counterMax) / counterMax / 3f) + 0.1f) * Projectile.spriteDirection;
				}
			}
			else
				Projectile.rotation += 0.1f * Projectile.spriteDirection;

			Projectile.velocity += new Vector2(0, 0.3f * Player.direction).RotatedBy(Projectile.velocity.ToRotation() + ((Projectile.direction != Projectile.spriteDirection) ? MathHelper.Pi : 0));

			for (int i = 0; i < 2; i++)
			{
				int[] dustType = Dusts;

				int width = 16;
				Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(width * ((i > 0) ? -1 : 1), 0).RotatedBy(Projectile.rotation), dustType[Main.rand.Next(dustType.Length)], null, 0, default, 1f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * .05f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!Main.dedServ)
				for (int i = 0; i < 8; i++)
					ParticleHandler.SpawnParticle(new GlowParticle(Projectile.position + Projectile.velocity, (Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(1.5f), GetColor(Subtype), Main.rand.NextFloat(0.03f, 0.08f), 20, 10));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			int vFrames = 4;
			Rectangle frame = new(0, texture.Height / vFrames * Style, texture.Width, (texture.Height / vFrames) - 2);

			SpriteEffects effects = (Projectile.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			if (Counter > 0 && !returning)
			{
				Texture2D outline = Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Projectiles/Blaster_Pulse").Value;
				Rectangle outFrame = new(0, outline.Height / vFrames * Style, outline.Width, (outline.Height / vFrames) - 2);

				for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
				{
					float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type] / 4 * MathHelper.Clamp(Projectile.velocity.Length() / 8, 0f, 1f);
					Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
					Main.EntitySpriteDraw(outline, drawPosition, outFrame, Projectile.GetAlpha(GetColor(Subtype)) * opacityMod,
						Projectile.rotation, outFrame.Size() / 2, Projectile.scale, effects, 0);
				}

				Texture2D glideTex = Mod.Assets.Request<Texture2D>("Textures/Ray_2").Value;
				Color color = (GetColor(Subtype) * Main.rand.NextFloat(0.8f, 1.2f)) with { A = 0 };
				Vector2 stretch = new Vector2(1.5f, Projectile.velocity.Length() / glideTex.Height * 10) * Projectile.scale;

				Main.EntitySpriteDraw(glideTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.velocity.ToRotation() + 1.57f, new Vector2(glideTex.Width / 2, 0), stretch, effects, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);

			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(GetColor(Subtype)), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}

		public override bool DoAudiovisuals => false;
	}
}
