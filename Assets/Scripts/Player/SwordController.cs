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
        if (!HasInputAuthority) return; // Assurez-vous que seul le propriétaire de l'objet puisse le contrôler

        if (Input.GetKeyDown(KeyCode.E)) // Touche pour dégainer/rengainer
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
        if (!HasStateAuthority) return; // Assurez-vous que seul l'objet ayant l'autorité d'état applique les dégâts

        // Code pour appliquer les dégâts à tous les ennemis dans la zone d'attaque
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject)
            {
                // Obtenez l'instance de ThirdPersonController à partir de hitCollider
                ThirdPersonController playerHealth = hitCollider.GetComponent<ThirdPersonController>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(swordDamage, gameObject); // Appel correct sur l'instance playerHealth
                }
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                // Vérifiez si l'ennemi a un composant EnemyAI et appliquez les dégâts
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
