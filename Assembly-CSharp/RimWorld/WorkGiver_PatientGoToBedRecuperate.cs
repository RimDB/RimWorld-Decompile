﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_PatientGoToBedRecuperate : WorkGiver
	{
		private static JobGiver_PatientGoToBed jgp = new JobGiver_PatientGoToBed
		{
			respectTimetable = false
		};

		public WorkGiver_PatientGoToBedRecuperate()
		{
		}

		public override Job NonScanJob(Pawn pawn)
		{
			ThinkResult thinkResult = WorkGiver_PatientGoToBedRecuperate.jgp.TryIssueJobPackage(pawn, default(JobIssueParams));
			Job result;
			if (thinkResult.IsValid)
			{
				result = thinkResult.Job;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static WorkGiver_PatientGoToBedRecuperate()
		{
		}
	}
}
