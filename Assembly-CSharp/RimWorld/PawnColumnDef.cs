﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class PawnColumnDef : Def
	{
		public Type workerClass = typeof(PawnColumnWorker);

		public bool sortable;

		public bool ignoreWhenCalculatingOptimalTableSize;

		[NoTranslate]
		public string headerIcon;

		public Vector2 headerIconSize;

		[MustTranslate]
		public string headerTip;

		public bool headerAlwaysInteractable;

		public bool paintable;

		public TrainableDef trainable;

		public int gap;

		public WorkTypeDef workType;

		public bool moveWorkTypeLabelDown;

		public int widthPriority;

		[Unsaved]
		private PawnColumnWorker workerInt;

		[Unsaved]
		private Texture2D headerIconTex;

		public PawnColumnDef()
		{
		}

		public PawnColumnWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (PawnColumnWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		public Texture2D HeaderIcon
		{
			get
			{
				if (this.headerIconTex == null && !this.headerIcon.NullOrEmpty())
				{
					this.headerIconTex = ContentFinder<Texture2D>.Get(this.headerIcon, true);
				}
				return this.headerIconTex;
			}
		}

		public Vector2 HeaderIconSize
		{
			get
			{
				Vector2 result;
				if (this.headerIconSize != default(Vector2))
				{
					result = this.headerIconSize;
				}
				else
				{
					Texture2D texture2D = this.HeaderIcon;
					if (texture2D != null)
					{
						result = new Vector2((float)texture2D.width, (float)texture2D.height);
					}
					else
					{
						result = Vector2.zero;
					}
				}
				return result;
			}
		}

		public bool HeaderInteractable
		{
			get
			{
				return this.sortable || !this.headerTip.NullOrEmpty() || this.headerAlwaysInteractable;
			}
		}
	}
}
