﻿using System;
using UnityEngine;

namespace Verse
{
	public struct IntRange : IEquatable<IntRange>
	{
		public int min;

		public int max;

		public IntRange(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public static IntRange zero
		{
			get
			{
				return new IntRange(0, 0);
			}
		}

		public static IntRange one
		{
			get
			{
				return new IntRange(1, 1);
			}
		}

		public int TrueMin
		{
			get
			{
				return Mathf.Min(this.min, this.max);
			}
		}

		public int TrueMax
		{
			get
			{
				return Mathf.Max(this.min, this.max);
			}
		}

		public float Average
		{
			get
			{
				return ((float)this.min + (float)this.max) / 2f;
			}
		}

		public int RandomInRange
		{
			get
			{
				return Rand.RangeInclusive(this.min, this.max);
			}
		}

		public int Lerped(float lerpFactor)
		{
			return this.min + Mathf.RoundToInt(lerpFactor * (float)(this.max - this.min));
		}

		public static IntRange FromString(string s)
		{
			string[] array = s.Split(new char[]
			{
				'~'
			});
			IntRange result;
			if (array.Length == 1)
			{
				int num = Convert.ToInt32(array[0]);
				result = new IntRange(num, num);
			}
			else
			{
				result = new IntRange(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
			}
			return result;
		}

		public override string ToString()
		{
			return this.min.ToString() + "~" + this.max.ToString();
		}

		public override int GetHashCode()
		{
			return Gen.HashCombineInt(this.min, this.max);
		}

		public override bool Equals(object obj)
		{
			return obj is IntRange && this.Equals((IntRange)obj);
		}

		public bool Equals(IntRange other)
		{
			return this.min == other.min && this.max == other.max;
		}

		public static bool operator ==(IntRange lhs, IntRange rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(IntRange lhs, IntRange rhs)
		{
			return !(lhs == rhs);
		}

		internal bool Includes(int val)
		{
			return val >= this.min && val <= this.max;
		}
	}
}
