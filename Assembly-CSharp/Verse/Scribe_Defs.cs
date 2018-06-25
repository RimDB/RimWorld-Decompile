﻿using System;

namespace Verse
{
	public static class Scribe_Defs
	{
		public static void Look<T>(ref T value, string label) where T : Def, new()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				string text;
				if (value == null)
				{
					text = "null";
				}
				else
				{
					text = value.defName;
				}
				Scribe_Values.Look<string>(ref text, label, "null", false);
			}
			else if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				value = ScribeExtractor.DefFromNode<T>(Scribe.loader.curXmlParent[label]);
			}
		}
	}
}
