using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CraftingRecipe
{
	public string itemName;
	public Dictionary<string, int> requiredMaterials;

	public CraftingRecipe(string name, Dictionary<string, int> materials)
	{
		itemName = name;
		requiredMaterials = materials;
	}
}

public class CraftingSystem : NetworkBehaviour
{
	public List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();

	public void AddRecipe(CraftingRecipe recipe)
	{
		if (Object.HasStateAuthority)
		{
			availableRecipes.Add(recipe);
		}
	}

	public void CraftItem(string recipeName, InventorySystem inventory)
	{
		if (Object.HasStateAuthority)
		{
			CraftingRecipe recipe = availableRecipes.Find(r => r.itemName == recipeName);

			if (recipe != null && HasRequiredMaterials(recipe, inventory))
			{
				// Enlève les matériaux requis et ajoute l'item à l'inventaire
				foreach (var material in recipe.requiredMaterials)
				{
					inventory.RemoveItem(new Item { itemName = material.Key, quantity = material.Value });
				}
				inventory.AddItem(new Item { itemName = recipe.itemName, quantity = 1 });
				RPC_NotifyCraftingSuccess(recipe.itemName);
			}
			else
			{
				UnityEngine.Debug.Log("Matériaux insuffisants pour crafter " + recipeName);
			}
		}
	}

    bool HasRequiredMaterials(CraftingRecipe recipe, InventorySystem inventory)
    {
        foreach (var material in recipe.requiredMaterials)
        {
            Item item = Array.Find(inventory.inventory, i => i != null && i.itemName == material.Key && i.quantity >= material.Value);
            if (item == null) return false;
        }
        return true;
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyCraftingSuccess(string itemName)
	{
		UnityEngine.Debug.Log("Item crafted successfully: " + itemName);
	}
}
