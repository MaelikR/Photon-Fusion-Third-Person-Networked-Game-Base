using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TalentSystem : NetworkBehaviour
{
	public Dictionary<string, int> talentPoints { get; set; } = new Dictionary<string, int>();

	public void UnlockTalent(string talentName)
	{
		if (Object.HasStateAuthority && talentPoints.ContainsKey(talentName))
		{
			if (talentPoints[talentName] > 0)
			{
				talentPoints[talentName]--;
				RPC_NotifyTalentUnlock(talentName);
			}
		}
	}

	public void AddTalentPoints(string talentName, int points)
	{
		if (Object.HasStateAuthority)
		{
			if (!talentPoints.ContainsKey(talentName))
			{
				talentPoints[talentName] = 0;
			}

			talentPoints[talentName] += points;
			RPC_NotifyTalentPoints(talentName, talentPoints[talentName]);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyTalentUnlock(string talentName)
	{
		UnityEngine.Debug.Log("Talent unlocked: " + talentName);
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyTalentPoints(string talentName, int points)
	{
		UnityEngine.Debug.Log("Talent points for " + talentName + ": " + points);
	}
}
