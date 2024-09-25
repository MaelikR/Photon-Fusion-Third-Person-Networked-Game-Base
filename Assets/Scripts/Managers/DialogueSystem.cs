using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Dialogue
{
	public string characterName;
	public List<string> dialogueLines;
	public List<string> playerChoices;

	public Dialogue(string name, List<string> lines, List<string> choices)
	{
		characterName = name;
		dialogueLines = lines;
		playerChoices = choices;
	}
}

public class DialogueSystem : NetworkBehaviour
{
	private Queue<string> dialogueQueue = new Queue<string>();
	public bool isTalking = false;
	public string playerResponse;

	public void StartDialogue(Dialogue dialogue)
	{
		if (Object.HasInputAuthority)
		{
			isTalking = true;
			dialogueQueue.Clear();

			foreach (string line in dialogue.dialogueLines)
			{
				dialogueQueue.Enqueue(line);
			}

			DisplayNextLine(dialogue);
		}
	}

	void DisplayNextLine(Dialogue dialogue)
	{
		if (dialogueQueue.Count == 0)
		{
			DisplayPlayerChoices(dialogue);
			return;
		}

		string currentLine = dialogueQueue.Dequeue();
		UnityEngine.Debug.Log(dialogue.characterName + ": " + currentLine);
	}

	void DisplayPlayerChoices(Dialogue dialogue)
	{
		UnityEngine.Debug.Log("Player, make your choice:");
		for (int i = 0; i < dialogue.playerChoices.Count; i++)
		{
			UnityEngine.Debug.Log(i + 1 + ". " + dialogue.playerChoices[i]);
		}

		// Placeholder for player's response
		playerResponse = dialogue.playerChoices[0]; // Simule un choix par défaut
		EndDialogue();
	}

	void EndDialogue()
	{
		isTalking = false;
		UnityEngine.Debug.Log("Dialogue ended with player response: " + playerResponse);
	}
}
