﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_QuestItemStashAICore : IncidentWorker_QuestItemStash
	{
		public IncidentWorker_QuestItemStashAICore()
		{
		}

		protected override List<Thing> GenerateItems(Faction siteFaction)
		{
			return new List<Thing>
			{
				ThingMaker.MakeThing(ThingDefOf.AIPersonaCore, null)
			};
		}
	}
}
