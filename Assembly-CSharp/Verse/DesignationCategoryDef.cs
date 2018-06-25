﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace Verse
{
	// Token: 0x02000B34 RID: 2868
	public class DesignationCategoryDef : Def
	{
		// Token: 0x04002932 RID: 10546
		public List<Type> specialDesignatorClasses = new List<Type>();

		// Token: 0x04002933 RID: 10547
		public int order = 0;

		// Token: 0x04002934 RID: 10548
		public bool showPowerGrid = false;

		// Token: 0x04002935 RID: 10549
		[Unsaved]
		private List<Designator> resolvedDesignators = new List<Designator>();

		// Token: 0x04002936 RID: 10550
		[Unsaved]
		public KeyBindingCategoryDef bindingCatDef;

		// Token: 0x04002937 RID: 10551
		[Unsaved]
		public string cachedHighlightClosedTag;

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06003F21 RID: 16161 RVA: 0x00213FC0 File Offset: 0x002123C0
		public IEnumerable<Designator> ResolvedAllowedDesignators
		{
			get
			{
				GameRules rules = Current.Game.Rules;
				for (int i = 0; i < this.resolvedDesignators.Count; i++)
				{
					Designator des = this.resolvedDesignators[i];
					if (rules.DesignatorAllowed(des))
					{
						yield return des;
					}
				}
				yield break;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06003F22 RID: 16162 RVA: 0x00213FEC File Offset: 0x002123EC
		public List<Designator> AllResolvedDesignators
		{
			get
			{
				return this.resolvedDesignators;
			}
		}

		// Token: 0x06003F23 RID: 16163 RVA: 0x00214007 File Offset: 0x00212407
		public override void ResolveReferences()
		{
			base.ResolveReferences();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.ResolveDesignators();
			});
			this.cachedHighlightClosedTag = "DesignationCategoryButton-" + this.defName + "-Closed";
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x0021403C File Offset: 0x0021243C
		private void ResolveDesignators()
		{
			this.resolvedDesignators.Clear();
			foreach (Type type in this.specialDesignatorClasses)
			{
				Designator designator = null;
				try
				{
					designator = (Designator)Activator.CreateInstance(type);
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"DesignationCategoryDef",
						this.defName,
						" could not instantiate special designator from class ",
						type,
						".\n Exception: \n",
						ex.ToString()
					}), false);
				}
				if (designator != null)
				{
					this.resolvedDesignators.Add(designator);
				}
			}
			IEnumerable<BuildableDef> enumerable = from tDef in DefDatabase<TerrainDef>.AllDefs.Cast<BuildableDef>().Concat(DefDatabase<ThingDef>.AllDefs.Cast<BuildableDef>())
			where tDef.designationCategory == this
			select tDef;
			Dictionary<DesignatorDropdownGroupDef, Designator_Dropdown> dictionary = new Dictionary<DesignatorDropdownGroupDef, Designator_Dropdown>();
			foreach (BuildableDef buildableDef in enumerable)
			{
				if (buildableDef.designatorDropdown != null)
				{
					if (!dictionary.ContainsKey(buildableDef.designatorDropdown))
					{
						dictionary[buildableDef.designatorDropdown] = new Designator_Dropdown();
						this.resolvedDesignators.Add(dictionary[buildableDef.designatorDropdown]);
					}
					dictionary[buildableDef.designatorDropdown].Add(new Designator_Build(buildableDef));
				}
				else
				{
					this.resolvedDesignators.Add(new Designator_Build(buildableDef));
				}
			}
		}
	}
}
