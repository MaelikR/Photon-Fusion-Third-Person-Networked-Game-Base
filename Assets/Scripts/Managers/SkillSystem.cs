using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : NetworkBehaviour
{
    public CharacterStats characterStats;
    public List<PlayerSkill> skills = new List<PlayerSkill>();

    void Start()
    {
        if (Object.HasStateAuthority)
        {
            // Ajoute des compétences initiales (exemple de compétences)
            skills.Add(new PlayerSkill("Fireball", 20, 5f, 50));
            skills.Add(new PlayerSkill("Heal", 15, 3f, -30));
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            foreach (PlayerSkill skill in skills)
            {
                if (skill.cooldownRemaining > 0)
                {
                    skill.cooldownRemaining -= Runner.DeltaTime;
                }
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_UseSkill(string skillName)
    {
        PlayerSkill skill = skills.Find(s => s.skillName == skillName && s.isUnlocked);
        if (skill != null && skill.cooldownRemaining <= 0 && characterStats.currentMana >= skill.manaCost)
        {
            // Utilise la compétence, réduit le mana et déclenche le cooldown
            characterStats.UseMana(skill.manaCost);
            skill.cooldownRemaining = skill.cooldown;
            Debug.Log("Used skill: " + skill.skillName + " for " + skill.damage + " damage/healing.");
        }
        else
        {
            Debug.Log("Skill " + skillName + " is not ready or insufficient mana.");
        }
    }

    public void UnlockSkill(string skillName)
    {
        if (Object.HasStateAuthority)
        {
            PlayerSkill skill = skills.Find(s => s.skillName == skillName);
            if (skill != null && !skill.isUnlocked)
            {
                skill.isUnlocked = true;
                Debug.Log(skillName + " unlocked!");
            }
        }
    }
}
