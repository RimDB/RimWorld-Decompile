﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Verse
{
	[StaticConstructorOnStartup]
	public static class ContentSourceUtility
	{
		public const float IconSize = 24f;

		private static readonly Texture2D ContentSourceIcon_LocalFolder = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/LocalFolder", true);

		private static readonly Texture2D ContentSourceIcon_SteamWorkshop = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/SteamWorkshop", true);

		public static Texture2D GetIcon(this ContentSource s)
		{
			if (s == ContentSource.Undefined)
			{
				return BaseContent.BadTex;
			}
			if (s == ContentSource.LocalFolder)
			{
				return ContentSourceUtility.ContentSourceIcon_LocalFolder;
			}
			if (s != ContentSource.SteamWorkshop)
			{
				throw new NotImplementedException();
			}
			return ContentSourceUtility.ContentSourceIcon_SteamWorkshop;
		}

		public static void DrawContentSource(Rect r, ContentSource source, Action clickAction = null)
		{
			Rect rect = new Rect(r.x, r.y + r.height / 2f - 12f, 24f, 24f);
			GUI.DrawTexture(rect, source.GetIcon());
			Widgets.DrawHighlightIfMouseover(rect);
			TooltipHandler.TipRegion(rect, () => "Source".Translate() + ": " + source.HumanLabel(), (int)(r.x + r.y * 56161f));
			if (clickAction != null && Widgets.ButtonInvisible(rect, false))
			{
				clickAction();
			}
		}

		public static string HumanLabel(this ContentSource s)
		{
			return ("ContentSource_" + s.ToString()).Translate();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ContentSourceUtility()
		{
		}

		[CompilerGenerated]
		private sealed class <DrawContentSource>c__AnonStorey0
		{
			internal ContentSource source;

			public <DrawContentSource>c__AnonStorey0()
			{
			}

			internal string <>m__0()
			{
				return "Source".Translate() + ": " + this.source.HumanLabel();
			}
		}
	}
}
