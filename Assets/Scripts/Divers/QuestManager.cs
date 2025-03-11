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

    public Quest(string name, string description, int goal)
    {
        questName = name;
        questDescription = description;
        this.goal = goal;
        currentProgress = 0;
        isCompleted = false;
    }
}

public class QuestManager : NetworkBehaviour
{
    [Header("Quests")]
    [SerializeField] private List<Quest> allQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();

    [Header("UI Manager")]
    public UIManager uiManager;

    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager not found in the scene. Please assign it in the inspector.");
                return;
            }
        }
      
      
    }
    public override void Spawned()
    {
        base.Spawned();

        if (Object == null || !Object.IsValid)
        {
            Debug.LogError("NetworkObject not valid in QuestManager.");
            return;
        }

        Debug.Log("QuestManager initialized and ready.");
        InitializeQuests();
    }

    private Dictionary<string, Quest> questDictionary = new Dictionary<string, Quest>();

    private void InitializeQuests()
    {
        allQuests.Add(new Quest("Collect Wood", "Collect 10 pieces of wood.", 10));
        allQuests.Add(new Quest("Build Shelter", "Build a small shelter.", 1));
        allQuests.Add(new Quest("Light Fire", "Light a fire at your shelter.", 1));

        foreach (Quest quest in allQuests)
        {
            questDictionary[quest.questName] = quest;

            // Appel correct de la méthode RPC dans UIManager
            if (uiManager != null)
            {
                uiManager.RPC_UpdateQuestLog($"Quest Added: {quest.questName}");
            }
            else
            {
                Debug.LogError("UIManager is not assigned.");
            }
        }
    }


    public void AddQuest(string name, string description, int goal)
    {
        if (IsStateAuthority() && !questDictionary.ContainsKey(name))
        {
            Quest newQuest = new Quest(name, description, goal);
            allQuests.Add(newQuest);
            questDictionary[name] = newQuest;
            uiManager.RPC_UpdateQuestLog($"New Quest Added: {name}");
        }
        else
        {
            Debug.LogWarning($"Quest {name} already exists!");
        }
    }

    public void UpdateQuestProgress(string questName, int progress)
    {
        if (questDictionary.TryGetValue(questName, out Quest quest))
        {
            if (!quest.isCompleted)
            {
                quest.currentProgress += progress;
                if (quest.currentProgress >= quest.goal)
                {
                    quest.isCompleted = true;
                    completedQuests.Add(quest);
                    uiManager?.RPC_UpdateQuestLog($"Quest Completed: {quest.questName}");
                    uiManager?.ShowNotification($"Quest Completed: {quest.questName}");
                }
                else
                {
                    uiManager?.RPC_UpdateQuestLog($"Quest Progress: {quest.questName} ({quest.currentProgress}/{quest.goal})");
                    uiManager?.UpdateProgressBar((float)quest.currentProgress / quest.goal);
                }
            }
        }
    }


    public void CompleteQuest(string questName)
    {
        if (Object.HasStateAuthority)
        {
            Quest quest = allQuests.Find(q => q.questName == questName);
            if (quest != null && !quest.isCompleted)
            {
                quest.isCompleted = true;
                completedQuests.Add(quest);
                uiManager.RPC_UpdateQuestLog($"Quest Completed: {quest.questName}");
            }
        }
    }

    public List<Quest> GetAllQuests()
    {
        return allQuests;
    }

    public List<Quest> GetCompletedQuests()
    {
        return completedQuests;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddQuestToLog(string questName)
    {
        uiManager.RPC_UpdateQuestLog($"New Quest: {questName}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateQuestProgress(string questName, int currentProgress, int goal)
    {
        uiManager.RPC_UpdateQuestLog($"Quest Progress: {questName} ({currentProgress}/{goal})");
    }
    private bool IsStateAuthority()
    {
        return Object.HasStateAuthority;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CompleteQuest(string questName)
    {
        uiManager.RPC_UpdateQuestLog($"Quest Completed: {questName}");
    }
}
