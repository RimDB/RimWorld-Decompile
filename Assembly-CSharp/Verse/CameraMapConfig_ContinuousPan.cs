﻿using System;

namespace Verse
{
	// Token: 0x02000AE9 RID: 2793
	public class CameraMapConfig_ContinuousPan : CameraMapConfig
	{
		// Token: 0x06003DF0 RID: 15856 RVA: 0x0020AF34 File Offset: 0x00209334
		public CameraMapConfig_ContinuousPan()
		{
			this.dollyRateKeys = 10f;
			this.dollyRateMouseDrag = 4f;
			this.dollyRateScreenEdge = 5f;
			this.camSpeedDecayFactor = 1f;
			this.moveSpeedScale = 1f;
		}
	}
}
