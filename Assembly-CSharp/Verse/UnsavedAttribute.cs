﻿using System;

namespace Verse
{
	[AttributeUsage(AttributeTargets.Field)]
	public class UnsavedAttribute : Attribute
	{
		public UnsavedAttribute()
		{
		}
	}
}
