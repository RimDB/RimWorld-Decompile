﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000B4C RID: 2892
	public class KeyBindingDef : Def
	{
		// Token: 0x040029D4 RID: 10708
		public KeyBindingCategoryDef category;

		// Token: 0x040029D5 RID: 10709
		public KeyCode defaultKeyCodeA;

		// Token: 0x040029D6 RID: 10710
		public KeyCode defaultKeyCodeB;

		// Token: 0x040029D7 RID: 10711
		public bool devModeOnly = false;

		// Token: 0x040029D8 RID: 10712
		[NoTranslate]
		public List<string> extraConflictTags;

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06003F5A RID: 16218 RVA: 0x00216578 File Offset: 0x00214978
		public KeyCode MainKey
		{
			get
			{
				KeyBindingData keyBindingData;
				if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out keyBindingData))
				{
					if (keyBindingData.keyBindingA != KeyCode.None)
					{
						return keyBindingData.keyBindingA;
					}
					if (keyBindingData.keyBindingB != KeyCode.None)
					{
						return keyBindingData.keyBindingB;
					}
				}
				return KeyCode.None;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06003F5B RID: 16219 RVA: 0x002165D8 File Offset: 0x002149D8
		public string MainKeyLabel
		{
			get
			{
				return this.MainKey.ToStringReadable();
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06003F5C RID: 16220 RVA: 0x002165F8 File Offset: 0x002149F8
		public bool KeyDownEvent
		{
			get
			{
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
				{
					KeyBindingData keyBindingData;
					if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out keyBindingData))
					{
						return (keyBindingData.keyBindingA == KeyCode.LeftCommand || keyBindingData.keyBindingA == KeyCode.RightCommand || keyBindingData.keyBindingB == KeyCode.LeftCommand || keyBindingData.keyBindingB == KeyCode.RightCommand || !Event.current.command) && (Event.current.keyCode == keyBindingData.keyBindingA || Event.current.keyCode == keyBindingData.keyBindingB);
					}
				}
				return false;
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06003F5D RID: 16221 RVA: 0x002166D0 File Offset: 0x00214AD0
		public bool IsDownEvent
		{
			get
			{
				KeyBindingData keyBindingData;
				return Event.current != null && KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out keyBindingData) && (this.KeyDownEvent || (Event.current.shift && (keyBindingData.keyBindingA == KeyCode.LeftShift || keyBindingData.keyBindingA == KeyCode.RightShift || keyBindingData.keyBindingB == KeyCode.LeftShift || keyBindingData.keyBindingB == KeyCode.RightShift)) || (Event.current.control && (keyBindingData.keyBindingA == KeyCode.LeftControl || keyBindingData.keyBindingA == KeyCode.RightControl || keyBindingData.keyBindingB == KeyCode.LeftControl || keyBindingData.keyBindingB == KeyCode.RightControl)) || (Event.current.alt && (keyBindingData.keyBindingA == KeyCode.LeftAlt || keyBindingData.keyBindingA == KeyCode.RightAlt || keyBindingData.keyBindingB == KeyCode.LeftAlt || keyBindingData.keyBindingB == KeyCode.RightAlt)) || (Event.current.command && (keyBindingData.keyBindingA == KeyCode.LeftCommand || keyBindingData.keyBindingA == KeyCode.RightCommand || keyBindingData.keyBindingB == KeyCode.LeftCommand || keyBindingData.keyBindingB == KeyCode.RightCommand)) || this.IsDown);
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06003F5E RID: 16222 RVA: 0x00216888 File Offset: 0x00214C88
		public bool JustPressed
		{
			get
			{
				KeyBindingData keyBindingData;
				return KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out keyBindingData) && (Input.GetKeyDown(keyBindingData.keyBindingA) || Input.GetKeyDown(keyBindingData.keyBindingB));
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06003F5F RID: 16223 RVA: 0x002168DC File Offset: 0x00214CDC
		public bool IsDown
		{
			get
			{
				KeyBindingData keyBindingData;
				return KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out keyBindingData) && (Input.GetKey(keyBindingData.keyBindingA) || Input.GetKey(keyBindingData.keyBindingB));
			}
		}

		// Token: 0x06003F60 RID: 16224 RVA: 0x00216930 File Offset: 0x00214D30
		public KeyCode GetDefaultKeyCode(KeyPrefs.BindingSlot slot)
		{
			KeyCode result;
			if (slot == KeyPrefs.BindingSlot.A)
			{
				result = this.defaultKeyCodeA;
			}
			else
			{
				if (slot != KeyPrefs.BindingSlot.B)
				{
					throw new InvalidOperationException();
				}
				result = this.defaultKeyCodeB;
			}
			return result;
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x0021696C File Offset: 0x00214D6C
		public static KeyBindingDef Named(string name)
		{
			return DefDatabase<KeyBindingDef>.GetNamedSilentFail(name);
		}
	}
}
