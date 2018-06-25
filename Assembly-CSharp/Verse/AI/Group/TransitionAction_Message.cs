﻿using System;
using System.Linq;
using RimWorld;

namespace Verse.AI.Group
{
	// Token: 0x02000A00 RID: 2560
	public class TransitionAction_Message : TransitionAction
	{
		// Token: 0x0400248A RID: 9354
		public string message;

		// Token: 0x0400248B RID: 9355
		public MessageTypeDef type;

		// Token: 0x0400248C RID: 9356
		public TargetInfo lookTarget;

		// Token: 0x0400248D RID: 9357
		public Func<TargetInfo> lookTargetGetter;

		// Token: 0x0400248E RID: 9358
		public string repeatAvoiderTag;

		// Token: 0x0400248F RID: 9359
		public float repeatAvoiderSeconds;

		// Token: 0x06003974 RID: 14708 RVA: 0x001E79BB File Offset: 0x001E5DBB
		public TransitionAction_Message(string message, string repeatAvoiderTag = null, float repeatAvoiderSeconds = 1f) : this(message, MessageTypeDefOf.NeutralEvent, repeatAvoiderTag, repeatAvoiderSeconds)
		{
		}

		// Token: 0x06003975 RID: 14709 RVA: 0x001E79CC File Offset: 0x001E5DCC
		public TransitionAction_Message(string message, MessageTypeDef messageType, string repeatAvoiderTag = null, float repeatAvoiderSeconds = 1f)
		{
			this.lookTarget = TargetInfo.Invalid;
			base..ctor();
			this.message = message;
			this.type = messageType;
			this.repeatAvoiderTag = repeatAvoiderTag;
			this.repeatAvoiderSeconds = repeatAvoiderSeconds;
		}

		// Token: 0x06003976 RID: 14710 RVA: 0x001E79FD File Offset: 0x001E5DFD
		public TransitionAction_Message(string message, MessageTypeDef messageType, TargetInfo lookTarget, string repeatAvoiderTag = null, float repeatAvoiderSeconds = 1f)
		{
			this.lookTarget = TargetInfo.Invalid;
			base..ctor();
			this.message = message;
			this.type = messageType;
			this.lookTarget = lookTarget;
			this.repeatAvoiderTag = repeatAvoiderTag;
			this.repeatAvoiderSeconds = repeatAvoiderSeconds;
		}

		// Token: 0x06003977 RID: 14711 RVA: 0x001E7A36 File Offset: 0x001E5E36
		public TransitionAction_Message(string message, MessageTypeDef messageType, Func<TargetInfo> lookTargetGetter, string repeatAvoiderTag = null, float repeatAvoiderSeconds = 1f)
		{
			this.lookTarget = TargetInfo.Invalid;
			base..ctor();
			this.message = message;
			this.type = messageType;
			this.lookTargetGetter = lookTargetGetter;
			this.repeatAvoiderTag = repeatAvoiderTag;
			this.repeatAvoiderSeconds = repeatAvoiderSeconds;
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x001E7A70 File Offset: 0x001E5E70
		public override void DoAction(Transition trans)
		{
			if (this.repeatAvoiderTag.NullOrEmpty() || MessagesRepeatAvoider.MessageShowAllowed(this.repeatAvoiderTag, this.repeatAvoiderSeconds))
			{
				TargetInfo target = (this.lookTargetGetter == null) ? this.lookTarget : this.lookTargetGetter();
				if (!target.IsValid)
				{
					target = trans.target.lord.ownedPawns.FirstOrDefault<Pawn>();
				}
				Messages.Message(this.message, target, this.type, true);
			}
		}
	}
}
