﻿using System;
using Verse;
using Verse.Grammar;

namespace RimWorld
{
	// Token: 0x02000665 RID: 1637
	public static class TaleTextGenerator
	{
		// Token: 0x04001377 RID: 4983
		private const float TalelessChanceWithTales = 0.2f;

		// Token: 0x06002242 RID: 8770 RVA: 0x00122BD8 File Offset: 0x00120FD8
		public static string GenerateTextFromTale(TextGenerationPurpose purpose, Tale tale, int seed, RulePackDef extraInclude)
		{
			Rand.PushState();
			Rand.Seed = seed;
			string rootKeyword = null;
			GrammarRequest request = default(GrammarRequest);
			request.Includes.Add(extraInclude);
			if (purpose == TextGenerationPurpose.ArtDescription)
			{
				rootKeyword = "r_art_description";
				if (tale != null && !Rand.Chance(0.2f))
				{
					request.Includes.Add(RulePackDefOf.ArtDescriptionRoot_HasTale);
					request.IncludesBare.AddRange(tale.GetTextGenerationIncludes());
					request.Rules.AddRange(tale.GetTextGenerationRules());
				}
				else
				{
					request.Includes.Add(RulePackDefOf.ArtDescriptionRoot_Taleless);
					request.Includes.Add(RulePackDefOf.TalelessImages);
				}
				request.Includes.Add(RulePackDefOf.ArtDescriptionUtility_Global);
			}
			else if (purpose == TextGenerationPurpose.ArtName)
			{
				rootKeyword = "r_art_name";
				if (tale != null)
				{
					request.IncludesBare.AddRange(tale.GetTextGenerationIncludes());
					request.Rules.AddRange(tale.GetTextGenerationRules());
				}
			}
			string result = GrammarResolver.Resolve(rootKeyword, request, (tale == null) ? "null_tale" : tale.def.defName, false);
			Rand.PopState();
			return result;
		}
	}
}
