﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Pawn_WorkSettings : IExposable
	{
		private Pawn pawn;

		private DefMap<WorkTypeDef, int> priorities = null;

		private bool workGiversDirty = true;

		private List<WorkGiver> workGiversInOrderEmerg = new List<WorkGiver>();

		private List<WorkGiver> workGiversInOrderNormal = new List<WorkGiver>();

		public const int LowestPriority = 4;

		public const int DefaultPriority = 3;

		private const int MaxInitialActiveWorks = 6;

		private static List<WorkTypeDef> wtsByPrio = new List<WorkTypeDef>();

		[CompilerGenerated]
		private static Func<WorkTypeDef, bool> <>f__am$cache0;

		[CompilerGenerated]
		private static Predicate<WorkGiverDef> <>f__am$cache1;

		public Pawn_WorkSettings()
		{
		}

		public Pawn_WorkSettings(Pawn pawn)
		{
			this.pawn = pawn;
		}

		public bool EverWork
		{
			get
			{
				return this.priorities != null;
			}
		}

		public List<WorkGiver> WorkGiversInOrderNormal
		{
			get
			{
				if (this.workGiversDirty)
				{
					this.CacheWorkGiversInOrder();
				}
				return this.workGiversInOrderNormal;
			}
		}

		public List<WorkGiver> WorkGiversInOrderEmergency
		{
			get
			{
				if (this.workGiversDirty)
				{
					this.CacheWorkGiversInOrder();
				}
				return this.workGiversInOrderEmerg;
			}
		}

		public void ExposeData()
		{
			Scribe_Deep.Look<DefMap<WorkTypeDef, int>>(ref this.priorities, "priorities", new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.priorities != null)
			{
				List<WorkTypeDef> disabledWorkTypes = this.pawn.story.DisabledWorkTypes;
				for (int i = 0; i < disabledWorkTypes.Count; i++)
				{
					this.Disable(disabledWorkTypes[i]);
				}
			}
		}

		public void EnableAndInitializeIfNotAlreadyInitialized()
		{
			if (this.priorities == null)
			{
				this.EnableAndInitialize();
			}
		}

		public void EnableAndInitialize()
		{
			if (this.priorities == null)
			{
				this.priorities = new DefMap<WorkTypeDef, int>();
			}
			this.priorities.SetAll(0);
			int num = 0;
			foreach (WorkTypeDef w4 in from w in DefDatabase<WorkTypeDef>.AllDefs
			where !w.alwaysStartActive && !this.pawn.story.WorkTypeIsDisabled(w)
			orderby this.pawn.skills.AverageOfRelevantSkillsFor(w) descending
			select w)
			{
				this.SetPriority(w4, 3);
				num++;
				if (num >= 6)
				{
					break;
				}
			}
			foreach (WorkTypeDef w2 in from w in DefDatabase<WorkTypeDef>.AllDefs
			where w.alwaysStartActive
			select w)
			{
				if (!this.pawn.story.WorkTypeIsDisabled(w2))
				{
					this.SetPriority(w2, 3);
				}
			}
			foreach (WorkTypeDef w3 in this.pawn.story.DisabledWorkTypes)
			{
				this.Disable(w3);
			}
		}

		private void ConfirmInitializedDebug()
		{
			if (this.priorities == null)
			{
				Log.Error(this.pawn + " did not have work settings initialized.", false);
				this.EnableAndInitialize();
			}
		}

		public void SetPriority(WorkTypeDef w, int priority)
		{
			this.ConfirmInitializedDebug();
			if (priority != 0 && this.pawn.story.WorkTypeIsDisabled(w))
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to change priority on disabled worktype ",
					w,
					" for pawn ",
					this.pawn
				}), false);
			}
			else
			{
				if (priority < 0 || priority > 4)
				{
					Log.Message("Trying to set work to invalid priority " + priority, false);
				}
				this.priorities[w] = priority;
				if (priority == 0)
				{
					this.pawn.mindState.Notify_WorkPriorityDisabled(w);
				}
				this.workGiversDirty = true;
			}
		}

		public int GetPriority(WorkTypeDef w)
		{
			this.ConfirmInitializedDebug();
			int num = this.priorities[w];
			int result;
			if (num > 0 && !Find.PlaySettings.useWorkPriorities)
			{
				result = 3;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public bool WorkIsActive(WorkTypeDef w)
		{
			this.ConfirmInitializedDebug();
			return this.GetPriority(w) > 0;
		}

		public void Disable(WorkTypeDef w)
		{
			this.ConfirmInitializedDebug();
			this.SetPriority(w, 0);
		}

		public void DisableAll()
		{
			this.ConfirmInitializedDebug();
			this.priorities.SetAll(0);
			this.workGiversDirty = true;
		}

		public void Notify_UseWorkPrioritiesChanged()
		{
			this.workGiversDirty = true;
		}

		public void Notify_GainedTrait()
		{
			if (this.priorities != null)
			{
				foreach (WorkTypeDef w in this.pawn.story.DisabledWorkTypes)
				{
					this.Disable(w);
				}
			}
		}

		private void CacheWorkGiversInOrder()
		{
			Pawn_WorkSettings.wtsByPrio.Clear();
			List<WorkTypeDef> allDefsListForReading = DefDatabase<WorkTypeDef>.AllDefsListForReading;
			int num = 999;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				WorkTypeDef workTypeDef = allDefsListForReading[i];
				int priority = this.GetPriority(workTypeDef);
				if (priority > 0)
				{
					if (priority < num)
					{
						if (workTypeDef.workGiversByPriority.Any((WorkGiverDef wg) => !wg.emergency))
						{
							num = priority;
						}
					}
					Pawn_WorkSettings.wtsByPrio.Add(workTypeDef);
				}
			}
			Pawn_WorkSettings.wtsByPrio.InsertionSort(delegate(WorkTypeDef a, WorkTypeDef b)
			{
				float value = (float)(a.naturalPriority + (4 - this.GetPriority(a)) * 100000);
				return ((float)(b.naturalPriority + (4 - this.GetPriority(b)) * 100000)).CompareTo(value);
			});
			this.workGiversInOrderEmerg.Clear();
			for (int j = 0; j < Pawn_WorkSettings.wtsByPrio.Count; j++)
			{
				WorkTypeDef workTypeDef2 = Pawn_WorkSettings.wtsByPrio[j];
				for (int k = 0; k < workTypeDef2.workGiversByPriority.Count; k++)
				{
					WorkGiver worker = workTypeDef2.workGiversByPriority[k].Worker;
					if (worker.def.emergency && this.GetPriority(worker.def.workType) <= num)
					{
						this.workGiversInOrderEmerg.Add(worker);
					}
				}
			}
			this.workGiversInOrderNormal.Clear();
			for (int l = 0; l < Pawn_WorkSettings.wtsByPrio.Count; l++)
			{
				WorkTypeDef workTypeDef3 = Pawn_WorkSettings.wtsByPrio[l];
				for (int m = 0; m < workTypeDef3.workGiversByPriority.Count; m++)
				{
					WorkGiver worker2 = workTypeDef3.workGiversByPriority[m].Worker;
					if (!worker2.def.emergency || this.GetPriority(worker2.def.workType) > num)
					{
						this.workGiversInOrderNormal.Add(worker2);
					}
				}
			}
			this.workGiversDirty = false;
		}

		public string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("WorkSettings for " + this.pawn);
			stringBuilder.AppendLine("Cached emergency WorkGivers in order:");
			for (int i = 0; i < this.WorkGiversInOrderEmergency.Count; i++)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"   ",
					i,
					": ",
					this.DebugStringFor(this.WorkGiversInOrderEmergency[i].def)
				}));
			}
			stringBuilder.AppendLine("Cached normal WorkGivers in order:");
			for (int j = 0; j < this.WorkGiversInOrderNormal.Count; j++)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"   ",
					j,
					": ",
					this.DebugStringFor(this.WorkGiversInOrderNormal[j].def)
				}));
			}
			return stringBuilder.ToString();
		}

		private string DebugStringFor(WorkGiverDef wg)
		{
			return string.Concat(new object[]
			{
				"[",
				this.GetPriority(wg.workType),
				" ",
				wg.workType.defName,
				"] - ",
				wg.defName,
				" (",
				wg.priorityInType,
				")"
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Pawn_WorkSettings()
		{
		}

		[CompilerGenerated]
		private bool <EnableAndInitialize>m__0(WorkTypeDef w)
		{
			return !w.alwaysStartActive && !this.pawn.story.WorkTypeIsDisabled(w);
		}

		[CompilerGenerated]
		private float <EnableAndInitialize>m__1(WorkTypeDef w)
		{
			return this.pawn.skills.AverageOfRelevantSkillsFor(w);
		}

		[CompilerGenerated]
		private static bool <EnableAndInitialize>m__2(WorkTypeDef w)
		{
			return w.alwaysStartActive;
		}

		[CompilerGenerated]
		private static bool <CacheWorkGiversInOrder>m__3(WorkGiverDef wg)
		{
			return !wg.emergency;
		}

		[CompilerGenerated]
		private int <CacheWorkGiversInOrder>m__4(WorkTypeDef a, WorkTypeDef b)
		{
			float value = (float)(a.naturalPriority + (4 - this.GetPriority(a)) * 100000);
			return ((float)(b.naturalPriority + (4 - this.GetPriority(b)) * 100000)).CompareTo(value);
		}
	}
}
