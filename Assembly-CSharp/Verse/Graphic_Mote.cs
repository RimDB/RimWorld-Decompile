﻿using System;
using UnityEngine;

namespace Verse
{
	[StaticConstructorOnStartup]
	public class Graphic_Mote : Graphic_Single
	{
		protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

		public Graphic_Mote()
		{
		}

		protected virtual bool ForcePropertyBlock
		{
			get
			{
				return false;
			}
		}

		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			this.DrawMoteInternal(loc, rot, thingDef, thing, 0);
		}

		public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
		{
			Mote mote = (Mote)thing;
			float alpha = mote.Alpha;
			if (alpha <= 0f)
			{
				return;
			}
			Color color = base.Color * mote.instanceColor;
			color.a *= alpha;
			Vector3 exactScale = mote.exactScale;
			exactScale.x *= this.data.drawSize.x;
			exactScale.z *= this.data.drawSize.y;
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
			Material matSingle = this.MatSingle;
			if (!this.ForcePropertyBlock && color.IndistinguishableFrom(matSingle.color))
			{
				Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0);
			}
			else
			{
				Graphic_Mote.propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
				Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0, Graphic_Mote.propertyBlock);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Mote(path=",
				this.path,
				", shader=",
				base.Shader,
				", color=",
				this.color,
				", colorTwo=unsupported)"
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Graphic_Mote()
		{
		}
	}
}
