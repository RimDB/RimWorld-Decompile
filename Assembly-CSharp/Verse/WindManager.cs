﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse.Noise;

namespace Verse
{
	// Token: 0x02000CB1 RID: 3249
	public class WindManager
	{
		// Token: 0x0400309A RID: 12442
		private Map map;

		// Token: 0x0400309B RID: 12443
		private static List<Material> plantMaterials = new List<Material>();

		// Token: 0x0400309C RID: 12444
		private float cachedWindSpeed;

		// Token: 0x0400309D RID: 12445
		private ModuleBase windNoise = null;

		// Token: 0x0400309E RID: 12446
		private float plantSwayHead = 0f;

		// Token: 0x0600479E RID: 18334 RVA: 0x0025C8D8 File Offset: 0x0025ACD8
		public WindManager(Map map)
		{
			this.map = map;
		}

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x0600479F RID: 18335 RVA: 0x0025C8FC File Offset: 0x0025ACFC
		public float WindSpeed
		{
			get
			{
				return this.cachedWindSpeed;
			}
		}

		// Token: 0x060047A0 RID: 18336 RVA: 0x0025C918 File Offset: 0x0025AD18
		public void WindManagerTick()
		{
			this.cachedWindSpeed = this.BaseWindSpeedAt(Find.TickManager.TicksAbs) * this.map.weatherManager.CurWindSpeedFactor;
			List<Thing> list = this.map.listerThings.ThingsInGroup(ThingRequestGroup.WindSource);
			for (int i = 0; i < list.Count; i++)
			{
				CompWindSource compWindSource = list[i].TryGetComp<CompWindSource>();
				this.cachedWindSpeed = Mathf.Max(this.cachedWindSpeed, compWindSource.wind);
			}
			if (Prefs.PlantWindSway)
			{
				this.plantSwayHead += Mathf.Min(this.WindSpeed, 1f);
			}
			else
			{
				this.plantSwayHead = 0f;
			}
			if (Find.CurrentMap == this.map)
			{
				for (int j = 0; j < WindManager.plantMaterials.Count; j++)
				{
					WindManager.plantMaterials[j].SetFloat(ShaderPropertyIDs.SwayHead, this.plantSwayHead);
				}
			}
		}

		// Token: 0x060047A1 RID: 18337 RVA: 0x0025CA20 File Offset: 0x0025AE20
		public static void Notify_PlantMaterialCreated(Material newMat)
		{
			WindManager.plantMaterials.Add(newMat);
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x0025CA30 File Offset: 0x0025AE30
		private float BaseWindSpeedAt(int ticksAbs)
		{
			if (this.windNoise == null)
			{
				int seed = Gen.HashCombineInt(this.map.Tile, 122049541) ^ Find.World.info.Seed;
				this.windNoise = new Perlin(3.9999998989515007E-05, 2.0, 0.5, 4, seed, QualityMode.Medium);
				this.windNoise = new ScaleBias(1.5, 0.5, this.windNoise);
				this.windNoise = new Clamp(0.039999999105930328, 2.0, this.windNoise);
			}
			return (float)this.windNoise.GetValue((double)ticksAbs, 0.0, 0.0);
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x0025CB08 File Offset: 0x0025AF08
		public string DebugString()
		{
			return string.Concat(new object[]
			{
				"WindSpeed: ",
				this.WindSpeed,
				"\nplantSwayHead: ",
				this.plantSwayHead
			});
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x0025CB54 File Offset: 0x0025AF54
		public void LogWindSpeeds()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Upcoming wind speeds:");
			for (int i = 0; i < 72; i++)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"Hour ",
					i,
					" - ",
					this.BaseWindSpeedAt(Find.TickManager.TicksAbs + 2500 * i).ToString("F2")
				}));
			}
			Log.Message(stringBuilder.ToString(), false);
		}
	}
}
