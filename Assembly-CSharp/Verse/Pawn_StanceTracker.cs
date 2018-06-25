﻿using System;
using RimWorld;

namespace Verse
{
	public class Pawn_StanceTracker : IExposable
	{
		public Pawn pawn;

		public Stance curStance = new Stance_Mobile();

		private int staggerUntilTick = -1;

		public StunHandler stunner;

		public const int StaggerMeleeAttackTicks = 95;

		public const int StaggerBulletImpactTicks = 95;

		public const int StaggerExplosionImpactTicks = 95;

		public bool debugLog = false;

		public Pawn_StanceTracker(Pawn newPawn)
		{
			this.pawn = newPawn;
			this.stunner = new StunHandler(this.pawn);
		}

		public bool FullBodyBusy
		{
			get
			{
				return this.stunner.Stunned || this.curStance.StanceBusy;
			}
		}

		public bool Staggered
		{
			get
			{
				return Find.TickManager.TicksGame < this.staggerUntilTick;
			}
		}

		public void StanceTrackerTick()
		{
			this.stunner.StunHandlerTick();
			if (!this.stunner.Stunned)
			{
				this.curStance.StanceTick();
			}
		}

		public void StanceTrackerDraw()
		{
			this.curStance.StanceDraw();
		}

		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.staggerUntilTick, "staggerUntilTick", 0, false);
			Scribe_Deep.Look<StunHandler>(ref this.stunner, "stunner", new object[]
			{
				this.pawn
			});
			Scribe_Deep.Look<Stance>(ref this.curStance, "curStance", new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars && this.curStance != null)
			{
				this.curStance.stanceTracker = this;
			}
		}

		public void StaggerFor(int ticks)
		{
			this.staggerUntilTick = Find.TickManager.TicksGame + ticks;
		}

		public void CancelBusyStanceSoft()
		{
			if (this.curStance is Stance_Warmup)
			{
				this.SetStance(new Stance_Mobile());
			}
		}

		public void CancelBusyStanceHard()
		{
			this.SetStance(new Stance_Mobile());
		}

		public void SetStance(Stance newStance)
		{
			if (this.debugLog)
			{
				Log.Message(string.Concat(new object[]
				{
					Find.TickManager.TicksGame,
					" ",
					this.pawn,
					" SetStance ",
					this.curStance,
					" -> ",
					newStance
				}), false);
			}
			newStance.stanceTracker = this;
			this.curStance = newStance;
			if (this.pawn.jobs.curDriver != null)
			{
				this.pawn.jobs.curDriver.Notify_StanceChanged();
			}
		}

		public void Notify_DamageTaken(DamageInfo dinfo)
		{
		}
	}
}
