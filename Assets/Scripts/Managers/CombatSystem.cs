using Fusion;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;

public class CombatSystem : NetworkBehaviour
{
	public CharacterStats playerStats;
	public CharacterStats enemyStats;

	public List<Skill> availableSkills = new List<Skill>();

	public override void FixedUpdateNetwork()
	{
		foreach (Skill skill in availableSkills)
		{
			if (skill.cooldownRemaining > 0)
			{
				skill.cooldownRemaining -= Runner.DeltaTime;
			}
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_Attack(string skillName)
	{
		Skill skill = availableSkills.Find(s => s.skillName == skillName);
		if (skill != null && skill.cooldownRemaining <= 0 && playerStats.currentMana >= skill.manaCost)
		{
			playerStats.UseMana(skill.manaCost);
			skill.cooldownRemaining = skill.cooldown;
			enemyStats.TakeDamage(skill.damage);
			UnityEngine.Debug.Log("Player used " + skill.skillName + " dealing " + skill.damage + " damage.");
		}
		else
		{
			UnityEngine.Debug.Log("Skill not ready or not enough mana.");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) && Object.HasInputAuthority)
		{
			RPC_Attack("Fireball");
		}
	}
}

[System.Serializable]
public class Skill
{
	public string skillName;
	public int damage;
	public int manaCost;
	public float cooldown;
    internal bool isUnlocked;

    [Networked] public float cooldownRemaining { get; set; }
}
