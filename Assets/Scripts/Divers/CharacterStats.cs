using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class CharaStats : NetworkBehaviour
{
	public int maxHealth { get; set; }
	public int currentHealth { get; set; }
	public int maxMana { get; set; }
	public int currentMana { get; set; }
    public int level;
    public float healthRegenRate = 1f;
	public float manaRegenRate = 1f;
    public bool isInCombat = false;

    private float healthRegenCooldown = 5f; // Temps avant de régénérer
	private float manaRegenCooldown = 5f;

	void Start()
	{
		if (HasStateAuthority)
		{
			maxHealth = 100;
			currentHealth = maxHealth;
			maxMana = 100;
			currentMana = maxMana;
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (HasStateAuthority)
		{
			RegenerateHealthAndMana();
		}
	}

	public void TakeDamage(int damage)
	{
		if (HasStateAuthority)
		{
			currentHealth -= damage;
			healthRegenCooldown = 5f; // Reset la régénération après avoir pris des dégâts
			if (currentHealth <= 0)
			{
				Die();
			}
		}
	}

	public void UseMana(int manaCost)
	{
		if (HasStateAuthority && currentMana >= manaCost)
		{
			currentMana -= manaCost;
			manaRegenCooldown = 5f; // Reset la régénération après avoir utilisé de la mana
		}
	}

	void RegenerateHealthAndMana()
	{
		if (currentHealth < maxHealth && healthRegenCooldown <= 0)
		{
			currentHealth += Mathf.CeilToInt(healthRegenRate * Runner.DeltaTime);
		}

		if (currentMana < maxMana && manaRegenCooldown <= 0)
		{
			currentMana += Mathf.CeilToInt(manaRegenRate * Runner.DeltaTime);
		}

		// Réduire le cooldown de la régénération
		if (healthRegenCooldown > 0)
		{
			healthRegenCooldown -= Runner.DeltaTime;
		}
		if (manaRegenCooldown > 0)
		{
			manaRegenCooldown -= Runner.DeltaTime;
		}
	}

	void Die()
	{
		UnityEngine.Debug.Log("Player has died.");
		// Gérer la mort ici (respawn, etc.)
	}
}
