﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005BB RID: 1467
	public class WorldGenStep_AncientRoads : WorldGenStep
	{
		// Token: 0x040010E1 RID: 4321
		public float maximumSiteCurve;

		// Token: 0x040010E2 RID: 4322
		public float minimumChain;

		// Token: 0x040010E3 RID: 4323
		public float maximumSegmentCurviness;

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x000F2738 File Offset: 0x000F0B38
		public override int SeedPart
		{
			get
			{
				return 773428712;
			}
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x000F2752 File Offset: 0x000F0B52
		public override void GenerateFresh(string seed)
		{
			this.GenerateAncientRoads();
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x000F275C File Offset: 0x000F0B5C
		private void GenerateAncientRoads()
		{
			Find.WorldPathGrid.RecalculateAllPerceivedPathCosts(new int?(0));
			List<List<int>> list = this.GenerateProspectiveRoads();
			list.Sort((List<int> lhs, List<int> rhs) => -lhs.Count.CompareTo(rhs.Count));
			HashSet<int> used = new HashSet<int>();
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].Any((int elem) => used.Contains(elem)))
				{
					if (list[i].Count < 4)
					{
						break;
					}
					foreach (int item in list[i])
					{
						used.Add(item);
					}
					int j = 0;
					while (j < list[i].Count - 1)
					{
						float num = Find.WorldGrid.ApproxDistanceInTiles(list[i][j], list[i][j + 1]) * this.maximumSegmentCurviness;
						float costCutoff = num * 12000f;
						using (WorldPath worldPath = Find.WorldPathFinder.FindPath(list[i][j], list[i][j + 1], null, (float cost) => cost > costCutoff))
						{
							if (worldPath != null && worldPath != WorldPath.NotFound)
							{
								List<int> nodesReversed = worldPath.NodesReversed;
								if ((float)nodesReversed.Count <= Find.WorldGrid.ApproxDistanceInTiles(list[i][j], list[i][j + 1]) * this.maximumSegmentCurviness)
								{
									for (int k = 0; k < nodesReversed.Count - 1; k++)
									{
										if (Find.WorldGrid.GetRoadDef(nodesReversed[k], nodesReversed[k + 1], false) != null)
										{
											Find.WorldGrid.OverlayRoad(nodesReversed[k], nodesReversed[k + 1], RoadDefOf.AncientAsphaltHighway);
										}
										else
										{
											Find.WorldGrid.OverlayRoad(nodesReversed[k], nodesReversed[k + 1], RoadDefOf.AncientAsphaltRoad);
										}
									}
								}
							}
						}
						IL_267:
						j++;
						continue;
						goto IL_267;
					}
				}
			}
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000F2A18 File Offset: 0x000F0E18
		private List<List<int>> GenerateProspectiveRoads()
		{
			List<int> ancientSites = Find.World.genData.ancientSites;
			List<List<int>> list = new List<List<int>>();
			for (int i = 0; i < ancientSites.Count; i++)
			{
				for (int j = 0; j < ancientSites.Count; j++)
				{
					List<int> list2 = new List<int>();
					list2.Add(ancientSites[i]);
					List<int> list3 = ancientSites;
					float ang = Find.World.grid.GetHeadingFromTo(i, j);
					int current = ancientSites[i];
					for (;;)
					{
						list3 = (from idx in list3
						where idx != current && Math.Abs(Find.World.grid.GetHeadingFromTo(current, idx) - ang) < this.maximumSiteCurve
						select idx).ToList<int>();
						if (list3.Count == 0)
						{
							break;
						}
						int num = list3.MinBy((int idx) => Find.World.grid.ApproxDistanceInTiles(current, idx));
						ang = Find.World.grid.GetHeadingFromTo(current, num);
						current = num;
						list2.Add(current);
					}
					list.Add(list2);
				}
			}
			return list;
		}
	}
}
