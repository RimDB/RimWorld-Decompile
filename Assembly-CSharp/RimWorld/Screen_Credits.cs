﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200083A RID: 2106
	public class Screen_Credits : Window
	{
		// Token: 0x040019BC RID: 6588
		private List<CreditsEntry> creds;

		// Token: 0x040019BD RID: 6589
		public bool wonGame = false;

		// Token: 0x040019BE RID: 6590
		private float timeUntilAutoScroll;

		// Token: 0x040019BF RID: 6591
		private float scrollPosition = 0f;

		// Token: 0x040019C0 RID: 6592
		private bool playedMusic = false;

		// Token: 0x040019C1 RID: 6593
		public float creationRealtime = -1f;

		// Token: 0x040019C2 RID: 6594
		private const int ColumnWidth = 800;

		// Token: 0x040019C3 RID: 6595
		private const float InitialAutoScrollDelay = 1f;

		// Token: 0x040019C4 RID: 6596
		private const float InitialAutoScrollDelayWonGame = 6f;

		// Token: 0x040019C5 RID: 6597
		private const float AutoScrollDelayAfterManualScroll = 3f;

		// Token: 0x040019C6 RID: 6598
		private const float SongStartDelay = 5f;

		// Token: 0x040019C7 RID: 6599
		private const GameFont Font = GameFont.Medium;

		// Token: 0x06002FA7 RID: 12199 RVA: 0x00198266 File Offset: 0x00196666
		public Screen_Credits() : this("")
		{
		}

		// Token: 0x06002FA8 RID: 12200 RVA: 0x00198274 File Offset: 0x00196674
		public Screen_Credits(string preCreditsMessage)
		{
			this.doWindowBackground = false;
			this.doCloseButton = false;
			this.doCloseX = false;
			this.forcePause = true;
			this.creds = CreditsAssembler.AllCredits().ToList<CreditsEntry>();
			this.creds.Insert(0, new CreditRecord_Space(100f));
			if (!preCreditsMessage.NullOrEmpty())
			{
				this.creds.Insert(1, new CreditRecord_Space(200f));
				this.creds.Insert(2, new CreditRecord_Text(preCreditsMessage, TextAnchor.UpperLeft));
				this.creds.Insert(3, new CreditRecord_Space(50f));
			}
			this.creds.Add(new CreditRecord_Space(300f));
			this.creds.Add(new CreditRecord_Text("ThanksForPlaying".Translate(), TextAnchor.UpperCenter));
		}

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06002FA9 RID: 12201 RVA: 0x0019836C File Offset: 0x0019676C
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)UI.screenWidth, (float)UI.screenHeight);
			}
		}

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06002FAA RID: 12202 RVA: 0x00198394 File Offset: 0x00196794
		protected override float Margin
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06002FAB RID: 12203 RVA: 0x001983B0 File Offset: 0x001967B0
		private float ViewWidth
		{
			get
			{
				return 800f;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06002FAC RID: 12204 RVA: 0x001983CC File Offset: 0x001967CC
		private float ViewHeight
		{
			get
			{
				GameFont font = Text.Font;
				Text.Font = GameFont.Medium;
				float result = this.creds.Sum((CreditsEntry c) => c.DrawHeight(this.ViewWidth)) + 20f;
				Text.Font = font;
				return result;
			}
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06002FAD RID: 12205 RVA: 0x00198414 File Offset: 0x00196814
		private float MaxScrollPosition
		{
			get
			{
				return Mathf.Max(this.ViewHeight - (float)UI.screenHeight / 2f, 0f);
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06002FAE RID: 12206 RVA: 0x00198448 File Offset: 0x00196848
		private float AutoScrollRate
		{
			get
			{
				float result;
				if (this.wonGame)
				{
					float num = SongDefOf.EndCreditsSong.clip.length + 5f - 6f;
					result = this.MaxScrollPosition / num;
				}
				else
				{
					result = 30f;
				}
				return result;
			}
		}

		// Token: 0x06002FAF RID: 12207 RVA: 0x00198498 File Offset: 0x00196898
		public override void PreOpen()
		{
			base.PreOpen();
			this.creationRealtime = Time.realtimeSinceStartup;
			if (this.wonGame)
			{
				this.timeUntilAutoScroll = 6f;
			}
			else
			{
				this.timeUntilAutoScroll = 1f;
			}
		}

		// Token: 0x06002FB0 RID: 12208 RVA: 0x001984D4 File Offset: 0x001968D4
		public override void WindowUpdate()
		{
			base.WindowUpdate();
			if (this.timeUntilAutoScroll > 0f)
			{
				this.timeUntilAutoScroll -= Time.deltaTime;
			}
			else
			{
				this.scrollPosition += this.AutoScrollRate * Time.deltaTime;
			}
			if (this.wonGame && !this.playedMusic && Time.realtimeSinceStartup > this.creationRealtime + 5f)
			{
				Find.MusicManagerPlay.ForceStartSong(SongDefOf.EndCreditsSong, true);
				this.playedMusic = true;
			}
		}

		// Token: 0x06002FB1 RID: 12209 RVA: 0x00198570 File Offset: 0x00196970
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
			GUI.DrawTexture(rect, BaseContent.BlackTex);
			Rect position = new Rect(rect);
			position.yMin += 30f;
			position.yMax -= 30f;
			position.xMin = rect.center.x - 400f;
			position.width = 800f;
			float viewWidth = this.ViewWidth;
			float viewHeight = this.ViewHeight;
			this.scrollPosition = Mathf.Clamp(this.scrollPosition, 0f, this.MaxScrollPosition);
			GUI.BeginGroup(position);
			Rect position2 = new Rect(0f, 0f, viewWidth, viewHeight);
			position2.y -= this.scrollPosition;
			GUI.BeginGroup(position2);
			Text.Font = GameFont.Medium;
			float num = 0f;
			foreach (CreditsEntry creditsEntry in this.creds)
			{
				float num2 = creditsEntry.DrawHeight(position2.width);
				Rect rect2 = new Rect(0f, num, position2.width, num2);
				creditsEntry.Draw(rect2);
				num += num2;
			}
			GUI.EndGroup();
			GUI.EndGroup();
			if (Event.current.type == EventType.ScrollWheel)
			{
				this.Scroll(Event.current.delta.y * 25f);
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.DownArrow)
				{
					this.Scroll(250f);
					Event.current.Use();
				}
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					this.Scroll(-250f);
					Event.current.Use();
				}
			}
		}

		// Token: 0x06002FB2 RID: 12210 RVA: 0x001987A0 File Offset: 0x00196BA0
		private void Scroll(float offset)
		{
			this.scrollPosition += offset;
			this.timeUntilAutoScroll = 3f;
		}
	}
}
