using Fusion;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using StarterAssets;
using UnityEngine.UI;
namespace Fusion
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : NetworkBehaviour, IDamageable
    {
        // Player settings
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float SpeedChangeRate = 10.0f;
        public float maxSpeed = 6f; // Vitesse maximale autorisée
        [Header("Water Settings")]
        // Variable liée à la natation
        public float swimSpeed = 5f;
        public float waterSurfaceLevel = -6.320007f;
        private bool isSwimming = false;

        [Tooltip("Layer Mask for water detection")]
        public LayerMask waterMask;

        [Tooltip("AudioClip for swimming sound")]
        public AudioClip swimSound;
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built-in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        private InventorySystem inventorySystem;
        // Internal variables for timeouts
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Existing variables
        public float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool _hasAnimator;

        // Animator ID variables
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        public GameObject spawnedPlayer;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float health = 20f;
        private UIManager uiManager;
        private CharacterStats characterStats;

 
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private void Awake()
        {
            // Initialisation des composants
     
         
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null && !HasInputAuthority)
            {
                _playerInput.enabled = false;
            }
#endif
            _mainCamera = GetComponentInChildren<Camera>(true)?.gameObject;
            _hasAnimator = TryGetComponent(out _animator);
            _input = GetComponent<StarterAssetsInputs>();
            _controller = GetComponent<CharacterController>();

            // Messages d'erreurs s'il manque des composants essentiels
            if (_mainCamera == null) Debug.LogError("MainCamera is missing.");

           
            if (_input == null) Debug.LogError("StarterAssetsInputs is missing.");
            if (_controller == null) Debug.LogError("CharacterController is missing.");
        }

        private void Start()
        {
            AssignAnimationIDs();
            if (HasInputAuthority)
            {
                OnStartAuthority();
            }
            else
            {
                OnStopAuthority();
            }
            characterStats = GetComponent<CharacterStats>();

            if (Object.HasInputAuthority)
            {
                // Trouver l'UIManager dans la scène
                UIManager foundManager = FindFirstObjectByType<UIManager>();

                if (uiManager != null)
                {
                    // Assigner cette instance de joueur à l'UIManager
                    uiManager.SetPlayerInstance(this);


                    uiManager.healthBar = transform.Find("Canvas/HealthBar").GetComponent<Slider>();
                    uiManager.manaBar = transform.Find("Canvas/ManaBar").GetComponent<Slider>();
                    uiManager.questLog = transform.Find("Canvas/QuestLogText").GetComponent<UnityEngine.UI.Text>();
                    // Assigner les références pour CharacterStats et InventorySystem
                    characterStats = GetComponent<CharacterStats>();
                    inventorySystem = GetComponent<InventorySystem>();

                    if (characterStats != null)
                    {
                        uiManager.playerStats = characterStats;
                    }

                    if (inventorySystem != null)
                    {
                        uiManager.inventorySystem = inventorySystem;
                    }
                }
            }
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }
        public float GetCurrentHealth()
        {
            return characterStats.currentHealth;
        }

        public float GetMaxHealth()
        {
            return characterStats.maxHealth;
        }

        public float GetCurrentMana()
        {
            return characterStats.currentMana;
        }

        public float GetMaxMana()
        {
            return characterStats.maxMana;
        }
        public void TakeDamage(float damage, GameObject attacker)
        {
            health -= damage;
            Debug.Log($"{attacker.name} dealt {damage} damage to player. Remaining health: {health}");

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Player died");
            Runner.Despawn(Object);
        }

        private void OnStartAuthority()
        {
           
            if (_mainCamera != null)
            {
                _mainCamera.SetActive(true);
            }

#if ENABLE_INPUT_SYSTEM
            if (_playerInput != null)
            {
                _playerInput.enabled = true;
            }
#endif
        }

        private void OnStopAuthority()
        {


            if (_mainCamera != null)
            {
                _mainCamera.SetActive(false);
            }

#if ENABLE_INPUT_SYSTEM
            if (_playerInput != null)
            {
                _playerInput.enabled = false;
            }


#endif
        }

        public GameObject[] objectsToDisableForRemotePlayers;
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} has joined the game.");

            NetworkObject playerObject = runner.GetPlayerObject(player);

            if (playerObject != null && playerObject.IsValid)
            {
                // Si le joueur est local, on ne fait rien
                if (runner.LocalPlayer == player)
                {
                    Debug.Log("This is the local player.");
                }
                else
                {
                    // Désactive les GameObjects spécifiquement liés à ce joueur distant
                    DisableRemotePlayerObjects(playerObject.gameObject);
                }
            }
        }

        private void DisableRemotePlayerObjects(GameObject remotePlayer)
        {
            // Désactive uniquement les objets appartenant à ce joueur distant
            foreach (var obj in objectsToDisableForRemotePlayers)
            {
                if (obj != null && obj.transform.IsChildOf(remotePlayer.transform))
                {
                    obj.SetActive(false);
                    Debug.Log($"{obj.name} has been disabled for remote player {remotePlayer.name}.");
                }
            }
        }


        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority) return;

            if (_input == null)
            {
                return;
            }

            GroundedCheck();
            HandleSwim();

            if (isSwimming)
            {
                SwimMovement();
            }
            else
            {
                JumpAndGravity();
                Move();
            }
        }
        private void HandleSwim()
        {
            // Détecte si le joueur est en contact avec l'eau en utilisant le LayerMask waterMask
            bool isInWater = Physics.CheckSphere(transform.position, 0.5f, waterMask);

            if (isInWater)
            {
                if (!isSwimming)
                {
                    Debug.Log("Player started swimming.");
                    StartSwimming();
                }
                SwimMovement();
            }
            else if (isSwimming)
            {
                Debug.Log("Player stopped swimming.");
                StopSwimming();
            }
        }


        // Start swimming and set animation state
        private void StartSwimming()
        {
            isSwimming = true;
            _animator.SetBool("isSwimming", true);
            PlaySound(swimSound);
            Debug.Log("Swim mode activated."); // Log when swim mode is activated
        }

        // Stop swimming and reset animation state
        private void StopSwimming()
        {
            isSwimming = false;
            _animator.SetBool("isSwimming", false);
            StopSound();
            Debug.Log("Swim mode deactivated."); // Log when swim mode is deactivated
        }

        // SwimMovement method handles the player's swimming movement


        // SwimMovement method handles the player's swimming movement
        private void SwimMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

            if (transform.position.y < waterSurfaceLevel)
            {
                moveDirection.y = Input.GetKey(KeyCode.Space) ? swimSpeed : Input.GetKey(KeyCode.LeftControl) ? -swimSpeed : 0;
            }
            else
            {
                moveDirection.y = -swimSpeed * 0.5f;
            }

            _controller.Move(moveDirection * swimSpeed * Time.deltaTime);
            _animator.SetBool("isSwimming", moveDirection != Vector3.zero);
        }
        private AudioSource _audioSource;



        private void PlaySound(AudioClip clip)
        {
            if (_audioSource != null && clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }

        private void StopSound()
        {
            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                    _input.jump = false;
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Runner.DeltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Runner.DeltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Runner.DeltaTime;
            }
        }

        private void Move()
        {
            if (_input == null) return;

            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Runner.DeltaTime * SpeedChangeRate);
                _speed = Mathf.Clamp(_speed, 0, maxSpeed);
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Runner.DeltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            if (_input.move != Vector2.zero)
            {
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                Vector3 cameraForward = _mainCamera.transform.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                Vector3 cameraRight = _mainCamera.transform.right;
                cameraRight.y = 0f;
                cameraRight.Normalize();

                Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
                moveDirection.Normalize();

                _targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                _controller.Move(moveDirection * _speed * Runner.DeltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Runner.DeltaTime);
            }
            else
            {
                _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Runner.DeltaTime);
            }

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }
    }
}