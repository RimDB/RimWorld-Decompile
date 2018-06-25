﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RimWorld.Planet
{
	public class CaravanArrivalAction_Enter : CaravanArrivalAction
	{
		private MapParent mapParent;

		public CaravanArrivalAction_Enter()
		{
		}

		public CaravanArrivalAction_Enter(MapParent mapParent)
		{
			this.mapParent = mapParent;
		}

		public override string Label
		{
			get
			{
				return "EnterMap".Translate(new object[]
				{
					this.mapParent.Label
				});
			}
		}

		public override string ReportString
		{
			get
			{
				return "CaravanEntering".Translate(new object[]
				{
					this.mapParent.Label
				});
			}
		}

		public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
		{
			FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
			FloatMenuAcceptanceReport result;
			if (!floatMenuAcceptanceReport)
			{
				result = floatMenuAcceptanceReport;
			}
			else if (this.mapParent != null && this.mapParent.Tile != destinationTile)
			{
				result = false;
			}
			else
			{
				result = CaravanArrivalAction_Enter.CanEnter(caravan, this.mapParent);
			}
			return result;
		}

		public override void Arrived(Caravan caravan)
		{
			Map map = this.mapParent.Map;
			if (map != null)
			{
				Pawn t = caravan.PawnsListForReading[0];
				CaravanDropInventoryMode dropInventoryMode = (!map.IsPlayerHome) ? CaravanDropInventoryMode.DoNotDrop : CaravanDropInventoryMode.UnloadIndividually;
				bool draftColonists = this.mapParent.Faction != null && this.mapParent.Faction.HostileTo(Faction.OfPlayer);
				CaravanEnterMapUtility.Enter(caravan, map, CaravanEnterMode.Edge, dropInventoryMode, draftColonists, null);
				if (this.mapParent.def == WorldObjectDefOf.Ambush)
				{
					Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
					Find.LetterStack.ReceiveLetter("LetterLabelCaravanEnteredAmbushMap".Translate(), "LetterCaravanEnteredAmbushMap".Translate(new object[]
					{
						caravan.Label
					}).CapitalizeFirst(), LetterDefOf.NeutralEvent, t, null, null);
				}
				else if (caravan.IsPlayerControlled || this.mapParent.Faction == Faction.OfPlayer)
				{
					Messages.Message("MessageCaravanEnteredWorldObject".Translate(new object[]
					{
						caravan.Label,
						this.mapParent.Label
					}).CapitalizeFirst(), t, MessageTypeDefOf.TaskCompletion, true);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<MapParent>(ref this.mapParent, "mapParent", false);
		}

		public static FloatMenuAcceptanceReport CanEnter(Caravan caravan, MapParent mapParent)
		{
			FloatMenuAcceptanceReport result;
			if (mapParent == null || !mapParent.Spawned || !mapParent.HasMap)
			{
				result = false;
			}
			else if (mapParent.EnterCooldownBlocksEntering())
			{
				result = FloatMenuAcceptanceReport.WithFailMessage("MessageEnterCooldownBlocksEntering".Translate(new object[]
				{
					mapParent.EnterCooldownDaysLeft().ToString("0.#")
				}));
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
		{
			return CaravanArrivalActionUtility.GetFloatMenuOptions<CaravanArrivalAction_Enter>(() => CaravanArrivalAction_Enter.CanEnter(caravan, mapParent), () => new CaravanArrivalAction_Enter(mapParent), "EnterMap".Translate(new object[]
			{
				mapParent.Label
			}), caravan, mapParent.Tile, mapParent);
		}

		[CompilerGenerated]
		private sealed class <GetFloatMenuOptions>c__AnonStorey0
		{
			internal Caravan caravan;

			internal MapParent mapParent;

			public <GetFloatMenuOptions>c__AnonStorey0()
			{
			}

			internal FloatMenuAcceptanceReport <>m__0()
			{
				return CaravanArrivalAction_Enter.CanEnter(this.caravan, this.mapParent);
			}

			internal CaravanArrivalAction_Enter <>m__1()
			{
				return new CaravanArrivalAction_Enter(this.mapParent);
			}
		}
	}
}
