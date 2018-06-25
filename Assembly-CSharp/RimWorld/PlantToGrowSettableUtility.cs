﻿using System;
using Verse;

namespace RimWorld
{
	public static class PlantToGrowSettableUtility
	{
		public static Command_SetPlantToGrow SetPlantToGrowCommand(IPlantToGrowSettable settable)
		{
			return new Command_SetPlantToGrow
			{
				defaultDesc = "CommandSelectPlantToGrowDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc1,
				settable = settable
			};
		}
	}
}
