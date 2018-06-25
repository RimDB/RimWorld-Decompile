﻿using System;
using System.Collections.Generic;
using RimWorld.Planet;

namespace RimWorld
{
	// Token: 0x0200027C RID: 636
	public class WorldObjectCompProperties_DefeatAllEnemiesQuest : WorldObjectCompProperties
	{
		// Token: 0x06000AE4 RID: 2788 RVA: 0x000628C5 File Offset: 0x00060CC5
		public WorldObjectCompProperties_DefeatAllEnemiesQuest()
		{
			this.compClass = typeof(DefeatAllEnemiesQuestComp);
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x000628E0 File Offset: 0x00060CE0
		public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
		{
			foreach (string e in this.<ConfigErrors>__BaseCallProxy0(parentDef))
			{
				yield return e;
			}
			if (!typeof(MapParent).IsAssignableFrom(parentDef.worldObjectClass))
			{
				yield return parentDef.defName + " has WorldObjectCompProperties_DefeatAllEnemiesQuest but it's not MapParent.";
			}
			yield break;
		}
	}
}
