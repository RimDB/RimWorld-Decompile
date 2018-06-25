﻿using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class Dialog_AssignBuildingOwner : Window
	{
		private IAssignableBuilding assignable;

		private Vector2 scrollPosition;

		private const float EntryHeight = 35f;

		public Dialog_AssignBuildingOwner(IAssignableBuilding assignable)
		{
			this.assignable = assignable;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(620f, 500f);
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			Rect outRect = new Rect(inRect);
			outRect.yMin += 20f;
			outRect.yMax -= 40f;
			outRect.width -= 16f;
			Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, (float)this.assignable.AssigningCandidates.Count<Pawn>() * 35f + 100f);
			Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);
			try
			{
				float num = 0f;
				bool flag = false;
				foreach (Pawn pawn in this.assignable.AssignedPawns)
				{
					flag = true;
					Rect rect = new Rect(0f, num, viewRect.width * 0.6f, 32f);
					Widgets.Label(rect, pawn.LabelCap);
					rect.x = rect.xMax;
					rect.width = viewRect.width * 0.4f;
					if (Widgets.ButtonText(rect, "BuildingUnassign".Translate(), true, false, true))
					{
						this.assignable.TryUnassignPawn(pawn);
						SoundDefOf.Click.PlayOneShotOnCamera(null);
						return;
					}
					num += 35f;
				}
				if (flag)
				{
					num += 15f;
				}
				foreach (Pawn pawn2 in this.assignable.AssigningCandidates)
				{
					if (!this.assignable.AssignedPawns.Contains(pawn2))
					{
						Rect rect2 = new Rect(0f, num, viewRect.width * 0.6f, 32f);
						Widgets.Label(rect2, pawn2.LabelCap);
						rect2.x = rect2.xMax;
						rect2.width = viewRect.width * 0.4f;
						string label = (!this.assignable.AssignedAnything(pawn2)) ? "BuildingAssign".Translate() : "BuildingReassign".Translate();
						if (Widgets.ButtonText(rect2, label, true, false, true))
						{
							this.assignable.TryAssignPawn(pawn2);
							if (this.assignable.MaxAssignedPawnsCount == 1)
							{
								this.Close(true);
							}
							else
							{
								SoundDefOf.Click.PlayOneShotOnCamera(null);
							}
							break;
						}
						num += 35f;
					}
				}
			}
			finally
			{
				Widgets.EndScrollView();
			}
		}
	}
}
