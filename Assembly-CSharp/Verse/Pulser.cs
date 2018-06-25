﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000EA1 RID: 3745
	public static class Pulser
	{
		// Token: 0x0600586E RID: 22638 RVA: 0x002D5294 File Offset: 0x002D3694
		public static float PulseBrightness(float frequency, float amplitude)
		{
			return Pulser.PulseBrightness(frequency, amplitude, Time.realtimeSinceStartup);
		}

		// Token: 0x0600586F RID: 22639 RVA: 0x002D52B8 File Offset: 0x002D36B8
		public static float PulseBrightness(float frequency, float amplitude, float time)
		{
			float num = time * 6.28318548f;
			num *= frequency;
			float t = (1f - Mathf.Cos(num)) * 0.5f;
			return Mathf.Lerp(1f - amplitude, 1f, t);
		}
	}
}
