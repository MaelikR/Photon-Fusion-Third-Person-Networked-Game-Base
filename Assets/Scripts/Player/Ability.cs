using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float cooldown;
    public KeyCode keyCode;

    private bool isOnCooldown;

    // Method to activate the ability
    public void Activate() // Removed 'override'
    {
        if (isOnCooldown) return;

        Debug.Log($"Ability {abilityName} activated!");
        // Trigger animation using the animator
        Animator animator = FindObjectOfType<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(abilityName); // Ensure your animator has a trigger matching this ability name
        }

        // Implement ability logic here
        AbilityManager.Instance.StartCoroutine(Cooldown());
    }

    // Cooldown logic
    private System.Collections.IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
