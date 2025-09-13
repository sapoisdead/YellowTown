using UnityEngine;
using System;

public class PlayerSwimAction : MonoBehaviour
{
    #region Events
    public event EventHandler OnSwimIdle;
    public event EventHandler OnSwim;
    #endregion

    private Rigidbody2D _rb;
    [SerializeField] private GameInput _gameInput;

    [Header("Swim Settings")]
    [SerializeField] private float _buoyancy = 12f;
    [SerializeField] private float _waterDrag = 4f;
    [SerializeField] private float _gravityScaleWater = 10f;

    private Vector2 _moveDir;

    [Header("Timer Fields")]
    [SerializeField] private float _timer = 5f;   
    [SerializeField] private float _timeMultiplier = 2f; 
    private float _timerAtStart;                  
    private bool _timerZero;                      
    private bool _isCountdownActive = false;      

    private float _penalityTime = 0.5f;
    private float _penalityTimeAtStart; 

    public bool IsDead { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Set gravity/drag
        _rb.gravityScale = _gravityScaleWater;
        _rb.drag = _waterDrag;

        // Subscribe to input events
        _gameInput.OnJump += GameInput_OnJump;
        _gameInput.OnMovePressed += GameInput_OnMovePressed;
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        _gameInput.OnJump -= GameInput_OnJump;
        _gameInput.OnMovePressed -= GameInput_OnMovePressed;
    }

    private void Start()
    {
        OnSwimIdle?.Invoke(this, EventArgs.Empty);
        // Cache starting timer
        _timerAtStart = _timer;
        _penalityTimeAtStart = _penalityTime;
        _timerZero = false;
        _isCountdownActive = false;  
    }

    private void Update()
    {
        TimerLogic();

        HandleVisuals();
    }

    private void TimerLogic()
    {
        // If the timer is zero, we go into "recharge" mode
        if (_timerZero)
        {
            _timer += Time.deltaTime * _timeMultiplier;  // Slowly refill the timer
            if (_timer >= _timerAtStart)
            {
                _timer = _timerAtStart;   // Clamp to max
                _timerZero = false;       // No longer at zero
                _isCountdownActive = false;
                // ^ important! we reset countdown so it doesn't start again automatically.
            }
        }
        else
        {
            // If not zero, we do a countdown ONLY if it's active
            if (_isCountdownActive && _timer > 0)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _timer = 0f;
                    _timerZero = true;   // Switch to recharge mode next frame
                }
            }
        }
    }

    private void HandleVisuals()
    {
        if (Mathf.Abs(_rb.velocity.x) > Mathf.Epsilon)
        {
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        if (_moveDir.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(_moveDir.x), 1f);
        }
    }

    private void GameInput_OnMovePressed(object sender, EventArgs e)
    {
        // If the timer is 0, we can't do anything
        if (_timerZero)
        {
            _penalityTime = _penalityTimeAtStart; 
            return;
        }

        // Start the countdown only if not already active
        if (!_isCountdownActive)
        {
            _isCountdownActive = true;
        }

        // Apply a small penalty (immediate -0.5)
        PenalityTimer();

        OnSwim?.Invoke(this, EventArgs.Empty);

        // Perform the swim action

        _moveDir = _gameInput.GetMovementVectorNormalized();
        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        _rb.AddForce(new Vector2(_moveDir.x * _buoyancy, -10  ), ForceMode2D.Impulse);
    }

    private void GameInput_OnJump(object sender, EventArgs e)
    {
        // If the timer is 0, we can't do anything
        if (_timerZero) return;

        // Start the countdown if not active
        if (!_isCountdownActive)
        {
            _isCountdownActive = true;
        }

        PenalityTimer();

        OnSwim?.Invoke(this, EventArgs.Empty);

        // Reset vertical velocity for a smoother jump
        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * _buoyancy, ForceMode2D.Impulse);
    }

    // Immediately subtracts 0.5 from the timer
    private void PenalityTimer()
    { 
        _timer = Mathf.Max(0f, _timer - _penalityTime);
        _penalityTime += 0.25f;
        if (_timer <= 0f)
        {
            _timer = 0f;
            _timerZero = true;  // Next frame, we begin recharging
        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 24;
    //    style.normal.textColor = Color.white;

    //    GUI.Label(new Rect(10, 10, 300, 50),
    //              $"Timer: {_timer:F2}", style);
    //}
}
