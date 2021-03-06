﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	public class WorldGenStep_Rivers : WorldGenStep
	{
		private static readonly SimpleCurve ElevationChangeCost = new SimpleCurve
		{
			{
				new CurvePoint(-1000f, 50f),
				true
			},
			{
				new CurvePoint(-100f, 100f),
				true
			},
			{
				new CurvePoint(0f, 400f),
				true
			},
			{
				new CurvePoint(0f, 5000f),
				true
			},
			{
				new CurvePoint(100f, 50000f),
				true
			},
			{
				new CurvePoint(1000f, 50000f),
				true
			}
		};

		private const float HillinessSmallHillsElevation = 15f;

		private const float HillinessLargeHillsElevation = 250f;

		private const float HillinessMountainousElevation = 500f;

		private const float HillinessImpassableElevation = 1000f;

		private const float NonRiverEvaporation = 0f;

		private const float EvaporationMultiple = 250f;

		[CompilerGenerated]
		private static Func<int, bool> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<RiverDef, int> <>f__am$cache1;

		[CompilerGenerated]
		private static Func<RiverDef.Branch, int> <>f__am$cache2;

		public WorldGenStep_Rivers()
		{
		}

		public override int SeedPart
		{
			get
			{
				return 605014749;
			}
		}

		public override void GenerateFresh(string seed)
		{
			this.GenerateRivers();
		}

		private void GenerateRivers()
		{
			Find.WorldPathGrid.RecalculateAllPerceivedPathCosts();
			List<int> coastalWaterTiles = this.GetCoastalWaterTiles();
			if (!coastalWaterTiles.Any<int>())
			{
				return;
			}
			List<int> neighbors = new List<int>();
			List<int>[] array = Find.WorldPathFinder.FloodPathsWithCostForTree(coastalWaterTiles, delegate(int st, int ed)
			{
				Tile tile = Find.WorldGrid[ed];
				Tile tile2 = Find.WorldGrid[st];
				Find.WorldGrid.GetTileNeighbors(ed, neighbors);
				int num = neighbors[0];
				for (int j = 0; j < neighbors.Count; j++)
				{
					if (WorldGenStep_Rivers.GetImpliedElevation(Find.WorldGrid[neighbors[j]]) < WorldGenStep_Rivers.GetImpliedElevation(Find.WorldGrid[num]))
					{
						num = neighbors[j];
					}
				}
				float num2 = 1f;
				if (num != st)
				{
					num2 = 2f;
				}
				return Mathf.RoundToInt(num2 * WorldGenStep_Rivers.ElevationChangeCost.Evaluate(WorldGenStep_Rivers.GetImpliedElevation(tile2) - WorldGenStep_Rivers.GetImpliedElevation(tile)));
			}, (int tid) => Find.WorldGrid[tid].WaterCovered, null);
			float[] flow = new float[array.Length];
			for (int i = 0; i < coastalWaterTiles.Count; i++)
			{
				this.AccumulateFlow(flow, array, coastalWaterTiles[i]);
				this.CreateRivers(flow, array, coastalWaterTiles[i]);
			}
		}

		private static float GetImpliedElevation(Tile tile)
		{
			float num = 0f;
			if (tile.hilliness == Hilliness.SmallHills)
			{
				num = 15f;
			}
			else if (tile.hilliness == Hilliness.LargeHills)
			{
				num = 250f;
			}
			else if (tile.hilliness == Hilliness.Mountainous)
			{
				num = 500f;
			}
			else if (tile.hilliness == Hilliness.Impassable)
			{
				num = 1000f;
			}
			return tile.elevation + num;
		}

		private List<int> GetCoastalWaterTiles()
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
			{
				Tile tile = Find.WorldGrid[i];
				if (tile.biome == BiomeDefOf.Ocean)
				{
					Find.WorldGrid.GetTileNeighbors(i, list2);
					bool flag = false;
					for (int j = 0; j < list2.Count; j++)
					{
						bool flag2 = Find.WorldGrid[list2[j]].biome != BiomeDefOf.Ocean;
						if (flag2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						list.Add(i);
					}
				}
			}
			return list;
		}

		private void AccumulateFlow(float[] flow, List<int>[] riverPaths, int index)
		{
			Tile tile = Find.WorldGrid[index];
			flow[index] += tile.rainfall;
			if (riverPaths[index] != null)
			{
				for (int i = 0; i < riverPaths[index].Count; i++)
				{
					this.AccumulateFlow(flow, riverPaths, riverPaths[index][i]);
					flow[index] += flow[riverPaths[index][i]];
				}
			}
			flow[index] = Mathf.Max(0f, flow[index] - WorldGenStep_Rivers.CalculateTotalEvaporation(flow[index], tile.temperature));
		}

		private void CreateRivers(float[] flow, List<int>[] riverPaths, int index)
		{
			List<int> list = new List<int>();
			Find.WorldGrid.GetTileNeighbors(index, list);
			for (int i = 0; i < list.Count; i++)
			{
				float targetFlow = flow[list[i]];
				RiverDef riverDef = (from rd in DefDatabase<RiverDef>.AllDefs
				where rd.spawnFlowThreshold > 0 && (float)rd.spawnFlowThreshold <= targetFlow
				select rd).MaxByWithFallback((RiverDef rd) => rd.spawnFlowThreshold, null);
				if (riverDef != null && Rand.Value < riverDef.spawnChance)
				{
					Find.WorldGrid.OverlayRiver(index, list[i], riverDef);
					this.ExtendRiver(flow, riverPaths, list[i], riverDef);
				}
			}
		}

		private void ExtendRiver(float[] flow, List<int>[] riverPaths, int index, RiverDef incomingRiver)
		{
			if (riverPaths[index] == null)
			{
				return;
			}
			int bestOutput = riverPaths[index].MaxBy((int ni) => flow[ni]);
			RiverDef riverDef = incomingRiver;
			while (riverDef != null && (float)riverDef.degradeThreshold > flow[bestOutput])
			{
				riverDef = riverDef.degradeChild;
			}
			if (riverDef != null)
			{
				Find.WorldGrid.OverlayRiver(index, bestOutput, riverDef);
				this.ExtendRiver(flow, riverPaths, bestOutput, riverDef);
			}
			if (incomingRiver.branches != null)
			{
				using (IEnumerator<int> enumerator = (from ni in riverPaths[index]
				where ni != bestOutput
				select ni).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int alternateRiver = enumerator.Current;
						RiverDef.Branch branch2 = (from branch in incomingRiver.branches
						where (float)branch.minFlow <= flow[alternateRiver]
						select branch).MaxByWithFallback((RiverDef.Branch branch) => branch.minFlow, null);
						if (branch2 != null && Rand.Value < branch2.chance)
						{
							Find.WorldGrid.OverlayRiver(index, alternateRiver, branch2.child);
							this.ExtendRiver(flow, riverPaths, alternateRiver, branch2.child);
						}
					}
				}
			}
		}

		public static float CalculateEvaporationConstant(float temperature)
		{
			float num = 0.61121f * Mathf.Exp((18.678f - temperature / 234.5f) * (temperature / (257.14f + temperature)));
			return num / (temperature + 273f);
		}

		public static float CalculateRiverSurfaceArea(float flow)
		{
			return Mathf.Pow(flow, 0.5f);
		}

		public static float CalculateEvaporativeArea(float flow)
		{
			return WorldGenStep_Rivers.CalculateRiverSurfaceArea(flow);
		}

		public static float CalculateTotalEvaporation(float flow, float temperature)
		{
			return WorldGenStep_Rivers.CalculateEvaporationConstant(temperature) * WorldGenStep_Rivers.CalculateEvaporativeArea(flow) * 250f;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static WorldGenStep_Rivers()
		{
		}

		[CompilerGenerated]
		private static bool <GenerateRivers>m__0(int tid)
		{
			return Find.WorldGrid[tid].WaterCovered;
		}

		[CompilerGenerated]
		private static int <CreateRivers>m__1(RiverDef rd)
		{
			return rd.spawnFlowThreshold;
		}

		[CompilerGenerated]
		private static int <ExtendRiver>m__2(RiverDef.Branch branch)
		{
			return branch.minFlow;
		}

		[CompilerGenerated]
		private sealed class <GenerateRivers>c__AnonStorey0
		{
			internal List<int> neighbors;

			public <GenerateRivers>c__AnonStorey0()
			{
			}

			internal int <>m__0(int st, int ed)
			{
				Tile tile = Find.WorldGrid[ed];
				Tile tile2 = Find.WorldGrid[st];
				Find.WorldGrid.GetTileNeighbors(ed, this.neighbors);
				int num = this.neighbors[0];
				for (int i = 0; i < this.neighbors.Count; i++)
				{
					if (WorldGenStep_Rivers.GetImpliedElevation(Find.WorldGrid[this.neighbors[i]]) < WorldGenStep_Rivers.GetImpliedElevation(Find.WorldGrid[num]))
					{
						num = this.neighbors[i];
					}
				}
				float num2 = 1f;
				if (num != st)
				{
					num2 = 2f;
				}
				return Mathf.RoundToInt(num2 * WorldGenStep_Rivers.ElevationChangeCost.Evaluate(WorldGenStep_Rivers.GetImpliedElevation(tile2) - WorldGenStep_Rivers.GetImpliedElevation(tile)));
			}
		}

		[CompilerGenerated]
		private sealed class <CreateRivers>c__AnonStorey1
		{
			internal float targetFlow;

			public <CreateRivers>c__AnonStorey1()
			{
			}

			internal bool <>m__0(RiverDef rd)
			{
				return rd.spawnFlowThreshold > 0 && (float)rd.spawnFlowThreshold <= this.targetFlow;
			}
		}

		[CompilerGenerated]
		private sealed class <ExtendRiver>c__AnonStorey2
		{
			internal float[] flow;

			internal int bestOutput;

			public <ExtendRiver>c__AnonStorey2()
			{
			}

			internal float <>m__0(int ni)
			{
				return this.flow[ni];
			}

			internal bool <>m__1(int ni)
			{
				return ni != this.bestOutput;
			}
		}

		[CompilerGenerated]
		private sealed class <ExtendRiver>c__AnonStorey3
		{
			internal int alternateRiver;

			internal WorldGenStep_Rivers.<ExtendRiver>c__AnonStorey2 <>f__ref$2;

			public <ExtendRiver>c__AnonStorey3()
			{
			}

			internal bool <>m__0(RiverDef.Branch branch)
			{
				return (float)branch.minFlow <= this.<>f__ref$2.flow[this.alternateRiver];
			}
		}
	}
}
