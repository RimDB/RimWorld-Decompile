﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RimWorld
{
	public sealed class History : IExposable
	{
		public Archive archive = new Archive();

		private List<HistoryAutoRecorderGroup> autoRecorderGroups;

		public SimpleCurveDrawerStyle curveDrawerStyle;

		[CompilerGenerated]
		private static Predicate<HistoryAutoRecorderGroup> <>f__am$cache0;

		[CompilerGenerated]
		private static Predicate<HistoryAutoRecorderGroup> <>f__am$cache1;

		public History()
		{
			this.autoRecorderGroups = new List<HistoryAutoRecorderGroup>();
			this.AddOrRemoveHistoryRecorderGroups();
			this.curveDrawerStyle = new SimpleCurveDrawerStyle();
			this.curveDrawerStyle.DrawMeasures = true;
			this.curveDrawerStyle.DrawPoints = false;
			this.curveDrawerStyle.DrawBackground = true;
			this.curveDrawerStyle.DrawBackgroundLines = false;
			this.curveDrawerStyle.DrawLegend = true;
			this.curveDrawerStyle.DrawCurveMousePoint = true;
			this.curveDrawerStyle.OnlyPositiveValues = true;
			this.curveDrawerStyle.UseFixedSection = true;
			this.curveDrawerStyle.UseAntiAliasedLines = true;
			this.curveDrawerStyle.PointsRemoveOptimization = true;
			this.curveDrawerStyle.MeasureLabelsXCount = 10;
			this.curveDrawerStyle.MeasureLabelsYCount = 5;
			this.curveDrawerStyle.XIntegersOnly = true;
			this.curveDrawerStyle.LabelX = "Day".Translate();
		}

		public void HistoryTick()
		{
			for (int i = 0; i < this.autoRecorderGroups.Count; i++)
			{
				this.autoRecorderGroups[i].Tick();
			}
		}

		public List<HistoryAutoRecorderGroup> Groups()
		{
			return this.autoRecorderGroups;
		}

		public void ExposeData()
		{
			Scribe_Deep.Look<Archive>(ref this.archive, "archive", new object[0]);
			Scribe_Collections.Look<HistoryAutoRecorderGroup>(ref this.autoRecorderGroups, "autoRecorderGroups", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				BackCompatibility.HistoryLoadingVars(this);
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.AddOrRemoveHistoryRecorderGroups();
			}
		}

		private void AddOrRemoveHistoryRecorderGroups()
		{
			if (this.autoRecorderGroups.RemoveAll((HistoryAutoRecorderGroup x) => x == null) != 0)
			{
				Log.Warning("Some history auto recorder groups were null.", false);
			}
			using (IEnumerator<HistoryAutoRecorderGroupDef> enumerator = DefDatabase<HistoryAutoRecorderGroupDef>.AllDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HistoryAutoRecorderGroupDef def = enumerator.Current;
					if (!this.autoRecorderGroups.Any((HistoryAutoRecorderGroup x) => x.def == def))
					{
						HistoryAutoRecorderGroup historyAutoRecorderGroup = new HistoryAutoRecorderGroup();
						historyAutoRecorderGroup.def = def;
						historyAutoRecorderGroup.AddOrRemoveHistoryRecorders();
						this.autoRecorderGroups.Add(historyAutoRecorderGroup);
					}
				}
			}
			this.autoRecorderGroups.RemoveAll((HistoryAutoRecorderGroup x) => x.def == null);
		}

		[CompilerGenerated]
		private static bool <AddOrRemoveHistoryRecorderGroups>m__0(HistoryAutoRecorderGroup x)
		{
			return x == null;
		}

		[CompilerGenerated]
		private static bool <AddOrRemoveHistoryRecorderGroups>m__1(HistoryAutoRecorderGroup x)
		{
			return x.def == null;
		}

		[CompilerGenerated]
		private sealed class <AddOrRemoveHistoryRecorderGroups>c__AnonStorey0
		{
			internal HistoryAutoRecorderGroupDef def;

			public <AddOrRemoveHistoryRecorderGroups>c__AnonStorey0()
			{
			}

			internal bool <>m__0(HistoryAutoRecorderGroup x)
			{
				return x.def == this.def;
			}
		}
	}
}
