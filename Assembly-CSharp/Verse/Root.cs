﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI;
using Verse.Profile;
using Verse.Sound;
using Verse.Steam;

namespace Verse
{
	public abstract class Root : MonoBehaviour
	{
		private static bool globalInitDone;

		private static bool prefsApplied;

		protected static bool checkedAutostartSaveFile;

		protected bool destroyed;

		public SoundRoot soundRoot;

		public UIRoot uiRoot;

		[CompilerGenerated]
		private static Action <>f__am$cache0;

		[CompilerGenerated]
		private static Action <>f__mg$cache0;

		protected Root()
		{
		}

		public virtual void Start()
		{
			try
			{
				CultureInfoUtility.EnsureEnglish();
				Current.Notify_LoadedSceneChanged();
				Root.CheckGlobalInit();
				Action action = delegate()
				{
					this.soundRoot = new SoundRoot();
					if (GenScene.InPlayScene)
					{
						this.uiRoot = new UIRoot_Play();
					}
					else if (GenScene.InEntryScene)
					{
						this.uiRoot = new UIRoot_Entry();
					}
					this.uiRoot.Init();
					Messages.Notify_LoadedLevelChanged();
					if (Current.SubcameraDriver != null)
					{
						Current.SubcameraDriver.Init();
					}
				};
				if (!PlayDataLoader.Loaded)
				{
					LongEventHandler.QueueLongEvent(delegate()
					{
						PlayDataLoader.LoadAllPlayData(false);
					}, null, true, null);
					LongEventHandler.QueueLongEvent(action, "InitializingInterface", false, null);
				}
				else
				{
					action();
				}
			}
			catch (Exception arg)
			{
				Log.Error("Critical error in root Start(): " + arg, false);
			}
		}

		private static void CheckGlobalInit()
		{
			if (Root.globalInitDone)
			{
				return;
			}
			UnityDataInitializer.CopyUnityData();
			SteamManager.InitIfNeeded();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs != null && commandLineArgs.Length > 1)
			{
				Log.Message("Command line arguments: " + GenText.ToSpaceList(commandLineArgs.Skip(1)), false);
			}
			VersionControl.LogVersionNumber();
			Application.targetFrameRate = 60;
			Prefs.Init();
			if (Prefs.DevMode)
			{
				StaticConstructorOnStartupUtility.ReportProbablyMissingAttributes();
			}
			if (Root.<>f__mg$cache0 == null)
			{
				Root.<>f__mg$cache0 = new Action(StaticConstructorOnStartupUtility.CallAll);
			}
			LongEventHandler.QueueLongEvent(Root.<>f__mg$cache0, null, false, null);
			Root.globalInitDone = true;
		}

		public virtual void Update()
		{
			try
			{
				RealTime.Update();
				bool flag;
				LongEventHandler.LongEventsUpdate(out flag);
				if (flag)
				{
					this.destroyed = true;
				}
				else if (!LongEventHandler.ShouldWaitForEvent)
				{
					Rand.EnsureStateStackEmpty();
					Widgets.EnsureMousePositionStackEmpty();
					SteamManager.Update();
					PortraitsCache.PortraitsCacheUpdate();
					AttackTargetsCache.AttackTargetsCacheStaticUpdate();
					Pawn_MeleeVerbs.PawnMeleeVerbsStaticUpdate();
					Storyteller.StorytellerStaticUpdate();
					CaravanInventoryUtility.CaravanInventoryUtilityStaticUpdate();
					this.uiRoot.UIRootUpdate();
					if (Time.frameCount > 3 && !Root.prefsApplied)
					{
						Root.prefsApplied = true;
						Prefs.Apply();
					}
					this.soundRoot.Update();
					try
					{
						MemoryTracker.Update();
					}
					catch (Exception arg)
					{
						Log.Error("Error in MemoryTracker: " + arg, false);
					}
					try
					{
						MapLeakTracker.Update();
					}
					catch (Exception arg2)
					{
						Log.Error("Error in MapLeakTracker: " + arg2, false);
					}
				}
			}
			catch (Exception arg3)
			{
				Log.Error("Root level exception in Update(): " + arg3, false);
			}
		}

		public void OnGUI()
		{
			try
			{
				if (!this.destroyed)
				{
					GUI.depth = 50;
					UI.ApplyUIScale();
					LongEventHandler.LongEventsOnGUI();
					if (LongEventHandler.ShouldWaitForEvent)
					{
						ScreenFader.OverlayOnGUI(new Vector2((float)UI.screenWidth, (float)UI.screenHeight));
					}
					else
					{
						this.uiRoot.UIRootOnGUI();
						ScreenFader.OverlayOnGUI(new Vector2((float)UI.screenWidth, (float)UI.screenHeight));
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Root level exception in OnGUI(): " + arg, false);
			}
		}

		public static void Shutdown()
		{
			SteamManager.ShutdownSteam();
			DirectoryInfo directoryInfo = new DirectoryInfo(GenFilePaths.TempFolderPath);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				fileInfo.Delete();
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				directoryInfo2.Delete(true);
			}
			Application.Quit();
		}

		[CompilerGenerated]
		private void <Start>m__0()
		{
			this.soundRoot = new SoundRoot();
			if (GenScene.InPlayScene)
			{
				this.uiRoot = new UIRoot_Play();
			}
			else if (GenScene.InEntryScene)
			{
				this.uiRoot = new UIRoot_Entry();
			}
			this.uiRoot.Init();
			Messages.Notify_LoadedLevelChanged();
			if (Current.SubcameraDriver != null)
			{
				Current.SubcameraDriver.Init();
			}
		}

		[CompilerGenerated]
		private static void <Start>m__1()
		{
			PlayDataLoader.LoadAllPlayData(false);
		}
	}
}
