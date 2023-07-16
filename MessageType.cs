﻿namespace SpiritMod
{
	public enum MessageType : byte
	{
		None = 0,
		AuroraData,
		ProjectileData,
		PlaceMapPin,
		Dodge,
		Dash,
		PlayerGlyph,
		BossSpawnFromClient,
		SpawnNPCFromClient,
		StartTide,
		TideData,
		TameAuroraStag,
		SpawnTrail,
		PlaceSuperSunFlower,
		DestroySuperSunFlower,
		SpawnExplosiveBarrel,
		SpawnStardustBomb,
		StarjinxData,
		BoonData,
		FathomlessData,
		PlayDeathSoundFromServer,
		RequestQuestManager,
		RecieveQuestManager,
		Quest,
		Sports,
	}

	public enum QuestMessageType : byte
	{
		Deactivate = 0,
		Activate,
		ProgressOrComplete,
		SyncOnNPCLoot,
		SyncOnEditSpawnPool,
		ObtainQuestBook,
		Unlock,
		SyncNPCQueue
	}

	public enum SportMessageType : byte
	{
		PlaceCourt = 0,
	}
}
