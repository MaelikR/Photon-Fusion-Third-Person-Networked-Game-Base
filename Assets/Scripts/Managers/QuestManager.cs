using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Quest
{
	public string questName;
	public string questDescription;
	public bool isCompleted;
	public int currentProgress;
	public int goal;
}

public class QuestManager : NetworkBehaviour
{
	public List<Quest> quests { get; set; } = new List<Quest>();

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

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_UpdateQuestLog(string questUpdate)
	{
		UnityEngine.Debug.Log("Quest Log Updated: " + questUpdate);
	}
}
