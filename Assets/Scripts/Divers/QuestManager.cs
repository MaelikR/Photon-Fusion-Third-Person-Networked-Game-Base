using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
    public bool isCompleted;
    public int currentProgress;
    public int goal;
}
namespace Unity.Cinemachine
{
    public class QuestManager : NetworkBehaviour
    {
        [SerializeField] // Permet d'afficher la liste des quêtes dans l'inspecteur Unity
        private List<Quest> quests = new List<Quest>();
        public List<Quest> Quests
        {
            get => quests;
            set => quests = value;
        }

        public void AddQuest(Quest newQuest)
        {
            if (Object.HasStateAuthority)
            {
                quests.Add(newQuest);
                RPC_UpdateQuestLog(newQuest.questName);
            }
        }


        public void UpdateQuestProgress(Quest quest, int progress)
        {
            if (Object.HasStateAuthority && quests.Contains(quest))
            {
                quest.currentProgress += progress;
                if (quest.currentProgress >= quest.goal)
                {
                    quest.isCompleted = true;
                    RPC_UpdateQuestLog(quest.questName + " completed");
                }
                else
                {
                    RPC_UpdateQuestLog(quest.questName + " progress: " + quest.currentProgress + "/" + quest.goal);
                }
            }
        }

        private UIManager uiManager;

        private void Start()
        {
            UIManager foundManager = FindFirstObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager not found. Make sure UIManager is present in the scene.");
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_UpdateQuestLog(string questUpdate)
        {
            Debug.Log("Quest Log Updated: " + questUpdate);

            if (uiManager != null)
            {
                uiManager.RPC_UpdateQuestLog(questUpdate);
            }
        }

    }
}