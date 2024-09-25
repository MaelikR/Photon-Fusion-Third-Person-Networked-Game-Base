using Fusion;
using UnityEngine;
using UnityEngine.AI; // Import the NavMesh for pathfinding

public class EnemyAI : NetworkBehaviour, IDamageable
{
    public Transform targetPlayer; // The player that the enemy will follow
    public float moveSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 7f;
    private Vector3 _input;
    private bool isGrounded;
    private NetworkObject networkObject;
    private CharacterController characterController;
    private Animator animator;
    private Vector3 velocity;
    public float gravity = -9.81f;
    private NavMeshAgent navMeshAgent; // NavMeshAgent for pathfinding
    public float detectionRange = 10f; // Range within which the enemy detects the player
    public float attackRange = 2f; // Range within which the enemy starts attacking
    private bool isSprinting;
    public float health = 10f; // Santé de l'ennemi

    // Implémentez correctement la méthode TakeDamage avec deux paramètres
    public void TakeDamage(float damage, GameObject attacker)
    {
        health -= damage;
        Debug.Log($"{attacker.name} dealt {damage} damage to enemy. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject); // Détruit l'ennemi
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        networkObject = GetComponent<NetworkObject>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the enemy GameObject.");
        }

        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on the enemy GameObject.");
        }

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy GameObject.");
        }

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = attackRange; // Distance to stop from the target
    }

    void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        HandleMovement();
        CheckForJump();
    }

    void HandleMovement()
    {
        if (targetPlayer == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        // If the player is within detection range, follow the player
        if (distanceToPlayer <= detectionRange)
        {
            navMeshAgent.SetDestination(targetPlayer.position);

            // If the player is close enough, start sprinting
            if (distanceToPlayer <= 5f)
            {
                isSprinting = true;
                navMeshAgent.speed = sprintSpeed;
            }
            else
            {
                isSprinting = false;
                navMeshAgent.speed = moveSpeed;
            }

            // Adjust the movement input based on the NavMeshAgent
            _input = navMeshAgent.velocity.normalized;

            // Rotate the enemy to face the player
            Vector3 lookDirection = targetPlayer.position - transform.position;
            lookDirection.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

            // Set the speed parameter for the animator
            if (animator != null)
            {
                animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            }
        }
        else
        {
            navMeshAgent.ResetPath();
            _input = Vector3.zero;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }

    void CheckForJump()
    {
        // If there's an obstacle in front of the enemy, make it jump
        if (isGrounded && Physics.Raycast(transform.position, transform.forward, 1f))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
            {
                velocity.y = jumpForce;
            }
            isGrounded = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        ApplyGravity();
        MoveEnemy();
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Runner.DeltaTime;
    }

    void MoveEnemy()
    {
        if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
        {
            characterController.Move((_input * navMeshAgent.speed + velocity) * Runner.DeltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
