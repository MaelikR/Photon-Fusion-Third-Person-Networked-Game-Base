using Fusion;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using Cinemachine;
using StarterAssets;

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

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built-in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // Internal variables for timeouts
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Existing variables
        private float _speed;
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

        private CinemachineFreeLook _freeLookCamera;
        private CinemachineBrain _cinemachineBrain;
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
            _freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
            _cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
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
            if (_freeLookCamera == null) Debug.LogError("CinemachineFreeLook is not found.");
            if (_cinemachineBrain == null) Debug.LogError("CinemachineBrain is not found.");
            if (_input == null) Debug.LogError("StarterAssetsInputs is missing.");
            if (_controller == null) Debug.LogError("CharacterController is missing.");
        }

        private void Start()
        {
            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
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

        private void OnEnable()
        {
            if (HasInputAuthority)
            {
                OnStartAuthority();
            }
            else
            {
                OnStopAuthority();
            }
        }

        private void OnDisable()
        {
            OnStopAuthority();
        }

        private void OnStartAuthority()
        {
            if (_freeLookCamera != null)
            {
                _freeLookCamera.Follow = transform;
                _freeLookCamera.LookAt = transform;
                _freeLookCamera.enabled = true;
            }

            if (_cinemachineBrain != null)
            {
                _cinemachineBrain.enabled = true;
            }

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
            if (_freeLookCamera != null)
            {
                _freeLookCamera.enabled = false;
            }

            if (_cinemachineBrain != null)
            {
                _cinemachineBrain.enabled = false;
            }

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

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority) return;

            if (_input == null)
            {
                return;
            }

            GroundedCheck();

            JumpAndGravity();
            Move();
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
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
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
