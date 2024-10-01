using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class CharacterStats : NetworkBehaviour
{
	public int maxHealth { get; set; }
	public int currentHealth { get; set; }
	public int maxMana { get; set; }
	public int currentMana { get; set; }
    public int level;
    public float healthRegenRate = 1f;
	public float manaRegenRate = 1f;

	private float healthRegenCooldown = 5f; // Temps avant de r�g�n�rer
	private float manaRegenCooldown = 5f;

	void Start()
	{
		if (Object.HasStateAuthority)
		{
			maxHealth = 100;
			currentHealth = maxHealth;
			maxMana = 100;
			currentMana = maxMana;
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (Object.HasStateAuthority)
		{
			RegenerateHealthAndMana();
		}
	}

	public void TakeDamage(int damage)
	{
		if (Object.HasStateAuthority)
		{
			currentHealth -= damage;
			healthRegenCooldown = 5f; // Reset la r�g�n�ration apr�s avoir pris des d�g�ts
			if (currentHealth <= 0)
			{
				Die();
			}
		}
	}

	public void UseMana(int manaCost)
	{
		if (Object.HasStateAuthority && currentMana >= manaCost)
		{
			currentMana -= manaCost;
			manaRegenCooldown = 5f; // Reset la r�g�n�ration apr�s avoir utilis� de la mana
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

		// R�duire le cooldown de la r�g�n�ration
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
		// G�rer la mort ici (respawn, etc.)
	}
}
