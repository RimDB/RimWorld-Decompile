﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ThingSetMaker_RandomOption : ThingSetMaker
	{
		public List<ThingSetMaker_RandomOption.Option> options;

		public ThingSetMaker_RandomOption()
		{
		}

		protected override bool CanGenerateSub(ThingSetMakerParams parms)
		{
			for (int i = 0; i < this.options.Count; i++)
			{
				if (this.options[i].thingSetMaker.CanGenerate(parms) && this.GetSelectionWeight(this.options[i], parms) > 0f)
				{
					return true;
				}
			}
			return false;
		}

		protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
		{
			ThingSetMaker_RandomOption.Option option;
			if (!(from x in this.options
			where x.thingSetMaker.CanGenerate(parms)
			select x).TryRandomElementByWeight((ThingSetMaker_RandomOption.Option x) => this.GetSelectionWeight(x, parms), out option))
			{
				return;
			}
			outThings.AddRange(option.thingSetMaker.Generate(parms));
		}

		private float GetSelectionWeight(ThingSetMaker_RandomOption.Option option, ThingSetMakerParams parms)
		{
			float? weightIfPlayerHasNoItem = option.weightIfPlayerHasNoItem;
			if (weightIfPlayerHasNoItem != null && !PlayerItemAccessibilityUtility.PlayerOrQuestRewardHas(option.weightIfPlayerHasNoItemItem))
			{
				return option.weightIfPlayerHasNoItem.Value;
			}
			return option.weight;
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			for (int i = 0; i < this.options.Count; i++)
			{
				this.options[i].thingSetMaker.ResolveReferences();
			}
		}

		protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
		{
			for (int i = 0; i < this.options.Count; i++)
			{
				float weight = this.options[i].weight;
				float? weightIfPlayerHasNoItem = this.options[i].weightIfPlayerHasNoItem;
				if (weightIfPlayerHasNoItem != null)
				{
					weight = Mathf.Max(weight, this.options[i].weightIfPlayerHasNoItem.Value);
				}
				if (weight > 0f)
				{
					foreach (ThingDef t in this.options[i].thingSetMaker.AllGeneratableThingsDebug(parms))
					{
						yield return t;
					}
				}
			}
			yield break;
		}

		public class Option
		{
			public ThingSetMaker thingSetMaker;

			public float weight;

			public float? weightIfPlayerHasNoItem;

			public ThingDef weightIfPlayerHasNoItemItem;

			public Option()
			{
			}
		}

		[CompilerGenerated]
		private sealed class <Generate>c__AnonStorey1
		{
			internal ThingSetMakerParams parms;

			internal ThingSetMaker_RandomOption $this;

			public <Generate>c__AnonStorey1()
			{
			}

			internal bool <>m__0(ThingSetMaker_RandomOption.Option x)
			{
				return x.thingSetMaker.CanGenerate(this.parms);
			}

			internal float <>m__1(ThingSetMaker_RandomOption.Option x)
			{
				return this.$this.GetSelectionWeight(x, this.parms);
			}
		}

		[CompilerGenerated]
		private sealed class <AllGeneratableThingsDebugSub>c__Iterator0 : IEnumerable, IEnumerable<ThingDef>, IEnumerator, IDisposable, IEnumerator<ThingDef>
		{
			internal int <i>__1;

			internal float <weight>__2;

			internal ThingSetMakerParams parms;

			internal IEnumerator<ThingDef> $locvar0;

			internal ThingDef <t>__3;

			internal ThingSetMaker_RandomOption $this;

			internal ThingDef $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <AllGeneratableThingsDebugSub>c__Iterator0()
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
					i = 0;
					goto IL_174;
				case 1u:
					Block_4:
					try
					{
						switch (num)
						{
						}
						if (enumerator.MoveNext())
						{
							t = enumerator.Current;
							this.$current = t;
							if (!this.$disposing)
							{
								this.$PC = 1;
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
					break;
				default:
					return false;
				}
				IL_166:
				i++;
				IL_174:
				if (i >= this.options.Count)
				{
					this.$PC = -1;
				}
				else
				{
					weight = this.options[i].weight;
					float? weightIfPlayerHasNoItem = this.options[i].weightIfPlayerHasNoItem;
					if (weightIfPlayerHasNoItem != null)
					{
						weight = Mathf.Max(weight, this.options[i].weightIfPlayerHasNoItem.Value);
					}
					if (weight <= 0f)
					{
						goto IL_166;
					}
					enumerator = this.options[i].thingSetMaker.AllGeneratableThingsDebug(parms).GetEnumerator();
					num = 4294967293u;
					goto Block_4;
				}
				return false;
			}

			ThingDef IEnumerator<ThingDef>.Current
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
				case 1u:
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
				return this.System.Collections.Generic.IEnumerable<Verse.ThingDef>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<ThingDef> IEnumerable<ThingDef>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				ThingSetMaker_RandomOption.<AllGeneratableThingsDebugSub>c__Iterator0 <AllGeneratableThingsDebugSub>c__Iterator = new ThingSetMaker_RandomOption.<AllGeneratableThingsDebugSub>c__Iterator0();
				<AllGeneratableThingsDebugSub>c__Iterator.$this = this;
				<AllGeneratableThingsDebugSub>c__Iterator.parms = parms;
				return <AllGeneratableThingsDebugSub>c__Iterator;
			}
		}
	}
}
