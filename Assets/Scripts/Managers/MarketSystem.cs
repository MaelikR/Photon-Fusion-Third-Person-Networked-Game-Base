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

	// Ajouter un objet � vendre sur le march�
	public void AddItemToMarket(MarketItem newItem)
	{
		if (Object.HasStateAuthority)
		{
			marketItems.Add(newItem);
			RPC_UpdateMarket();
		}
	}

	// Acheter un objet du march�
	public void BuyItemFromMarket(string itemName, int quantity, int playerMoney)
	{
		if (Object.HasStateAuthority)
		{
			MarketItem item = marketItems.Find(i => i.itemName == itemName && i.quantity >= quantity);
			if (item != null && playerMoney >= item.price * quantity)
			{
				// Transaction r�ussie
				item.quantity -= quantity;
				RPC_UpdateMarket();
				UnityEngine.Debug.Log("Transaction r�ussie : " + quantity + " " + itemName);
			}
			else
			{
                UnityEngine.Debug.Log("Transaction �chou�e : item ou fonds insuffisants.");
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
