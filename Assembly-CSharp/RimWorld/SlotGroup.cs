﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000692 RID: 1682
	public class SlotGroup
	{
		// Token: 0x040013EB RID: 5099
		public ISlotGroupParent parent;

		// Token: 0x060023AA RID: 9130 RVA: 0x00132090 File Offset: 0x00130490
		public SlotGroup(ISlotGroupParent parent)
		{
			this.parent = parent;
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060023AB RID: 9131 RVA: 0x001320A0 File Offset: 0x001304A0
		private Map Map
		{
			get
			{
				return this.parent.Map;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060023AC RID: 9132 RVA: 0x001320C0 File Offset: 0x001304C0
		public StorageSettings Settings
		{
			get
			{
				return this.parent.GetStoreSettings();
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060023AD RID: 9133 RVA: 0x001320E0 File Offset: 0x001304E0
		public IEnumerable<Thing> HeldThings
		{
			get
			{
				List<IntVec3> cellsList = this.CellsList;
				Map map = this.Map;
				for (int i = 0; i < cellsList.Count; i++)
				{
					List<Thing> thingList = map.thingGrid.ThingsListAt(cellsList[i]);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].def.EverStorable(false))
						{
							yield return thingList[j];
						}
					}
				}
				yield break;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060023AE RID: 9134 RVA: 0x0013210C File Offset: 0x0013050C
		public List<IntVec3> CellsList
		{
			get
			{
				return this.parent.AllSlotCellsList();
			}
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x0013212C File Offset: 0x0013052C
		public IEnumerator<IntVec3> GetEnumerator()
		{
			List<IntVec3> cellsList = this.CellsList;
			for (int i = 0; i < cellsList.Count; i++)
			{
				yield return cellsList[i];
			}
			yield break;
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x0013214E File Offset: 0x0013054E
		public void Notify_AddedCell(IntVec3 c)
		{
			this.Map.haulDestinationManager.SetCellFor(c, this);
			this.Map.listerHaulables.RecalcAllInCell(c);
			this.Map.listerMergeables.RecalcAllInCell(c);
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x00132185 File Offset: 0x00130585
		public void Notify_LostCell(IntVec3 c)
		{
			this.Map.haulDestinationManager.ClearCellFor(c, this);
			this.Map.listerHaulables.RecalcAllInCell(c);
			this.Map.listerMergeables.RecalcAllInCell(c);
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x001321BC File Offset: 0x001305BC
		public override string ToString()
		{
			string result;
			if (this.parent != null)
			{
				result = this.parent.ToString();
			}
			else
			{
				result = "NullParent";
			}
			return result;
		}
	}
}
