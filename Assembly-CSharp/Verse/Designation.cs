﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	public class Designation : IExposable
	{
		public DesignationManager designationManager;

		public DesignationDef def;

		public LocalTargetInfo target;

		public const float ClaimedDesignationDrawAltitude = 15f;

		public Designation()
		{
		}

		public Designation(LocalTargetInfo target, DesignationDef def)
		{
			this.target = target;
			this.def = def;
		}

		private Map Map
		{
			get
			{
				return this.designationManager.map;
			}
		}

		public float DesignationDrawAltitude
		{
			get
			{
				return AltitudeLayer.MetaOverlays.AltitudeFor();
			}
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<DesignationDef>(ref this.def, "def");
			Scribe_TargetInfo.Look(ref this.target, "target");
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				if (this.def == DesignationDefOf.Haul && !this.target.HasThing)
				{
					Log.Error("Haul designation has no target! Deleting.", false);
					this.Delete();
				}
			}
		}

		public void Notify_Added()
		{
			if (this.def == DesignationDefOf.Haul)
			{
				this.Map.listerHaulables.HaulDesignationAdded(this.target.Thing);
			}
		}

		internal void Notify_Removing()
		{
			if (this.def == DesignationDefOf.Haul && this.target.HasThing)
			{
				this.Map.listerHaulables.HaulDesignationRemoved(this.target.Thing);
			}
		}

		public virtual void DesignationDraw()
		{
			if (!this.target.HasThing || this.target.Thing.Spawned)
			{
				Vector3 position = default(Vector3);
				if (this.target.HasThing)
				{
					position = this.target.Thing.DrawPos;
					position.y = this.DesignationDrawAltitude;
				}
				else
				{
					position = this.target.Cell.ToVector3ShiftedWithAltitude(this.DesignationDrawAltitude);
				}
				Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, this.def.iconMat, 0);
			}
		}

		public void Delete()
		{
			this.Map.designationManager.RemoveDesignation(this);
		}

		public override string ToString()
		{
			return string.Format(string.Concat(new object[]
			{
				"(",
				this.def.defName,
				" target=",
				this.target,
				")"
			}), new object[0]);
		}
	}
}
