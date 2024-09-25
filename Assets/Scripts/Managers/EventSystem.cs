using Fusion;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class EventSystem : NetworkBehaviour
{
	[Networked] public string eventName { get; set; }
	[Networked] public bool eventActive { get; set; }

	public void StartEvent(string newEvent)
	{
		if (Object.HasStateAuthority)
		{
			eventName = newEvent;
			eventActive = true;
			UnityEngine.Debug.Log("Event " + eventName + " has started!");
			StartCoroutine(HandleEvent());
		}
	}

	public void EndEvent()
	{
		if (Object.HasStateAuthority)
		{
			eventActive = false;
			UnityEngine.Debug.Log("Event " + eventName + " has ended.");
		}
	}

	IEnumerator HandleEvent()
	{
		yield return new WaitForSeconds(5f);
		UnityEngine.Debug.Log("Handling event logic for " + eventName);
	}
}
