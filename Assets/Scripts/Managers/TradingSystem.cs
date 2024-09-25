using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TradingSystem : NetworkBehaviour
{
	private Dictionary<string, Item> playerOneItems = new Dictionary<string, Item>();
	private Dictionary<string, Item> playerTwoItems = new Dictionary<string, Item>();

	public void OfferItemForTrade(string playerName, Item item)
	{
		if (Object.HasStateAuthority)
		{
			if (playerName == "PlayerOne")
			{
				playerOneItems.Add(item.itemName, item);
			}
			else if (playerName == "PlayerTwo")
			{
				playerTwoItems.Add(item.itemName, item);
			}
			RPC_UpdateTradeStatus(playerName, item.itemName);
		}
	}

	public void AcceptTrade()
	{
		if (Object.HasStateAuthority)
		{
			// Effectuer l'échange
			foreach (var item in playerOneItems)
			{
				UnityEngine.Debug.Log("PlayerTwo reçoit " + item.Key);
			}

			foreach (var item in playerTwoItems)
			{
				UnityEngine.Debug.Log("PlayerOne reçoit " + item.Key);
			}

			// Réinitialiser les échanges après la transaction
			playerOneItems.Clear();
			playerTwoItems.Clear();
			RPC_ConfirmTrade();
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_UpdateTradeStatus(string playerName, string itemName)
	{
		UnityEngine.Debug.Log(playerName + " a offert " + itemName + " pour échange.");
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_ConfirmTrade()
	{
		UnityEngine.Debug.Log("Échange confirmé.");
	}
}
