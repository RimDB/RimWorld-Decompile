﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x02000400 RID: 1024
	public static class GridShapeMaker
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x00098650 File Offset: 0x00096A50
		public static IEnumerable<IntVec3> IrregularLump(IntVec3 center, Map map, int numCells)
		{
			List<IntVec3> lumpCells = new List<IntVec3>();
			for (int i = 0; i < numCells * 2; i++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[i];
				if (intVec.InBounds(map))
				{
					lumpCells.Add(intVec);
				}
			}
			Func<IntVec3, int> NumNeighbors = delegate(IntVec3 sq)
			{
				int num2 = 0;
				for (int k = 0; k < 4; k++)
				{
					IntVec3 item = sq + GenAdj.CardinalDirections[k];
					if (lumpCells.Contains(item))
					{
						num2++;
					}
				}
				return num2;
			};
			while (lumpCells.Count > numCells)
			{
				int fewestNeighbors = 99;
				for (int j = 0; j < lumpCells.Count; j++)
				{
					IntVec3 arg = lumpCells[j];
					int num = NumNeighbors(arg);
					if (num < fewestNeighbors)
					{
						fewestNeighbors = num;
					}
				}
				List<IntVec3> source = (from sq in lumpCells
				where NumNeighbors(sq) == fewestNeighbors
				select sq).ToList<IntVec3>();
				lumpCells.Remove(source.RandomElement<IntVec3>());
			}
			return lumpCells;
		}
	}
}
