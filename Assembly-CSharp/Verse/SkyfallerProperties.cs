﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000B2C RID: 2860
	public class SkyfallerProperties
	{
		// Token: 0x040028CF RID: 10447
		public bool hitRoof = true;

		// Token: 0x040028D0 RID: 10448
		public IntRange ticksToImpactRange = new IntRange(120, 200);

		// Token: 0x040028D1 RID: 10449
		public bool reversed;

		// Token: 0x040028D2 RID: 10450
		public float explosionRadius = 3f;

		// Token: 0x040028D3 RID: 10451
		public DamageDef explosionDamage;

		// Token: 0x040028D4 RID: 10452
		public float explosionDamageFactor = 1f;

		// Token: 0x040028D5 RID: 10453
		public IntRange metalShrapnelCountRange = IntRange.zero;

		// Token: 0x040028D6 RID: 10454
		public IntRange rubbleShrapnelCountRange = IntRange.zero;

		// Token: 0x040028D7 RID: 10455
		public float shrapnelDistanceFactor = 1f;

		// Token: 0x040028D8 RID: 10456
		public SkyfallerMovementType movementType = SkyfallerMovementType.Accelerate;

		// Token: 0x040028D9 RID: 10457
		public float speed = 1f;

		// Token: 0x040028DA RID: 10458
		public string shadow = "Things/Skyfaller/SkyfallerShadowCircle";

		// Token: 0x040028DB RID: 10459
		public Vector2 shadowSize = Vector2.one;

		// Token: 0x040028DC RID: 10460
		public float cameraShake;

		// Token: 0x040028DD RID: 10461
		public SoundDef impactSound;

		// Token: 0x040028DE RID: 10462
		public bool rotateGraphicTowardsDirection;

		// Token: 0x040028DF RID: 10463
		public SoundDef anticipationSound;

		// Token: 0x040028E0 RID: 10464
		public int anticipationSoundTicks = 100;

		// Token: 0x040028E1 RID: 10465
		public int motesPerCell = 3;

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06003F04 RID: 16132 RVA: 0x002132F8 File Offset: 0x002116F8
		public bool MakesShrapnel
		{
			get
			{
				return this.metalShrapnelCountRange.max > 0 || this.rubbleShrapnelCountRange.max > 0;
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06003F05 RID: 16133 RVA: 0x00213330 File Offset: 0x00211730
		public bool CausesExplosion
		{
			get
			{
				return this.explosionDamage != null && this.explosionRadius > 0f;
			}
		}
	}
}
