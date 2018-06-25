﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000BA6 RID: 2982
	public class ThingCategoryDef : Def
	{
		// Token: 0x04002B89 RID: 11145
		public ThingCategoryDef parent = null;

		// Token: 0x04002B8A RID: 11146
		[NoTranslate]
		public string iconPath = null;

		// Token: 0x04002B8B RID: 11147
		public bool resourceReadoutRoot = false;

		// Token: 0x04002B8C RID: 11148
		[Unsaved]
		public TreeNode_ThingCategory treeNode = null;

		// Token: 0x04002B8D RID: 11149
		[Unsaved]
		public List<ThingCategoryDef> childCategories = new List<ThingCategoryDef>();

		// Token: 0x04002B8E RID: 11150
		[Unsaved]
		public List<ThingDef> childThingDefs = new List<ThingDef>();

		// Token: 0x04002B8F RID: 11151
		[Unsaved]
		public List<SpecialThingFilterDef> childSpecialFilters = new List<SpecialThingFilterDef>();

		// Token: 0x04002B90 RID: 11152
		[Unsaved]
		public Texture2D icon = BaseContent.BadTex;

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x06004082 RID: 16514 RVA: 0x0021EA60 File Offset: 0x0021CE60
		public IEnumerable<ThingCategoryDef> Parents
		{
			get
			{
				if (this.parent != null)
				{
					yield return this.parent;
					foreach (ThingCategoryDef cat in this.parent.Parents)
					{
						yield return cat;
					}
				}
				yield break;
			}
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06004083 RID: 16515 RVA: 0x0021EA8C File Offset: 0x0021CE8C
		public IEnumerable<ThingCategoryDef> ThisAndChildCategoryDefs
		{
			get
			{
				yield return this;
				foreach (ThingCategoryDef child in this.childCategories)
				{
					foreach (ThingCategoryDef subChild in child.ThisAndChildCategoryDefs)
					{
						yield return subChild;
					}
				}
				yield break;
			}
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06004084 RID: 16516 RVA: 0x0021EAB8 File Offset: 0x0021CEB8
		public IEnumerable<ThingDef> DescendantThingDefs
		{
			get
			{
				foreach (ThingCategoryDef childCatDef in this.ThisAndChildCategoryDefs)
				{
					foreach (ThingDef def in childCatDef.childThingDefs)
					{
						yield return def;
					}
				}
				yield break;
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06004085 RID: 16517 RVA: 0x0021EAE4 File Offset: 0x0021CEE4
		public IEnumerable<SpecialThingFilterDef> DescendantSpecialThingFilterDefs
		{
			get
			{
				foreach (ThingCategoryDef childCatDef in this.ThisAndChildCategoryDefs)
				{
					foreach (SpecialThingFilterDef sf in childCatDef.childSpecialFilters)
					{
						yield return sf;
					}
				}
				yield break;
			}
		}

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06004086 RID: 16518 RVA: 0x0021EB10 File Offset: 0x0021CF10
		public IEnumerable<SpecialThingFilterDef> ParentsSpecialThingFilterDefs
		{
			get
			{
				foreach (ThingCategoryDef cat in this.Parents)
				{
					foreach (SpecialThingFilterDef filter in cat.childSpecialFilters)
					{
						yield return filter;
					}
				}
				yield break;
			}
		}

		// Token: 0x06004087 RID: 16519 RVA: 0x0021EB3A File Offset: 0x0021CF3A
		public override void PostLoad()
		{
			this.treeNode = new TreeNode_ThingCategory(this);
			if (!this.iconPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.icon = ContentFinder<Texture2D>.Get(this.iconPath, true);
				});
			}
		}

		// Token: 0x06004088 RID: 16520 RVA: 0x0021EB6C File Offset: 0x0021CF6C
		public static ThingCategoryDef Named(string defName)
		{
			return DefDatabase<ThingCategoryDef>.GetNamed(defName, true);
		}

		// Token: 0x06004089 RID: 16521 RVA: 0x0021EB88 File Offset: 0x0021CF88
		public override int GetHashCode()
		{
			return this.defName.GetHashCode();
		}
	}
}
