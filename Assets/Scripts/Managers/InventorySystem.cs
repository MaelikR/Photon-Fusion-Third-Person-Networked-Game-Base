using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : NetworkBehaviour
{
    public Item[] inventory; // Utilise un tableau au lieu d'une liste

    void Start()
    {
        inventory = new Item[20]; // Taille définie, ajuste selon tes besoins
    }

    public void AddItem(Item newItem)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = newItem;
                RPC_NotifyItemAdded(newItem.itemName);
                break;
            }
        }
    }
    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].itemName == itemToRemove.itemName)
            {
                inventory[i].quantity -= itemToRemove.quantity;
                if (inventory[i].quantity <= 0)
                {
                    inventory[i] = null; // Supprime l'item s'il n'en reste plus
                }
                break;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NotifyItemAdded(string itemName)
    {
        Debug.Log("Item ajouté : " + itemName);
    }
}

[System.Serializable]
public class Item
{
    public string itemName;
    public int quantity;
}
