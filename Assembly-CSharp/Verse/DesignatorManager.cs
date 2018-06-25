﻿using System;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	public class DesignatorManager
	{
		private Designator selectedDesignator;

		private DesignationDragger dragger = new DesignationDragger();

		public DesignatorManager()
		{
		}

		public Designator SelectedDesignator
		{
			get
			{
				return this.selectedDesignator;
			}
		}

		public DesignationDragger Dragger
		{
			get
			{
				return this.dragger;
			}
		}

		public void Select(Designator des)
		{
			this.Deselect();
			this.selectedDesignator = des;
			this.selectedDesignator.Selected();
		}

		public void Deselect()
		{
			if (this.selectedDesignator != null)
			{
				this.selectedDesignator = null;
				this.dragger.EndDrag();
			}
		}

		private bool CheckSelectedDesignatorValid()
		{
			bool result;
			if (this.selectedDesignator == null)
			{
				result = false;
			}
			else if (!this.selectedDesignator.CanRemainSelected())
			{
				this.Deselect();
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void ProcessInputEvents()
		{
			if (this.CheckSelectedDesignatorValid())
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					if (this.selectedDesignator.DraggableDimensions == 0)
					{
						Designator designator = this.selectedDesignator;
						AcceptanceReport acceptanceReport = this.selectedDesignator.CanDesignateCell(UI.MouseCell());
						if (acceptanceReport.Accepted)
						{
							designator.DesignateSingleCell(UI.MouseCell());
							designator.Finalize(true);
						}
						else
						{
							Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.SilentInput, false);
							this.selectedDesignator.Finalize(false);
						}
					}
					else
					{
						this.dragger.StartDrag();
					}
					Event.current.Use();
				}
				if ((Event.current.type == EventType.MouseDown && Event.current.button == 1) || KeyBindingDefOf.Cancel.KeyDownEvent)
				{
					SoundDefOf.CancelMode.PlayOneShotOnCamera(null);
					this.Deselect();
					this.dragger.EndDrag();
					Event.current.Use();
					TutorSystem.Notify_Event("ClearDesignatorSelection");
				}
				if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
				{
					if (this.dragger.Dragging)
					{
						this.selectedDesignator.DesignateMultiCell(this.dragger.DragCells);
						this.dragger.EndDrag();
						Event.current.Use();
					}
				}
			}
		}

		public void DesignationManagerOnGUI()
		{
			this.dragger.DraggerOnGUI();
			if (this.CheckSelectedDesignatorValid())
			{
				this.selectedDesignator.DrawMouseAttachments();
			}
		}

		public void DesignatorManagerUpdate()
		{
			this.dragger.DraggerUpdate();
			if (this.CheckSelectedDesignatorValid())
			{
				this.selectedDesignator.SelectedUpdate();
			}
		}
	}
}
