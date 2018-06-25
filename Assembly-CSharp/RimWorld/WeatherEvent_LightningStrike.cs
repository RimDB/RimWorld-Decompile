﻿using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x0200044B RID: 1099
	[StaticConstructorOnStartup]
	public class WeatherEvent_LightningStrike : WeatherEvent_LightningFlash
	{
		// Token: 0x04000BA0 RID: 2976
		private IntVec3 strikeLoc = IntVec3.Invalid;

		// Token: 0x04000BA1 RID: 2977
		private Mesh boltMesh = null;

		// Token: 0x04000BA2 RID: 2978
		private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);

		// Token: 0x0600131C RID: 4892 RVA: 0x000A4A90 File Offset: 0x000A2E90
		public WeatherEvent_LightningStrike(Map map) : base(map)
		{
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x000A4AAC File Offset: 0x000A2EAC
		public WeatherEvent_LightningStrike(Map map, IntVec3 forcedStrikeLoc) : base(map)
		{
			this.strikeLoc = forcedStrikeLoc;
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x000A4AD0 File Offset: 0x000A2ED0
		public override void FireEvent()
		{
			base.FireEvent();
			if (!this.strikeLoc.IsValid)
			{
				this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
			}
			this.boltMesh = LightningBoltMeshPool.RandomBoltMesh;
			if (!this.strikeLoc.Fogged(this.map))
			{
				GenExplosion.DoExplosion(this.strikeLoc, this.map, 1.9f, DamageDefOf.Flame, null, -1, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
				Vector3 loc = this.strikeLoc.ToVector3Shifted();
				for (int i = 0; i < 4; i++)
				{
					MoteMaker.ThrowSmoke(loc, this.map, 1.5f);
					MoteMaker.ThrowMicroSparks(loc, this.map);
					MoteMaker.ThrowLightningGlow(loc, this.map, 1.5f);
				}
			}
			SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
			SoundDefOf.Thunder_OnMap.PlayOneShot(info);
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x000A4BE3 File Offset: 0x000A2FE3
		public override void WeatherEventDraw()
		{
			Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_LightningStrike.LightningMat, base.LightningBrightness), 0);
		}
	}
}
