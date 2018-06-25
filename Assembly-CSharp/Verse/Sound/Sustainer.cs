﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound
{
	// Token: 0x02000DC2 RID: 3522
	public class Sustainer
	{
		// Token: 0x0400345A RID: 13402
		public SoundDef def;

		// Token: 0x0400345B RID: 13403
		public SoundInfo info;

		// Token: 0x0400345C RID: 13404
		internal GameObject worldRootObject;

		// Token: 0x0400345D RID: 13405
		private int lastMaintainTick;

		// Token: 0x0400345E RID: 13406
		private int lastMaintainFrame;

		// Token: 0x0400345F RID: 13407
		private float endRealTime = -1f;

		// Token: 0x04003460 RID: 13408
		private List<SubSustainer> subSustainers = new List<SubSustainer>();

		// Token: 0x04003461 RID: 13409
		public SoundParams externalParams = new SoundParams();

		// Token: 0x04003462 RID: 13410
		public SustainerScopeFader scopeFader = new SustainerScopeFader();

		// Token: 0x06004EA4 RID: 20132 RVA: 0x0029155C File Offset: 0x0028F95C
		public Sustainer(SoundDef def, SoundInfo info)
		{
			this.def = def;
			this.info = info;
			if (def.subSounds.Count > 0)
			{
				foreach (KeyValuePair<string, float> keyValuePair in info.DefinedParameters)
				{
					this.externalParams[keyValuePair.Key] = keyValuePair.Value;
				}
				if (def.HasSubSoundsInWorld)
				{
					if (info.IsOnCamera)
					{
						Log.Error("Playing sound " + def.ToString() + " on camera, but it has sub-sounds in the world.", false);
					}
					this.worldRootObject = new GameObject("SustainerRootObject_" + def.defName);
					this.UpdateRootObjectPosition();
				}
				else if (!info.IsOnCamera)
				{
					info = SoundInfo.OnCamera(info.Maintenance);
				}
				Find.SoundRoot.sustainerManager.RegisterSustainer(this);
				if (!info.IsOnCamera)
				{
					Find.SoundRoot.sustainerManager.UpdateAllSustainerScopes();
				}
				for (int i = 0; i < def.subSounds.Count; i++)
				{
					this.subSustainers.Add(new SubSustainer(this, def.subSounds[i]));
				}
			}
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.lastMaintainTick = Find.TickManager.TicksGame;
				this.lastMaintainFrame = Time.frameCount;
			});
		}

		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x06004EA5 RID: 20133 RVA: 0x0029170C File Offset: 0x0028FB0C
		public bool Ended
		{
			get
			{
				return this.endRealTime >= 0f;
			}
		}

		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x06004EA6 RID: 20134 RVA: 0x00291734 File Offset: 0x0028FB34
		public float TimeSinceEnd
		{
			get
			{
				return Time.realtimeSinceStartup - this.endRealTime;
			}
		}

		// Token: 0x17000CB5 RID: 3253
		// (get) Token: 0x06004EA7 RID: 20135 RVA: 0x00291758 File Offset: 0x0028FB58
		public float CameraDistanceSquared
		{
			get
			{
				float result;
				if (this.info.IsOnCamera)
				{
					result = 0f;
				}
				else if (this.worldRootObject == null)
				{
					if (Prefs.DevMode)
					{
						Log.Error(string.Concat(new object[]
						{
							"Sustainer ",
							this.def,
							" info is ",
							this.info,
							" but its worldRootObject is null"
						}), false);
					}
					result = 0f;
				}
				else
				{
					result = (float)(Find.CameraDriver.MapPosition - this.worldRootObject.transform.position.ToIntVec3()).LengthHorizontalSquared;
				}
				return result;
			}
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x00291820 File Offset: 0x0028FC20
		public void SustainerUpdate()
		{
			if (!this.Ended)
			{
				if (this.info.Maintenance == MaintenanceType.PerTick)
				{
					if (Find.TickManager.TicksGame > this.lastMaintainTick + 1)
					{
						this.End();
						return;
					}
				}
				else if (this.info.Maintenance == MaintenanceType.PerFrame)
				{
					if (Time.frameCount > this.lastMaintainFrame + 1)
					{
						this.End();
						return;
					}
				}
			}
			else if (this.TimeSinceEnd > this.def.sustainFadeoutTime)
			{
				this.Cleanup();
			}
			if (this.def.subSounds.Count > 0)
			{
				if (!this.info.IsOnCamera && this.info.Maker.HasThing)
				{
					this.UpdateRootObjectPosition();
				}
				this.scopeFader.SustainerScopeUpdate();
				for (int i = 0; i < this.subSustainers.Count; i++)
				{
					this.subSustainers[i].SubSustainerUpdate();
				}
			}
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x0029194C File Offset: 0x0028FD4C
		private void UpdateRootObjectPosition()
		{
			if (this.worldRootObject != null)
			{
				this.worldRootObject.transform.position = this.info.Maker.Cell.ToVector3ShiftedWithAltitude(0f);
			}
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x0029199C File Offset: 0x0028FD9C
		public void Maintain()
		{
			if (this.Ended)
			{
				Log.Error("Tried to maintain ended sustainer: " + this.def, false);
			}
			else if (this.info.Maintenance == MaintenanceType.PerTick)
			{
				this.lastMaintainTick = Find.TickManager.TicksGame;
			}
			else if (this.info.Maintenance == MaintenanceType.PerFrame)
			{
				this.lastMaintainFrame = Time.frameCount;
			}
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x00291A13 File Offset: 0x0028FE13
		public void End()
		{
			this.endRealTime = Time.realtimeSinceStartup;
			if (this.def.sustainFadeoutTime < 0.001f)
			{
				this.Cleanup();
			}
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x00291A3C File Offset: 0x0028FE3C
		private void Cleanup()
		{
			if (this.def.subSounds.Count > 0)
			{
				Find.SoundRoot.sustainerManager.DeregisterSustainer(this);
				for (int i = 0; i < this.subSustainers.Count; i++)
				{
					this.subSustainers[i].Cleanup();
				}
			}
			if (this.def.sustainStopSound != null)
			{
				if (this.worldRootObject != null)
				{
					Map map = this.info.Maker.Map;
					if (map != null)
					{
						SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(this.worldRootObject.transform.position.ToIntVec3(), map, false), MaintenanceType.None);
						this.def.sustainStopSound.PlayOneShot(soundInfo);
					}
				}
				else
				{
					this.def.sustainStopSound.PlayOneShot(SoundInfo.OnCamera(MaintenanceType.None));
				}
			}
			if (this.worldRootObject != null)
			{
				UnityEngine.Object.Destroy(this.worldRootObject);
			}
			DebugSoundEventsLog.Notify_SustainerEnded(this, this.info);
		}

		// Token: 0x06004EAD RID: 20141 RVA: 0x00291B5C File Offset: 0x0028FF5C
		public string DebugString()
		{
			string text = this.def.defName;
			text = text + "\n  inScopePercent=" + this.scopeFader.inScopePercent;
			text = text + "\n  CameraDistanceSquared=" + this.CameraDistanceSquared;
			foreach (SubSustainer arg in this.subSustainers)
			{
				text = text + "\n  sub: " + arg;
			}
			return text;
		}
	}
}
