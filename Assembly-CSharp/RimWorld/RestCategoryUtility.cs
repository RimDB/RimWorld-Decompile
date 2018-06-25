﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000502 RID: 1282
	public static class RestCategoryUtility
	{
		// Token: 0x0600170B RID: 5899 RVA: 0x000CB468 File Offset: 0x000C9868
		public static string GetLabel(this RestCategory fatigue)
		{
			string result;
			switch (fatigue)
			{
			case RestCategory.Rested:
				result = "HungerLevel_Rested".Translate();
				break;
			case RestCategory.Tired:
				result = "HungerLevel_Tired".Translate();
				break;
			case RestCategory.VeryTired:
				result = "HungerLevel_VeryTired".Translate();
				break;
			case RestCategory.Exhausted:
				result = "HungerLevel_Exhausted".Translate();
				break;
			default:
				throw new InvalidOperationException();
			}
			return result;
		}
	}
}
