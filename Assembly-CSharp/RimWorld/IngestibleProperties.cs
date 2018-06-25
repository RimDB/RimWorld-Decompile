﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Verse;

namespace RimWorld
{
	public class IngestibleProperties
	{
		[Unsaved]
		public ThingDef parent;

		public int maxNumToIngestAtOnce = 20;

		public List<IngestionOutcomeDoer> outcomeDoers = null;

		public int baseIngestTicks = 500;

		public float chairSearchRadius = 32f;

		public bool useEatingSpeedStat = true;

		public ThoughtDef tasteThought = null;

		public ThoughtDef specialThoughtDirect = null;

		public ThoughtDef specialThoughtAsIngredient = null;

		public EffecterDef ingestEffect = null;

		public EffecterDef ingestEffectEat = null;

		public SoundDef ingestSound = null;

		[MustTranslate]
		public string ingestCommandString = null;

		[MustTranslate]
		public string ingestReportString = null;

		[MustTranslate]
		public string ingestReportStringEat = null;

		public HoldOffsetSet ingestHoldOffsetStanding = null;

		public bool ingestHoldUsesTable = true;

		public FoodTypeFlags foodType = FoodTypeFlags.None;

		public float joy = 0f;

		public JoyKindDef joyKind = null;

		public ThingDef sourceDef;

		public FoodPreferability preferability = FoodPreferability.Undefined;

		public bool nurseable = false;

		public float optimalityOffsetHumanlikes = 0f;

		public float optimalityOffsetFeedingAnimals = 0f;

		public DrugCategory drugCategory = DrugCategory.None;

		[Unsaved]
		private float cachedNutrition = -1f;

		public IngestibleProperties()
		{
		}

		public JoyKindDef JoyKind
		{
			get
			{
				return (this.joyKind == null) ? JoyKindDefOf.Gluttonous : this.joyKind;
			}
		}

		public bool HumanEdible
		{
			get
			{
				return (FoodTypeFlags.OmnivoreHuman & this.foodType) != FoodTypeFlags.None;
			}
		}

		public bool IsMeal
		{
			get
			{
				return this.preferability >= FoodPreferability.MealAwful && this.preferability <= FoodPreferability.MealLavish;
			}
		}

		public float CachedNutrition
		{
			get
			{
				if (this.cachedNutrition == -1f)
				{
					this.cachedNutrition = this.parent.GetStatValueAbstract(StatDefOf.Nutrition, null);
				}
				return this.cachedNutrition;
			}
		}

		public IEnumerable<string> ConfigErrors()
		{
			if (this.preferability == FoodPreferability.Undefined)
			{
				yield return "undefined preferability";
			}
			if (this.foodType == FoodTypeFlags.None)
			{
				yield return "no foodType";
			}
			if (this.parent.GetStatValueAbstract(StatDefOf.Nutrition, null) == 0f && this.preferability != FoodPreferability.NeverForNutrition)
			{
				yield return string.Concat(new object[]
				{
					"Nutrition == 0 but preferability is ",
					this.preferability,
					" instead of ",
					FoodPreferability.NeverForNutrition
				});
			}
			if (!this.parent.IsCorpse && this.preferability > FoodPreferability.DesperateOnlyForHumanlikes && !this.parent.socialPropernessMatters && this.parent.EverHaulable)
			{
				yield return "ingestible preferability > DesperateOnlyForHumanlikes but socialPropernessMatters=false. This will cause bugs wherein wardens will look in prison cells for food to give to prisoners and so will repeatedly pick up and drop food inside the cell.";
			}
			if (this.joy > 0f && this.joyKind == null)
			{
				yield return "joy > 0 with no joy kind";
			}
			yield break;
		}

