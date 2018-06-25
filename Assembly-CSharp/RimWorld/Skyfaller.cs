﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Skyfaller : Thing, IThingHolder
	{
		public ThingOwner innerContainer;

		public int ticksToImpact;

		public float angle;

		public float shrapnelDirection;

		private Material cachedShadowMaterial;

		private bool anticipationSoundPlayed;

		private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();

		public const float DefaultAngle = -33.7f;

		private const int RoofHitPreDelay = 15;

		private const int LeaveMapAfterTicks = 220;

		[CompilerGenerated]
		private static Action<Thing, int> <>f__am$cache0;

		public Skyfaller()
		{
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		public override Graphic Graphic
		{
			get
			{
				Thing thingForGraphic = this.GetThingForGraphic();
				Graphic result;
				if (thingForGraphic == this)
				{
					result = base.Graphic;
				}
				else
				{
					result = thingForGraphic.Graphic.ExtractInnerGraphicFor(thingForGraphic).GetShadowlessGraphic();
				}
				return result;
			}
		}

		public override Vector3 DrawPos
		{
			get
			{
				Vector3 result;
				switch (this.def.skyfaller.movementType)
				{
				case SkyfallerMovementType.Accelerate:
					result = SkyfallerDrawPosUtility.DrawPos_Accelerate(base.DrawPos, this.ticksToImpact, this.angle, this.def.skyfaller.speed);
					break;
				case SkyfallerMovementType.ConstantSpeed:
					result = SkyfallerDrawPosUtility.DrawPos_ConstantSpeed(base.DrawPos, this.ticksToImpact, this.angle, this.def.skyfaller.speed);
					break;
				case SkyfallerMovementType.Decelerate:
					result = SkyfallerDrawPosUtility.DrawPos_Decelerate(base.DrawPos, this.ticksToImpact, this.angle, this.def.skyfaller.speed);
					break;
				default:
					Log.ErrorOnce("SkyfallerMovementType not handled: " + this.def.skyfaller.movementType, this.thingIDNumber ^ 1948576711, false);
					result = SkyfallerDrawPosUtility.DrawPos_Accelerate(base.DrawPos, this.ticksToImpact, this.angle, this.def.skyfaller.speed);
					break;
				}
				return result;
			}
		}

		private Material ShadowMaterial
		{
			get
			{
				if (this.cachedShadowMaterial == null && !this.def.skyfaller.shadow.NullOrEmpty())
				{
					this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
				}
				return this.cachedShadowMaterial;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
			Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
			Scribe_Values.Look<float>(ref this.shrapnelDirection, "shrapnelDirection", 0f, false);
		}

		public override void PostMake()
		{
			base.PostMake();
			if (this.def.skyfaller.MakesShrapnel)
			{
				this.shrapnelDirection = Rand.Range(0f, 360f);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.ticksToImpact = this.def.skyfaller.ticksToImpactRange.RandomInRange;
				if (this.def.skyfaller.MakesShrapnel)
				{
					float num = GenMath.PositiveMod(this.shrapnelDirection, 360f);
					if (num < 270f && num >= 90f)
					{
						this.angle = Rand.Range(0f, 33f);
					}
					else
					{
						this.angle = Rand.Range(-33f, 0f);
					}
				}
				else
				{
					this.angle = -33.7f;
				}
				if (this.def.rotatable && this.innerContainer.Any)
				{
					base.Rotation = this.innerContainer[0].Rotation;
				}
			}
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			base.Destroy(mode);
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
		}

		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			Thing thingForGraphic = this.GetThingForGraphic();
			float extraRotation = (!this.def.skyfaller.rotateGraphicTowardsDirection) ? 0f : this.angle;
			this.Graphic.Draw(drawLoc, (!flip) ? thingForGraphic.Rotation : thingForGraphic.Rotation.Opposite, thingForGraphic, extraRotation);
			this.DrawDropSpotShadow();
		}

		public override void Tick()
		{
			this.innerContainer.ThingOwnerTick(true);
			if (this.def.skyfaller.reversed)
			{
				this.ticksToImpact++;
				if (!this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact > this.def.skyfaller.anticipationSoundTicks)
				{
					this.anticipationSoundPlayed = true;
					this.def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				if (this.ticksToImpact == 220)
				{
					this.LeaveMap();
				}
				else if (this.ticksToImpact > 220)
				{
					Log.Error("ticksToImpact > LeaveMapAfterTicks. Was there an exception? Destroying skyfaller.", false);
					this.Destroy(DestroyMode.Vanish);
				}
			}
			else
			{
				this.ticksToImpact--;
				if (this.ticksToImpact == 15)
				{
					this.HitRoof();
				}
				if (!this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact < this.def.skyfaller.anticipationSoundTicks)
				{
					this.anticipationSoundPlayed = true;
					this.def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				if (this.ticksToImpact == 0)
				{
					this.Impact();
				}
				else if (this.ticksToImpact < 0)
				{
					Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.", false);
					this.Destroy(DestroyMode.Vanish);
				}
			}
		}

		protected virtual void HitRoof()
		{
			if (this.def.skyfaller.hitRoof)
			{
				CellRect cr = this.OccupiedRect();
				if (cr.Cells.Any((IntVec3 x) => x.Roofed(this.Map)))
				{
					RoofDef roof = cr.Cells.First((IntVec3 x) => x.Roofed(this.Map)).GetRoof(base.Map);
					if (!roof.soundPunchThrough.NullOrUndefined())
					{
						roof.soundPunchThrough.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
					}
					RoofCollapserImmediate.DropRoofInCells(cr.ExpandedBy(1).ClipInsideMap(base.Map).Cells.Where(delegate(IntVec3 c)
					{
						bool result;
						if (!c.InBounds(this.Map))
						{
							result = false;
						}
						else if (cr.Contains(c))
						{
							result = true;
						}
						else if (c.GetFirstPawn(this.Map) != null)
						{
							result = false;
						}
						else
						{
							Building edifice = c.GetEdifice(this.Map);
							result = (edifice == null || !edifice.def.holdsRoof);
						}
						return result;
					}), base.Map, null);
				}
			}
		}

		protected virtual void Impact()
		{
			if (this.def.skyfaller.CausesExplosion)
			{
				GenExplosion.DoExplosion(base.Position, base.Map, this.def.skyfaller.explosionRadius, this.def.skyfaller.explosionDamage, null, GenMath.RoundRandom((float)this.def.skyfaller.explosionDamage.defaultDamage * this.def.skyfaller.explosionDamageFactor), null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
			}
			for (int i = this.innerContainer.Count - 1; i >= 0; i--)
			{
				GenPlace.TryPlaceThing(this.innerContainer[i], base.Position, base.Map, ThingPlaceMode.Near, delegate(Thing thing, int count)
				{
					PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
				}, null);
			}
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			CellRect cellRect = this.OccupiedRect();
			for (int j = 0; j < cellRect.Area * this.def.skyfaller.motesPerCell; j++)
			{
				MoteMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
			}
			if (this.def.skyfaller.MakesShrapnel)
			{
				SkyfallerShrapnelUtility.MakeShrapnel(base.Position, base.Map, this.shrapnelDirection, this.def.skyfaller.shrapnelDistanceFactor, this.def.skyfaller.metalShrapnelCountRange.RandomInRange, this.def.skyfaller.rubbleShrapnelCountRange.RandomInRange, true);
			}
			if (this.def.skyfaller.cameraShake > 0f && base.Map == Find.CurrentMap)
			{
				Find.CameraDriver.shaker.DoShake(this.def.skyfaller.cameraShake);
			}
			if (this.def.skyfaller.impactSound != null)
			{
				this.def.skyfaller.impactSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
			}
			this.Destroy(DestroyMode.Vanish);
		}

		protected virtual void LeaveMap()
		{
			this.Destroy(DestroyMode.Vanish);
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		private Thing GetThingForGraphic()
		{
			Thing result;
			if (this.def.graphicData != null || !this.innerContainer.Any)
			{
				result = this;
			}
			else
			{
				result = this.innerContainer[0];
			}
			return result;
		}

		private void DrawDropSpotShadow()
		{
			Material shadowMaterial = this.ShadowMaterial;
			if (!(shadowMaterial == null))
			{
				Skyfaller.DrawDropSpotShadow(base.DrawPos, base.Rotation, shadowMaterial, this.def.skyfaller.shadowSize, this.ticksToImpact);
			}
		}

		public static void DrawDropSpotShadow(Vector3 center, Rot4 rot, Material material, Vector2 shadowSize, int ticksToImpact)
		{
			if (rot.IsHorizontal)
			{
				Gen.Swap<float>(ref shadowSize.x, ref shadowSize.y);
			}
			ticksToImpact = Mathf.Max(ticksToImpact, 0);
			Vector3 pos = center;
			pos.y = AltitudeLayer.Shadows.AltitudeFor();
			float num = 1f + (float)ticksToImpact / 100f;
			Vector3 s = new Vector3(num * shadowSize.x, 1f, num * shadowSize.y);
			Color white = Color.white;
			if (ticksToImpact > 150)
			{
				white.a = Mathf.InverseLerp(200f, 150f, (float)ticksToImpact);
			}
			Skyfaller.shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(pos, rot.AsQuat, s);
			Graphics.DrawMesh(MeshPool.plane10Back, matrix, material, 0, null, 0, Skyfaller.shadowPropertyBlock);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Skyfaller()
		{
		}

		[CompilerGenerated]
		private static void <Impact>m__0(Thing thing, int count)
		{
			PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
		}

		[CompilerGenerated]
		private sealed class <HitRoof>c__AnonStorey0
		{
			internal CellRect cr;

			internal Skyfaller $this;

			public <HitRoof>c__AnonStorey0()
			{
			}

			internal bool <>m__0(IntVec3 x)
			{
				return x.Roofed(this.$this.Map);
			}

			internal bool <>m__1(IntVec3 x)
			{
				return x.Roofed(this.$this.Map);
			}

			internal bool <>m__2(IntVec3 c)
			{
				bool result;
				if (!c.InBounds(this.$this.Map))
				{
					result = false;
				}
				else if (this.cr.Contains(c))
				{
					result = true;
				}
				else if (c.GetFirstPawn(this.$this.Map) != null)
				{
					result = false;
				}
				else
				{
					Building edifice = c.GetEdifice(this.$this.Map);
					result = (edifice == null || !edifice.def.holdsRoof);
				}
				return result;
			}
		}
	}
}
