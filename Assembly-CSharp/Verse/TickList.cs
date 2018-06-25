﻿using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Verse
{
	// Token: 0x02000BD1 RID: 3025
	public class TickList
	{
		// Token: 0x04002D0E RID: 11534
		private TickerType tickType;

		// Token: 0x04002D0F RID: 11535
		private List<List<Thing>> thingLists = new List<List<Thing>>();

		// Token: 0x04002D10 RID: 11536
		private List<Thing> thingsToRegister = new List<Thing>();

		// Token: 0x04002D11 RID: 11537
		private List<Thing> thingsToDeregister = new List<Thing>();

		// Token: 0x060041EE RID: 16878 RVA: 0x0022C134 File Offset: 0x0022A534
		public TickList(TickerType tickType)
		{
			this.tickType = tickType;
			for (int i = 0; i < this.TickInterval; i++)
			{
				this.thingLists.Add(new List<Thing>());
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x060041EF RID: 16879 RVA: 0x0022C19C File Offset: 0x0022A59C
		private int TickInterval
		{
			get
			{
				TickerType tickerType = this.tickType;
				int result;
				if (tickerType != TickerType.Normal)
				{
					if (tickerType != TickerType.Rare)
					{
						if (tickerType != TickerType.Long)
						{
							result = -1;
						}
						else
						{
							result = 2000;
						}
					}
					else
					{
						result = 250;
					}
				}
				else
				{
					result = 1;
				}
				return result;
			}
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x0022C1F0 File Offset: 0x0022A5F0
		public void Reset()
		{
			for (int i = 0; i < this.thingLists.Count; i++)
			{
				this.thingLists[i].Clear();
			}
			this.thingsToRegister.Clear();
			this.thingsToDeregister.Clear();
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x0022C244 File Offset: 0x0022A644
		public void RemoveWhere(Predicate<Thing> predicate)
		{
			for (int i = 0; i < this.thingLists.Count; i++)
			{
				this.thingLists[i].RemoveAll(predicate);
			}
			this.thingsToRegister.RemoveAll(predicate);
			this.thingsToDeregister.RemoveAll(predicate);
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x0022C29D File Offset: 0x0022A69D
		public void RegisterThing(Thing t)
		{
			this.thingsToRegister.Add(t);
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x0022C2AC File Offset: 0x0022A6AC
		public void DeregisterThing(Thing t)
		{
			this.thingsToDeregister.Add(t);
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x0022C2BC File Offset: 0x0022A6BC
		public void Tick()
		{
			for (int i = 0; i < this.thingsToRegister.Count; i++)
			{
				this.BucketOf(this.thingsToRegister[i]).Add(this.thingsToRegister[i]);
			}
			this.thingsToRegister.Clear();
			for (int j = 0; j < this.thingsToDeregister.Count; j++)
			{
				this.BucketOf(this.thingsToDeregister[j]).Remove(this.thingsToDeregister[j]);
			}
			this.thingsToDeregister.Clear();
			if (DebugSettings.fastEcology)
			{
				Find.World.tileTemperatures.ClearCaches();
				for (int k = 0; k < this.thingLists.Count; k++)
				{
					List<Thing> list = this.thingLists[k];
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].def.category == ThingCategory.Plant)
						{
							list[l].TickLong();
						}
					}
				}
			}
			List<Thing> list2 = this.thingLists[Find.TickManager.TicksGame % this.TickInterval];
			for (int m = 0; m < list2.Count; m++)
			{
				if (!list2[m].Destroyed)
				{
					try
					{
						Profiler.BeginSample(list2[m].def.defName);
						TickerType tickerType = this.tickType;
						if (tickerType != TickerType.Normal)
						{
							if (tickerType != TickerType.Rare)
							{
								if (tickerType == TickerType.Long)
								{
									list2[m].TickLong();
								}
							}
							else
							{
								list2[m].TickRare();
							}
						}
						else
						{
							list2[m].Tick();
						}
						Profiler.EndSample();
					}
					catch (Exception ex)
					{
						string text = (!list2[m].Spawned) ? "" : (" (at " + list2[m].Position + ")");
						if (Prefs.DevMode)
						{
							Log.Error(string.Concat(new object[]
							{
								"Exception ticking ",
								list2[m].ToStringSafe<Thing>(),
								text,
								": ",
								ex
							}), false);
						}
						else
						{
							Log.ErrorOnce(string.Concat(new object[]
							{
								"Exception ticking ",
								list2[m].ToStringSafe<Thing>(),
								text,
								". Suppressing further errors. Exception: ",
								ex
							}), list2[m].thingIDNumber ^ 576876901, false);
						}
					}
				}
			}
		}

		// Token: 0x060041F5 RID: 16885 RVA: 0x0022C5B8 File Offset: 0x0022A9B8
		private List<Thing> BucketOf(Thing t)
		{
			int num = t.GetHashCode();
			if (num < 0)
			{
				num *= -1;
			}
			int index = num % this.TickInterval;
			return this.thingLists[index];
		}
	}
}
