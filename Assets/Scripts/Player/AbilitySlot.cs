using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AbilitySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon; // Image to display the ability icon
    public KeyCode activationKey; // Key to activate the ability
    private Ability assignedAbility; // The ability assigned to this slot

    // Assign an ability to this slot
    public void AssignAbility(Ability ability)
    {
        assignedAbility = ability;
        icon.sprite = ability.icon;
        icon.enabled = true;
    }

    // Clear the slot
    public void ClearSlot()
    {
        assignedAbility = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // Check if the slot has an assigned ability
    public bool HasAbility()
    {
        return assignedAbility != null;
    }

    // Handle mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && assignedAbility != null)
        {
            ActivateAbility();
        }
    }

    // Method to activate the ability
    public void ActivateAbility()
    {
        if (assignedAbility != null)
        {
            Debug.Log($"Activating {assignedAbility.name}");
            assignedAbility.Activate();
        }
    }

    // Update method to check for keyboard input
    private void Update()
    {
        if (HasAbility() && Input.GetKeyDown(activationKey))
        {
            ActivateAbility();
        }
    }
}
