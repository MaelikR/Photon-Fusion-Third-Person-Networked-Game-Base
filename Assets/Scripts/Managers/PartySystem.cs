using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PartySystem : NetworkBehaviour
{
	public List<string> partyMembers { get; set; } = new List<string>();
	public int maxPartySize = 5;

	public void AddPlayerToParty(string playerName)
	{
		if (Object.HasStateAuthority && partyMembers.Count < maxPartySize)
		{
			partyMembers.Add(playerName);
			RPC_NotifyPartyUpdate(playerName + " has joined the party.");
		}
	}

	public void RemovePlayerFromParty(string playerName)
	{
		if (Object.HasStateAuthority && partyMembers.Contains(playerName))
		{
			partyMembers.Remove(playerName);
			RPC_NotifyPartyUpdate(playerName + " has left the party.");
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyPartyUpdate(string message)
	{
		UnityEngine.Debug.Log(message);
	}
}
