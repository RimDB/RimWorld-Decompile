﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000F25 RID: 3877
	public abstract class SubEffecter_Sprayer : SubEffecter
	{
		// Token: 0x06005CE3 RID: 23779 RVA: 0x002F1B49 File Offset: 0x002EFF49
		public SubEffecter_Sprayer(SubEffecterDef def, Effecter parent) : base(def, parent)
		{
		}

		// Token: 0x06005CE4 RID: 23780 RVA: 0x002F1B54 File Offset: 0x002EFF54
		protected void MakeMote(TargetInfo A, TargetInfo B)
		{
			Vector3 vector = Vector3.zero;
			switch (this.def.spawnLocType)
			{
			case MoteSpawnLocType.OnSource:
				vector = A.CenterVector3;
				break;
			case MoteSpawnLocType.BetweenPositions:
			{
				Vector3 vector2 = (!A.HasThing) ? A.Cell.ToVector3Shifted() : A.Thing.DrawPos;
				Vector3 vector3 = (!B.HasThing) ? B.Cell.ToVector3Shifted() : B.Thing.DrawPos;
				if (A.HasThing && !A.Thing.Spawned)
				{
					vector = vector3;
				}
				else if (B.HasThing && !B.Thing.Spawned)
				{
					vector = vector2;
				}
				else
				{
					vector = vector2 * this.def.positionLerpFactor + vector3 * (1f - this.def.positionLerpFactor);
				}
				break;
			}
			case MoteSpawnLocType.BetweenTouchingCells:
				vector = A.Cell.ToVector3Shifted() + (B.Cell - A.Cell).ToVector3().normalized * 0.5f;
				break;
			case MoteSpawnLocType.RandomCellOnTarget:
			{
				CellRect cellRect;
				if (B.HasThing)
				{
					cellRect = B.Thing.OccupiedRect();
				}
				else
				{
					cellRect = CellRect.CenteredOn(B.Cell, 0);
				}
				vector = cellRect.RandomCell.ToVector3Shifted();
				break;
			}
			}
			if (this.parent != null)
			{
				Rand.PushState(this.parent.GetHashCode());
				if (A.CenterVector3 != B.CenterVector3)
				{
					vector += (B.CenterVector3 - A.CenterVector3).normalized * this.parent.def.offsetTowardsTarget.RandomInRange;
				}
				vector += Gen.RandomHorizontalVector(this.parent.def.positionRadius);
				Rand.PopState();
			}
			Map map = A.Map ?? B.Map;
			float num = (!this.def.absoluteAngle) ? (B.Cell - A.Cell).AngleFlat : 0f;
			if (map != null && vector.ShouldSpawnMotesAt(map))
			{
				int randomInRange = this.def.burstCount.RandomInRange;
				for (int i = 0; i < randomInRange; i++)
				{
					Mote mote = (Mote)ThingMaker.MakeThing(this.def.moteDef, null);
					GenSpawn.Spawn(mote, vector.ToIntVec3(), map, WipeMode.Vanish);
					mote.Scale = this.def.scale.RandomInRange;
					mote.exactPosition = vector + Gen.RandomHorizontalVector(this.def.positionRadius);
					mote.rotationRate = this.def.rotationRate.RandomInRange;
					mote.exactRotation = this.def.rotation.RandomInRange + num;
					MoteThrown moteThrown = mote as MoteThrown;
					if (moteThrown != null)
					{
						moteThrown.airTimeLeft = this.def.airTime.RandomInRange;
						moteThrown.SetVelocity(this.def.angle.RandomInRange + num, this.def.speed.RandomInRange);
					}
				}
			}
		}
	}
}
