﻿using System;
using System.Collections.Generic;

namespace Verse
{
	// Token: 0x02000F45 RID: 3909
	public static class GenList
	{
		// Token: 0x06005E65 RID: 24165 RVA: 0x00300098 File Offset: 0x002FE498
		public static int CountAllowNull<T>(this IList<T> list)
		{
			return (list == null) ? 0 : list.Count;
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x003000C0 File Offset: 0x002FE4C0
		public static bool NullOrEmpty<T>(this IList<T> list)
		{
			return list == null || list.Count == 0;
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x003000E8 File Offset: 0x002FE4E8
		public static List<T> ListFullCopy<T>(this List<T> source)
		{
			List<T> list = new List<T>(source.Count);
			for (int i = 0; i < source.Count; i++)
			{
				list.Add(source[i]);
			}
			return list;
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x00300130 File Offset: 0x002FE530
		public static List<T> ListFullCopyOrNull<T>(this List<T> source)
		{
			List<T> result;
			if (source == null)
			{
				result = null;
			}
			else
			{
				result = source.ListFullCopy<T>();
			}
			return result;
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x00300158 File Offset: 0x002FE558
		public static void RemoveDuplicates<T>(this List<T> list) where T : class
		{
			if (list.Count > 1)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					for (int j = 0; j < i; j++)
					{
						if (list[i] == list[j])
						{
							list.RemoveAt(i);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x003001D0 File Offset: 0x002FE5D0
		public static void Shuffle<T>(this IList<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int index = Rand.RangeInclusive(0, i);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x00300220 File Offset: 0x002FE620
		public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
		{
			int count = list.Count;
			for (int i = 1; i < count; i++)
			{
				T t = list[i];
				int num = i - 1;
				while (num >= 0 && comparison(list[num], t) > 0)
				{
					list[num + 1] = list[num];
					num--;
				}
				list[num + 1] = t;
			}
		}
	}
}
