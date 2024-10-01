using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

namespace Fusion
{
    public class UIManager : NetworkBehaviour
    {
        public Slider healthBar;
        public Slider manaBar;
        public UnityEngine.UI.Text questLog;
        public InventorySystem inventorySystem;
        public CharacterStats playerStats;
        private ThirdPersonController playerInstance;
        void Start()
        {
            if (Object.HasInputAuthority)
            {
                CharacterStats foundManager = FindFirstObjectByType<CharacterStats>();
            }

            // Assurez-vous que questLog est assigné
            if (questLog == null)
            {
                questLog = GameObject.Find("QuestLogText").GetComponent<UnityEngine.UI.Text>();
            }
        }

        public void SetPlayerInstance(ThirdPersonController player)
        {
            playerInstance = player;
        }

        public override void FixedUpdateNetwork()
        {
            if (playerInstance != null && Object.HasInputAuthority)
            {
                UpdateHealthBar();
                UpdateManaBar();
            }
        }


        void UpdateHealthBar()
        {
            if (playerInstance != null)
            {
                healthBar.maxValue = playerInstance.GetMaxHealth();
                healthBar.value = playerInstance.GetCurrentHealth();
            }
        }

        void UpdateManaBar()
        {
            if (playerInstance != null)
            {
                manaBar.maxValue = playerInstance.GetMaxMana();
                manaBar.value = playerInstance.GetCurrentMana();
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_UpdateQuestLog(string newQuest)
        {
            if (Object.HasStateAuthority || Object.HasInputAuthority)
            {
                UnityEngine.Debug.Log("Updating quest log with: " + newQuest);
                questLog.text += "\n" + newQuest;
            }
        }


        public void UpdateInventoryUI()
        {
            foreach (Item item in inventorySystem.inventory)
            {
                UnityEngine.Debug.Log("Inventory Item: " + item.itemName);
            }
        }
    }
}