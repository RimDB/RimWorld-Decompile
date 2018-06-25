﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ScenPart_PawnModifier : ScenPart
	{
		protected float chance = 1f;

		protected PawnGenerationContext context = PawnGenerationContext.All;

		protected bool hideOffMap;

		private string chanceBuf;

		public ScenPart_PawnModifier()
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.chance, "chance", 0f, false);
			Scribe_Values.Look<PawnGenerationContext>(ref this.context, "context", PawnGenerationContext.All, false);
			Scribe_Values.Look<bool>(ref this.hideOffMap, "hideOffMap", false, false);
		}

		protected void DoPawnModifierEditInterface(Rect rect)
		{
			Rect rect2 = rect.TopHalf();
			Rect rect3 = rect2.LeftPart(0.333f).Rounded();
			Rect rect4 = rect2.RightPart(0.666f).Rounded();
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect3, "chance".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.TextFieldPercent(rect4, ref this.chance, ref this.chanceBuf, 0f, 1f);
			Rect rect5 = rect.BottomHalf();
			Rect rect6 = rect5.LeftPart(0.333f).Rounded();
			Rect rect7 = rect5.RightPart(0.666f).Rounded();
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect6, "context".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			if (Widgets.ButtonText(rect7, this.context.ToStringHuman(), true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				IEnumerator enumerator = Enum.GetValues(typeof(PawnGenerationContext)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						PawnGenerationContext localCont2 = (PawnGenerationContext)obj;
						PawnGenerationContext localCont = localCont2;
						list.Add(new FloatMenuOption(localCont.ToStringHuman(), delegate()
						{
							this.context = localCont;
						}, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		public override void Randomize()
		{
			this.chance = GenMath.RoundedHundredth(Rand.Range(0.05f, 1f));
			this.context = PawnGenerationContextUtility.GetRandom();
			this.hideOffMap = false;
		}

		public override void Notify_NewPawnGenerating(Pawn pawn, PawnGenerationContext context)
		{
			if (this.context.Includes(context))
			{
				if (!this.hideOffMap || context != PawnGenerationContext.PlayerStarter)
				{
					if (Rand.Chance(this.chance) && pawn.RaceProps.Humanlike)
					{
						this.ModifyNewPawn(pawn);
					}
				}
			}
		}

		public override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context, bool redressed)
		{
			if (this.context.Includes(context))
			{
				if (!this.hideOffMap || context != PawnGenerationContext.PlayerStarter)
				{
					if (Rand.Chance(this.chance) && pawn.RaceProps.Humanlike)
					{
						this.ModifyPawnPostGenerate(pawn, redressed);
					}
				}
			}
		}

		public override void PostMapGenerate(Map map)
		{
			if (Find.GameInitData != null)
			{
				if (this.hideOffMap && this.context.Includes(PawnGenerationContext.PlayerStarter))
				{
					foreach (Pawn pawn in Find.GameInitData.startingAndOptionalPawns)
					{
						if (Rand.Chance(this.chance) && pawn.RaceProps.Humanlike)
						{
							this.ModifyHideOffMapStartingPawnPostMapGenerate(pawn);
						}
					}
				}
			}
		}

		protected virtual void ModifyNewPawn(Pawn p)
		{
		}

		protected virtual void ModifyPawnPostGenerate(Pawn p, bool redressed)
		{
		}

		protected virtual void ModifyHideOffMapStartingPawnPostMapGenerate(Pawn p)
		{
		}

		[CompilerGenerated]
		private sealed class <DoPawnModifierEditInterface>c__AnonStorey0
		{
			internal PawnGenerationContext localCont;

			internal ScenPart_PawnModifier $this;

			public <DoPawnModifierEditInterface>c__AnonStorey0()
			{
			}

			internal void <>m__0()
			{
				this.$this.context = this.localCont;
			}
		}
	}
}
