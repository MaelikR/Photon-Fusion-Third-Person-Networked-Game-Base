using Fusion;
using UnityEngine;

public class SwordController : NetworkBehaviour
{
    [Header("Attack Settings")]
    public Animator animator;
    public Transform swordTransform;
    public LayerMask enemyLayer;
    public float attackRange = 1.5f;
    public int baseDamage = 10;
    private float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    public int heavyAttackDamage = 25;
    private float heavyAttackCooldown = 3f;
    private float nextHeavyAttackTime = 0f;

    [Header("Animation Settings")]
    public string[] attackAnimations; // Liste des animations pour attaque normale
    public string heavyAttackAnimation = "HeavyAttack";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip swordSwingSound;
    public AudioClip heavyAttackSound;
    public AudioClip criticalHitSound;

    private bool isAttacking = false;

    private void Update()
    {
        if (HasStateAuthority)
        {
            // Attaque normale (clic gauche)
            if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                isAttacking = true;
                RPC_PerformAttack(false);
            }

            // Attaque puissante (clic droit)
            if (Input.GetMouseButtonDown(1) && Time.time >= nextHeavyAttackTime)
            {
                nextHeavyAttackTime = Time.time + heavyAttackCooldown;
                isAttacking = true;
                RPC_PerformAttack(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            float distance = Vector3.Distance(swordTransform.position, other.transform.position);
            if (distance <= attackRange)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    float finalDamage = SetDamage(baseDamage);
                    RPC_ApplyDamage(other.GetComponent<NetworkObject>(), finalDamage);
                }
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_PerformAttack(bool isHeavy)
    {
        if (animator != null)
        {
            if (isHeavy)
            {
                animator.SetTrigger(heavyAttackAnimation);
            }
            else
            {
                // Sélection aléatoire d'une animation parmi celles définies
                if (attackAnimations.Length > 0)
                {
                    string randomAttack = attackAnimations[Random.Range(0, attackAnimations.Length)];
                    animator.SetTrigger(randomAttack);
                }
            }
        }

        if (audioSource != null)
        {
            audioSource.clip = isHeavy ? heavyAttackSound : swordSwingSound;
            audioSource.Play();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ApplyDamage(NetworkObject enemyObject, float damage)
    {
        if (enemyObject != null)
        {
            IDamageable damageable = enemyObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, gameObject);
            }
        }
    }

    private float SetDamage(float damage)
    {
        float finalDamage = damage;
        if (Random.value > 0.9f)
        {
            finalDamage *= 2;
            if (audioSource != null && criticalHitSound != null)
            {
                audioSource.clip = criticalHitSound;
                audioSource.Play();
            }
        }
        return finalDamage;
    }
}
