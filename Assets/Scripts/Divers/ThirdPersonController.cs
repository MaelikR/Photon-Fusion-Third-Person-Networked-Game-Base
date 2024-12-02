using UnityEngine;
using Fusion;

public class ThirdPersonController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float LookSensitivity = 1.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -9.81f;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public LayerMask GroundLayers;
    public Camera PlayerCamera;

    [Header("Flying Settings")]
    public float FlySpeed = 5.0f;
    private bool _isFlying = false;

    [Header("Swimming Settings")]
    public float SwimSpeed = 3.0f;
    public float SurfaceHeight = 5f;
    public LayerMask WaterLayer;
    private bool _isSwimming = false;

    private CharacterController _controller;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    private Vector2 _inputMove;
    private Vector2 _inputLook;
    private bool _inputJump;

    private bool _isGrounded;
    private Animator _animator;

    [Networked] private bool IsLocalPlayer { get; set; }
    [Networked] private NetworkBool IsFlying { get; set; }

    // Références vers d'autres systèmes
    private GameManager gameManager;
    private ReincarnationManager reincarnationManager;
    private TransformationController transformationController;

    public override void Spawned()
    {
        IsLocalPlayer = HasStateAuthority;

        if (!IsLocalPlayer && PlayerCamera != null)
        {
            PlayerCamera.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            Debug.LogError("CharacterController is missing!");
        }

        _animator = GetComponent<Animator>();

        // Obtenir les références vers les systèmes centraux
        gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            reincarnationManager = gameManager.reincarnationManager;
            transformationController = gameManager.transformationController;
        }

        if (!HasStateAuthority && PlayerCamera != null)
        {
            PlayerCamera.gameObject.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        GetInput(out NetworkInputData input);
        ProcessInput(input);

        if (_isSwimming)
        {
            Swim();
        }
        else if (_isFlying)
        {
            FlyMove();
        }
        else
        {
            JumpAndGravity();
            Move();
        }

        Look();
        UpdateAnimations();
    }

    private void GetInput(out NetworkInputData inputData)
    {
        inputData = new NetworkInputData
        {
            MoveX = Input.GetAxis("Horizontal"),
            MoveY = Input.GetAxis("Vertical"),
            LookX = Input.GetAxis("Mouse X") * LookSensitivity,
            LookY = Input.GetAxis("Mouse Y") * LookSensitivity,
            Jump = Input.GetButton("Jump"),
            ToggleFlying = Input.GetKeyDown(KeyCode.F)
        };
    }

    private void ProcessInput(NetworkInputData inputData)
    {
        _inputMove = new Vector2(inputData.MoveX, inputData.MoveY);
        _inputLook = new Vector2(inputData.LookX, inputData.LookY);
        _inputJump = inputData.Jump;

        // Gestion du basculement en mode vol
        if (inputData.ToggleFlying)
        {
            if (transformationController != null && transformationController.GetCurrentForm() == "Werewolf")
            {
                Debug.LogWarning("Cannot fly in Werewolf form!");
                return; // Ne pas permettre le vol si en forme de loup-garou
            }

            IsFlying = !IsFlying;
            _isFlying = IsFlying;

            // Interaction avec TransformationController pour des effets
            if (transformationController != null)
            {
                transformationController.ToggleFlyingForm(_isFlying);
            }
        }
    }


    private void Swim()
    {
        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * _inputMove.y + right * _inputMove.x;
        Vector3 swimMovement = moveDirection.normalized * SwimSpeed * Runner.DeltaTime;

        swimMovement.y = 0; // Maintenir la hauteur à la surface
        transform.position = new Vector3(transform.position.x, SurfaceHeight, transform.position.z);

        _controller.Move(swimMovement);

        if (_animator != null)
        {
            _animator.SetBool("isSwimming", true);
        }
    }

    private void Move()
    {
        float targetSpeed = _inputMove.magnitude > 0.5f ? SprintSpeed : MoveSpeed;

        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 moveDirection = (forward * _inputMove.y + right * _inputMove.x).normalized;
        Vector3 move = moveDirection * targetSpeed * Runner.DeltaTime;
        _controller.Move(move + new Vector3(0, _verticalVelocity, 0) * Runner.DeltaTime);
    }

    private void FlyMove()
    {
        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;

        Vector3 moveDirection = forward * _inputMove.y + right * _inputMove.x;
        float verticalMove = 0f;

        if (Input.GetKey(KeyCode.E)) verticalMove = 1;
        if (Input.GetKey(KeyCode.Q)) verticalMove = -1;

        Vector3 flyMovement = (moveDirection + Vector3.up * verticalMove).normalized * FlySpeed * Runner.DeltaTime;
        _controller.Move(flyMovement);
    }

    private void JumpAndGravity()
    {
        if (_isGrounded && !_isFlying)
        {
            _verticalVelocity = -2f;

            if (_inputJump)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        else if (!_isFlying)
        {
            _verticalVelocity += Gravity * Runner.DeltaTime;
        }

        _controller.Move(new Vector3(0, _verticalVelocity, 0) * Runner.DeltaTime);
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * _inputLook.x);

        _cameraPitch -= _inputLook.y;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);

        PlayerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
    }

    private void UpdateAnimations()
    {
        if (_animator != null)
        {
            _animator.SetBool("isFlying", _isFlying);
            _animator.SetBool("isGrounded", _isGrounded);
            _animator.SetBool("isWalking", _inputMove.magnitude > 0);
            _animator.SetBool("isSwimming", _isSwimming);
        }
    }

    private struct NetworkInputData : INetworkInput
    {
        public float MoveX;
        public float MoveY;
        public float LookX;
        public float LookY;
        public bool Jump;
        public bool ToggleFlying;
    }
}
