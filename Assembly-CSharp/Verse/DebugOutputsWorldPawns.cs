﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;

namespace Verse
{
	// Token: 0x02000E22 RID: 3618
	[HasDebugOutput]
	internal class DebugOutputsWorldPawns
	{
		// Token: 0x060054ED RID: 21741 RVA: 0x002B9B48 File Offset: 0x002B7F48
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void ColonistRelativeChance()
		{
			HashSet<Pawn> hashSet = new HashSet<Pawn>(Find.WorldPawns.AllPawnsAliveOrDead);
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i < 500; i++)
			{
				PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, Faction.OfAncients, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
				Pawn pawn = PawnGenerator.GeneratePawn(request);
				list.Add(pawn);
				if (!pawn.IsWorldPawn())
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
				}
			}
			int num = list.Count((Pawn x) => PawnRelationUtility.GetMostImportantColonyRelative(x) != null);
			Log.Message(string.Concat(new object[]
			{
				"Colony relatives: ",
				((float)num / 500f).ToStringPercent(),
				" (",
				num,
				" of ",
				500,
				")"
			}), false);
			foreach (Pawn pawn2 in Find.WorldPawns.AllPawnsAliveOrDead.ToList<Pawn>())
			{
				if (!hashSet.Contains(pawn2))
				{
					Find.WorldPawns.RemovePawn(pawn2);
					Find.WorldPawns.PassToWorld(pawn2, PawnDiscardDecideMode.Discard);
				}
			}
		}

		// Token: 0x060054EE RID: 21742 RVA: 0x002B9D04 File Offset: 0x002B8104
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void KidnappedPawns()
		{
			Find.FactionManager.LogKidnappedPawns();
		}

		// Token: 0x060054EF RID: 21743 RVA: 0x002B9D11 File Offset: 0x002B8111
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void WorldPawnList()
		{
			Find.WorldPawns.LogWorldPawns();
		}

		// Token: 0x060054F0 RID: 21744 RVA: 0x002B9D1E File Offset: 0x002B811E
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void WorldPawnMothballInfo()
		{
			Find.WorldPawns.LogWorldPawnMothballPrevention();
		}

		// Token: 0x060054F1 RID: 21745 RVA: 0x002B9D2B File Offset: 0x002B812B
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void WorldPawnGcBreakdown()
		{
			Find.WorldPawns.gc.LogGC();
		}

		// Token: 0x060054F2 RID: 21746 RVA: 0x002B9D3D File Offset: 0x002B813D
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void WorldPawnDotgraph()
		{
			Find.WorldPawns.gc.LogDotgraph();
		}

		// Token: 0x060054F3 RID: 21747 RVA: 0x002B9D4F File Offset: 0x002B814F
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void RunWorldPawnGc()
		{
			Find.WorldPawns.gc.RunGC();
		}

		// Token: 0x060054F4 RID: 21748 RVA: 0x002B9D61 File Offset: 0x002B8161
		[DebugOutput]
		[Category("World pawns")]
		[ModeRestrictionPlay]
		public static void RunWorldPawnMothball()
		{
			Find.WorldPawns.DebugRunMothballProcessing();
		}
	}
}
