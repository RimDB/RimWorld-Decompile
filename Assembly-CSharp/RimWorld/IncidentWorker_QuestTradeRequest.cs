﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000357 RID: 855
	public class IncidentWorker_QuestTradeRequest : IncidentWorker
	{
		// Token: 0x06000EC8 RID: 3784 RVA: 0x0007CCB0 File Offset: 0x0007B0B0
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map;
			return base.CanFireNowSub(parms) && this.TryGetRandomAvailableTargetMap(out map) && IncidentWorker_QuestTradeRequest.RandomNearbyTradeableSettlement(map.Tile) != null;
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0007CD08 File Offset: 0x0007B108
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map;
			bool result;
			if (!this.TryGetRandomAvailableTargetMap(out map))
			{
				result = false;
			}
			else
			{
				Settlement settlement = IncidentWorker_QuestTradeRequest.RandomNearbyTradeableSettlement(map.Tile);
				if (settlement == null)
				{
					result = false;
				}
				else
				{
					TradeRequestComp component = settlement.GetComponent<TradeRequestComp>();
					if (!this.TryGenerateTradeRequest(component, map))
					{
						result = false;
					}
					else
					{
						Find.LetterStack.ReceiveLetter("LetterLabelCaravanRequest".Translate(), "LetterCaravanRequest".Translate(new object[]
						{
							settlement.Label,
							GenLabel.ThingLabel(component.requestThingDef, null, component.requestCount).CapitalizeFirst(),
							(component.requestThingDef.GetStatValueAbstract(StatDefOf.MarketValue, null) * (float)component.requestCount).ToStringMoney(),
							GenThing.ThingsToCommaList(component.rewards, true, true).CapitalizeFirst(),
							GenThing.GetMarketValue(component.rewards).ToStringMoney(),
							(component.expiration - Find.TickManager.TicksGame).ToStringTicksToDays("F0"),
							CaravanArrivalTimeEstimator.EstimatedTicksToArrive(map.Tile, settlement.Tile, null).ToStringTicksToDays("0.#")
						}), LetterDefOf.PositiveEvent, settlement, settlement.Faction, null);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0007CE48 File Offset: 0x0007B248
		public bool TryGenerateTradeRequest(TradeRequestComp target, Map map)
		{
			int num = this.RandomOfferDurationTicks(map.Tile, target.parent.Tile);
			bool result;
			if (num < 1)
			{
				result = false;
			}
			else if (!IncidentWorker_QuestTradeRequest.TryFindRandomRequestedThingDef(map, out target.requestThingDef, out target.requestCount))
			{
				result = false;
			}
			else
			{
				target.rewards.ClearAndDestroyContents(DestroyMode.Vanish);
				target.rewards.TryAddRangeOrTransfer(IncidentWorker_QuestTradeRequest.GenerateRewardsFor(target.requestThingDef, target.requestCount, target.parent.Faction, map), true, true);
				target.expiration = Find.TickManager.TicksGame + num;
				result = true;
			}
			return result;
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0007CEEC File Offset: 0x0007B2EC
		public static Settlement RandomNearbyTradeableSettlement(int originTile)
		{
			return (from settlement in Find.WorldObjects.Settlements
			where settlement.Visitable && settlement.GetComponent<TradeRequestComp>() != null && !settlement.GetComponent<TradeRequestComp>().ActiveRequest && Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile) < 36f && Find.WorldReachability.CanReach(originTile, settlement.Tile)
			select settlement).RandomElementWithFallback(null);
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0007CF30 File Offset: 0x0007B330
		private static bool TryFindRandomRequestedThingDef(Map map, out ThingDef thingDef, out int count)
		{
			IncidentWorker_QuestTradeRequest.requestCountDict.Clear();
			Func<ThingDef, bool> globalValidator = delegate(ThingDef td)
			{
				bool result2;
				if (td.BaseMarketValue / td.BaseMass < 5f)
				{
					result2 = false;
				}
				else if (!td.alwaysHaulable)
				{
					result2 = false;
				}
				else
				{
					CompProperties_Rottable compProperties = td.GetCompProperties<CompProperties_Rottable>();
					if (compProperties != null && compProperties.daysToRotStart < 10f)
					{
						result2 = false;
					}
					else if (td.ingestible != null && td.ingestible.HumanEdible)
					{
						result2 = false;
					}
					else if (td == ThingDefOf.Silver)
					{
						result2 = false;
					}
					else if (!td.PlayerAcquirable)
					{
						result2 = false;
					}
					else
					{
						int num = IncidentWorker_QuestTradeRequest.RandomRequestCount(td, map);
						IncidentWorker_QuestTradeRequest.requestCountDict.Add(td, num);
						result2 = PlayerItemAccessibilityUtility.PossiblyAccessible(td, num, map);
					}
				}
				return result2;
			};
			bool result;
			if ((from td in ThingSetMakerUtility.allGeneratableItems
			where globalValidator(td)
			select td).TryRandomElement(out thingDef))
			{
				count = IncidentWorker_QuestTradeRequest.requestCountDict[thingDef];
				result = true;
			}
			else
			{
				count = 0;
				result = false;
			}
			return result;
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0007CFAC File Offset: 0x0007B3AC
		private bool TryGetRandomAvailableTargetMap(out Map map)
		{
			IncidentWorker_QuestTradeRequest.tmpAvailableMaps.Clear();
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].IsPlayerHome && this.AtLeast2HealthyColonists(maps[i]) && IncidentWorker_QuestTradeRequest.RandomNearbyTradeableSettlement(maps[i].Tile) != null)
				{
					IncidentWorker_QuestTradeRequest.tmpAvailableMaps.Add(maps[i]);
				}
			}
			bool result = IncidentWorker_QuestTradeRequest.tmpAvailableMaps.TryRandomElement(out map);
			IncidentWorker_QuestTradeRequest.tmpAvailableMaps.Clear();
			return result;
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0007D050 File Offset: 0x0007B450
		private static int RandomRequestCount(ThingDef thingDef, Map map)
		{
			float num = (float)IncidentWorker_QuestTradeRequest.BaseValueWantedRange.RandomInRange;
			num *= IncidentWorker_QuestTradeRequest.ValueWantedFactorFromWealthCurve.Evaluate(map.wealthWatcher.WealthTotal);
			return ThingUtility.RoundedResourceStackCount(Mathf.Max(1, Mathf.RoundToInt(num / thingDef.BaseMarketValue)));
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0007D0A4 File Offset: 0x0007B4A4
		private static List<Thing> GenerateRewardsFor(ThingDef thingDef, int quantity, Faction faction, Map map)
		{
			ThingSetMakerParams parms = default(ThingSetMakerParams);
			parms.totalMarketValueRange = new FloatRange?(IncidentWorker_QuestTradeRequest.RewardValueFactorRange * IncidentWorker_QuestTradeRequest.RewardValueFactorFromWealthCurve.Evaluate(map.wealthWatcher.WealthTotal) * thingDef.BaseMarketValue * (float)quantity);
			parms.validator = ((ThingDef td) => td != thingDef);
			List<Thing> list = null;
			for (int i = 0; i < 10; i++)
			{
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].Destroy(DestroyMode.Vanish);
					}
				}
				list = ThingSetMakerDefOf.Reward_TradeRequest.root.Generate(parms);
				float num = 0f;
				for (int k = 0; k < list.Count; k++)
				{
					num += list[k].MarketValue * (float)list[k].stackCount;
				}
				if (num > thingDef.BaseMarketValue * (float)quantity)
				{
					break;
				}
			}
			return list;
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0007D1E4 File Offset: 0x0007B5E4
		private int RandomOfferDurationTicks(int tileIdFrom, int tileIdTo)
		{
			int randomInRange = IncidentWorker_QuestTradeRequest.RandomDurationRangeDays.RandomInRange;
			int num = CaravanArrivalTimeEstimator.EstimatedTicksToArrive(tileIdFrom, tileIdTo, null);
			float num2 = (float)num / 60000f;
			int num3 = Mathf.CeilToInt(Mathf.Max(num2 + 6f, num2 * 1.35f));
			int result;
			if (num3 > 40)
			{
				result = -1;
			}
			else
			{
				int num4 = Mathf.Max(randomInRange, num3);
				result = 60000 * num4;
			}
			return result;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0007D258 File Offset: 0x0007B658
		private bool AtLeast2HealthyColonists(Map map)
		{
			List<Pawn> list = map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].IsFreeColonist)
				{
					if (!HealthAIUtility.ShouldSeekMedicalRest(list[i]))
					{
						num++;
						if (num >= 2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04000911 RID: 2321
		private static readonly IntRange RandomDurationRangeDays = new IntRange(18, 38);

		// Token: 0x04000912 RID: 2322
		private const int MaxDurationDays = 40;

		// Token: 0x04000913 RID: 2323
		private const float MinTravelTimeFraction = 0.35f;

		// Token: 0x04000914 RID: 2324
		private const float MinTravelTimeAbsolute = 6f;

		// Token: 0x04000915 RID: 2325
		private const int MaxTileDistance = 36;

		// Token: 0x04000916 RID: 2326
		private static readonly IntRange BaseValueWantedRange = new IntRange(500, 2500);

		// Token: 0x04000917 RID: 2327
		private static readonly SimpleCurve ValueWantedFactorFromWealthCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.5f),
				true
			},
			{
				new CurvePoint(50000f, 1f),
				true
			},
			{
				new CurvePoint(300000f, 2f),
				true
			}
		};

		// Token: 0x04000918 RID: 2328
		private static readonly FloatRange RewardValueFactorRange = new FloatRange(2f, 3f);

		// Token: 0x04000919 RID: 2329
		private static readonly SimpleCurve RewardValueFactorFromWealthCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 1.5f),
				true
			},
			{
				new CurvePoint(50000f, 1f),
				true
			},
			{
				new CurvePoint(300000f, 0.75f),
				true
			}
		};

		// Token: 0x0400091A RID: 2330
		private static Dictionary<ThingDef, int> requestCountDict = new Dictionary<ThingDef, int>();

		// Token: 0x0400091B RID: 2331
		private static List<Map> tmpAvailableMaps = new List<Map>();
	}
}