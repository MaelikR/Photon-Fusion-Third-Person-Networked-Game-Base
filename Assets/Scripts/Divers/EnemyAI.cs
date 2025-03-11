using Fusion;
using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking,
    Idle
}

public class EnemyAIAttack : NetworkBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public Transform[] waypoints;
    public float waypointTolerance = 0.5f;
    public float pauseDuration = 2f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip chaseSound;
    public AudioClip deathSound;
    public AudioClip patrolSound;

    [Header("Combat Settings")]
    public float health = 100f;
    public float attackRange = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;

    private NetworkCharacterController characterController;
    private Animator animator;
    private Transform targetPlayer;
    private float nextAttackTime = 0f;
    private EnemyState currentState = EnemyState.Patrolling;

    private bool isPaused = false;
    private int currentWaypointIndex = 0;

    void Start()
    {
        characterController = GetComponent<NetworkCharacterController>();
        animator = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        HandleMovement();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    private void HandleMovement()
    {
        if (targetPlayer == null) return;

        if (currentState == EnemyState.Chasing)
        {
            MoveTowards(targetPlayer.position);
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 velocity = direction * moveSpeed;

        characterController.Move(velocity);

        float speed = velocity.magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("isWalking", speed > 0.1f);
    }

    private void Patrol()
    {
        if (waypoints.Length == 0 || isPaused) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (patrolSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(patrolSound);
        }

        if (distanceToWaypoint <= waypointTolerance)
        {
            StartCoroutine(PauseAtWaypoint());
            return;
        }

        MoveTowards(targetWaypoint.position);
    }

    private void Chase()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            if (chaseSound != null && audioSource != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(chaseSound);
            }

            MoveTowards(targetPlayer.position);
        }
    }
    private void Attack()
    {
        if (targetPlayer == null) return;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        animator.SetTrigger("Attack");

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(0.5f);

        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
            if (distanceToPlayer <= attackRange)
            {
                IDamageable damageable = targetPlayer.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (attackSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(attackSound);
                    }

                    damageable.TakeDamage(attackDamage, gameObject);  // Use IDamageable interface
                    Debug.Log($"Attack dealt {attackDamage} damage to player at position {targetPlayer.position}.");
                }
            }
        }
    }

    private IEnumerator PauseAtWaypoint()
    {
        isPaused = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(pauseDuration);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isPaused = false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TakeDamage(float damage, NetworkObject attacker)
    {
        TakeDamage(damage, attacker != null ? attacker.gameObject : null);
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        if (!HasStateAuthority) return;

        health -= damage;
        Debug.Log($"Enemy took {damage} damage from {attacker?.name ?? "unknown"}. Current health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Debug.Log("Enemy died.");
        animator.SetTrigger("Die");
        Destroy(gameObject, 2.3f);
    }

    // Trigger-based player detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player detected, start chasing
            targetPlayer = other.transform;
            currentState = EnemyState.Chasing;
            Debug.Log("Player detected in range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player leaves the detection area
            if (targetPlayer == other.transform)
            {
                targetPlayer = null;
                currentState = EnemyState.Patrolling;
                Debug.Log("Player left the detection range.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
