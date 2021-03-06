﻿using System;
using Verse;

namespace RimWorld
{
	public class PawnRelationWorker_GrandnephewOrGrandniece : PawnRelationWorker
	{
		public PawnRelationWorker_GrandnephewOrGrandniece()
		{
		}

		public override bool InRelation(Pawn me, Pawn other)
		{
			if (me == other)
			{
				return false;
			}
			PawnRelationWorker worker = PawnRelationDefOf.NephewOrNiece.Worker;
			return (other.GetMother() != null && worker.InRelation(me, other.GetMother())) || (other.GetFather() != null && worker.InRelation(me, other.GetFather()));
		}
	}
}
