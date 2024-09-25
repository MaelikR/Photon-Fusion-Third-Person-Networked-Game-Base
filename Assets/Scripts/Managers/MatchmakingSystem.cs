using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MatchmakingSystem : NetworkBehaviour
{
	private List<GameObject> queuedPlayers = new List<GameObject>();
	public int playersNeededForRaid = 5;

	public void QueueForRaid(GameObject player)
	{
		if (Object.HasStateAuthority)
		{
			queuedPlayers.Add(player);
			UnityEngine.Debug.Log(player.name + " is queued for raid.");

			if (queuedPlayers.Count >= playersNeededForRaid)
			{
				StartRaid();
			}
		}
	}

	private void StartRaid()
	{
		UnityEngine.Debug.Log("Raid started with " + queuedPlayers.Count + " players!");
		foreach (var player in queuedPlayers)
		{
			// Commencer le raid avec les joueurs groupés
			UnityEngine.Debug.Log(player.name + " is participating in the raid.");
		}

		queuedPlayers.Clear();
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyRaidMatch()
	{
		UnityEngine.Debug.Log("Matchmaking successful. Raid starting.");
	}
}
