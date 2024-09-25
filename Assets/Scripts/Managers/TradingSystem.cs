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
			// Effectuer l'�change
			foreach (var item in playerOneItems)
			{
				UnityEngine.Debug.Log("PlayerTwo re�oit " + item.Key);
			}

			foreach (var item in playerTwoItems)
			{
				UnityEngine.Debug.Log("PlayerOne re�oit " + item.Key);
			}

			// R�initialiser les �changes apr�s la transaction
			playerOneItems.Clear();
			playerTwoItems.Clear();
			RPC_ConfirmTrade();
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_UpdateTradeStatus(string playerName, string itemName)
	{
		UnityEngine.Debug.Log(playerName + " a offert " + itemName + " pour �change.");
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_ConfirmTrade()
	{
		UnityEngine.Debug.Log("�change confirm�.");
	}
}
