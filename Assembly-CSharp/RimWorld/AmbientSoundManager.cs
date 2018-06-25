﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x02000656 RID: 1622
	public static class AmbientSoundManager
	{
		// Token: 0x0400132C RID: 4908
		private static List<Sustainer> biomeAmbientSustainers = new List<Sustainer>();

		// Token: 0x0400132D RID: 4909
		private static Action recreateMapSustainers;

		// Token: 0x0400132E RID: 4910
		[CompilerGenerated]
		private static Action <>f__mg$cache0;

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x060021D7 RID: 8663 RVA: 0x0011F480 File Offset: 0x0011D880
		private static bool WorldAmbientSoundCreated
		{
			get
			{
				return Find.SoundRoot.sustainerManager.SustainerExists(SoundDefOf.Ambient_Space);
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x060021D8 RID: 8664 RVA: 0x0011F4AC File Offset: 0x0011D8AC
		private static bool AltitudeWindSoundCreated
		{
			get
			{
				return Find.SoundRoot.sustainerManager.SustainerExists(SoundDefOf.Ambient_AltitudeWind);
			}
		}

		// Token: 0x060021D9 RID: 8665 RVA: 0x0011F4D5 File Offset: 0x0011D8D5
		public static void EnsureWorldAmbientSoundCreated()
		{
			if (!AmbientSoundManager.WorldAmbientSoundCreated)
			{
				SoundDefOf.Ambient_Space.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.None));
			}
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x0011F4F3 File Offset: 0x0011D8F3
		public static void Notify_SwitchedMap()
		{
			LongEventHandler.ExecuteWhenFinished(AmbientSoundManager.recreateMapSustainers);
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x0011F500 File Offset: 0x0011D900
		private static void RecreateMapSustainers()
		{
			if (!AmbientSoundManager.AltitudeWindSoundCreated)
			{
				SoundDefOf.Ambient_AltitudeWind.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.None));
			}
			SustainerManager sustainerManager = Find.SoundRoot.sustainerManager;
			for (int i = 0; i < AmbientSoundManager.biomeAmbientSustainers.Count; i++)
			{
				Sustainer sustainer = AmbientSoundManager.biomeAmbientSustainers[i];
				if (sustainerManager.AllSustainers.Contains(sustainer) && !sustainer.Ended)
				{
					sustainer.End();
				}
			}
			AmbientSoundManager.biomeAmbientSustainers.Clear();
			if (Find.CurrentMap != null)
			{
				List<SoundDef> soundsAmbient = Find.CurrentMap.Biome.soundsAmbient;
				for (int j = 0; j < soundsAmbient.Count; j++)
				{
					Sustainer item = soundsAmbient[j].TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.None));
					AmbientSoundManager.biomeAmbientSustainers.Add(item);
				}
			}
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x0011F5E3 File Offset: 0x0011D9E3
		// Note: this type is marked as 'beforefieldinit'.
		static AmbientSoundManager()
		{
			if (AmbientSoundManager.<>f__mg$cache0 == null)
			{
				AmbientSoundManager.<>f__mg$cache0 = new Action(AmbientSoundManager.RecreateMapSustainers);
			}
			AmbientSoundManager.recreateMapSustainers = AmbientSoundManager.<>f__mg$cache0;
		}
	}
}
