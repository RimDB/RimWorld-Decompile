﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D3A RID: 3386
	public class HediffSet : IExposable
	{
		// Token: 0x0400325B RID: 12891
		public Pawn pawn;

		// Token: 0x0400325C RID: 12892
		public List<Hediff> hediffs = new List<Hediff>();

		// Token: 0x0400325D RID: 12893
		private List<Hediff_MissingPart> cachedMissingPartsCommonAncestors = null;

		// Token: 0x0400325E RID: 12894
		private float cachedPain = -1f;

		// Token: 0x0400325F RID: 12895
		private float cachedBleedRate = -1f;

		// Token: 0x04003260 RID: 12896
		private bool? cachedHasHead;

		// Token: 0x04003261 RID: 12897
		private Stack<BodyPartRecord> coveragePartsStack = new Stack<BodyPartRecord>();

		// Token: 0x04003262 RID: 12898
		private HashSet<BodyPartRecord> coverageRejectedPartsSet = new HashSet<BodyPartRecord>();

		// Token: 0x04003263 RID: 12899
		private Queue<BodyPartRecord> missingPartsCommonAncestorsQueue = new Queue<BodyPartRecord>();

		// Token: 0x06004A95 RID: 19093 RVA: 0x0026E4C4 File Offset: 0x0026C8C4
		public HediffSet(Pawn newPawn)
		{
			this.pawn = newPawn;
		}

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x06004A96 RID: 19094 RVA: 0x0026E528 File Offset: 0x0026C928
		public float PainTotal
		{
			get
			{
				if (this.cachedPain < 0f)
				{
					this.cachedPain = this.CalculatePain();
				}
				return this.cachedPain;
			}
		}

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x06004A97 RID: 19095 RVA: 0x0026E560 File Offset: 0x0026C960
		public float BleedRateTotal
		{
			get
			{
				if (this.cachedBleedRate < 0f)
				{
					this.cachedBleedRate = this.CalculateBleedRate();
				}
				return this.cachedBleedRate;
			}
		}

		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x06004A98 RID: 19096 RVA: 0x0026E598 File Offset: 0x0026C998
		public bool HasHead
		{
			get
			{
				bool? flag = this.cachedHasHead;
				if (flag == null)
				{
					this.cachedHasHead = new bool?(this.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null).Any((BodyPartRecord x) => x.def == BodyPartDefOf.Head));
				}
				return this.cachedHasHead.Value;
			}
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x06004A99 RID: 19097 RVA: 0x0026E604 File Offset: 0x0026CA04
		public float HungerRateFactor
		{
			get
			{
				float num = 1f;
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					HediffStage curStage = this.hediffs[i].CurStage;
					if (curStage != null)
					{
						num *= curStage.hungerRateFactor;
					}
				}
				for (int j = 0; j < this.hediffs.Count; j++)
				{
					HediffStage curStage2 = this.hediffs[j].CurStage;
					if (curStage2 != null)
					{
						num += curStage2.hungerRateFactorOffset;
					}
				}
				return Mathf.Max(num, 0f);
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x06004A9A RID: 19098 RVA: 0x0026E6B0 File Offset: 0x0026CAB0
		public float RestFallFactor
		{
			get
			{
				float num = 1f;
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					HediffStage curStage = this.hediffs[i].CurStage;
					if (curStage != null)
					{
						num *= curStage.restFallFactor;
					}
				}
				for (int j = 0; j < this.hediffs.Count; j++)
				{
					HediffStage curStage2 = this.hediffs[j].CurStage;
					if (curStage2 != null)
					{
						num += curStage2.restFallFactorOffset;
					}
				}
				return Mathf.Max(num, 0f);
			}
		}

		// Token: 0x06004A9B RID: 19099 RVA: 0x0026E75C File Offset: 0x0026CB5C
		public void ExposeData()
		{
			Scribe_Collections.Look<Hediff>(ref this.hediffs, "hediffs", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					this.hediffs[i].pawn = this.pawn;
				}
				this.DirtyCache();
			}
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x0026E7CC File Offset: 0x0026CBCC
		public void AddDirect(Hediff hediff, DamageInfo? dinfo = null, DamageWorker.DamageResult damageResult = null)
		{
			if (hediff.def == null)
			{
				Log.Error("Tried to add health diff with null def. Canceling.", false);
			}
			else if (hediff.Part != null && !this.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null).Contains(hediff.Part))
			{
				Log.Error("Tried to add health diff to missing part " + hediff.Part, false);
			}
			else
			{
				hediff.ageTicks = 0;
				hediff.pawn = this.pawn;
				bool flag = false;
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (this.hediffs[i].TryMergeWith(hediff))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.hediffs.Add(hediff);
					hediff.PostAdd(dinfo);
					if (this.pawn.needs != null && this.pawn.needs.mood != null)
					{
						this.pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
					}
				}
				bool flag2 = hediff is Hediff_MissingPart;
				if (!(hediff is Hediff_MissingPart) && hediff.Part != null && hediff.Part != this.pawn.RaceProps.body.corePart && this.GetPartHealth(hediff.Part) == 0f)
				{
					if (hediff.Part != this.pawn.RaceProps.body.corePart)
					{
						bool flag3 = this.HasDirectlyAddedPartFor(hediff.Part);
						Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.pawn, null);
						hediff_MissingPart.IsFresh = !flag3;
						hediff_MissingPart.lastInjury = hediff.def;
						this.pawn.health.AddHediff(hediff_MissingPart, hediff.Part, dinfo, null);
						if (damageResult != null)
						{
							damageResult.AddHediff(hediff_MissingPart);
						}
						if (flag3)
						{
							if (dinfo != null)
							{
								hediff_MissingPart.lastInjury = HealthUtility.GetHediffDefFromDamage(dinfo.Value.Def, this.pawn, hediff.Part);
							}
							else
							{
								hediff_MissingPart.lastInjury = null;
							}
						}
						flag2 = true;
					}
				}
				this.DirtyCache();
				if (flag2 && this.pawn.apparel != null)
				{
					this.pawn.apparel.Notify_LostBodyPart();
				}
				if (hediff.def.causesNeed != null && !this.pawn.Dead)
				{
					this.pawn.needs.AddOrRemoveNeedsAsAppropriate();
				}
			}
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x0026EA6C File Offset: 0x0026CE6C
		public void DirtyCache()
		{
			this.CacheMissingPartsCommonAncestors();
			this.cachedPain = -1f;
			this.cachedBleedRate = -1f;
			this.cachedHasHead = null;
			this.pawn.health.capacities.Notify_CapacityLevelsDirty();
			this.pawn.health.summaryHealth.Notify_HealthChanged();
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x0026EAD0 File Offset: 0x0026CED0
		public IEnumerable<T> GetHediffs<T>() where T : Hediff
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				T t = this.hediffs[i] as T;
				if (t != null)
				{
					yield return t;
				}
			}
			yield break;
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x0026EAFC File Offset: 0x0026CEFC
		public Hediff GetFirstHediffOfDef(HediffDef def, bool mustBeVisible = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def && (!mustBeVisible || this.hediffs[i].Visible))
				{
					return this.hediffs[i];
				}
			}
			return null;
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x0026EB78 File Offset: 0x0026CF78
		public bool PartIsMissing(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_MissingPart)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x0026EBE0 File Offset: 0x0026CFE0
		public float GetPartHealth(BodyPartRecord part)
		{
			float result;
			if (part == null)
			{
				result = 0f;
			}
			else
			{
				float num = part.def.GetMaxHealth(this.pawn);
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (this.hediffs[i] is Hediff_MissingPart && this.hediffs[i].Part == part)
					{
						return 0f;
					}
					if (this.hediffs[i].Part == part)
					{
						Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
						if (hediff_Injury != null)
						{
							num -= hediff_Injury.Severity;
						}
					}
				}
				if (num < 0f)
				{
					num = 0f;
				}
				result = (float)Mathf.RoundToInt(num);
			}
			return result;
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x0026ECC0 File Offset: 0x0026D0C0
		public BodyPartRecord GetBrain()
		{
			foreach (BodyPartRecord bodyPartRecord in this.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null))
			{
				if (bodyPartRecord.def.tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
				{
					return bodyPartRecord;
				}
			}
			return null;
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x0026ED44 File Offset: 0x0026D144
		public bool HasHediff(HediffDef def, bool mustBeVisible = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def && (!mustBeVisible || this.hediffs[i].Visible))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AA4 RID: 19108 RVA: 0x0026EDB4 File Offset: 0x0026D1B4
		public bool HasHediff(HediffDef def, BodyPartRecord bodyPart, bool mustBeVisible = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == def && this.hediffs[i].Part == bodyPart && (!mustBeVisible || this.hediffs[i].Visible))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AA5 RID: 19109 RVA: 0x0026EE3C File Offset: 0x0026D23C
		public IEnumerable<Verb> GetHediffsVerbs()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				HediffComp_VerbGiver vg = this.hediffs[i].TryGetComp<HediffComp_VerbGiver>();
				if (vg != null)
				{
					List<Verb> verbList = vg.VerbTracker.AllVerbs;
					for (int j = 0; j < verbList.Count; j++)
					{
						yield return verbList[j];
					}
				}
			}
			yield break;
		}

		// Token: 0x06004AA6 RID: 19110 RVA: 0x0026EE68 File Offset: 0x0026D268
		public IEnumerable<Hediff> GetHediffsTendable()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].TendableNow(false))
				{
					yield return this.hediffs[i];
				}
			}
			yield break;
		}

		// Token: 0x06004AA7 RID: 19111 RVA: 0x0026EE94 File Offset: 0x0026D294
		public bool HasTendableHediff(bool forAlert = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!forAlert || this.hediffs[i].def.makesAlert)
				{
					if (this.hediffs[i].TendableNow(false))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004AA8 RID: 19112 RVA: 0x0026EF10 File Offset: 0x0026D310
		public IEnumerable<Hediff_Injury> GetInjuriesTendable()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury inj = this.hediffs[i] as Hediff_Injury;
				if (inj != null && inj.TendableNow(false))
				{
					yield return inj;
				}
			}
			yield break;
		}

		// Token: 0x06004AA9 RID: 19113 RVA: 0x0026EF3C File Offset: 0x0026D33C
		public bool HasTendableInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.TendableNow(false))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x0026EF9C File Offset: 0x0026D39C
		public bool HasNaturallyHealingInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.CanHealNaturally())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x0026EFFC File Offset: 0x0026D3FC
		public bool HasTendedAndHealingInjury()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = this.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && hediff_Injury.CanHealFromTending() && hediff_Injury.Severity > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x0026F06C File Offset: 0x0026D46C
		public bool HasTemperatureInjury(TemperatureInjuryStage minStage)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].def == HediffDefOf.Hypothermia || this.hediffs[i].def == HediffDefOf.Heatstroke)
				{
					if (this.hediffs[i].CurStageIndex >= (int)minStage)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x0026F0F8 File Offset: 0x0026D4F8
		public IEnumerable<BodyPartRecord> GetInjuredParts()
		{
			return (from x in this.hediffs
			where x is Hediff_Injury
			select x.Part).Distinct<BodyPartRecord>();
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x0026F15C File Offset: 0x0026D55C
		public IEnumerable<BodyPartRecord> GetNaturallyHealingInjuredParts()
		{
			foreach (BodyPartRecord part in this.GetInjuredParts())
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					Hediff_Injury injury = this.hediffs[i] as Hediff_Injury;
					if (injury != null && this.hediffs[i].Part == part && injury.CanHealNaturally())
					{
						yield return part;
						break;
					}
				}
			}
			yield break;
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x0026F188 File Offset: 0x0026D588
		public List<Hediff_MissingPart> GetMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.CacheMissingPartsCommonAncestors();
			}
			return this.cachedMissingPartsCommonAncestors;
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x0026F1B4 File Offset: 0x0026D5B4
		public IEnumerable<BodyPartRecord> GetNotMissingParts(BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined, BodyPartTagDef tag = null)
		{
			List<BodyPartRecord> allPartsList = this.pawn.def.race.body.AllParts;
			for (int i = 0; i < allPartsList.Count; i++)
			{
				BodyPartRecord part = allPartsList[i];
				if (!this.PartIsMissing(part))
				{
					if (height == BodyPartHeight.Undefined || part.height == height)
					{
						if (depth == BodyPartDepth.Undefined || part.depth == depth)
						{
							if (tag == null || part.def.tags.Contains(tag))
							{
								yield return part;
							}
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x0026F1F4 File Offset: 0x0026D5F4
		public BodyPartRecord GetRandomNotMissingPart(DamageDef damDef, BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined)
		{
			IEnumerable<BodyPartRecord> notMissingParts;
			if (this.GetNotMissingParts(height, depth, null).Any<BodyPartRecord>())
			{
				notMissingParts = this.GetNotMissingParts(height, depth, null);
			}
			else
			{
				if (!this.GetNotMissingParts(BodyPartHeight.Undefined, depth, null).Any<BodyPartRecord>())
				{
					return null;
				}
				notMissingParts = this.GetNotMissingParts(BodyPartHeight.Undefined, depth, null);
			}
			BodyPartRecord bodyPartRecord;
			BodyPartRecord result;
			if (notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs * x.def.GetHitChanceFactorFor(damDef), out bodyPartRecord))
			{
				result = bodyPartRecord;
			}
			else
			{
				if (!notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs, out bodyPartRecord))
				{
					throw new InvalidOperationException();
				}
				result = bodyPartRecord;
			}
			return result;
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x0026F2B8 File Offset: 0x0026D6B8
		public float GetCoverageOfNotMissingNaturalParts(BodyPartRecord part)
		{
			float result;
			if (this.PartIsMissing(part))
			{
				result = 0f;
			}
			else if (this.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				result = 0f;
			}
			else
			{
				this.coverageRejectedPartsSet.Clear();
				List<Hediff_MissingPart> missingPartsCommonAncestors = this.GetMissingPartsCommonAncestors();
				for (int i = 0; i < missingPartsCommonAncestors.Count; i++)
				{
					this.coverageRejectedPartsSet.Add(missingPartsCommonAncestors[i].Part);
				}
				for (int j = 0; j < this.hediffs.Count; j++)
				{
					if (this.hediffs[j] is Hediff_AddedPart)
					{
						this.coverageRejectedPartsSet.Add(this.hediffs[j].Part);
					}
				}
				float num = 0f;
				this.coveragePartsStack.Clear();
				this.coveragePartsStack.Push(part);
				while (this.coveragePartsStack.Any<BodyPartRecord>())
				{
					BodyPartRecord bodyPartRecord = this.coveragePartsStack.Pop();
					num += bodyPartRecord.coverageAbs;
					for (int k = 0; k < bodyPartRecord.parts.Count; k++)
					{
						if (!this.coverageRejectedPartsSet.Contains(bodyPartRecord.parts[k]))
						{
							this.coveragePartsStack.Push(bodyPartRecord.parts[k]);
						}
					}
				}
				this.coveragePartsStack.Clear();
				this.coverageRejectedPartsSet.Clear();
				result = num;
			}
			return result;
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x0026F450 File Offset: 0x0026D850
		private void CacheMissingPartsCommonAncestors()
		{
			if (this.cachedMissingPartsCommonAncestors == null)
			{
				this.cachedMissingPartsCommonAncestors = new List<Hediff_MissingPart>();
			}
			else
			{
				this.cachedMissingPartsCommonAncestors.Clear();
			}
			this.missingPartsCommonAncestorsQueue.Clear();
			this.missingPartsCommonAncestorsQueue.Enqueue(this.pawn.def.race.body.corePart);
			while (this.missingPartsCommonAncestorsQueue.Count != 0)
			{
				BodyPartRecord node = this.missingPartsCommonAncestorsQueue.Dequeue();
				if (!this.PartOrAnyAncestorHasDirectlyAddedParts(node))
				{
					Hediff_MissingPart hediff_MissingPart = (from x in this.GetHediffs<Hediff_MissingPart>()
					where x.Part == node
					select x).FirstOrDefault<Hediff_MissingPart>();
					if (hediff_MissingPart != null)
					{
						this.cachedMissingPartsCommonAncestors.Add(hediff_MissingPart);
					}
					else
					{
						for (int i = 0; i < node.parts.Count; i++)
						{
							this.missingPartsCommonAncestorsQueue.Enqueue(node.parts[i]);
						}
					}
				}
			}
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x0026F570 File Offset: 0x0026D970
		public bool HasDirectlyAddedPartFor(BodyPartRecord part)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (this.hediffs[i].Part == part && this.hediffs[i] is Hediff_AddedPart)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AB5 RID: 19125 RVA: 0x0026F5D8 File Offset: 0x0026D9D8
		public bool PartOrAnyAncestorHasDirectlyAddedParts(BodyPartRecord part)
		{
			return this.HasDirectlyAddedPartFor(part) || (part.parent != null && this.PartOrAnyAncestorHasDirectlyAddedParts(part.parent));
		}

		// Token: 0x06004AB6 RID: 19126 RVA: 0x0026F624 File Offset: 0x0026DA24
		public bool AncestorHasDirectlyAddedParts(BodyPartRecord part)
		{
			return part.parent != null && this.PartOrAnyAncestorHasDirectlyAddedParts(part.parent);
		}

		// Token: 0x06004AB7 RID: 19127 RVA: 0x0026F660 File Offset: 0x0026DA60
		public IEnumerable<Hediff> GetTendableNonInjuryNonMissingPartHediffs()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!(this.hediffs[i] is Hediff_Injury))
				{
					if (!(this.hediffs[i] is Hediff_MissingPart))
					{
						if (this.hediffs[i].TendableNow(false))
						{
							yield return this.hediffs[i];
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06004AB8 RID: 19128 RVA: 0x0026F68C File Offset: 0x0026DA8C
		public bool HasTendableNonInjuryNonMissingPartHediff(bool forAlert = false)
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!forAlert || this.hediffs[i].def.makesAlert)
				{
					if (!(this.hediffs[i] is Hediff_Injury))
					{
						if (!(this.hediffs[i] is Hediff_MissingPart))
						{
							if (this.hediffs[i].TendableNow(false))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06004AB9 RID: 19129 RVA: 0x0026F740 File Offset: 0x0026DB40
		public bool HasTendedImmunizableNotImmuneHediff()
		{
			for (int i = 0; i < this.hediffs.Count; i++)
			{
				if (!(this.hediffs[i] is Hediff_Injury) && !(this.hediffs[i] is Hediff_MissingPart) && this.hediffs[i].Visible && this.hediffs[i].IsTended() && this.hediffs[i].def.PossibleToDevelopImmunityNaturally() && !this.hediffs[i].FullyImmune())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x06004ABA RID: 19130 RVA: 0x0026F80C File Offset: 0x0026DC0C
		public bool AnyHediffMakesSickThought
		{
			get
			{
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					if (this.hediffs[i].def.makesSickThought)
					{
						if (this.hediffs[i].Visible)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x0026F884 File Offset: 0x0026DC84
		private float CalculateBleedRate()
		{
			float result;
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.health.Dead)
			{
				result = 0f;
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					num += this.hediffs[i].BleedRate;
				}
				float num2 = num / this.pawn.HealthScale;
				result = num2;
			}
			return result;
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x0026F918 File Offset: 0x0026DD18
		private float CalculatePain()
		{
			float result;
			if (!this.pawn.RaceProps.IsFlesh || this.pawn.Dead)
			{
				result = 0f;
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < this.hediffs.Count; i++)
				{
					num += this.hediffs[i].PainOffset;
				}
				float num2 = num / this.pawn.HealthScale;
				for (int j = 0; j < this.hediffs.Count; j++)
				{
					num2 *= this.hediffs[j].PainFactor;
				}
				result = Mathf.Clamp(num2, 0f, 1f);
			}
			return result;
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x0026F9EA File Offset: 0x0026DDEA
		public void Clear()
		{
			this.hediffs.Clear();
			this.DirtyCache();
		}
	}
}
