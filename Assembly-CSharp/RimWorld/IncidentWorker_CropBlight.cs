﻿using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000327 RID: 807
	public class IncidentWorker_CropBlight : IncidentWorker
	{
		// Token: 0x040008C3 RID: 2243
		private const float Radius = 16f;

		// Token: 0x040008C4 RID: 2244
		private const float BaseBlightChance = 0.1f;

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00075DF0 File Offset: 0x000741F0
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Plant plant;
			return this.TryFindRandomBlightablePlant((Map)parms.target, out plant);
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x00075E18 File Offset: 0x00074218
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Plant plant;
			bool result;
			if (!this.TryFindRandomBlightablePlant(map, out plant))
			{
				result = false;
			}
			else
			{
				Room room = plant.GetRoom(RegionType.Set_Passable);
				plant.CropBlighted();
				int i = 0;
				int num = GenRadial.NumCellsInRadius(16f);
				while (i < num)
				{
					IntVec3 intVec = plant.Position + GenRadial.RadialPattern[i];
					if (intVec.InBounds(map) && intVec.GetRoom(map, RegionType.Set_Passable) == room)
					{
						Plant firstBlightableNowPlant = BlightUtility.GetFirstBlightableNowPlant(intVec, map);
						if (firstBlightableNowPlant != null && firstBlightableNowPlant != plant)
						{
							if (Rand.Chance(0.1f * this.BlightChanceFactor(firstBlightableNowPlant.Position, plant.Position)))
							{
								firstBlightableNowPlant.CropBlighted();
							}
						}
					}
					i++;
				}
				Find.LetterStack.ReceiveLetter("LetterLabelCropBlight".Translate(), "LetterCropBlight".Translate(), LetterDefOf.NegativeEvent, new TargetInfo(plant.Position, map, false), null, null);
				result = true;
			}
			return result;
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x00075F40 File Offset: 0x00074340
		private bool TryFindRandomBlightablePlant(Map map, out Plant plant)
		{
			Thing thing;
			bool result = (from x in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
			where ((Plant)x).BlightableNow
			select x).TryRandomElement(out thing);
			plant = (Plant)thing;
			return result;
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00075F98 File Offset: 0x00074398
		private float BlightChanceFactor(IntVec3 c, IntVec3 root)
		{
			return Mathf.InverseLerp(16f, 8f, c.DistanceTo(root));
		}
	}
}
