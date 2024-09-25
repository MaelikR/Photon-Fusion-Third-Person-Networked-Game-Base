using Fusion;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    public float jumpForce = 7f;
    public float gravity = -15.0f;

    private Vector3 _input;
    private bool isGrounded;
    private NetworkObject networkObject;
    private CharacterController characterController;
    private Animator animator;
    private Vector3 velocity;

    // Variables de rotation et autres pour gérer le mouvement
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float RotationSmoothTime = 0.12f;
    private GameObject _mainCamera;
    // Timeout variables for jump and fall
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    // Variables Input System
    private PlayerInputActions playerInputActions;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        networkObject = GetComponent<NetworkObject>();
        // Récupère les composants requis
    
       

        if (networkObject == null)
        {
            Debug.LogError("NetworkObject component is missing on the player GameObject.");
        }

        if (_input == null)
        {
            Debug.LogError("StarterAssetsInputs component is missing on the player GameObject.");
        }

        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera not found. Please ensure there's a camera tagged as 'MainCamera'.");
            }
        }
   
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the player GameObject.");
        }

        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on the player GameObject.");
        }

        // Initialize the Input Actions
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); // Enable the player input actions
    }

    void OnDestroy()
    {
        // Disable the Input Actions when the object is destroyed
        playerInputActions.Player.Disable();
    }

    private void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        GroundedCheck();
        JumpAndGravity();
        HandleInput();
    }

    void HandleInput()
    {
        // Get movement input from Input System
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        // Set movement direction relative to player's local space
        _input = new Vector3(horizontal, 0, vertical);

        if (animator != null)
        {
            float speed = _input.magnitude;
            animator.SetFloat("Speed", speed);

            // Ajouter des transitions pour le sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsSprinting", true);
            }
            else
            {
                animator.SetBool("IsSprinting", false);
            }

            // Déclencher l'animation de saut
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                animator.SetTrigger("Jump");
            }
        }

        // Jump with the "Space" key
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Sprint with the "Left Shift" key
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _input *= sprintSpeed;
        }
        else
        {
            _input *= moveSpeed;
        }
    }

    public override void FixedUpdateNetwork()
    { 
        // Vérifiez l'autorité d'état avant d'autoriser le mouvement
        if (networkObject == null || HasStateAuthority == false)
        {
            return;
        }

        ApplyGravity();
        MovePlayer();
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Runner.DeltaTime;
            if (velocity.y < _terminalVelocity)
            {
                velocity.y += gravity * Runner.DeltaTime;
            }
        }
    }

    void MovePlayer()
    {
        if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
        {
            Vector3 moveDirection = new Vector3(_input.x, 0.0f, _input.z).normalized;
            if (moveDirection != Vector3.zero)
            {
                // Calculate rotation
                float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // Rotate player to face direction
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // Déplacer le joueur
            characterController.Move((_input * (_input.magnitude > 0 ? moveSpeed : 0) + velocity) * Runner.DeltaTime);

            if (animator != null)
            {
                animator.SetFloat("Speed", _input.magnitude);
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
            isGrounded = false;
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.14f, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, 0.28f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

        // Update animator
        if (animator != null)
        {
            animator.SetBool("Grounded", isGrounded);
        }
    }

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // Reset fall timeout
            _fallTimeoutDelta = FallTimeout;

            if (animator != null)
            {
                animator.SetBool("Jump", false);
                //animator.SetBool("FreeFall", false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (playerInputActions.Player.Jump.triggered && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                if (animator != null)
                {
                    animator.SetBool("Jump", true);
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (animator != null)
                {
                   // animator.SetBool("FreeFall", true);
                }
            }

            playerInputActions.Player.Jump.Disable();
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
