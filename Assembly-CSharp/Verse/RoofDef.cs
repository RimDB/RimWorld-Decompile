﻿using System;

namespace Verse
{
	// Token: 0x02000B6A RID: 2922
	public class RoofDef : Def
	{
		// Token: 0x04002ABD RID: 10941
		public bool isNatural = false;

		// Token: 0x04002ABE RID: 10942
		public bool isThickRoof = false;

		// Token: 0x04002ABF RID: 10943
		public ThingDef collapseLeavingThingDef = null;

		// Token: 0x04002AC0 RID: 10944
		public ThingDef filthLeaving = null;

		// Token: 0x04002AC1 RID: 10945
		public SoundDef soundPunchThrough;

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06003FDF RID: 16351 RVA: 0x0021AE14 File Offset: 0x00219214
		public bool VanishOnCollapse
		{
			get
			{
				return !this.isThickRoof;
			}
		}
	}
}
