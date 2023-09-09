using System;

namespace ExitGames.Client.Photon
{
	public enum EventCaching : byte
	{
		DoNotCache,
		[Obsolete]
		MergeCache,
		[Obsolete]
		ReplaceCache,
		[Obsolete]
		RemoveCache,
		AddToRoomCache,
		AddToRoomCacheGlobal,
		RemoveFromRoomCache,
		RemoveFromRoomCacheForActorsLeft,
		SliceIncreaseIndex = 10,
		SliceSetIndex,
		SlicePurgeIndex,
		SlicePurgeUpToIndex
	}
}
