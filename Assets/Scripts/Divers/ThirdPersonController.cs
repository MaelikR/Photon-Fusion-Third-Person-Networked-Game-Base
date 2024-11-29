using UnityEngine;
//using TensorFlow
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float LookSensitivity = 1.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public LayerMask GroundLayers;
    public Camera PlayerCamera;

    [Header("Flying Settings")]
    public float FlySpeed = 5.0f; // Vitesse en mode vol
    private bool _isFlying = false; // Indicateur de mode vol

    public CharacterController _controller;
    private float _verticalVelocity;
    private Vector2 _inputMove;
    private Vector2 _inputLook;
    private bool _inputJump;
    private bool _isGrounded;
    private float _cameraPitch = 0f;
    private bool _isMouseLocked = false;
    private Animator _animator;
    [Header("TensorFlow Settings")]
    //public TextAsset TensorFlowModel; // Add your model here
    // private TFGraph _graph;
    // private TFSession _session;
    // [Header("Networking")]
    public bool IsLocalPlayer = false; // Définit si le joueur est local
    private void OnDestroy()
    {

    }
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            Debug.LogError("CharacterController is missing!");
        }

        _animator = GetComponent<Animator>();

        // Initialize TensorFlow graph and session
        //if (TensorFlowModel != null)
       // {
            //_graph = new TFGraph();
            //_graph.Import(TensorFlowModel.bytes);
         //   _session = new TFSession(_graph);
      //  }
      //  else
      //  {
      //      Debug.LogError("No TensorFlow model assigned!");
       // }

        _animator = GetComponent<Animator>();

        // Désactiver la caméra si ce n'est pas un joueur local
        if (!IsLocalPlayer && PlayerCamera != null)
        {
            PlayerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Ignorer les entrées pour les joueurs distants
        if (!IsLocalPlayer)
        {
            return;
        }

        if (_controller == null || PlayerCamera == null || _animator == null)
        {
            Debug.LogError("Un composant requis est manquant ! Assurez-vous que tous les champs sont assignés.");
            return;
        }

      
        GetInput();
        ToggleMouseLock();
        GroundedCheck();

        if (_isFlying)
        {
            _animator.SetBool("isFlying", true); // Active l'animation de vol
            FlyMove();
        }
        else
        {
            _animator.SetBool("isFlying", false); // Désactive l'animation de vol
            JumpAndGravity();
            Move();
        }

        // Met à jour l'état "isGrounded" dans l'Animator
        _animator.SetBool("isGrounded", _isGrounded);
        Look();
    }
    //
 //   private string ProcessVoiceCommand()
 //   {
        //
 //   }
    private void GetInput()
    {
        // Entrée du déplacement horizontal et vertical
        _inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Entrée de la caméra (souris)
        _inputLook = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * LookSensitivity;

        // Saut
        _inputJump = Input.GetButton("Jump");

        // Basculer en mode vol (touche F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlyingMode();
        }
    }

    private void ToggleFlyingMode()
    {
        _isFlying = !_isFlying;
        _verticalVelocity = 0; // Réinitialise la vitesse verticale pour éviter l'accumulation
    }

    private void ToggleMouseLock()
    {
        if (Input.GetMouseButtonDown(2)) // Clic central
        {
            _isMouseLocked = !_isMouseLocked;

            if (_isMouseLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void GroundedCheck()
    {
        // Vérifie si le joueur est au sol
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.14f, 0.28f, GroundLayers);
    }

    private void Move()
    {
        float targetSpeed = MoveSpeed;
        bool isMoving = _inputMove != Vector2.zero;

        if (isMoving)
        {
            targetSpeed = MoveSpeed;

            if (_inputMove.magnitude > 0.5f) // Intensité du mouvement élevée = course
            {
                _animator.SetBool("isRunning", true);
                targetSpeed = SprintSpeed;
            }
            else
            {
                _animator.SetBool("isRunning", false);
            }

            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", false);
        }

        // Déplacement
        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * _inputMove.y + right * _inputMove.x).normalized;
        Vector3 move = moveDirection * targetSpeed * Time.deltaTime;
        _controller.Move(move + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void FlyMove()
    {
        if (!_isFlying) return;

        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;

        Vector3 moveDirection = forward * _inputMove.y + right * _inputMove.x;
        float flyVertical = 0f;

        if (Input.GetKey(KeyCode.E)) flyVertical = 1; // Monter
        if (Input.GetKey(KeyCode.Q)) flyVertical = -1; // Descendre

        Vector3 flyMovement = (moveDirection + Vector3.up * flyVertical).normalized * FlySpeed * Time.deltaTime;
        _controller.Move(flyMovement);
    }

    private void JumpAndGravity()
    {
        if (_isGrounded && !_isFlying)
        {
            // Réinitialise la vitesse verticale au sol
            _verticalVelocity = -2f;

            if (_inputJump)
            {
                // Calcul de la vélocité du saut
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _animator.SetBool("isJumping", true); // Animation de saut
            }
            else
            {
                _animator.SetBool("isJumping", false); // Stop animation de saut
            }
        }
        else if (!_isGrounded && !_isFlying)
        {
            // Appliquer la gravité
            _verticalVelocity += Gravity * Time.deltaTime;

            // Limiter la vitesse de chute
            if (_verticalVelocity < -50f) // Limite de la vitesse de chute
            {
                _verticalVelocity = -50f;
            }
        }

        // Appliquer la vélocité verticale
        Vector3 verticalMove = new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;
        _controller.Move(verticalMove);
    }


    private void Look()
    {
        // Mouvement horizontal
        transform.Rotate(Vector3.up * _inputLook.x);

        // Mouvement vertical (caméra)
        _cameraPitch -= _inputLook.y;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);

        PlayerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
    }
    private void ExecuteVoiceCommand(string command)
    {

       
    }
}
