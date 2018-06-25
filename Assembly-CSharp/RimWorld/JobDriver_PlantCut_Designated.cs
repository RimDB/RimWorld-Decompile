﻿using System;
using Verse;

namespace RimWorld
{
	public class JobDriver_PlantCut_Designated : JobDriver_PlantCut
	{
		public JobDriver_PlantCut_Designated()
		{
		}

		protected override DesignationDef RequiredDesignation
		{
			get
			{
				return DesignationDefOf.CutPlant;
			}
		}
	}
}
