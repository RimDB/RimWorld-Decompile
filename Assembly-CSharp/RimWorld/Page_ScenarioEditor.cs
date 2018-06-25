﻿using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld
{
	// Token: 0x02000833 RID: 2099
	public class Page_ScenarioEditor : Page
	{
		// Token: 0x040019A2 RID: 6562
		private Scenario curScen = null;

		// Token: 0x040019A3 RID: 6563
		private Vector2 infoScrollPosition = Vector2.zero;

		// Token: 0x040019A4 RID: 6564
		private string seed;

		// Token: 0x040019A5 RID: 6565
		private bool seedIsValid = true;

		// Token: 0x040019A6 RID: 6566
		private bool editMode = false;

		// Token: 0x06002F6A RID: 12138 RVA: 0x001962C0 File Offset: 0x001946C0
		public Page_ScenarioEditor(Scenario scen)
		{
			if (scen != null)
			{
				this.curScen = scen;
				this.seedIsValid = false;
			}
			else
			{
				this.RandomizeSeedAndScenario();
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06002F6B RID: 12139 RVA: 0x00196318 File Offset: 0x00194718
		public override string PageTitle
		{
			get
			{
				return "ScenarioEditor".Translate();
			}
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06002F6C RID: 12140 RVA: 0x00196338 File Offset: 0x00194738
		public Scenario EditingScenario
		{
			get
			{
				return this.curScen;
			}
		}

		// Token: 0x06002F6D RID: 12141 RVA: 0x00196353 File Offset: 0x00194753
		public override void PreOpen()
		{
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
		}

		// Token: 0x06002F6E RID: 12142 RVA: 0x00196368 File Offset: 0x00194768
		public override void DoWindowContents(Rect rect)
		{
			base.DrawPageTitle(rect);
			Rect mainRect = base.GetMainRect(rect, 0f, false);
			GUI.BeginGroup(mainRect);
			Rect rect2 = new Rect(0f, 0f, mainRect.width * 0.35f, mainRect.height).Rounded();
			this.DoConfigControls(rect2);
			Rect rect3 = new Rect(rect2.xMax + 17f, 0f, mainRect.width - rect2.width - 17f, mainRect.height).Rounded();
			if (!this.editMode)
			{
				ScenarioUI.DrawScenarioInfo(rect3, this.curScen, ref this.infoScrollPosition);
			}
			else
			{
				ScenarioUI.DrawScenarioEditInterface(rect3, this.curScen, ref this.infoScrollPosition);
			}
			GUI.EndGroup();
			base.DoBottomButtons(rect, null, null, null, true);
		}

		// Token: 0x06002F6F RID: 12143 RVA: 0x00196440 File Offset: 0x00194840
		private void RandomizeSeedAndScenario()
		{
			this.seed = GenText.RandomSeedString();
			this.curScen = ScenarioMaker.GenerateNewRandomScenario(this.seed);
		}

		// Token: 0x06002F70 RID: 12144 RVA: 0x00196460 File Offset: 0x00194860
		private void DoConfigControls(Rect rect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = 200f;
			listing_Standard.Begin(rect);
			if (listing_Standard.ButtonText("Load".Translate(), null))
			{
				Find.WindowStack.Add(new Dialog_ScenarioList_Load(delegate(Scenario loadedScen)
				{
					this.curScen = loadedScen;
					this.seedIsValid = false;
				}));
			}
			if (listing_Standard.ButtonText("Save".Translate(), null))
			{
				if (Page_ScenarioEditor.CheckAllPartsCompatible(this.curScen))
				{
					Find.WindowStack.Add(new Dialog_ScenarioList_Save(this.curScen));
				}
			}
			if (listing_Standard.ButtonText("RandomizeSeed".Translate(), null))
			{
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
				this.RandomizeSeedAndScenario();
				this.seedIsValid = true;
			}
			if (this.seedIsValid)
			{
				listing_Standard.Label("Seed".Translate().CapitalizeFirst(), -1f, null);
				string a = listing_Standard.TextEntry(this.seed, 1);
				if (a != this.seed)
				{
					this.seed = a;
					this.curScen = ScenarioMaker.GenerateNewRandomScenario(this.seed);
				}
			}
			else
			{
				listing_Standard.Gap(Text.LineHeight + Text.LineHeight + 2f);
			}
			listing_Standard.CheckboxLabeled("EditMode".Translate().CapitalizeFirst(), ref this.editMode, null);
			if (this.editMode)
			{
				this.seedIsValid = false;
				if (listing_Standard.ButtonText("AddPart".Translate(), null))
				{
					this.OpenAddScenPartMenu();
				}
				if (SteamManager.Initialized && (this.curScen.Category == ScenarioCategory.CustomLocal || this.curScen.Category == ScenarioCategory.SteamWorkshop))
				{
					if (listing_Standard.ButtonText(Workshop.UploadButtonLabel(this.curScen.GetPublishedFileId()), null))
					{
						if (Page_ScenarioEditor.CheckAllPartsCompatible(this.curScen))
						{
							AcceptanceReport acceptanceReport = this.curScen.TryUploadReport();
							if (!acceptanceReport.Accepted)
							{
								Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
							}
							else
							{
								SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
								Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSteamWorkshopUpload".Translate(), delegate
								{
									SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
									Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmContentAuthor".Translate(), delegate
									{
										SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
										Workshop.Upload(this.curScen);
									}, true, null));
								}, true, null));
							}
						}
					}
				}
			}
			listing_Standard.End();
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x001966C0 File Offset: 0x00194AC0
		private static bool CheckAllPartsCompatible(Scenario scen)
		{
			foreach (ScenPart scenPart in scen.AllParts)
			{
				int num = 0;
				foreach (ScenPart scenPart2 in scen.AllParts)
				{
					if (scenPart2.def == scenPart.def)
					{
						num++;
					}
					if (num > scenPart.def.maxUses)
					{
						Messages.Message("TooMany".Translate(new object[]
						{
							scenPart.def.maxUses
						}) + ": " + scenPart.def.label, MessageTypeDefOf.RejectInput, false);
						return false;
					}
					if (scenPart != scenPart2 && !scenPart.CanCoexistWith(scenPart2))
					{
						Messages.Message(string.Concat(new string[]
						{
							"Incompatible".Translate(),
							": ",
							scenPart.def.label,
							", ",
							scenPart2.def.label
						}), MessageTypeDefOf.RejectInput, false);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x00196860 File Offset: 0x00194C60
		private void OpenAddScenPartMenu()
		{
			FloatMenuUtility.MakeMenu<ScenPartDef>(from p in ScenarioMaker.AddableParts(this.curScen)
			where p.category != ScenPartCategory.Fixed
			orderby p.label
			select p, (ScenPartDef p) => p.LabelCap, (ScenPartDef p) => delegate()
			{
				this.AddScenPart(p);
			});
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x001968EC File Offset: 0x00194CEC
		private void AddScenPart(ScenPartDef def)
		{
			ScenPart scenPart = ScenarioMaker.MakeScenPart(def);
			scenPart.Randomize();
			this.curScen.parts.Add(scenPart);
		}

		// Token: 0x06002F74 RID: 12148 RVA: 0x00196918 File Offset: 0x00194D18
		protected override bool CanDoNext()
		{
			bool result;
			if (!base.CanDoNext())
			{
				result = false;
			}
			else if (this.curScen == null)
			{
				result = false;
			}
			else if (!Page_ScenarioEditor.CheckAllPartsCompatible(this.curScen))
			{
				result = false;
			}
			else
			{
				Page_SelectScenario.BeginScenarioConfiguration(this.curScen, this);
				result = true;
			}
			return result;
		}
	}
}
