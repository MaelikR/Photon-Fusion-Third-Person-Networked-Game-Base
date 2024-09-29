using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
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
    [SerializeField] private Text dialogueText; // Texte pour afficher les lignes de dialogue
    [SerializeField] private GameObject choicesPanel; // Panneau contenant les boutons de choix
    [SerializeField] private Button[] choiceButtons; // Tableau de boutons de choix

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
        dialogueText.text = dialogue.characterName + ": " + currentLine;
    }

    void DisplayPlayerChoices(Dialogue dialogue)
    {
        choicesPanel.SetActive(true);
        for (int i = 0; i < dialogue.playerChoices.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<Text>().text = dialogue.playerChoices[i];

            int choiceIndex = i; // Nécessaire pour capturer correctement l'index dans un contexte de fermeture
            choiceButtons[i].onClick.AddListener(() => OnPlayerChoiceMade(choiceIndex, dialogue));
        }
    }

    void OnPlayerChoiceMade(int choiceIndex, Dialogue dialogue)
    {
        playerResponse = dialogue.playerChoices[choiceIndex];
        Debug.Log("Player chose: " + playerResponse);
        EndDialogue();
    }

    void EndDialogue()
    {
        isTalking = false;
        dialogueText.text = "";
        choicesPanel.SetActive(false);
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
        Debug.Log("Dialogue ended with player response: " + playerResponse);
    }
}
