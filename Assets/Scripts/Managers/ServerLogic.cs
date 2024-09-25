using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class ServerLogic : NetworkBehaviour
{
	public void ValidatePlayerAction(string actionType, GameObject player)
	{
		if (Object.HasStateAuthority)
		{
			if (actionType == "Attack")
			{
				UnityEngine.Debug.Log("Validating attack for " + player.name);
				// Logique de validation d'attaque ici
			}
			else if (actionType == "Move")
			{
				UnityEngine.Debug.Log("Validating movement for " + player.name);
				// Logique de validation de mouvement ici
			}
		}
	}
}
