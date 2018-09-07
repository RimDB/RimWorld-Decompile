﻿using System;
using UnityEngine;

namespace Verse
{
	internal struct DebugLine
	{
		public Vector3 a;

		public Vector3 b;

		private int deathTick;

		private SimpleColor color;

		public DebugLine(Vector3 a, Vector3 b, int ticksLeft = 100, SimpleColor color = SimpleColor.White)
		{
			this.a = a;
			this.b = b;
			this.deathTick = Find.TickManager.TicksGame + ticksLeft;
			this.color = color;
		}

		public bool Done
		{
			get
			{
				return this.deathTick <= Find.TickManager.TicksGame;
			}
		}

		public void Draw()
		{
			GenDraw.DrawLineBetween(this.a, this.b, this.color);
		}
	}
}
