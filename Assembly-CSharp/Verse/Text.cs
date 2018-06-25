﻿using System;
using System.Collections;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000E8E RID: 3726
	public static class Text
	{
		// Token: 0x04003A24 RID: 14884
		private static GameFont fontInt = GameFont.Small;

		// Token: 0x04003A25 RID: 14885
		private static TextAnchor anchorInt = TextAnchor.UpperLeft;

		// Token: 0x04003A26 RID: 14886
		private static bool wordWrapInt = true;

		// Token: 0x04003A27 RID: 14887
		public static readonly GUIStyle[] fontStyles = new GUIStyle[3];

		// Token: 0x04003A28 RID: 14888
		public static readonly GUIStyle[] textFieldStyles = new GUIStyle[3];

		// Token: 0x04003A29 RID: 14889
		public static readonly GUIStyle[] textAreaStyles = new GUIStyle[3];

		// Token: 0x04003A2A RID: 14890
		public static readonly GUIStyle[] textAreaReadOnlyStyles = new GUIStyle[3];

		// Token: 0x04003A2B RID: 14891
		private static readonly float[] lineHeights = new float[3];

		// Token: 0x04003A2C RID: 14892
		private static readonly float[] spaceBetweenLines = new float[3];

		// Token: 0x04003A2D RID: 14893
		private static GUIContent tmpTextGUIContent = new GUIContent();

		// Token: 0x04003A2E RID: 14894
		private const int NumFonts = 3;

		// Token: 0x060057F3 RID: 22515 RVA: 0x002D2004 File Offset: 0x002D0404
		static Text()
		{
			Font font = (Font)Resources.Load("Fonts/Calibri_tiny");
			Font font2 = (Font)Resources.Load("Fonts/Arial_small");
			Font font3 = (Font)Resources.Load("Fonts/Arial_medium");
			Text.fontStyles[0] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[0].font = font;
			Text.fontStyles[1] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[1].font = font2;
			Text.fontStyles[1].contentOffset = new Vector2(0f, -1f);
			Text.fontStyles[2] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[2].font = font3;
			for (int i = 0; i < Text.textFieldStyles.Length; i++)
			{
				Text.textFieldStyles[i] = new GUIStyle(GUI.skin.textField);
				Text.textFieldStyles[i].alignment = TextAnchor.MiddleLeft;
			}
			Text.textFieldStyles[0].font = font;
			Text.textFieldStyles[1].font = font2;
			Text.textFieldStyles[2].font = font3;
			for (int j = 0; j < Text.textAreaStyles.Length; j++)
			{
				Text.textAreaStyles[j] = new GUIStyle(Text.textFieldStyles[j]);
				Text.textAreaStyles[j].alignment = TextAnchor.UpperLeft;
				Text.textAreaStyles[j].wordWrap = true;
			}
			for (int k = 0; k < Text.textAreaReadOnlyStyles.Length; k++)
			{
				Text.textAreaReadOnlyStyles[k] = new GUIStyle(Text.textAreaStyles[k]);
				GUIStyle guistyle = Text.textAreaReadOnlyStyles[k];
				guistyle.normal.background = null;
				guistyle.active.background = null;
				guistyle.onHover.background = null;
				guistyle.hover.background = null;
				guistyle.onFocused.background = null;
				guistyle.focused.background = null;
			}
			GUI.skin.settings.doubleClickSelectsWord = true;
			int num = 0;
			IEnumerator enumerator = Enum.GetValues(typeof(GameFont)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					GameFont font4 = (GameFont)obj;
					Text.Font = font4;
					Text.lineHeights[num] = Text.CalcHeight("W", 999f);
					Text.spaceBetweenLines[num] = Text.CalcHeight("W\nW", 999f) - Text.CalcHeight("W", 999f) * 2f;
					num++;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Text.Font = GameFont.Small;
		}

		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x060057F4 RID: 22516 RVA: 0x002D2330 File Offset: 0x002D0730
		// (set) Token: 0x060057F5 RID: 22517 RVA: 0x002D234A File Offset: 0x002D074A
		public static GameFont Font
		{
			get
			{
				return Text.fontInt;
			}
			set
			{
				if (value == GameFont.Tiny && !LongEventHandler.AnyEventNowOrWaiting && !LanguageDatabase.activeLanguage.info.canBeTiny)
				{
					Text.fontInt = GameFont.Small;
				}
				else
				{
					Text.fontInt = value;
				}
			}
		}

		// Token: 0x17000DE9 RID: 3561
		// (get) Token: 0x060057F6 RID: 22518 RVA: 0x002D2384 File Offset: 0x002D0784
		// (set) Token: 0x060057F7 RID: 22519 RVA: 0x002D239E File Offset: 0x002D079E
		public static TextAnchor Anchor
		{
			get
			{
				return Text.anchorInt;
			}
			set
			{
				Text.anchorInt = value;
			}
		}

		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x060057F8 RID: 22520 RVA: 0x002D23A8 File Offset: 0x002D07A8
		// (set) Token: 0x060057F9 RID: 22521 RVA: 0x002D23C2 File Offset: 0x002D07C2
		public static bool WordWrap
		{
			get
			{
				return Text.wordWrapInt;
			}
			set
			{
				Text.wordWrapInt = value;
			}
		}

		// Token: 0x17000DEB RID: 3563
		// (get) Token: 0x060057FA RID: 22522 RVA: 0x002D23CC File Offset: 0x002D07CC
		public static float LineHeight
		{
			get
			{
				return Text.lineHeights[(int)Text.Font];
			}
		}

		// Token: 0x17000DEC RID: 3564
		// (get) Token: 0x060057FB RID: 22523 RVA: 0x002D23EC File Offset: 0x002D07EC
		public static float SpaceBetweenLines
		{
			get
			{
				return Text.spaceBetweenLines[(int)Text.Font];
			}
		}

		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x060057FC RID: 22524 RVA: 0x002D240C File Offset: 0x002D080C
		internal static GUIStyle CurFontStyle
		{
			get
			{
				GUIStyle guistyle;
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					guistyle = Text.fontStyles[0];
					break;
				case GameFont.Small:
					guistyle = Text.fontStyles[1];
					break;
				case GameFont.Medium:
					guistyle = Text.fontStyles[2];
					break;
				default:
					throw new NotImplementedException();
				}
				guistyle.alignment = Text.anchorInt;
				guistyle.wordWrap = Text.wordWrapInt;
				return guistyle;
			}
		}

		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x060057FD RID: 22525 RVA: 0x002D2484 File Offset: 0x002D0884
		public static GUIStyle CurTextFieldStyle
		{
			get
			{
				GameFont gameFont = Text.fontInt;
				GUIStyle result;
				if (gameFont != GameFont.Tiny)
				{
					if (gameFont != GameFont.Small)
					{
						if (gameFont != GameFont.Medium)
						{
							throw new NotImplementedException();
						}
						result = Text.textFieldStyles[2];
					}
					else
					{
						result = Text.textFieldStyles[1];
					}
				}
				else
				{
					result = Text.textFieldStyles[0];
				}
				return result;
			}
		}

		// Token: 0x17000DEF RID: 3567
		// (get) Token: 0x060057FE RID: 22526 RVA: 0x002D24E0 File Offset: 0x002D08E0
		public static GUIStyle CurTextAreaStyle
		{
			get
			{
				GameFont gameFont = Text.fontInt;
				GUIStyle result;
				if (gameFont != GameFont.Tiny)
				{
					if (gameFont != GameFont.Small)
					{
						if (gameFont != GameFont.Medium)
						{
							throw new NotImplementedException();
						}
						result = Text.textAreaStyles[2];
					}
					else
					{
						result = Text.textAreaStyles[1];
					}
				}
				else
				{
					result = Text.textAreaStyles[0];
				}
				return result;
			}
		}

		// Token: 0x17000DF0 RID: 3568
		// (get) Token: 0x060057FF RID: 22527 RVA: 0x002D253C File Offset: 0x002D093C
		public static GUIStyle CurTextAreaReadOnlyStyle
		{
			get
			{
				GameFont gameFont = Text.fontInt;
				GUIStyle result;
				if (gameFont != GameFont.Tiny)
				{
					if (gameFont != GameFont.Small)
					{
						if (gameFont != GameFont.Medium)
						{
							throw new NotImplementedException();
						}
						result = Text.textAreaReadOnlyStyles[2];
					}
					else
					{
						result = Text.textAreaReadOnlyStyles[1];
					}
				}
				else
				{
					result = Text.textAreaReadOnlyStyles[0];
				}
				return result;
			}
		}

		// Token: 0x06005800 RID: 22528 RVA: 0x002D2598 File Offset: 0x002D0998
		public static float CalcHeight(string text, float width)
		{
			Text.tmpTextGUIContent.text = text;
			return Text.CurFontStyle.CalcHeight(Text.tmpTextGUIContent, width);
		}

		// Token: 0x06005801 RID: 22529 RVA: 0x002D25C8 File Offset: 0x002D09C8
		public static Vector2 CalcSize(string text)
		{
			Text.tmpTextGUIContent.text = text;
			return Text.CurFontStyle.CalcSize(Text.tmpTextGUIContent);
		}

		// Token: 0x06005802 RID: 22530 RVA: 0x002D25F8 File Offset: 0x002D09F8
		internal static void StartOfOnGUI()
		{
			if (!Text.WordWrap)
			{
				Log.ErrorOnce("Word wrap was false at end of frame.", 764362, false);
				Text.WordWrap = true;
			}
			if (Text.Anchor != TextAnchor.UpperLeft)
			{
				Log.ErrorOnce("Alignment was " + Text.Anchor + " at end of frame.", 15558, false);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			Text.Font = GameFont.Small;
		}
	}
}
