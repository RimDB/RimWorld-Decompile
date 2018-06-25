﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020006E9 RID: 1769
	public static class SkyfallerShrapnelUtility
	{
		// Token: 0x04001574 RID: 5492
		private const float ShrapnelDistanceFront = 6f;

		// Token: 0x04001575 RID: 5493
		private const float ShrapnelDistanceSide = 4f;

		// Token: 0x04001576 RID: 5494
		private const float ShrapnelDistanceBack = 30f;

		// Token: 0x04001577 RID: 5495
		private const int MotesPerShrapnel = 2;

		// Token: 0x04001578 RID: 5496
		private static readonly SimpleCurve ShrapnelDistanceFromAngle = new SimpleCurve
		{
			{
				new CurvePoint(0f, 6f),
				true
			},
			{
				new CurvePoint(90f, 4f),
				true
			},
			{
				new CurvePoint(135f, 4f),
				true
			},
			{
				new CurvePoint(180f, 30f),
				true
			},
			{
				new CurvePoint(225f, 4f),
				true
			},
			{
				new CurvePoint(270f, 4f),
				true
			},
			{
				new CurvePoint(360f, 6f),
				true
			}
		};

		// Token: 0x04001579 RID: 5497
		private static readonly SimpleCurve ShrapnelAngleDistribution = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0f),
				true
			},
			{
				new CurvePoint(0.1f, 90f),
				true
			},
			{
				new CurvePoint(0.25f, 135f),
				true
			},
			{
				new CurvePoint(0.5f, 180f),
				true
			},
			{
				new CurvePoint(0.75f, 225f),
				true
			},
			{
				new CurvePoint(0.9f, 270f),
				true
			},
			{
				new CurvePoint(1f, 360f),
				true
			}
		};

		// Token: 0x0600268C RID: 9868 RVA: 0x0014A22C File Offset: 0x0014862C
		public static void MakeShrapnel(IntVec3 center, Map map, float angle, float distanceFactor, int metalShrapnelCount, int rubbleShrapnelCount, bool spawnMotes)
		{
			angle -= 90f;
			SkyfallerShrapnelUtility.SpawnShrapnel(ThingDefOf.ChunkSlagSteel, metalShrapnelCount, center, map, angle, distanceFactor);
			SkyfallerShrapnelUtility.SpawnShrapnel(ThingDefOf.Filth_RubbleBuilding, rubbleShrapnelCount, center, map, angle, distanceFactor);
			if (spawnMotes)
			{
				SkyfallerShrapnelUtility.ThrowShrapnelMotes((metalShrapnelCount + rubbleShrapnelCount) * 2, center, map, angle, distanceFactor);
			}
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x0014A27C File Offset: 0x0014867C
		private static void SpawnShrapnel(ThingDef def, int quantity, IntVec3 center, Map map, float angle, float distanceFactor)
		{
			for (int i = 0; i < quantity; i++)
			{
				IntVec3 intVec = SkyfallerShrapnelUtility.GenerateShrapnelLocation(center, angle, distanceFactor);
				if (SkyfallerShrapnelUtility.IsGoodShrapnelCell(intVec, map))
				{
					if (def.category != ThingCategory.Item || intVec.GetFirstItem(map) == null)
					{
						if (intVec.GetFirstThing(map, def) == null)
						{
							GenSpawn.Spawn(def, intVec, map, WipeMode.Vanish);
						}
					}
				}
			}
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x0014A2F4 File Offset: 0x001486F4
		private static void ThrowShrapnelMotes(int count, IntVec3 center, Map map, float angle, float distanceFactor)
		{
			for (int i = 0; i < count; i++)
			{
				IntVec3 c = SkyfallerShrapnelUtility.GenerateShrapnelLocation(center, angle, distanceFactor);
				if (SkyfallerShrapnelUtility.IsGoodShrapnelCell(c, map))
				{
					MoteMaker.ThrowDustPuff(c.ToVector3Shifted() + Gen.RandomHorizontalVector(0.5f), map, 2f);
				}
			}
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x0014A354 File Offset: 0x00148754
		private static bool IsGoodShrapnelCell(IntVec3 c, Map map)
		{
			bool result;
			if (!c.InBounds(map))
			{
				result = false;
			}
			else if (c.Impassable(map) || c.Filled(map))
			{
				result = false;
			}
			else
			{
				RoofDef roofDef = map.roofGrid.RoofAt(c);
				result = (roofDef == null);
			}
			return result;
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x0014A3B8 File Offset: 0x001487B8
		private static IntVec3 GenerateShrapnelLocation(IntVec3 center, float angleOffset, float distanceFactor)
		{
			float num = SkyfallerShrapnelUtility.ShrapnelAngleDistribution.Evaluate(Rand.Value);
			float d = SkyfallerShrapnelUtility.ShrapnelDistanceFromAngle.Evaluate(num) * Rand.Value * distanceFactor;
			return (Vector3Utility.HorizontalVectorFromAngle(num + angleOffset) * d).ToIntVec3() + center;
		}
	}
}
