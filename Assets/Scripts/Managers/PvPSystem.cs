using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class PvPSystem : NetworkBehaviour
{
	public bool isInPvPZone;

	public void EnterPvPZone()
	{
		if (Object.HasInputAuthority)
		{
			isInPvPZone = true;
			RPC_NotifyPvPStatus("You have entered a PvP zone.");
		}
	}

	public void ExitPvPZone()
	{
		if (Object.HasInputAuthority)
		{
			isInPvPZone = false;
			RPC_NotifyPvPStatus("You have left the PvP zone.");
		}
	}

	public void AttackPlayer(GameObject targetPlayer, int damage)
	{
		if (isInPvPZone && Object.HasInputAuthority)
		{
			targetPlayer.GetComponent<CharacterStats>().TakeDamage(damage);
			UnityEngine.Debug.Log("Player attacked with " + damage + " damage.");
		}
		else
		{
			UnityEngine.Debug.Log("Not in PvP zone.");
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public void RPC_NotifyPvPStatus(string message)
	{
		UnityEngine.Debug.Log(message);
	}
}
