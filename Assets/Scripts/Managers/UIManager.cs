using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : NetworkBehaviour
{
	public Slider healthBar;
	public Slider manaBar;
	public UnityEngine.UI.Text questLog;
	public InventorySystem inventorySystem;
	public CharacterStats playerStats;

	void Start()
	{
		if (Object.HasInputAuthority)
		{
			playerStats = FindObjectOfType<CharacterStats>();
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (Object.HasInputAuthority)
		{
			UpdateHealthBar();
			UpdateManaBar();
		}
	}

	void UpdateHealthBar()
	{
		healthBar.maxValue = playerStats.maxHealth;
		healthBar.value = playerStats.currentHealth;
	}

	void UpdateManaBar()
	{
		manaBar.maxValue = playerStats.maxMana;
		manaBar.value = playerStats.currentMana;
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_UpdateQuestLog(string newQuest)
	{
		questLog.text += "\n" + newQuest;
	}

	public void UpdateInventoryUI()
	{
		foreach (Item item in inventorySystem.inventory)
		{
			UnityEngine.Debug.Log("Inventory Item: " + item.itemName);
		}
	}
}
