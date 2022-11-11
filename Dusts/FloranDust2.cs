﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Dusts
{
	public class FloranDust2 : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = false;
			dust.color.R = 75;
			dust.color.G = 151;
			dust.color.B = 49;
		}
		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}
		public override bool Update(Dust dust)
		{
			//Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), 0.196f/2, 0.870588235f/2, 0.464705882f/2);
			dust.position += dust.velocity;
			dust.noGravity = true;
			dust.rotation += 0.05f;
			dust.alpha += 5;
			dust.scale *= 0.9f;
			if (dust.alpha > 220) {
				dust.active = false;
			}
			return false;
		}
	}
}
