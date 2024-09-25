using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ReputationSystem : NetworkBehaviour
{
	public Dictionary<string, int> factionReputation { get; set; } = new Dictionary<string, int>();

	public void ModifyReputation(string factionName, int reputationChange)
	{
		if (Object.HasStateAuthority)
		{
			if (!factionReputation.ContainsKey(factionName))
			{
				factionReputation[factionName] = 0;
			}

			factionReputation[factionName] += reputationChange;
			RPC_NotifyReputationChange(factionName, factionReputation[factionName]);
		}
	}

	public void CheckReputation(string factionName)
	{
		if (Object.HasInputAuthority && factionReputation.ContainsKey(factionName))
		{
			UnityEngine.Debug.Log("Reputation with " + factionName + ": " + factionReputation[factionName]);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyReputationChange(string factionName, int newReputation)
	{
		UnityEngine.Debug.Log("Reputation with " + factionName + " is now: " + newReputation);
	}
}
