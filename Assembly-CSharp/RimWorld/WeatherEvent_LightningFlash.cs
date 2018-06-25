﻿using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class WeatherEvent_LightningFlash : WeatherEvent
	{
		private int duration;

		private Vector2 shadowVector;

		private int age = 0;

		private const int FlashFadeInTicks = 3;

		private const int MinFlashDuration = 15;

		private const int MaxFlashDuration = 60;

		private const float FlashShadowDistance = 5f;

		private static readonly SkyColorSet LightningFlashColors = new SkyColorSet(new Color(0.9f, 0.95f, 1f), new Color(0.784313738f, 0.8235294f, 0.847058833f), new Color(0.9f, 0.95f, 1f), 1.15f);

		public WeatherEvent_LightningFlash(Map map) : base(map)
		{
			this.duration = Rand.Range(15, 60);
			this.shadowVector = new Vector2(Rand.Range(-5f, 5f), Rand.Range(-5f, 0f));
		}

		public override bool Expired
		{
			get
			{
				return this.age > this.duration;
			}
		}

		public override SkyTarget SkyTarget
		{
			get
			{
				return new SkyTarget(1f, WeatherEvent_LightningFlash.LightningFlashColors, 1f, 1f);
			}
		}

		public override Vector2? OverrideShadowVector
		{
			get
			{
				return new Vector2?(this.shadowVector);
			}
		}

		public override float SkyTargetLerpFactor
		{
			get
			{
				return this.LightningBrightness;
			}
		}

		protected float LightningBrightness
		{
			get
			{
				float result;
				if (this.age <= 3)
				{
					result = (float)this.age / 3f;
				}
				else
				{
					result = 1f - (float)this.age / (float)this.duration;
				}
				return result;
			}
		}

		public override void FireEvent()
		{
			SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera(this.map);
		}

		public override void WeatherEventTick()
		{
			this.age++;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static WeatherEvent_LightningFlash()
		{
		}
	}
}
