﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;

namespace SpiritMod.Skies
{
	/// <summary>
	/// This sky is literally here so the clouds can be slightly transparent and so the game doesn't crash trying to handle the overlay ¯\_(ツ)_/¯
	/// </summary>
	public class AuroraSky : CustomSky
	{
		private bool skyActive;
		private float opacity;

		public override void Deactivate(params object[] args) => skyActive = false;

		public override void Reset() => skyActive = false;

		public override bool IsActive() => skyActive || opacity > 0f;

		public override void Activate(Vector2 position, params object[] args) => skyActive = true;

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth) { }

		public override void Update(GameTime gameTime)
		{
			if (skyActive && opacity < 1f)
				opacity += 0.01f;
			else if (!skyActive && opacity > 0f)
				opacity -= 0.005f;
		}

		public override float GetCloudAlpha() => (1f - opacity) * 0.9f + 0.1f;
	}
}
