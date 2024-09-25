using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    public AbilitySlot[] abilitySlots; // Array to hold references to the UI slots
    public List<Ability> availableAbilities; // List of available abilities

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize the slots with available abilities
        AssignAbilities();
    }

    private void AssignAbilities()
    {
        for (int i = 0; i < abilitySlots.Length && i < availableAbilities.Count; i++)
        {
            abilitySlots[i].AssignAbility(availableAbilities[i]);
            abilitySlots[i].activationKey = availableAbilities[i].keyCode;
        }
    }

    // Method to reassign abilities if needed
    public void ReassignAbilities()
    {
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            if (i < availableAbilities.Count)
            {
                abilitySlots[i].AssignAbility(availableAbilities[i]);
            }
            else
            {
                abilitySlots[i].ClearSlot();
            }
        }
    }
}
