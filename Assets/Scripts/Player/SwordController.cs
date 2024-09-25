using Fusion;
using StarterAssets;
using UnityEngine;

public class SwordController : NetworkBehaviour
{
    public Animator animator;
    public int swordDamage = 50;
    private bool isSwordDrawn = false;
    public float attackRadius = 1.2f;

    void Update()
    {
        if (!HasInputAuthority) return; // Assurez-vous que seul le propri�taire de l'objet puisse le contr�ler

        if (Input.GetKeyDown(KeyCode.E)) // Touche pour d�gainer/rengainer
        {
            if (isSwordDrawn)
            {
                RPC_SheathSword();
            }
            else
            {
                RPC_DrawSword();
            }
        }

        if (isSwordDrawn && Input.GetMouseButtonDown(0)) // Touche pour attaquer
        {
            RPC_Attack();
            ApplyDamage();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_DrawSword()
    {
        isSwordDrawn = true;
        animator.SetBool("isSwordDrawn", true);
        animator.SetTrigger("DrawSword");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SheathSword()
    {
        isSwordDrawn = false;
        animator.SetBool("isSwordDrawn", false);
        animator.SetTrigger("SheathSword");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_Attack()
    {
        animator.SetTrigger("isAttacking");
    }

    private void ApplyDamage()
    {
        if (!HasStateAuthority) return; // Assurez-vous que seul l'objet ayant l'autorit� d'�tat applique les d�g�ts

        // Code pour appliquer les d�g�ts � tous les ennemis dans la zone d'attaque
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject)
            {
                // Obtenez l'instance de ThirdPersonController � partir de hitCollider
                ThirdPersonController playerHealth = hitCollider.GetComponent<ThirdPersonController>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(swordDamage, gameObject); // Appel correct sur l'instance playerHealth
                }
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                // V�rifiez si l'ennemi a un composant EnemyAI et appliquez les d�g�ts
                EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Debug.Log($"Dealt {swordDamage} damage to enemy using FFAAI");
                    enemyAI.TakeDamage(swordDamage, gameObject); // Appel correct sur l'instance enemyAI
                }
            }
        }
    }
}
