﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200022D RID: 557
	public class ThoughtWorker_PassionateWork : ThoughtWorker
	{
		// Token: 0x06000A28 RID: 2600 RVA: 0x00059BEC File Offset: 0x00057FEC
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			JobDriver curDriver = p.jobs.curDriver;
			ThoughtState result;
			if (curDriver == null)
			{
				result = ThoughtState.Inactive;
			}
			else if (p.skills == null)
			{
				result = ThoughtState.Inactive;
			}
			else if (curDriver.ActiveSkill == null)
			{
				result = ThoughtState.Inactive;
			}
			else
			{
				SkillRecord skill = p.skills.GetSkill(curDriver.ActiveSkill);
				if (skill == null)
				{
					result = ThoughtState.Inactive;
				}
				else if (skill.passion == Passion.Minor)
				{
					result = ThoughtState.ActiveAtStage(0);
				}
				else if (skill.passion == Passion.Major)
				{
					result = ThoughtState.ActiveAtStage(1);
				}
				else
				{
					result = ThoughtState.Inactive;
				}
			}
			return result;
		}
	}
}
