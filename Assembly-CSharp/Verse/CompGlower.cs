﻿using System;
using RimWorld;

namespace Verse
{
	public class CompGlower : ThingComp
	{
		private bool glowOnInt = false;

		public CompGlower()
		{
		}

		public CompProperties_Glower Props
		{
			get
			{
				return (CompProperties_Glower)this.props;
			}
		}

		private bool ShouldBeLitNow
		{
			get
			{
				bool result;
				if (!this.parent.Spawned)
				{
					result = false;
				}
				else if (!FlickUtility.WantsToBeOn(this.parent))
				{
					result = false;
				}
				else
				{
					CompPowerTrader compPowerTrader = this.parent.TryGetComp<CompPowerTrader>();
					if (compPowerTrader != null && !compPowerTrader.PowerOn)
					{
						result = false;
					}
					else
					{
						CompRefuelable compRefuelable = this.parent.TryGetComp<CompRefuelable>();
						result = (compRefuelable == null || compRefuelable.HasFuel);
					}
				}
				return result;
			}
		}

		public void UpdateLit(Map map)
		{
			bool shouldBeLitNow = this.ShouldBeLitNow;
			if (this.glowOnInt != shouldBeLitNow)
			{
				this.glowOnInt = shouldBeLitNow;
				if (!this.glowOnInt)
				{
					map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
					map.glowGrid.DeRegisterGlower(this);
				}
				else
				{
					map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
					map.glowGrid.RegisterGlower(this);
				}
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (this.ShouldBeLitNow)
			{
				this.UpdateLit(this.parent.Map);
				this.parent.Map.glowGrid.RegisterGlower(this);
			}
			else
			{
				this.UpdateLit(this.parent.Map);
			}
		}

		public override void ReceiveCompSignal(string signal)
		{
			if (signal == "PowerTurnedOn" || signal == "PowerTurnedOff" || signal == "FlickedOn" || signal == "FlickedOff" || signal == "Refueled" || signal == "RanOutOfFuel" || signal == "ScheduledOn" || signal == "ScheduledOff")
			{
				this.UpdateLit(this.parent.Map);
			}
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look<bool>(ref this.glowOnInt, "glowOn", false, false);
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			this.UpdateLit(map);
		}
	}
}
