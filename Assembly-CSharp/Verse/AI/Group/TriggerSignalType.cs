﻿using System;

namespace Verse.AI.Group
{
	public enum TriggerSignalType : byte
	{
		Undefined,
		Tick,
		Memo,
		PawnDamaged,
		PawnArrestAttempted,
		PawnLost,
		BuildingDamaged,
		FactionRelationsChanged
	}
}
