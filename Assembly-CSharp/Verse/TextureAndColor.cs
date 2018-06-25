﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D25 RID: 3365
	public struct TextureAndColor
	{
		// Token: 0x04003236 RID: 12854
		private Texture2D texture;

		// Token: 0x04003237 RID: 12855
		private Color color;

		// Token: 0x06004A21 RID: 18977 RVA: 0x0026BE69 File Offset: 0x0026A269
		public TextureAndColor(Texture2D texture, Color color)
		{
			this.texture = texture;
			this.color = color;
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06004A22 RID: 18978 RVA: 0x0026BE7C File Offset: 0x0026A27C
		public bool HasValue
		{
			get
			{
				return this.texture != null;
			}
		}

		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x06004A23 RID: 18979 RVA: 0x0026BEA0 File Offset: 0x0026A2A0
		public Texture2D Texture
		{
			get
			{
				return this.texture;
			}
		}

		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x06004A24 RID: 18980 RVA: 0x0026BEBC File Offset: 0x0026A2BC
		public Color Color
		{
			get
			{
				return this.color;
			}
		}

		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x06004A25 RID: 18981 RVA: 0x0026BED8 File Offset: 0x0026A2D8
		public static TextureAndColor None
		{
			get
			{
				return new TextureAndColor(null, Color.white);
			}
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x0026BEF8 File Offset: 0x0026A2F8
		public static implicit operator TextureAndColor(Texture2D texture)
		{
			return new TextureAndColor(texture, Color.white);
		}
	}
}
