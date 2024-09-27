using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class ServerLogic : NetworkBehaviour
{
    public AudioClip attackSound;
    public AudioClip moveSound;
    public ParticleSystem attackEffect;
    public ParticleSystem moveEffect;
    public UnityEngine.UI.Text combatLog;

    public void ValidatePlayerAction(string actionType, GameObject player)
    {
        if (Object.HasStateAuthority)
        {
            int playerLevel = player.GetComponent<CharacterStats>().level;

            if (actionType == "Attack")
            {
                UnityEngine.Debug.Log("Validating attack for " + player.name);

                float successChance = Mathf.Clamp01(playerLevel / 100f);

                if (UnityEngine.Random.value < successChance)
                {
                    UnityEngine.Debug.Log("Attack validated successfully for " + player.name);
                    PlayFeedback("Attack", player.transform.position);
           

                    if (UnityEngine.Random.value > 0.95f)
                    {
                        LogCombatAction(player.name + " executed a Critical Hit!");
                    }
                    else
                    {
                        LogCombatAction(player.name + " successfully attacked!");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Attack failed for " + player.name);
                    LogCombatAction(player.name + " missed the attack.");
                }
            }
            else if (actionType == "Move")
            {
                UnityEngine.Debug.Log("Validating movement for " + player.name);

                float maxSpeed = playerLevel * 0.1f;
                var moveComponent = player.GetComponent<ThirdPersonController>();

                if (moveComponent != null && moveComponent._speed <= maxSpeed)
                {
                    UnityEngine.Debug.Log("Movement validated for " + player.name);
                    PlayFeedback("Move", player.transform.position);
                    LogCombatAction(player.name + " moved successfully.");
                }
                else
                {
                    UnityEngine.Debug.Log("Movement too fast for level: " + player.name);
                    LogCombatAction(player.name + " tried to move too fast and failed.");
                }
            }
        }
    }

    public void PlayFeedback(string actionType, Vector3 position)
    {
        if (actionType == "Attack")
        {
            if (attackSound != null)
                AudioSource.PlayClipAtPoint(attackSound, position);

            if (attackEffect != null)
            {
                var effect = Instantiate(attackEffect, position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
        }
        else if (actionType == "Move")
        {
            if (moveSound != null)
                AudioSource.PlayClipAtPoint(moveSound, position);

            if (moveEffect != null)
            {
                var effect = Instantiate(moveEffect, position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
        }
    }


    public void LogCombatAction(string actionDescription)
    {
        if (combatLog != null)
        {
            combatLog.text += actionDescription + "\n";
            UnityEngine.Debug.Log(actionDescription);
        }
    }
}
