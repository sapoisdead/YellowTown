using UnityEngine;
using System;

public class PlayerAction : MonoBehaviour, IAction
{
    #region Singleton
    public static PlayerAction Instance { get; private set; }
    #endregion

    #region Events
    public event EventHandler OnIdle;
    public event EventHandler OnRun;
    public event EventHandler OnClimb;
    public event EventHandler OnJump;
    public event EventHandler OnFall;
    public event EventHandler OnDeathGround;
    public event EventHandler OnDeathDivoured; 
    #endregion

    #region Serialized Fields
    [Header("Linked Scripts")]
    [SerializeField] private Inventory _inventory;
    [SerializeField] private GameInput _gameInput;

    [Header("Actions Data")]
    public float RunSpeed { get; set; } = 8f;
    public float ClimbSpeed { get; set; } = 5f;
    public float JumpForce { get; set; } = 26f;
    [SerializeField] private float _fallDeathThreshold = -36f;

    [Header("Ground & Ladder Detection")]
    [SerializeField] private float _rayDistance = 1f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Vector2 _boxColliderSize = new(0.7f, 2);
    [SerializeField] private LayerMask _ladderMask;
    #endregion

    #region Private Fields
    private float _jumpForceAtStart;
    private float _climbSpeedAtStart;
    private float _runSpeedAtStart;
    private float _gravityScaleAtStart;
    private float _dragAtStart; 

    private Vector2 _moveInput;
    private Rigidbody2D _rb;

    private readonly float _gravityScale = 10;
    private readonly float _linearDrag = 0;

    private bool _isGrounded;
    private bool _canClimb;
    #endregion

    #region Public Properties
    public bool IsDead { get; set; } = false;
    public bool IsPerformingOneShotAction { get; set; } = false;
    public bool IsGravityInvincible { get; set; } = false;
    public bool IsJumping { get; set; } = false;
    public bool IsInteracting { get; set; } = false;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _gameInput.OnJump += GameInput_OnJump;
    }

    private void OnDisable()
    {
        _gameInput.OnJump -= GameInput_OnJump;
    }

    private void Start()
    {
        LoadPlayerConfiguration();
    }

    private void LoadPlayerConfiguration()
    {
        _gravityScaleAtStart = _gravityScale;
        _dragAtStart = _linearDrag; 
        _runSpeedAtStart = RunSpeed;
        _climbSpeedAtStart = ClimbSpeed;
        _jumpForceAtStart = JumpForce;
    }

    private void Update()
    {
        if (IsDead) return;

        Run();
        ClimbLadder();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        _moveInput = _gameInput.GetMovementVectorNormalized();
        CheckGround();
        CheckFatalFall();
        CheckLadder();
    }
    #endregion

    #region Movement
    private void Run()
    {
        if (IsPerformingOneShotAction) return;

        Vector2 playerVelocity = new(_moveInput.x * RunSpeed, _rb.velocity.y);
        _rb.velocity = playerVelocity;

        if (_isGrounded)
        {
            if (Mathf.Abs(_rb.velocity.x) > Mathf.Epsilon)
            {
                FlipSprite();
                OnRun?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnIdle?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void ClimbLadder()
    {
        if (!_canClimb || IsJumping)
        {
            _rb.gravityScale = _gravityScaleAtStart;
            return;
        }

        _rb.velocity = new Vector2(_rb.velocity.x, _moveInput.y * ClimbSpeed);
        _rb.gravityScale = 0f;

        if (Mathf.Abs(_rb.velocity.y) > Mathf.Epsilon)
        {
            OnClimb?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnIdle?.Invoke(this, EventArgs.Empty);
        }
    }

    private void FlipSprite()
    {
        transform.localScale = new Vector2(Mathf.Sign(_rb.velocity.x), 1f);
    }
    #endregion

    #region Input Handling
    private void GameInput_OnJump(object sender, EventArgs e)
    {
        if (!_isGrounded) return;

        IsJumping = true;
        _rb.velocity += new Vector2(0f, JumpForce);
        OnJump?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Collision Checks
    private void CheckGround()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _rayDistance, _groundMask);
        if (_isGrounded) IsJumping = false;
    }

    private void CheckLadder()
    {
        _canClimb = false;
        Collider2D hit = Physics2D.OverlapBox(transform.position, _boxColliderSize, 0, _ladderMask);
        if (hit == null) return;

        if (hit.TryGetComponent(out IClimbable climbable))
        {
            climbable.CanClimb();
        }
    }

    private void CheckFatalFall()
    {
        if (IsGravityInvincible) return;

        float fallingVelocity = _rb.velocity.y;

        if (fallingVelocity < _fallDeathThreshold && !IsDead)
        {
            OnFall?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0.3f;
        }

        if (fallingVelocity >= 0 && Time.timeScale < 1f && _isGrounded)
        {
            IsDead = true;
            OnDeathGround?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1f;
        }
    }

    public void DeathDivoured()
    {
        OnDeathDivoured?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Utility Methods
    public void ChangeStateToDeath()
    {
        _inventory.ClearInventory();
    }
    #endregion

    #region Getters & Setters
    public void SetIsGrounded(bool isGrounded) => _isGrounded = isGrounded;
    public void SetCanClimb(bool canClimb) => _canClimb = canClimb;
    public float GetRunSpeedAtStart() => _runSpeedAtStart;
    public float GetClimbSpeedAtStart() => _climbSpeedAtStart;
    public float GetJumpForceAtStart() => _jumpForceAtStart;
    public float GetGravityScaleAtStart() => _gravityScaleAtStart;
    public float GetDragAtStart() => _dragAtStart;
    #endregion

    #region Debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * _rayDistance); // Ground check raycast

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _moveInput * _rayDistance); // Collision check raycast
    }
    #endregion
}
