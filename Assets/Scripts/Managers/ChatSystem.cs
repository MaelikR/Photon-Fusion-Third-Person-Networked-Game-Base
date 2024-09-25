using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class ChatSystem : NetworkBehaviour
{
	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public void RPC_SendMessageToGlobal(string playerName, string message)
	{
        UnityEngine.Debug.Log("[Global] " + playerName + ": " + message);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public void RPC_SendMessageToParty(string playerName, string message)
	{
		UnityEngine.Debug.Log("[Party] " + playerName + ": " + message);
	}

	public void SendMessageToGlobal(string playerName, string message)
	{
		if (Object.HasInputAuthority)
		{
			RPC_SendMessageToGlobal(playerName, message);
		}
	}

	public void SendMessageToParty(string playerName, string message)
	{
		if (Object.HasInputAuthority)
		{
			RPC_SendMessageToParty(playerName, message);
		}
	}
}
