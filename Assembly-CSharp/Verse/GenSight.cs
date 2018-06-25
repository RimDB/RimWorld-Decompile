﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000F4C RID: 3916
	public static class GenSight
	{
		// Token: 0x06005EB4 RID: 24244 RVA: 0x003031DC File Offset: 0x003015DC
		public static bool LineOfSight(IntVec3 start, IntVec3 end, Map map, bool skipFirstCell = false, Func<IntVec3, bool> validator = null, int halfXOffset = 0, int halfZOffset = 0)
		{
			bool result;
			if (!start.InBounds(map) || !end.InBounds(map))
			{
				result = false;
			}
			else
			{
				bool flag;
				if (start.x == end.x)
				{
					flag = (start.z < end.z);
				}
				else
				{
					flag = (start.x < end.x);
				}
				int num = Mathf.Abs(end.x - start.x);
				int num2 = Mathf.Abs(end.z - start.z);
				int num3 = start.x;
				int num4 = start.z;
				int i = 1 + num + num2;
				int num5 = (end.x <= start.x) ? -1 : 1;
				int num6 = (end.z <= start.z) ? -1 : 1;
				num *= 4;
				num2 *= 4;
				num += halfXOffset * 2;
				num2 += halfZOffset * 2;
				int num7 = num / 2 - num2 / 2;
				IntVec3 intVec = default(IntVec3);
				while (i > 1)
				{
					intVec.x = num3;
					intVec.z = num4;
					if (!skipFirstCell || !(intVec == start))
					{
						if (!intVec.CanBeSeenOverFast(map))
						{
							return false;
						}
						if (validator != null && !validator(intVec))
						{
							return false;
						}
					}
					if (num7 > 0 || (num7 == 0 && flag))
					{
						num3 += num5;
						num7 -= num2;
					}
					else
					{
						num4 += num6;
						num7 += num;
					}
					i--;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06005EB5 RID: 24245 RVA: 0x00303394 File Offset: 0x00301794
		public static bool LineOfSight(IntVec3 start, IntVec3 end, Map map, CellRect startRect, CellRect endRect, Func<IntVec3, bool> validator = null)
		{
			bool result;
			if (!start.InBounds(map) || !end.InBounds(map))
			{
				result = false;
			}
			else
			{
				bool flag;
				if (start.x == end.x)
				{
					flag = (start.z < end.z);
				}
				else
				{
					flag = (start.x < end.x);
				}
				int num = Mathf.Abs(end.x - start.x);
				int num2 = Mathf.Abs(end.z - start.z);
				int num3 = start.x;
				int num4 = start.z;
				int i = 1 + num + num2;
				int num5 = (end.x <= start.x) ? -1 : 1;
				int num6 = (end.z <= start.z) ? -1 : 1;
				int num7 = num - num2;
				num *= 2;
				num2 *= 2;
				IntVec3 intVec = default(IntVec3);
				while (i > 1)
				{
					intVec.x = num3;
					intVec.z = num4;
					if (endRect.Contains(intVec))
					{
						return true;
					}
					if (!startRect.Contains(intVec))
					{
						if (!intVec.CanBeSeenOverFast(map))
						{
							return false;
						}
						if (validator != null && !validator(intVec))
						{
							return false;
						}
					}
					if (num7 > 0 || (num7 == 0 && flag))
					{
						num3 += num5;
						num7 -= num2;
					}
					else
					{
						num4 += num6;
						num7 += num;
					}
					i--;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06005EB6 RID: 24246 RVA: 0x00303548 File Offset: 0x00301948
		public static IEnumerable<IntVec3> PointsOnLineOfSight(IntVec3 start, IntVec3 end)
		{
			bool sideOnEqual;
			if (start.x == end.x)
			{
				sideOnEqual = (start.z < end.z);
			}
			else
			{
				sideOnEqual = (start.x < end.x);
			}
			int dx = Mathf.Abs(end.x - start.x);
			int dz = Mathf.Abs(end.z - start.z);
			int x = start.x;
			int z = start.z;
			int i = 1 + dx + dz;
			int x_inc = (end.x <= start.x) ? -1 : 1;
			int z_inc = (end.z <= start.z) ? -1 : 1;
			int error = dx - dz;
			dx *= 2;
			dz *= 2;
			IntVec3 c = default(IntVec3);
			while (i > 1)
			{
				c.x = x;
				c.z = z;
				yield return c;
				if (error > 0 || (error == 0 && sideOnEqual))
				{
					x += x_inc;
					error -= dz;
				}
				else
				{
					z += z_inc;
					error += dx;
				}
				i--;
			}
			yield break;
		}

		// Token: 0x06005EB7 RID: 24247 RVA: 0x0030357C File Offset: 0x0030197C
		public static bool LineOfSightToEdges(IntVec3 start, IntVec3 end, Map map, bool skipFirstCell = false, Func<IntVec3, bool> validator = null)
		{
			bool result;
			if (GenSight.LineOfSight(start, end, map, skipFirstCell, validator, 0, 0))
			{
				result = true;
			}
			else
			{
				int num = (start * 2).DistanceToSquared(end * 2);
				for (int i = 0; i < 4; i++)
				{
					if ((start * 2).DistanceToSquared(end * 2 + GenAdj.CardinalDirections[i]) <= num)
					{
						if (GenSight.LineOfSight(start, end, map, skipFirstCell, validator, GenAdj.CardinalDirections[i].x, GenAdj.CardinalDirections[i].z))
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
