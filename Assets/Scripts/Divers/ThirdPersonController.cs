using Fusion;
using UnityEngine;

public class ThirdPersonController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float LookSensitivity = 1.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -9.81f;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public LayerMask GroundLayers;

    [Header("Camera Settings")]
    public Camera PlayerCamera;

    [Header("Flying Settings")]
    public float FlySpeed = 5.0f;
    
    private bool isMoving;
    private bool isFlyingSoundPlaying;
   
    [Header("Swimming Settings")]
    public float SwimSpeed = 3.0f;
    public float SwimSurfaceAdjustmentSpeed = 2.0f;
    public LayerMask WaterLayer;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip swimSound;
    public AudioClip flySound;
    public AudioClip waterEnterSound;
    public AudioClip waterExitSound;
    [Networked] private NetworkBool IsFlying { get; set; }
    [Networked] private NetworkBool IsSwimming { get; set; }

    private float waterSurfaceHeight;

    [SerializeField]
    private CharacterController _controller;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    private Vector2 _inputMove;
    private Vector2 _inputLook;
    private bool _inputJump;

    private bool _isGrounded;
    private Animator _animator;

    [Networked] public bool IsLocalPlayer { get; set; }


    [Header("Components to Disable for Remote Players")]
    public MonoBehaviour[] ComponentsToDisable;
    public Renderer[] RenderersToDisable;

    [Header("GameObjects to Disable for Remote Players")]
    public GameObject[] GameObjectsToDisable;

    public override void Spawned()
    {
        IsLocalPlayer = HasStateAuthority;

        Debug.Log($"Spawned: {gameObject.name}, IsLocalPlayer: {IsLocalPlayer}");

        if (!IsLocalPlayer)
        {
            DisableNonLocalComponents();
        }
        else
        {
            if (PlayerCamera != null)
            {
                PlayerCamera.gameObject.SetActive(true);
                Debug.Log("Local Player Camera activated.");
            }
        }
    }



    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
    }

    private void DisableNonLocalComponents()
    {
        if (_controller != null)
        {
            _controller.enabled = false;
            Debug.Log("CharacterController disabled for remote player.");
        }

        foreach (var component in ComponentsToDisable)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }

        foreach (var renderer in RenderersToDisable)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        foreach (var obj in GameObjectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        Debug.Log($"Non-local components and GameObjects disabled for {gameObject.name}.");
    }

    private void UpdateStates()
    {
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.1f, 0.2f, GroundLayers);

        // Ne passe en natation que si le joueur n'est pas en vol
        if (!IsFlying)
        {
            IsSwimming = Physics.CheckSphere(transform.position, 0.5f, WaterLayer);
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        GetInput(out NetworkInputData input);
        ProcessInput(input);
        if (IsFlying)
        {
            FlyMove();
        }
        else if (IsSwimming)
        {
            Swim();
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

        if (inputData.ToggleFlying)
        {
            IsFlying = !IsFlying;
        }
    }
    private void Swim()
    {
        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;

        forward.y = 0; // Empêche le mouvement vertical
        right.y = 0;

        Vector3 moveDirection = (forward * _inputMove.y + right * _inputMove.x).normalized * SwimSpeed;

        // Ajuste progressivement la position du joueur pour rester à la surface
        float targetY = Mathf.Lerp(transform.position.y, waterSurfaceHeight, SwimSurfaceAdjustmentSpeed * Time.deltaTime);
        Vector3 swimMovement = new Vector3(moveDirection.x, targetY - transform.position.y, moveDirection.z);

        _controller.Move(swimMovement * Runner.DeltaTime);
        // Gestion du son de nage
        if (!audioSource.isPlaying)
        {
            PlaySound(swimSound);
        }
    }

    private void Move()
    {
        float targetSpeed = _inputMove.magnitude > 0.5f ? SprintSpeed : MoveSpeed;

        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 moveDirection = (forward * _inputMove.y + right * _inputMove.x).normalized;
        Vector3 move = moveDirection * targetSpeed * Runner.DeltaTime;

        _controller.Move(move + new Vector3(0, _verticalVelocity, 0) * Runner.DeltaTime);
        // Gestion du son des pas
        if (_inputMove.magnitude > 0 && _isGrounded && !isMoving)
        {
            isMoving = true;
            PlaySoundLoop(footstepSound);
        }
        else if (_inputMove.magnitude == 0 || !_isGrounded)
        {
            isMoving = false;
            StopSound();
        }
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
        // Gestion du son de vol
        if (!isFlyingSoundPlaying)
        {
            isFlyingSoundPlaying = true;
            PlaySoundLoop(flySound);
        }
        _controller.Move(flyMovement);
    }

    private void JumpAndGravity()
    {
        if (IsFlying || IsSwimming)
        {
            _verticalVelocity = 0f;
            return;
            if (_isGrounded)
            {
                _verticalVelocity = -2f;
                if (_inputJump)
                {
                    PlaySound(jumpSound);
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
               
            }
        }
        else
        {
            _verticalVelocity += Gravity * Runner.DeltaTime;
        }

        _controller.Move(new Vector3(0, _verticalVelocity, 0) * Runner.DeltaTime);
    }


    //gravity scale updated
    private float gravityScale = 1.0f;

    private void updateGravityScale()
    {
        if (IsFlying)
            gravityScale = 0.1f;
        else if (IsSwimming)
            gravityScale = 0.2f;
        else
            gravityScale = 1.0f;
        Gravity = -9.81f * gravityScale;
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
            _animator.SetBool("isFlying", IsFlying);
            _animator.SetBool("isGrounded", _isGrounded);
            _animator.SetBool("isWalking", _inputMove.magnitude > 0);
            _animator.SetBool("isSwimming", IsSwimming);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            IsSwimming = true;
            waterSurfaceHeight = other.bounds.max.y;
            PlaySound(waterEnterSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            IsSwimming = false;
            PlaySound(waterExitSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void PlaySoundLoop(AudioClip clip)
    {
        if (clip != null && audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopSound()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void OnDestroy()
    {
        Debug.Log($"Cleaning up ThirdPersonController resources for {gameObject.name}");

        // Désactiver la caméra locale uniquement si elle appartient à ce joueur
        if (IsLocalPlayer && PlayerCamera != null)
        {
            PlayerCamera.gameObject.SetActive(false);
            Debug.Log("Local Player Camera deactivated.");
        }

        // Nettoyage des animations (si nécessaires)
        if (_animator != null)
        {
            _animator.enabled = false;
            Debug.Log("Animator disabled.");
        }

        // Désactiver ou nettoyer les composants réseau
        if (Object != null && Object.HasStateAuthority)
        {
            // Si l'objet possède une autorité réseau, faites un nettoyage spécifique
            Runner.Despawn(Object);
            Debug.Log("Network object despawned.");
        }

        // Désactiver ou réinitialiser tous les composants spécifiques au joueur
        DisableNonLocalComponents();

        // Libérer les références locales
        _controller = null;
        _animator = null;

        Debug.Log("ThirdPersonController fully cleaned up.");
    }


    public struct NetworkInputData : INetworkInput
    {
        public float MoveX;
        public float MoveY;
        public float LookX;
        public float LookY;
        public bool Jump;
        public bool ToggleFlying;
    }
}
