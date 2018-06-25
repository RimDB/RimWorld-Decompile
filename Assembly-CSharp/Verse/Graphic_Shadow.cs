﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000DE0 RID: 3552
	public class Graphic_Shadow : Graphic
	{
		// Token: 0x040034C7 RID: 13511
		private Mesh shadowMesh;

		// Token: 0x040034C8 RID: 13512
		private ShadowData shadowInfo;

		// Token: 0x040034C9 RID: 13513
		[TweakValue("Graphics_Shadow", -5f, 5f)]
		private static float GlobalShadowPosOffsetX = 0f;

		// Token: 0x040034CA RID: 13514
		[TweakValue("Graphics_Shadow", -5f, 5f)]
		private static float GlobalShadowPosOffsetZ = 0f;

		// Token: 0x06004F93 RID: 20371 RVA: 0x00296B20 File Offset: 0x00294F20
		public Graphic_Shadow(ShadowData shadowInfo)
		{
			this.shadowInfo = shadowInfo;
			if (shadowInfo == null)
			{
				throw new ArgumentNullException("shadowInfo");
			}
			this.shadowMesh = ShadowMeshPool.GetShadowMesh(shadowInfo);
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x00296B50 File Offset: 0x00294F50
		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			if (this.shadowMesh != null)
			{
				if (thingDef != null && this.shadowInfo != null && (Find.CurrentMap == null || !loc.ToIntVec3().InBounds(Find.CurrentMap) || !Find.CurrentMap.roofGrid.Roofed(loc.ToIntVec3())) && DebugViewSettings.drawShadows)
				{
					Vector3 position = loc + this.shadowInfo.offset;
					position.y = AltitudeLayer.Shadows.AltitudeFor();
					Graphics.DrawMesh(this.shadowMesh, position, rot.AsQuat, MatBases.SunShadowFade, 0);
				}
			}
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x00296C00 File Offset: 0x00295000
		public override void Print(SectionLayer layer, Thing thing)
		{
			Vector3 center = thing.TrueCenter() + (this.shadowInfo.offset + new Vector3(Graphic_Shadow.GlobalShadowPosOffsetX, 0f, Graphic_Shadow.GlobalShadowPosOffsetZ)).RotatedBy(thing.Rotation);
			center.y = AltitudeLayer.Shadows.AltitudeFor();
			Printer_Shadow.PrintShadow(layer, center, this.shadowInfo, thing.Rotation);
		}

		// Token: 0x06004F96 RID: 20374 RVA: 0x00296C6C File Offset: 0x0029506C
		public override string ToString()
		{
			return "Graphic_Shadow(" + this.shadowInfo + ")";
		}
	}
}