		internal IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			if (this.joy > 0f)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Joy".Translate(), this.joy.ToStringPercent("F2") + " (" + this.JoyKind.label + ")", 0, "");
			}
			if (this.outcomeDoers != null)
			{
				for (int i = 0; i < this.outcomeDoers.Count; i++)
				{
					foreach (StatDrawEntry s in this.outcomeDoers[i].SpecialDisplayStats(this.parent))
					{
						yield return s;
					}
				}
			}
			yield break;
		}

		[CompilerGenerated]
		private sealed class <ConfigErrors>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
		{
			internal IngestibleProperties $this;

			internal string $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <ConfigErrors>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					if (this.preferability == FoodPreferability.Undefined)
					{
						this.$current = "undefined preferability";
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					break;
				case 1u:
					break;
				case 2u:
					goto IL_90;
				case 3u:
					goto IL_112;
				case 4u:
					goto IL_181;
				case 5u:
					goto IL_1C5;
				default:
					return false;
				}
				if (this.foodType == FoodTypeFlags.None)
				{
					this.$current = "no foodType";
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_90:
				if (this.parent.GetStatValueAbstract(StatDefOf.Nutrition, null) == 0f && this.preferability != FoodPreferability.NeverForNutrition)
				{
					this.$current = string.Concat(new object[]
					{
						"Nutrition == 0 but preferability is ",
						this.preferability,
						" instead of ",
						FoodPreferability.NeverForNutrition
					});
					if (!this.$disposing)
					{
						this.$PC = 3;
					}
					return true;
				}
				IL_112:
				if (!this.parent.IsCorpse && this.preferability > FoodPreferability.DesperateOnlyForHumanlikes && !this.parent.socialPropernessMatters && this.parent.EverHaulable)
				{
					this.$current = "ingestible preferability > DesperateOnlyForHumanlikes but socialPropernessMatters=false. This will cause bugs wherein wardens will look in prison cells for food to give to prisoners and so will repeatedly pick up and drop food inside the cell.";
					if (!this.$disposing)
					{
						this.$PC = 4;
					}
					return true;
				}
				IL_181:
				if (this.joy > 0f && this.joyKind == null)
				{
					this.$current = "joy > 0 with no joy kind";
					if (!this.$disposing)
					{
						this.$PC = 5;
					}
					return true;
				}
				IL_1C5:
				this.$PC = -1;
				return false;
			}

			string IEnumerator<string>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				IngestibleProperties.<ConfigErrors>c__Iterator0 <ConfigErrors>c__Iterator = new IngestibleProperties.<ConfigErrors>c__Iterator0();
				<ConfigErrors>c__Iterator.$this = this;
				return <ConfigErrors>c__Iterator;
			}
		}

		[CompilerGenerated]
		private sealed class <SpecialDisplayStats>c__Iterator1 : IEnumerable, IEnumerable<StatDrawEntry>, IEnumerator, IDisposable, IEnumerator<StatDrawEntry>
		{
			internal int <i>__1;

			internal IEnumerator<StatDrawEntry> $locvar0;

			internal StatDrawEntry <s>__2;

			internal IngestibleProperties $this;

			internal StatDrawEntry $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <SpecialDisplayStats>c__Iterator1()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					if (this.joy > 0f)
					{
						this.$current = new StatDrawEntry(StatCategoryDefOf.Basics, "Joy".Translate(), this.joy.ToStringPercent("F2") + " (" + base.JoyKind.label + ")", 0, "");
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					break;
				case 1u:
					break;
				case 2u:
					Block_5:
					try
					{
						switch (num)
						{
						}
						if (enumerator.MoveNext())
						{
							s = enumerator.Current;
							this.$current = s;
							if (!this.$disposing)
							{
								this.$PC = 2;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
					}
					i++;
					goto IL_17D;
				default:
					return false;
				}
				if (this.outcomeDoers == null)
				{
					goto IL_199;
				}
				i = 0;
				IL_17D:
				if (i < this.outcomeDoers.Count)
				{
					enumerator = this.outcomeDoers[i].SpecialDisplayStats(this.parent).GetEnumerator();
					num = 4294967293u;
					goto Block_5;
				}
				IL_199:
				this.$PC = -1;
				return false;
			}

			StatDrawEntry IEnumerator<StatDrawEntry>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)this.$PC;
				this.$disposing = true;
				this.$PC = -1;
				switch (num)
				{
				case 2u:
					try
					{
					}
					finally
					{
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<RimWorld.StatDrawEntry>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<StatDrawEntry> IEnumerable<StatDrawEntry>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				IngestibleProperties.<SpecialDisplayStats>c__Iterator1 <SpecialDisplayStats>c__Iterator = new IngestibleProperties.<SpecialDisplayStats>c__Iterator1();
				<SpecialDisplayStats>c__Iterator.$this = this;
				return <SpecialDisplayStats>c__Iterator;
			}
		}
	}
}
