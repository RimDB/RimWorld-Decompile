﻿using System;
using TMPro;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000575 RID: 1397
	[StaticConstructorOnStartup]
	public class WorldFeatureTextMesh_TextMeshPro : WorldFeatureTextMesh
	{
		// Token: 0x04000F78 RID: 3960
		private TextMeshPro textMesh;

		// Token: 0x04000F79 RID: 3961
		public static readonly GameObject WorldTextPrefab = Resources.Load<GameObject>("Prefabs/WorldText");

		// Token: 0x04000F7A RID: 3962
		[TweakValue("Interface.World", 0f, 5f)]
		private static float TextScale = 1f;

		// Token: 0x06001A95 RID: 6805 RVA: 0x000E5062 File Offset: 0x000E3462
		private static void TextScale_Changed()
		{
			Find.WorldFeatures.textsCreated = false;
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001A96 RID: 6806 RVA: 0x000E5070 File Offset: 0x000E3470
		public override bool Active
		{
			get
			{
				return this.textMesh.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001A97 RID: 6807 RVA: 0x000E5098 File Offset: 0x000E3498
		public override Vector3 Position
		{
			get
			{
				return this.textMesh.transform.position;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x000E50C0 File Offset: 0x000E34C0
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x000E50E0 File Offset: 0x000E34E0
		public override Color Color
		{
			get
			{
				return this.textMesh.color;
			}
			set
			{
				this.textMesh.color = value;
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x000E50F0 File Offset: 0x000E34F0
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x000E5110 File Offset: 0x000E3510
		public override string Text
		{
			get
			{
				return this.textMesh.text;
			}
			set
			{
				this.textMesh.text = value;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (set) Token: 0x06001A9C RID: 6812 RVA: 0x000E511F File Offset: 0x000E351F
		public override float Size
		{
			set
			{
				this.textMesh.fontSize = value * WorldFeatureTextMesh_TextMeshPro.TextScale;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001A9D RID: 6813 RVA: 0x000E5134 File Offset: 0x000E3534
		// (set) Token: 0x06001A9E RID: 6814 RVA: 0x000E5159 File Offset: 0x000E3559
		public override Quaternion Rotation
		{
			get
			{
				return this.textMesh.transform.rotation;
			}
			set
			{
				this.textMesh.transform.rotation = value;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06001A9F RID: 6815 RVA: 0x000E5170 File Offset: 0x000E3570
		// (set) Token: 0x06001AA0 RID: 6816 RVA: 0x000E5195 File Offset: 0x000E3595
		public override Vector3 LocalPosition
		{
			get
			{
				return this.textMesh.transform.localPosition;
			}
			set
			{
				this.textMesh.transform.localPosition = value;
			}
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x000E51A9 File Offset: 0x000E35A9
		public override void SetActive(bool active)
		{
			this.textMesh.gameObject.SetActive(active);
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000E51BD File Offset: 0x000E35BD
		public override void Destroy()
		{
			UnityEngine.Object.Destroy(this.textMesh.gameObject);
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x000E51D0 File Offset: 0x000E35D0
		public override void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(WorldFeatureTextMesh_TextMeshPro.WorldTextPrefab);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			this.textMesh = gameObject.GetComponent<TextMeshPro>();
			this.Color = new Color(1f, 1f, 1f, 0f);
			Material[] sharedMaterials = this.textMesh.GetComponent<MeshRenderer>().sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				sharedMaterials[i].renderQueue = WorldMaterials.FeatureNameRenderQueue;
			}
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x000E5250 File Offset: 0x000E3650
		public override void WrapAroundPlanetSurface()
		{
			this.textMesh.ForceMeshUpdate();
			TMP_TextInfo textInfo = this.textMesh.textInfo;
			int characterCount = textInfo.characterCount;
			if (characterCount != 0)
			{
				Vector3[] vertices = this.textMesh.mesh.vertices;
				float num = this.textMesh.bounds.extents.x * 2f;
				float num2 = Find.WorldGrid.DistOnSurfaceToAngle(num);
				Matrix4x4 localToWorldMatrix = this.textMesh.transform.localToWorldMatrix;
				Matrix4x4 worldToLocalMatrix = this.textMesh.transform.worldToLocalMatrix;
				for (int i = 0; i < characterCount; i++)
				{
					TMP_CharacterInfo tmp_CharacterInfo = textInfo.characterInfo[i];
					if (tmp_CharacterInfo.isVisible)
					{
						int vertexIndex = tmp_CharacterInfo.vertexIndex;
						Vector3 vector = vertices[vertexIndex] + vertices[vertexIndex + 1] + vertices[vertexIndex + 2] + vertices[vertexIndex + 3];
						vector /= 4f;
						float num3 = vector.x / (num / 2f);
						bool flag = num3 >= 0f;
						num3 = Mathf.Abs(num3);
						float num4 = num2 / 2f * num3;
						float num5 = (180f - num4) / 2f;
						float num6 = 200f * Mathf.Tan(num4 / 2f * 0.0174532924f);
						Vector3 vector2 = new Vector3(Mathf.Sin(num5 * 0.0174532924f) * num6 * ((!flag) ? -1f : 1f), vector.y, Mathf.Cos(num5 * 0.0174532924f) * num6);
						Vector3 b = vector2 - vector;
						Vector3 vector3 = vertices[vertexIndex] + b;
						Vector3 vector4 = vertices[vertexIndex + 1] + b;
						Vector3 vector5 = vertices[vertexIndex + 2] + b;
						Vector3 vector6 = vertices[vertexIndex + 3] + b;
						Quaternion rotation = Quaternion.Euler(0f, num4 * ((!flag) ? 1f : -1f), 0f);
						vector3 = rotation * (vector3 - vector2) + vector2;
						vector4 = rotation * (vector4 - vector2) + vector2;
						vector5 = rotation * (vector5 - vector2) + vector2;
						vector6 = rotation * (vector6 - vector2) + vector2;
						vector3 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector3).normalized * (100f + WorldAltitudeOffsets.WorldText));
						vector4 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector4).normalized * (100f + WorldAltitudeOffsets.WorldText));
						vector5 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector5).normalized * (100f + WorldAltitudeOffsets.WorldText));
						vector6 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector6).normalized * (100f + WorldAltitudeOffsets.WorldText));
						vertices[vertexIndex] = vector3;
						vertices[vertexIndex + 1] = vector4;
						vertices[vertexIndex + 2] = vector5;
						vertices[vertexIndex + 3] = vector6;
					}
				}
				this.textMesh.mesh.vertices = vertices;
			}
		}
	}
}
