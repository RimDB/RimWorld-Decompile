using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimWorld.Planet
{
	public class Caravan_TraderTracker : IExposable
	{
		private Caravan caravan;

		private List<Pawn> soldPrisoners = new List<Pawn>();

		public TraderKindDef TraderKind
		{
			get
			{
				List<Pawn> pawnsListForReading = this.caravan.PawnsListForReading;
				for (int i = 0; i < pawnsListForReading.Count; i++)
				{
					Pawn pawn = pawnsListForReading[i];
					if (this.caravan.IsOwner(pawn) && pawn.TraderKind != null)
					{
						return pawn.TraderKind;
					}
				}
				return null;
			}
		}

		public IEnumerable<Thing> Goods
		{
			get
			{
				Caravan_TraderTracker.<>c__Iterator100 <>c__Iterator = new Caravan_TraderTracker.<>c__Iterator100();
				<>c__Iterator.<>f__this = this;
				Caravan_TraderTracker.<>c__Iterator100 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public int RandomPriceFactorSeed
		{
			get
			{
				return Gen.HashCombineInt(this.caravan.ID, 1048142365);
			}
		}

		public string TraderName
		{
			get
			{
				return this.caravan.LabelCap;
			}
		}

		public bool CanTradeNow
		{
			get
			{
				return this.TraderKind != null && !this.caravan.AllOwnersDowned && this.caravan.Faction != Faction.OfPlayer && this.Goods.Any((Thing x) => this.TraderKind.WillTrade(x.def));
			}
		}

		public Caravan_TraderTracker(Caravan caravan)
		{
			this.caravan = caravan;
		}

		public void ExposeData()
		{
			Scribe_Collections.Look<Pawn>(ref this.soldPrisoners, "soldPrisoners", LookMode.Reference, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.soldPrisoners.RemoveAll((Pawn x) => x == null);
			}
		}

		[DebuggerHidden]
		public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__Iterator101 <ColonyThingsWillingToBuy>c__Iterator = new Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__Iterator101();
			<ColonyThingsWillingToBuy>c__Iterator.playerNegotiator = playerNegotiator;
			<ColonyThingsWillingToBuy>c__Iterator.<$>playerNegotiator = playerNegotiator;
			Caravan_TraderTracker.<ColonyThingsWillingToBuy>c__Iterator101 expr_15 = <ColonyThingsWillingToBuy>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			if (this.Goods.Contains(toGive))
			{
				Log.Error("Tried to add " + toGive + " to stock (pawn's trader tracker), but it's already here.");
				return;
			}
			Caravan caravan = playerNegotiator.GetCaravan();
			Thing thing = toGive.SplitOff(countToGive);
			thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this.caravan);
			Pawn pawn = thing as Pawn;
			if (pawn != null)
			{
				CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(pawn, caravan.PawnsListForReading, null);
				this.caravan.AddPawn(pawn, false);
				if (pawn.IsWorldPawn() && !this.caravan.Spawned)
				{
					Find.WorldPawns.RemovePawn(pawn);
				}
				if (pawn.RaceProps.Humanlike)
				{
					this.soldPrisoners.Add(pawn);
				}
			}
			else
			{
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(thing, this.caravan.PawnsListForReading, null, null);
				if (pawn2 == null)
				{
					Log.Error("Could not find pawn to move sold thing to (sold by player). thing=" + thing);
					thing.Destroy(DestroyMode.Vanish);
				}
				else if (!pawn2.inventory.innerContainer.TryAdd(thing, true))
				{
					Log.Error("Could not add item to inventory.");
					thing.Destroy(DestroyMode.Vanish);
				}
			}
		}

		public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			Caravan caravan = playerNegotiator.GetCaravan();
			Thing thing = toGive.SplitOff(countToGive);
			thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this.caravan);
			Pawn pawn = thing as Pawn;
			if (pawn != null)
			{
				CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(pawn, this.caravan.PawnsListForReading, null);
				caravan.AddPawn(pawn, true);
				if (!pawn.IsWorldPawn() && caravan.Spawned)
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
				}
				this.soldPrisoners.Remove(pawn);
			}
			else
			{
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(thing, caravan.PawnsListForReading, null, null);
				if (pawn2 == null)
				{
					Log.Error("Could not find pawn to move bought thing to (bought by player). thing=" + thing);
					thing.Destroy(DestroyMode.Vanish);
				}
				else if (!pawn2.inventory.innerContainer.TryAdd(thing, true))
				{
					Log.Error("Could not add item to inventory.");
					thing.Destroy(DestroyMode.Vanish);
				}
			}
		}
	}
}
