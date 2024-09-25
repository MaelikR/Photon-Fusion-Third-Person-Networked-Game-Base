using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class TutorialSystem : NetworkBehaviour
{
	private int currentStep = 0;
	private string[] tutorialSteps = {
		"Bienvenue! Utilisez W/A/S/D pour vous d�placer.",
		"Appuyez sur Espace pour sauter.",
		"Appuyez sur I pour ouvrir votre inventaire.",
		"F�licitations, tutoriel termin�!"
	};

	void Start()
	{
		if (Object.HasInputAuthority)
		{
			DisplayNextStep();
		}
	}

	public void DisplayNextStep()
	{
		if (currentStep < tutorialSteps.Length)
		{
			UnityEngine.Debug.Log(tutorialSteps[currentStep]);
			currentStep++;
		}
		else
		{
			EndTutorial();
		}
	}

	void EndTutorial()
	{
		UnityEngine.Debug.Log("Tutoriel termin�!");
	}
}
