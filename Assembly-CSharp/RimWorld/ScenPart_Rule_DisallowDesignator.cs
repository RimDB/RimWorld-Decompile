﻿using System;
using Verse;

namespace RimWorld
{
	public class ScenPart_Rule_DisallowDesignator : ScenPart_Rule
	{
		public ScenPart_Rule_DisallowDesignator()
		{
		}

		protected override void ApplyRule()
		{
			Current.Game.Rules.SetAllowDesignator(this.def.designatorType, false);
		}
	}
}
