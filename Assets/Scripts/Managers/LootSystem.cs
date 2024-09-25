using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LootSystem : NetworkBehaviour
{
	public List<Loot> possibleLoot = new List<Loot>();

	public void DropLoot()
	{
		if (Object.HasStateAuthority)
		{
			foreach (Loot loot in possibleLoot)
			{
				int randomValue = UnityEngine.Random.Range(1, 101);
				if (randomValue <= loot.dropChance)
				{
                    UnityEngine.Debug.Log(loot.itemName + " dropped!");
					RPC_NotifyLootDrop(loot.itemName);
				}
			}
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyLootDrop(string itemName)
	{
		UnityEngine.Debug.Log("Loot dropped: " + itemName);
	}
}

[System.Serializable]
public class Loot
{
	public string itemName;
	public int dropChance;
}
