﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	public class FloatMenu : Window
	{
		public bool givesColonistOrders;

		public bool vanishIfMouseDistant = true;

		public Action onCloseCallback;

		protected List<FloatMenuOption> options;

		private string title;

		private bool needSelection;

		private Color baseColor = Color.white;

		private Vector2 scrollPosition;

		private static readonly Vector2 TitleOffset = new Vector2(30f, -25f);

		private const float OptionSpacing = -1f;

		private const float MaxScreenHeightPercent = 0.9f;

		private const float MinimumColumnWidth = 70f;

		private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);

		private const float FadeStartMouseDist = 5f;

		private const float FadeFinishMouseDist = 100f;

		[CompilerGenerated]
		private static Func<FloatMenuOption, MenuOptionPriority> <>f__am$cache0;

		public FloatMenu(List<FloatMenuOption> options)
		{
			if (options.NullOrEmpty<FloatMenuOption>())
			{
				Log.Error("Created FloatMenu with no options. Closing.", false);
				this.Close(true);
			}
			this.options = (from op in options
			orderby op.Priority descending
			select op).ToList<FloatMenuOption>();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(this.SizeMode);
			}
			this.layer = WindowLayer.Super;
			this.closeOnClickedOutside = true;
			this.doWindowBackground = false;
			this.drawShadow = false;
			SoundDefOf.FloatMenu_Open.PlayOneShotOnCamera(null);
		}

		public FloatMenu(List<FloatMenuOption> options, string title, bool needSelection = false) : this(options)
		{
			this.title = title;
			this.needSelection = needSelection;
		}

		protected override float Margin
		{
			get
			{
				return 0f;
			}
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(this.TotalWidth, this.TotalWindowHeight);
			}
		}

		private float MaxWindowHeight
		{
			get
			{
				return (float)UI.screenHeight * 0.9f;
			}
		}

		private float TotalWindowHeight
		{
			get
			{
				return Mathf.Min(this.TotalViewHeight, this.MaxWindowHeight) + 1f;
			}
		}

		private float MaxViewHeight
		{
			get
			{
				if (this.UsingScrollbar)
				{
					float num = 0f;
					float num2 = 0f;
					for (int i = 0; i < this.options.Count; i++)
					{
						float requiredHeight = this.options[i].RequiredHeight;
						if (requiredHeight > num)
						{
							num = requiredHeight;
						}
						num2 += requiredHeight + -1f;
					}
					int columnCount = this.ColumnCount;
					num2 += (float)columnCount * num;
					return num2 / (float)columnCount;
				}
				return this.MaxWindowHeight;
			}
		}

		private float TotalViewHeight
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				float maxViewHeight = this.MaxViewHeight;
				for (int i = 0; i < this.options.Count; i++)
				{
					float requiredHeight = this.options[i].RequiredHeight;
					if (num2 + requiredHeight + -1f > maxViewHeight)
					{
						if (num2 > num)
						{
							num = num2;
						}
						num2 = requiredHeight;
					}
					else
					{
						num2 += requiredHeight + -1f;
					}
				}
				return Mathf.Max(num, num2);
			}
		}

		private float TotalWidth
		{
			get
			{
				float num = (float)this.ColumnCount * this.ColumnWidth;
				if (this.UsingScrollbar)
				{
					num += 16f;
				}
				return num;
			}
		}

		private float ColumnWidth
		{
			get
			{
				float num = 70f;
				for (int i = 0; i < this.options.Count; i++)
				{
					float requiredWidth = this.options[i].RequiredWidth;
					if (requiredWidth >= 300f)
					{
						return 300f;
					}
					if (requiredWidth > num)
					{
						num = requiredWidth;
					}
				}
				return Mathf.Round(num);
			}
		}

		private int MaxColumns
		{
			get
			{
				return Mathf.FloorToInt(((float)UI.screenWidth - 16f) / this.ColumnWidth);
			}
		}

		private bool UsingScrollbar
		{
			get
			{
				return this.ColumnCountIfNoScrollbar > this.MaxColumns;
			}
		}

		private int ColumnCount
		{
			get
			{
				return Mathf.Min(this.ColumnCountIfNoScrollbar, this.MaxColumns);
			}
		}

		private int ColumnCountIfNoScrollbar
		{
			get
			{
				if (this.options == null)
				{
					return 1;
				}
				Text.Font = GameFont.Small;
				int num = 1;
				float num2 = 0f;
				float maxWindowHeight = this.MaxWindowHeight;
				for (int i = 0; i < this.options.Count; i++)
				{
					float requiredHeight = this.options[i].RequiredHeight;
					if (num2 + requiredHeight + -1f > maxWindowHeight)
					{
						num2 = requiredHeight;
						num++;
					}
					else
					{
						num2 += requiredHeight + -1f;
					}
				}
				return num;
			}
		}

		public FloatMenuSizeMode SizeMode
		{
			get
			{
				if (this.options.Count > 60)
				{
					return FloatMenuSizeMode.Tiny;
				}
				return FloatMenuSizeMode.Normal;
			}
		}

		protected override void SetInitialSizeAndPosition()
		{
			Vector2 vector = UI.MousePositionOnUIInverted + FloatMenu.InitialPositionShift;
			if (vector.x + this.InitialSize.x > (float)UI.screenWidth)
			{
				vector.x = (float)UI.screenWidth - this.InitialSize.x;
			}
			if (vector.y + this.InitialSize.y > (float)UI.screenHeight)
			{
				vector.y = (float)UI.screenHeight - this.InitialSize.y;
			}
			this.windowRect = new Rect(vector.x, vector.y, this.InitialSize.x, this.InitialSize.y);
		}

		public override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			if (!this.title.NullOrEmpty())
			{
				Vector2 vector = new Vector2(this.windowRect.x, this.windowRect.y);
				Text.Font = GameFont.Small;
				float width = Mathf.Max(150f, 15f + Text.CalcSize(this.title).x);
				Rect titleRect = new Rect(vector.x + FloatMenu.TitleOffset.x, vector.y + FloatMenu.TitleOffset.y, width, 23f);
				Find.WindowStack.ImmediateWindow(6830963, titleRect, WindowLayer.Super, delegate
				{
					GUI.color = this.baseColor;
					Text.Font = GameFont.Small;
					Rect position = titleRect.AtZero();
					position.width = 150f;
					GUI.DrawTexture(position, TexUI.TextBGBlack);
					Rect rect = titleRect.AtZero();
					rect.x += 15f;
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect, this.title);
					Text.Anchor = TextAnchor.UpperLeft;
				}, false, false, 0f);
			}
		}

		public override void DoWindowContents(Rect rect)
		{
			if (this.needSelection && Find.Selector.SingleSelectedThing == null)
			{
				Find.WindowStack.TryRemove(this, true);
				return;
			}
			this.UpdateBaseColor();
			bool usingScrollbar = this.UsingScrollbar;
			GUI.color = this.baseColor;
			Text.Font = GameFont.Small;
			Vector2 zero = Vector2.zero;
			float maxViewHeight = this.MaxViewHeight;
			float columnWidth = this.ColumnWidth;
			if (usingScrollbar)
			{
				rect.width -= 10f;
				Widgets.BeginScrollView(rect, ref this.scrollPosition, new Rect(0f, 0f, this.TotalWidth - 16f, this.TotalViewHeight), true);
			}
			for (int i = 0; i < this.options.Count; i++)
			{
				FloatMenuOption floatMenuOption = this.options[i];
				float requiredHeight = floatMenuOption.RequiredHeight;
				if (zero.y + requiredHeight + -1f > maxViewHeight)
				{
					zero.y = 0f;
					zero.x += columnWidth + -1f;
				}
				Rect rect2 = new Rect(zero.x, zero.y, columnWidth, requiredHeight);
				zero.y += requiredHeight + -1f;
				bool flag = floatMenuOption.DoGUI(rect2, this.givesColonistOrders, this);
				if (flag)
				{
					Find.WindowStack.TryRemove(this, true);
					break;
				}
			}
			if (usingScrollbar)
			{
				Widgets.EndScrollView();
			}
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				this.Close(true);
			}
			GUI.color = Color.white;
		}

		public override void PostClose()
		{
			base.PostClose();
			if (this.onCloseCallback != null)
			{
				this.onCloseCallback();
			}
		}

		public void Cancel()
		{
			SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera(null);
			Find.WindowStack.TryRemove(this, true);
		}

		public virtual void PreOptionChosen(FloatMenuOption opt)
		{
		}

		private void UpdateBaseColor()
		{
			this.baseColor = Color.white;
			if (this.vanishIfMouseDistant)
			{
				Rect r = new Rect(0f, 0f, this.TotalWidth, this.TotalWindowHeight).ContractedBy(-5f);
				if (!r.Contains(Event.current.mousePosition))
				{
					float num = GenUI.DistFromRect(r, Event.current.mousePosition);
					this.baseColor = new Color(1f, 1f, 1f, 1f - num / 95f);
					if (num > 95f)
					{
						this.Close(false);
						this.Cancel();
						return;
					}
				}
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FloatMenu()
		{
		}

		[CompilerGenerated]
		private static MenuOptionPriority <FloatMenu>m__0(FloatMenuOption op)
		{
			return op.Priority;
		}

		[CompilerGenerated]
		private sealed class <ExtraOnGUI>c__AnonStorey0
		{
			internal Rect titleRect;

			internal FloatMenu $this;

			public <ExtraOnGUI>c__AnonStorey0()
			{
			}

			internal void <>m__0()
			{
				GUI.color = this.$this.baseColor;
				Text.Font = GameFont.Small;
				Rect position = this.titleRect.AtZero();
				position.width = 150f;
				GUI.DrawTexture(position, TexUI.TextBGBlack);
				Rect rect = this.titleRect.AtZero();
				rect.x += 15f;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect, this.$this.title);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}
	}
}
