﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005D6 RID: 1494
	[StaticConstructorOnStartup]
	public class Caravan : WorldObject, IThingHolder, IIncidentTarget, ITrader, ILoadReferenceable
	{
		// Token: 0x06001D18 RID: 7448 RVA: 0x000F9E64 File Offset: 0x000F8264
		public Caravan()
		{
			this.pawns = new ThingOwner<Pawn>(this, false, LookMode.Reference);
			this.pather = new Caravan_PathFollower(this);
			this.gotoMote = new Caravan_GotoMoteRenderer();
			this.tweener = new Caravan_Tweener(this);
			this.trader = new Caravan_TraderTracker(this);
			this.forage = new Caravan_ForageTracker(this);
			this.storyState = new StoryState(this);
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001D19 RID: 7449 RVA: 0x000F9EE4 File Offset: 0x000F82E4
		public List<Pawn> PawnsListForReading
		{
			get
			{
				return this.pawns.InnerListForReading;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001D1A RID: 7450 RVA: 0x000F9F04 File Offset: 0x000F8304
		public override Material Material
		{
			get
			{
				if (this.cachedMat == null)
				{
					Color color;
					if (base.Faction == null)
					{
						color = Color.white;
					}
					else if (base.Faction.IsPlayer)
					{
						color = Caravan.PlayerCaravanColor;
					}
					else
					{
						color = base.Faction.Color;
					}
					this.cachedMat = MaterialPool.MatFrom(this.def.texture, ShaderDatabase.WorldOverlayTransparentLit, color, WorldMaterials.DynamicObjectRenderQueue);
				}
				return this.cachedMat;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x000F9F90 File Offset: 0x000F8390
		// (set) Token: 0x06001D1C RID: 7452 RVA: 0x000F9FAB File Offset: 0x000F83AB
		public string Name
		{
			get
			{
				return this.nameInt;
			}
			set
			{
				this.nameInt = value;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001D1D RID: 7453 RVA: 0x000F9FB8 File Offset: 0x000F83B8
		public override Vector3 DrawPos
		{
			get
			{
				return this.tweener.TweenedPos;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001D1E RID: 7454 RVA: 0x000F9FD8 File Offset: 0x000F83D8
		public bool IsPlayerControlled
		{
			get
			{
				return base.Faction == Faction.OfPlayer;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001D1F RID: 7455 RVA: 0x000F9FFC File Offset: 0x000F83FC
		public bool ImmobilizedByMass
		{
			get
			{
				bool result;
				if (Find.TickManager.TicksGame - this.cachedImmobilizedForTicks < 60)
				{
					result = this.cachedImmobilized;
				}
				else
				{
					this.cachedImmobilized = (this.MassUsage > this.MassCapacity);
					this.cachedImmobilizedForTicks = Find.TickManager.TicksGame;
					result = this.cachedImmobilized;
				}
				return result;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001D20 RID: 7456 RVA: 0x000FA060 File Offset: 0x000F8460
		public Pair<float, float> DaysWorthOfFood
		{
			get
			{
				Pair<float, float> result;
				if (Find.TickManager.TicksGame - this.cachedDaysWorthOfFoodForTicks < 3000)
				{
					result = this.cachedDaysWorthOfFood;
				}
				else
				{
					this.cachedDaysWorthOfFood = new Pair<float, float>(DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood(this), DaysUntilRotCalculator.ApproxDaysUntilRot(this));
					this.cachedDaysWorthOfFoodForTicks = Find.TickManager.TicksGame;
					result = this.cachedDaysWorthOfFood;
				}
				return result;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x000FA0CC File Offset: 0x000F84CC
		public bool CantMove
		{
			get
			{
				return this.Resting || this.AllOwnersHaveMentalBreak || this.AllOwnersDowned || this.ImmobilizedByMass;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001D22 RID: 7458 RVA: 0x000FA10C File Offset: 0x000F850C
		public float MassCapacity
		{
			get
			{
				return CollectionsMassCalculator.Capacity<Pawn>(this.PawnsListForReading, null);
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001D23 RID: 7459 RVA: 0x000FA130 File Offset: 0x000F8530
		public string MassCapacityExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CollectionsMassCalculator.Capacity<Pawn>(this.PawnsListForReading, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001D24 RID: 7460 RVA: 0x000FA160 File Offset: 0x000F8560
		public float MassUsage
		{
			get
			{
				return CollectionsMassCalculator.MassUsage<Pawn>(this.PawnsListForReading, IgnorePawnsInventoryMode.DontIgnore, false, false);
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x000FA184 File Offset: 0x000F8584
		public bool AllOwnersDowned
		{
			get
			{
				for (int i = 0; i < this.pawns.Count; i++)
				{
					if (this.IsOwner(this.pawns[i]) && !this.pawns[i].Downed)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001D26 RID: 7462 RVA: 0x000FA1EC File Offset: 0x000F85EC
		public bool AllOwnersHaveMentalBreak
		{
			get
			{
				for (int i = 0; i < this.pawns.Count; i++)
				{
					if (this.IsOwner(this.pawns[i]) && !this.pawns[i].InMentalState)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001D27 RID: 7463 RVA: 0x000FA254 File Offset: 0x000F8654
		public bool Resting
		{
			get
			{
				return (!this.pather.Moving || this.pather.nextTile != this.pather.Destination || !Caravan_PathFollower.IsValidFinalPushDestination(this.pather.Destination) || Mathf.CeilToInt(this.pather.nextTileCostLeft / 1f) > 10000) && CaravanRestUtility.RestingNowAt(base.Tile);
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001D28 RID: 7464 RVA: 0x000FA2DC File Offset: 0x000F86DC
		public int LeftRestTicks
		{
			get
			{
				int result;
				if (!this.Resting)
				{
					result = 0;
				}
				else
				{
					result = CaravanRestUtility.LeftRestTicksAt(base.Tile);
				}
				return result;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001D29 RID: 7465 RVA: 0x000FA310 File Offset: 0x000F8710
		public int LeftNonRestTicks
		{
			get
			{
				int result;
				if (this.Resting)
				{
					result = 0;
				}
				else
				{
					result = CaravanRestUtility.LeftNonRestTicksAt(base.Tile);
				}
				return result;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001D2A RID: 7466 RVA: 0x000FA344 File Offset: 0x000F8744
		public override string Label
		{
			get
			{
				string label;
				if (this.nameInt != null)
				{
					label = this.nameInt;
				}
				else
				{
					label = base.Label;
				}
				return label;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001D2B RID: 7467 RVA: 0x000FA378 File Offset: 0x000F8778
		public int TicksPerMove
		{
			get
			{
				return CaravanTicksPerMoveUtility.GetTicksPerMove(this, null);
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001D2C RID: 7468 RVA: 0x000FA394 File Offset: 0x000F8794
		public override bool AppendFactionToInspectString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001D2D RID: 7469 RVA: 0x000FA3AC File Offset: 0x000F87AC
		public float Visibility
		{
			get
			{
				return CaravanVisibilityCalculator.Visibility(this, null);
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001D2E RID: 7470 RVA: 0x000FA3C8 File Offset: 0x000F87C8
		public string VisibilityExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CaravanVisibilityCalculator.Visibility(this, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x000FA3F4 File Offset: 0x000F87F4
		public string TicksPerMoveExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CaravanTicksPerMoveUtility.GetTicksPerMove(this, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001D30 RID: 7472 RVA: 0x000FA420 File Offset: 0x000F8820
		public StoryState StoryState
		{
			get
			{
				return this.storyState;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001D31 RID: 7473 RVA: 0x000FA43C File Offset: 0x000F883C
		public GameConditionManager GameConditionManager
		{
			get
			{
				Log.ErrorOnce("Attempted to retrieve condition manager directly from caravan", 13291050, false);
				return null;
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001D32 RID: 7474 RVA: 0x000FA464 File Offset: 0x000F8864
		public float PlayerWealthForStoryteller
		{
			get
			{
				float result;
				if (!this.IsPlayerControlled)
				{
					result = 0f;
				}
				else
				{
					float num = 0f;
					for (int i = 0; i < this.pawns.Count; i++)
					{
						num += WealthWatcher.GetEquipmentApparelAndInventoryWealth(this.pawns[i]);
						if (this.pawns[i].RaceProps.Animal && this.pawns[i].Faction == Faction.OfPlayer)
						{
							num += this.pawns[i].MarketValue;
						}
					}
					result = num * 0.5f;
				}
				return result;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001D33 RID: 7475 RVA: 0x000FA518 File Offset: 0x000F8918
		public IEnumerable<Pawn> PlayerPawnsForStoryteller
		{
			get
			{
				IEnumerable<Pawn> result;
				if (!this.IsPlayerControlled)
				{
					result = Enumerable.Empty<Pawn>();
				}
				else
				{
					result = from x in this.PawnsListForReading
					where x.Faction == Faction.OfPlayer
					select x;
				}
				return result;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001D34 RID: 7476 RVA: 0x000FA56C File Offset: 0x000F896C
		public FloatRange IncidentPointsRandomFactorRange
		{
			get
			{
				return StorytellerUtility.CaravanPointsRandomFactorRange;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001D35 RID: 7477 RVA: 0x000FA588 File Offset: 0x000F8988
		public IEnumerable<Thing> AllThings
		{
			get
			{
				return CaravanInventoryUtility.AllInventoryItems(this).Concat(this.pawns);
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001D36 RID: 7478 RVA: 0x000FA5B0 File Offset: 0x000F89B0
		public TraderKindDef TraderKind
		{
			get
			{
				return this.trader.TraderKind;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001D37 RID: 7479 RVA: 0x000FA5D0 File Offset: 0x000F89D0
		public IEnumerable<Thing> Goods
		{
			get
			{
				return this.trader.Goods;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001D38 RID: 7480 RVA: 0x000FA5F0 File Offset: 0x000F89F0
		public int RandomPriceFactorSeed
		{
			get
			{
				return this.trader.RandomPriceFactorSeed;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001D39 RID: 7481 RVA: 0x000FA610 File Offset: 0x000F8A10
		public string TraderName
		{
			get
			{
				return this.trader.TraderName;
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001D3A RID: 7482 RVA: 0x000FA630 File Offset: 0x000F8A30
		public bool CanTradeNow
		{
			get
			{
				return this.trader.CanTradeNow;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x000FA650 File Offset: 0x000F8A50
		public float TradePriceImprovementOffsetForPlayer
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x000FA66C File Offset: 0x000F8A6C
		public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			return this.trader.ColonyThingsWillingToBuy(playerNegotiator);
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000FA68D File Offset: 0x000F8A8D
		public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			this.trader.GiveSoldThingToTrader(toGive, countToGive, playerNegotiator);
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x000FA69E File Offset: 0x000F8A9E
		public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			this.trader.GiveSoldThingToPlayer(toGive, countToGive, playerNegotiator);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x000FA6B0 File Offset: 0x000F8AB0
		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.pawns.RemoveAll((Pawn x) => x.Destroyed);
			}
			Scribe_Values.Look<string>(ref this.nameInt, "name", null, false);
			Scribe_Deep.Look<ThingOwner<Pawn>>(ref this.pawns, "pawns", new object[]
			{
				this
			});
			Scribe_Values.Look<bool>(ref this.autoJoinable, "autoJoinable", false, false);
			Scribe_Deep.Look<Caravan_PathFollower>(ref this.pather, "pather", new object[]
			{
				this
			});
			Scribe_Deep.Look<Caravan_TraderTracker>(ref this.trader, "trader", new object[]
			{
				this
			});
			Scribe_Deep.Look<Caravan_ForageTracker>(ref this.forage, "forage", new object[]
			{
				this
			});
			Scribe_Deep.Look<StoryState>(ref this.storyState, "storyState", new object[]
			{
				this
			});
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				BackCompatibility.CaravanPostLoadInit(this);
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000FA7AF File Offset: 0x000F8BAF
		public override void PostAdd()
		{
			base.PostAdd();
			Find.ColonistBar.MarkColonistsDirty();
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x000FA7C2 File Offset: 0x000F8BC2
		public override void PostRemove()
		{
			base.PostRemove();
			this.pather.StopDead();
			Find.ColonistBar.MarkColonistsDirty();
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x000FA7E0 File Offset: 0x000F8BE0
		public override void Tick()
		{
			base.Tick();
			this.CheckAnyNonWorldPawns();
			this.pather.PatherTick();
			this.tweener.TweenerTick();
			this.forage.ForageTrackerTick();
			CaravanPawnsNeedsUtility.TrySatisfyPawnsNeeds(this);
			if (this.IsHashIntervalTick(120))
			{
				CaravanDrugPolicyUtility.TryTakeScheduledDrugs(this);
			}
			if (this.IsHashIntervalTick(2000))
			{
				CaravanTendUtility.TryTendToRandomPawn(this);
			}
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000FA84A File Offset: 0x000F8C4A
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.tweener.ResetTweenedPosToRoot();
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x000FA85E File Offset: 0x000F8C5E
		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			if (this.IsPlayerControlled && this.pather.curPath != null)
			{
				this.pather.curPath.DrawPath(this);
			}
			this.gotoMote.RenderMote();
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x000FA8A0 File Offset: 0x000F8CA0
		public void AddPawn(Pawn p, bool addCarriedPawnToWorldPawnsIfAny)
		{
			if (p == null)
			{
				Log.Warning("Tried to add a null pawn to " + this, false);
			}
			else if (p.Dead)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to add ",
					p,
					" to ",
					this,
					", but this pawn is dead."
				}), false);
			}
			else
			{
				Pawn pawn = p.carryTracker.CarriedThing as Pawn;
				if (p.Spawned)
				{
					p.DeSpawn(DestroyMode.Vanish);
				}
				if (this.pawns.TryAdd(p, true))
				{
					if (this.ShouldAutoCapture(p))
					{
						p.guest.CapturedBy(base.Faction, null);
					}
					if (pawn != null)
					{
						p.carryTracker.innerContainer.Remove(pawn);
						if (this.ShouldAutoCapture(pawn))
						{
							pawn.guest.CapturedBy(base.Faction, p);
						}
						this.AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny);
						if (addCarriedPawnToWorldPawnsIfAny)
						{
							Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
						}
					}
				}
				else
				{
					Log.Error("Couldn't add pawn " + p + " to caravan.", false);
				}
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x000FA9D0 File Offset: 0x000F8DD0
		public void AddPawnOrItem(Thing thing, bool addCarriedPawnToWorldPawnsIfAny)
		{
			if (thing == null)
			{
				Log.Warning("Tried to add a null thing to " + this, false);
			}
			else
			{
				Pawn pawn = thing as Pawn;
				if (pawn != null)
				{
					this.AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny);
				}
				else
				{
					CaravanInventoryUtility.GiveThing(this, thing);
				}
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x000FAA1C File Offset: 0x000F8E1C
		public bool ContainsPawn(Pawn p)
		{
			return this.pawns.Contains(p);
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000FAA3D File Offset: 0x000F8E3D
		public void RemovePawn(Pawn p)
		{
			this.pawns.Remove(p);
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x000FAA4D File Offset: 0x000F8E4D
		public void RemoveAllPawns()
		{
			this.pawns.Clear();
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x000FAA5C File Offset: 0x000F8E5C
		public bool IsOwner(Pawn p)
		{
			return this.pawns.Contains(p) && CaravanUtility.IsOwner(p, base.Faction);
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000FAA94 File Offset: 0x000F8E94
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < this.pawns.Count; i++)
			{
				if (this.pawns[i].IsColonist)
				{
					num++;
				}
				else if (this.pawns[i].RaceProps.Animal)
				{
					num2++;
				}
				else if (this.pawns[i].IsPrisoner)
				{
					num3++;
				}
				if (this.pawns[i].Downed)
				{
					num4++;
				}
				if (this.pawns[i].InMentalState)
				{
					num5++;
				}
			}
			stringBuilder.Append("CaravanColonistsCount".Translate(new object[]
			{
				num,
				(num != 1) ? Faction.OfPlayer.def.pawnsPlural : Faction.OfPlayer.def.pawnSingular
			}));
			if (num2 == 1)
			{
				stringBuilder.Append(", " + "CaravanAnimal".Translate());
			}
			else if (num2 > 1)
			{
				stringBuilder.Append(", " + "CaravanAnimalsCount".Translate(new object[]
				{
					num2
				}));
			}
			if (num3 == 1)
			{
				stringBuilder.Append(", " + "CaravanPrisoner".Translate());
			}
			else if (num3 > 1)
			{
				stringBuilder.Append(", " + "CaravanPrisonersCount".Translate(new object[]
				{
					num3
				}));
			}
			stringBuilder.AppendLine();
			if (num5 > 0)
			{
				stringBuilder.Append("CaravanPawnsInMentalState".Translate(new object[]
				{
					num5
				}));
			}
			if (num4 > 0)
			{
				if (num5 > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("CaravanPawnsDowned".Translate(new object[]
				{
					num4
				}));
			}
			if (num5 > 0 || num4 > 0)
			{
				stringBuilder.AppendLine();
			}
			if (this.pather.Moving)
			{
				if (this.pather.ArrivalAction != null)
				{
					stringBuilder.Append(this.pather.ArrivalAction.ReportString);
				}
				else
				{
					stringBuilder.Append("CaravanTraveling".Translate());
				}
			}
			else
			{
				Settlement settlement = CaravanVisitUtility.SettlementVisitedNow(this);
				if (settlement != null)
				{
					stringBuilder.Append("CaravanVisiting".Translate(new object[]
					{
						settlement.Label
					}));
				}
				else
				{
					stringBuilder.Append("CaravanWaiting".Translate());
				}
			}
			if (this.pather.Moving)
			{
				float num6 = (float)CaravanArrivalTimeEstimator.EstimatedTicksToArrive(this, true) / 60000f;
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanEstimatedTimeToDestination".Translate(new object[]
				{
					num6.ToString("0.#")
				}));
			}
			if (this.AllOwnersDowned)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("AllCaravanMembersDowned".Translate());
			}
			else if (this.AllOwnersHaveMentalBreak)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("AllCaravanMembersMentalBreak".Translate());
			}
			else if (this.ImmobilizedByMass)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanImmobilizedByMass".Translate());
			}
			string text;
			if (CaravanPawnsNeedsUtility.AnyPawnOutOfFood(this, out text))
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanOutOfFood".Translate());
				if (!text.NullOrEmpty())
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(text);
					stringBuilder.Append(".");
				}
			}
			if (this.Resting)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanResting".Translate());
			}
			if (this.pather.Paused)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanPaused".Translate());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000FAF2C File Offset: 0x000F932C
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.WorldSelector.SingleSelectedObject == this)
			{
				yield return new Gizmo_CaravanInfo(this);
			}
			foreach (Gizmo g in this.<GetGizmos>__BaseCallProxy0())
			{
				yield return g;
			}
			if (this.IsPlayerControlled)
			{
				if (Find.WorldSelector.SingleSelectedObject == this)
				{
					yield return SettleInEmptyTileUtility.SettleCommand(this);
				}
				if (Find.WorldSelector.SingleSelectedObject == this)
				{
					if (this.PawnsListForReading.Count((Pawn x) => x.IsColonist) >= 2)
					{
						yield return new Command_Action
						{
							defaultLabel = "CommandSplitCaravan".Translate(),
							defaultDesc = "CommandSplitCaravanDesc".Translate(),
							icon = Caravan.SplitCommand,
							action = delegate()
							{
								Find.WindowStack.Add(new Dialog_SplitCaravan(this));
							}
						};
					}
				}
				if (this.pather.Moving)
				{
					yield return new Command_Toggle
					{
						hotKey = KeyBindingDefOf.Misc1,
						isActive = (() => this.pather.Paused),
						toggleAction = delegate()
						{
							if (this.pather.Moving)
							{
								this.pather.Paused = !this.pather.Paused;
							}
						},
						defaultDesc = "CommandToggleCaravanPauseDesc".Translate(new object[]
						{
							2f.ToString("0.#"),
							0.3f.ToStringPercent()
						}),
						icon = TexCommand.PauseCaravan,
						defaultLabel = "CommandPauseCaravan".Translate()
					};
				}
				if (CaravanMergeUtility.ShouldShowMergeCommand)
				{
					yield return CaravanMergeUtility.MergeCommand(this);
				}
				foreach (Gizmo g2 in this.forage.GetGizmos())
				{
					yield return g2;
				}
				foreach (WorldObject wo in Find.WorldObjects.ObjectsAt(base.Tile))
				{
					foreach (Gizmo gizmo in wo.GetCaravanGizmos(this))
					{
						yield return gizmo;
					}
				}
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Dev: Mental break",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where x.RaceProps.Humanlike && !x.InMentalState
						select x).TryRandomElement(out pawn))
						{
							pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad, null, false, false, null, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Make random pawn hungry",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where x.needs.food != null
						select x).TryRandomElement(out pawn))
						{
							pawn.needs.food.CurLevelPercentage = 0f;
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Kill random pawn",
					action = delegate()
					{
						Pawn pawn;
						if (this.PawnsListForReading.TryRandomElement(out pawn))
						{
							pawn.Kill(null, null);
							Messages.Message("Dev: Killed " + pawn.LabelShort, this, MessageTypeDefOf.TaskCompletion, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Harm random pawn",
					action = delegate()
					{
						Pawn pawn;
						if (this.PawnsListForReading.TryRandomElement(out pawn))
						{
							DamageInfo dinfo = new DamageInfo(DamageDefOf.Scratch, 10f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
							pawn.TakeDamage(dinfo);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Down random pawn",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where !x.Downed
						select x).TryRandomElement(out pawn))
						{
							HealthUtility.DamageUntilDowned(pawn);
							Messages.Message("Dev: Downed " + pawn.LabelShort, this, MessageTypeDefOf.TaskCompletion, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Teleport to destination",
					action = delegate()
					{
						base.Tile = this.pather.Destination;
						this.pather.StopDead();
					}
				};
			}
			yield break;
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x000FAF58 File Offset: 0x000F9358
		public override IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
		{
			foreach (FloatMenuOption o in this.<GetTransportPodsFloatMenuOptions>__BaseCallProxy1(pods, representative))
			{
				yield return o;
			}
			foreach (FloatMenuOption f in TransportPodsArrivalAction_GiveToCaravan.GetFloatMenuOptions(representative, pods, this))
			{
				yield return f;
			}
			yield break;
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x000FAF90 File Offset: 0x000F9390
		public void RecacheImmobilizedNow()
		{
			this.cachedImmobilizedForTicks = -99999;
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x000FAF9E File Offset: 0x000F939E
		public void RecacheDaysWorthOfFood()
		{
			this.cachedDaysWorthOfFoodForTicks = -99999;
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x000FAFAC File Offset: 0x000F93AC
		public virtual void Notify_MemberDied(Pawn member)
		{
			if (!this.PawnsListForReading.Any((Pawn x) => x != member && this.IsOwner(x)))
			{
				this.RemovePawn(member);
				if (base.Faction == Faction.OfPlayer)
				{
					Find.LetterStack.ReceiveLetter("LetterLabelAllCaravanColonistsDied".Translate(), "LetterAllCaravanColonistsDied".Translate(new object[]
					{
						this.Name
					}).CapitalizeFirst(), LetterDefOf.NegativeEvent, new GlobalTargetInfo(base.Tile), null, null);
				}
				Find.WorldObjects.Remove(this);
			}
			else
			{
				member.Strip();
				this.RemovePawn(member);
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x000FB07C File Offset: 0x000F947C
		public virtual void Notify_Merged(List<Caravan> group)
		{
			this.notifiedOutOfFood = false;
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000FB086 File Offset: 0x000F9486
		public virtual void Notify_StartedTrading()
		{
			this.notifiedOutOfFood = false;
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000FB090 File Offset: 0x000F9490
		private void CheckAnyNonWorldPawns()
		{
			for (int i = this.pawns.Count - 1; i >= 0; i--)
			{
				if (!this.pawns[i].IsWorldPawn())
				{
					Log.Error("Caravan member " + this.pawns[i] + " is not a world pawn. Removing...", false);
					this.pawns.Remove(this.pawns[i]);
				}
			}
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x000FB110 File Offset: 0x000F9510
		private bool ShouldAutoCapture(Pawn p)
		{
			return CaravanUtility.ShouldAutoCapture(p, base.Faction);
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x000FB131 File Offset: 0x000F9531
		public void Notify_PawnRemoved(Pawn p)
		{
			Find.ColonistBar.MarkColonistsDirty();
			this.RecacheImmobilizedNow();
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000FB14A File Offset: 0x000F954A
		public void Notify_PawnAdded(Pawn p)
		{
			Find.ColonistBar.MarkColonistsDirty();
			this.RecacheImmobilizedNow();
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x000FB163 File Offset: 0x000F9563
		public void Notify_DestinationOrPauseStatusChanged()
		{
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x000FB16C File Offset: 0x000F956C
		public void Notify_Teleported()
		{
			this.tweener.ResetTweenedPosToRoot();
			this.pather.Notify_Teleported_Int();
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x000FB188 File Offset: 0x000F9588
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.pawns;
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x000FB1A3 File Offset: 0x000F95A3
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		// Token: 0x0400115D RID: 4445
		private string nameInt;

		// Token: 0x0400115E RID: 4446
		public ThingOwner<Pawn> pawns;

		// Token: 0x0400115F RID: 4447
		public bool autoJoinable;

		// Token: 0x04001160 RID: 4448
		public Caravan_PathFollower pather;

		// Token: 0x04001161 RID: 4449
		public Caravan_GotoMoteRenderer gotoMote;

		// Token: 0x04001162 RID: 4450
		public Caravan_Tweener tweener;

		// Token: 0x04001163 RID: 4451
		public Caravan_TraderTracker trader;

		// Token: 0x04001164 RID: 4452
		public Caravan_ForageTracker forage;

		// Token: 0x04001165 RID: 4453
		public StoryState storyState;

		// Token: 0x04001166 RID: 4454
		private Material cachedMat;

		// Token: 0x04001167 RID: 4455
		private bool cachedImmobilized;

		// Token: 0x04001168 RID: 4456
		private int cachedImmobilizedForTicks = -99999;

		// Token: 0x04001169 RID: 4457
		private Pair<float, float> cachedDaysWorthOfFood;

		// Token: 0x0400116A RID: 4458
		private int cachedDaysWorthOfFoodForTicks = -99999;

		// Token: 0x0400116B RID: 4459
		public bool notifiedOutOfFood;

		// Token: 0x0400116C RID: 4460
		private const int ImmobilizedCacheDuration = 60;

		// Token: 0x0400116D RID: 4461
		private const int DaysWorthOfFoodCacheDuration = 3000;

		// Token: 0x0400116E RID: 4462
		private const int TendIntervalTicks = 2000;

		// Token: 0x0400116F RID: 4463
		private const int TryTakeScheduledDrugsIntervalTicks = 120;

		// Token: 0x04001170 RID: 4464
		private static readonly Texture2D SplitCommand = ContentFinder<Texture2D>.Get("UI/Commands/SplitCaravan", true);

		// Token: 0x04001171 RID: 4465
		private static readonly Color PlayerCaravanColor = new Color(1f, 0.863f, 0.33f);
	}
}