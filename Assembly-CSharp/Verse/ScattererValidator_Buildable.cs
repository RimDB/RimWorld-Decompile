﻿using System;

namespace Verse
{
	// Token: 0x02000C5E RID: 3166
	public class ScattererValidator_Buildable : ScattererValidator
	{
		// Token: 0x04002F9F RID: 12191
		public int radius = 1;

		// Token: 0x04002FA0 RID: 12192
		public TerrainAffordanceDef affordance;

		// Token: 0x060045AA RID: 17834 RVA: 0x0024CDEC File Offset: 0x0024B1EC
		public override bool Allows(IntVec3 c, Map map)
		{
			CellRect cellRect = CellRect.CenteredOn(c, this.radius);
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 c2 = new IntVec3(j, 0, i);
					if (!c2.InBounds(map))
					{
						return false;
					}
					if (c2.InNoBuildEdgeArea(map))
					{
						return false;
					}
					if (this.affordance != null && !c2.GetTerrain(map).affordances.Contains(this.affordance))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
