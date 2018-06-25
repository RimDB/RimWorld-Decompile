﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200078F RID: 1935
	public class Alert_FireInHomeArea : Alert_Critical
	{
		// Token: 0x06002AEE RID: 10990 RVA: 0x0016AFAD File Offset: 0x001693AD
		public Alert_FireInHomeArea()
		{
			this.defaultLabel = "FireInHomeArea".Translate();
			this.defaultExplanation = "FireInHomeAreaDesc".Translate();
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002AEF RID: 10991 RVA: 0x0016AFD8 File Offset: 0x001693D8
		private Fire FireInHomeArea
		{
			get
			{
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					List<Thing> list = maps[i].listerThings.ThingsOfDef(ThingDefOf.Fire);
					for (int j = 0; j < list.Count; j++)
					{
						Thing thing = list[j];
						if (maps[i].areaManager.Home[thing.Position] && !thing.Position.Fogged(thing.Map))
						{
							return (Fire)thing;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x0016B090 File Offset: 0x00169490
		public override AlertReport GetReport()
		{
			return this.FireInHomeArea;
		}
	}
}
