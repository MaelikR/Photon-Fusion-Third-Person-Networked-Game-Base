using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MarketItem
{
	public string itemName;
	public int price;
	public int quantity;

	public MarketItem(string name, int price, int quantity)
	{
		itemName = name;
		this.price = price;
		this.quantity = quantity;
	}
}

public class MarketSystem : NetworkBehaviour
{
	public List<MarketItem> marketItems { get; set; } = new List<MarketItem>();

	// Ajouter un objet à vendre sur le marché
	public void AddItemToMarket(MarketItem newItem)
	{
		if (Object.HasStateAuthority)
		{
			marketItems.Add(newItem);
			RPC_UpdateMarket();
		}
	}

	// Acheter un objet du marché
	public void BuyItemFromMarket(string itemName, int quantity, int playerMoney)
	{
		if (Object.HasStateAuthority)
		{
			MarketItem item = marketItems.Find(i => i.itemName == itemName && i.quantity >= quantity);
			if (item != null && playerMoney >= item.price * quantity)
			{
				// Transaction réussie
				item.quantity -= quantity;
				RPC_UpdateMarket();
				UnityEngine.Debug.Log("Transaction réussie : " + quantity + " " + itemName);
			}
			else
			{
                UnityEngine.Debug.Log("Transaction échouée : item ou fonds insuffisants.");
			}
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_UpdateMarket()
	{
		foreach (MarketItem item in marketItems)
		{
			UnityEngine.Debug.Log("Market Updated: " + item.itemName + " - " + item.quantity + " items available at " + item.price + " each.");
		}
	}
}
