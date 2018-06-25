﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	public class Command_Toggle : Command
	{
		public Func<bool> isActive = null;

		public Action toggleAction;

		public SoundDef turnOnSound = SoundDefOf.Checkbox_TurnedOn;

		public SoundDef turnOffSound = SoundDefOf.Checkbox_TurnedOff;

		public Command_Toggle()
		{
		}

		public override SoundDef CurActivateSound
		{
			get
			{
				SoundDef result;
				if (this.isActive())
				{
					result = this.turnOffSound;
				}
				else
				{
					result = this.turnOnSound;
				}
				return result;
			}
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			this.toggleAction();
		}

		public override GizmoResult GizmoOnGUI(Vector2 loc, float maxWidth)
		{
			GizmoResult result = base.GizmoOnGUI(loc, maxWidth);
			Rect rect = new Rect(loc.x, loc.y, this.GetWidth(maxWidth), 75f);
			Rect position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
			Texture2D image = (!this.isActive()) ? Widgets.CheckboxOffTex : Widgets.CheckboxOnTex;
			GUI.DrawTexture(position, image);
			return result;
		}

		public override bool InheritInteractionsFrom(Gizmo other)
		{
			Command_Toggle command_Toggle = other as Command_Toggle;
			return command_Toggle != null && command_Toggle.isActive() == this.isActive();
		}
	}
}
