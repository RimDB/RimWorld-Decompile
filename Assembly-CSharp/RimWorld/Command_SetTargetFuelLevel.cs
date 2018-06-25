﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Command_SetTargetFuelLevel : Command
	{
		public CompRefuelable refuelable;

		private List<CompRefuelable> refuelables;

		[CompilerGenerated]
		private static Func<int, string> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<int, string> <>f__am$cache1;

		public Command_SetTargetFuelLevel()
		{
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			if (this.refuelables == null)
			{
				this.refuelables = new List<CompRefuelable>();
			}
			if (!this.refuelables.Contains(this.refuelable))
			{
				this.refuelables.Add(this.refuelable);
			}
			int num = int.MaxValue;
			for (int i = 0; i < this.refuelables.Count; i++)
			{
				if ((int)this.refuelables[i].Props.fuelCapacity < num)
				{
					num = (int)this.refuelables[i].Props.fuelCapacity;
				}
			}
			int startingValue = num / 2;
			for (int j = 0; j < this.refuelables.Count; j++)
			{
				if ((int)this.refuelables[j].TargetFuelLevel <= num)
				{
					startingValue = (int)this.refuelables[j].TargetFuelLevel;
					break;
				}
			}
			Func<int, string> textGetter;
			if (this.refuelable.parent.def.building.hasFuelingPort)
			{
				textGetter = ((int x) => "SetPodLauncherTargetFuelLevel".Translate(new object[]
				{
					x,
					CompLaunchable.MaxLaunchDistanceAtFuelLevel((float)x)
				}));
			}
			else
			{
				textGetter = ((int x) => "SetTargetFuelLevel".Translate(new object[]
				{
					x
				}));
			}
			Dialog_Slider window = new Dialog_Slider(textGetter, 0, num, delegate(int value)
			{
				for (int k = 0; k < this.refuelables.Count; k++)
				{
					this.refuelables[k].TargetFuelLevel = (float)value;
				}
			}, startingValue);
			Find.WindowStack.Add(window);
		}

		public override bool InheritInteractionsFrom(Gizmo other)
		{
			if (this.refuelables == null)
			{
				this.refuelables = new List<CompRefuelable>();
			}
			this.refuelables.Add(((Command_SetTargetFuelLevel)other).refuelable);
			return false;
		}

		[CompilerGenerated]
		private static string <ProcessInput>m__0(int x)
		{
			return "SetPodLauncherTargetFuelLevel".Translate(new object[]
			{
				x,
				CompLaunchable.MaxLaunchDistanceAtFuelLevel((float)x)
			});
		}

		[CompilerGenerated]
		private static string <ProcessInput>m__1(int x)
		{
			return "SetTargetFuelLevel".Translate(new object[]
			{
				x
			});
		}

		[CompilerGenerated]
		private void <ProcessInput>m__2(int value)
		{
			for (int i = 0; i < this.refuelables.Count; i++)
			{
				this.refuelables[i].TargetFuelLevel = (float)value;
			}
		}
	}
}
