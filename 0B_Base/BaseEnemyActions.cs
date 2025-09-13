using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyState
{
    IDLE,
    WALK,
    INVERTWALK,
    CHASE,
    ATTACK,
    HIT,
    DEATH,
}

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public abstract class BaseEnemyActions : MonoBehaviour, IAction, IObstacle, IPoolableObject, IWalkable
{
    [Header("References")]
    protected Rigidbody2D _rb;
    protected EnemyHealth _enemyHealth;

    [Header("Movement Settings")]
    [SerializeField] protected float _walkingSpeed = 2f;
    [SerializeField] protected float _chasingSpeed = 2f;
    [SerializeField, Range(6, 20)] protected float _maxRandomDistance = 10f;
    [SerializeField, Range(1, 5)] protected float _minRandomDistance = 5f;

    [Header("Combat Settings")]
    [SerializeField] protected float _targetRange = 10f;
    [SerializeField] protected float _attackRange = 2f;
    [SerializeField] protected int _damage = 5;

    [Header("AI Timers")]
    [SerializeField] protected float _stuckTimeThreshold = 1f;

    [Header("State Information")]
    protected EnemyState _state;
    protected bool _isStuck = false;
    protected bool _isMovingToTarget = false;
    public bool IsPerformingOneShotAction { get; set; } = false;
    public bool IsDead { get; set; } = false;

    [Header("Object Pooling")]
    protected ObjectPool<GameObject> _pool;

    [Header("Position Variables")]
    protected Vector2 _currentRandomPosition;
    protected Vector2 _invertedCurrentRandomPosition;
    protected Vector2 _startingPosition;
    protected Vector2 _lastPosition;
    protected Coroutine _stuckCoroutine = null;

    private void OnEnable()
    {
        _enemyHealth.OnTakeDamage += EnemyHealth_OnTakeDamage;
    }
    private void OnDisable()
    {
        _enemyHealth.OnTakeDamage -= EnemyHealth_OnTakeDamage;
    }

    private void EnemyHealth_OnTakeDamage(object sender, System.EventArgs e)
    {
        if (IsDead) return;  // Ignore damage if already dead
        _state = EnemyState.HIT;
    }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _state = EnemyState.IDLE;
    }

    protected virtual void FixedUpdate()
    {
        if (IsDead) return;
        if (_state == EnemyState.DEATH) return;

        UpdateState();

        switch (_state)
        {
            case EnemyState.IDLE:
                HandleIdle();
                break;
            case EnemyState.HIT:
                HandleHit();
                break;
            case EnemyState.WALK:
                HandleWalk();
                break;
            case EnemyState.INVERTWALK:
                HandleInvertWalk();
                break;
            case EnemyState.CHASE:
                HandleChase();
                break;
            case EnemyState.ATTACK:
                HandleAttack();
                break;
            case EnemyState.DEATH:
                HandleDeath();
                break;
        }
    }

    protected void UpdateState()
    {
        if (_state == EnemyState.DEATH) return;
        if (IsPerformingOneShotAction) return;

        Vector2 playerPosition = PlayerAction.Instance.transform.position;
        float distanceToPlayer = DistanceToPlayer(playerPosition);

        switch (_state)
        {
            case EnemyState.IDLE:
                // Se il giocatore è vicino, inizia a inseguirlo
                if (distanceToPlayer <= _targetRange)
                {
                    ChangeState(EnemyState.CHASE);
                }
                else
                {
                    // Se il giocatore è lontano, c'è una probabilità che il nemico inizi a camminare
                    int randomChance = Random.Range(0, 100);
                    if (randomChance < 30)  // 30% di possibilità di camminare
                    {
                        ChangeState(EnemyState.WALK);
                    }
                    else
                    {
                        StartCoroutine(StayInIdleForRandomTime());  // Aspetta un po' prima di decidere nuovamente
                    }
                }
                break;

            case EnemyState.HIT:
                if (distanceToPlayer <= _targetRange && !_isStuck)
                {
                    ChangeState(EnemyState.CHASE);
                }
                else
                {
                    TryEnterIdleState();
                }
                break;
            case EnemyState.INVERTWALK:
                if (distanceToPlayer <= _targetRange && !_isStuck)
                {
                    ChangeState(EnemyState.CHASE);
                }
                break;

            case EnemyState.CHASE:
                if (distanceToPlayer <= _attackRange)
                {
                    ChangeState(EnemyState.ATTACK);
                }
                else if (distanceToPlayer > _targetRange)
                {
                    TryEnterIdleState();
                }
                else if (_isStuck)
                {
                    ChangeState(EnemyState.IDLE);
                }
                break;

            case EnemyState.ATTACK:
                if (distanceToPlayer > _attackRange && !_isStuck)
                {
                    ChangeState(EnemyState.CHASE);
                }
                break;
        }
    }

    protected void HandleIdle()
    {
        int randomNumber = Random.Range(0, 100);

        if (randomNumber < 30)
        {
            ChangeState(EnemyState.WALK);
        }
        else
        {
            StartCoroutine(StayInIdleForRandomTime());
        }
    }

    protected void HandleWalk()
    {
        if (!_isMovingToTarget)
        {
            _currentRandomPosition = GetRandomPosition();
            _isMovingToTarget = true;
        }
        MoveTo(_currentRandomPosition);
    }

    protected void HandleInvertWalk()
    {
        if (!_isMovingToTarget)
        {
            _invertedCurrentRandomPosition = GetInvertedPosition();
            _isMovingToTarget = true;
        }
        MoveTo(_invertedCurrentRandomPosition);
    }

    protected void HandleChase()
    {
        if (_stuckCoroutine == null)
        {
            _stuckCoroutine = StartCoroutine(CheckIfStuckCoroutine());
        }

        if (_isStuck)
        {
            ChangeState(EnemyState.INVERTWALK);
        }
        else
        {
            MoveTo(PlayerAction.Instance.transform.position, _chasingSpeed);
        }
    }

    protected void HandleHit()
    {
        if (IsPerformingOneShotAction)
        {
            return;
        }

        ChangeState(EnemyState.IDLE);
    }

    protected void HandleDeath()
    {
        IsDead = true;
        _enemyHealth.IsInvulnerable = true;
    }

    protected void TryEnterIdleState()
    {
        int randomNumber = Random.Range(0, 100);

        if (randomNumber <= 50)
        {
            ChangeState(EnemyState.IDLE);
        }
    }

    protected Vector2 GetRandomPosition()
    {
        _startingPosition = transform.position;
        int randomDirection = Random.Range(-1, 2);
        float randomDistance = Random.Range(_minRandomDistance, _maxRandomDistance);
        Vector2 randomPosition = _startingPosition + new Vector2(randomDirection * randomDistance, 0);

        FlipSprite(randomDirection);

        return randomPosition;
    }

    protected Vector2 GetInvertedPosition()
    {
        _startingPosition = transform.position;
        float invertedDirection = Mathf.Sign(_rb.position.x) * -1;
        float randomDistance = Random.Range(_minRandomDistance, _maxRandomDistance);
        Vector2 randomInvertedPosition = _startingPosition + new Vector2(invertedDirection * randomDistance, 0);

        FlipSprite(invertedDirection);

        return randomInvertedPosition;
    }

    protected virtual void MoveTo(Vector2 targetPosition, float speedMultiplier = 1)
    {
        Vector2 newPosition = Vector2.MoveTowards(_rb.position, new Vector2(targetPosition.x, _rb.position.y), _walkingSpeed * speedMultiplier * Time.fixedDeltaTime);
        float directionToTarget = targetPosition.x - _rb.position.x;

        FlipSprite(directionToTarget);
        _rb.MovePosition(newPosition);

        if (Vector2.Distance(_rb.position, targetPosition) < _attackRange)
        {
            _isMovingToTarget = false;

            if (_state == EnemyState.INVERTWALK)
            {
                ChangeState(EnemyState.WALK);
            }
        }
    }

    protected void FlipSprite(float direction)
    {
        if (Mathf.Abs(direction) > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction) * -1, 1f, 1f);
        }
    }

    protected abstract void HandleAttack();

    protected float DistanceToPlayer(Vector2 playerPosition)
    {
        return Mathf.Abs(transform.position.x - playerPosition.x);
    }

    public void ChangeState(EnemyState newState)
    {
        // If the enemy is dead, ignore state changes
        if (_state == EnemyState.DEATH)
            return;

        _isMovingToTarget = false;
        _state = newState;

        // Stop stuck coroutine if transitioning out of current state
        if (_stuckCoroutine != null)
        {
            StopCoroutine(_stuckCoroutine);
            _stuckCoroutine = null;
            _isStuck = false;
        }
    }

    public void ChangeStateToDeath()
    {
        _state = EnemyState.DEATH;
        SpawnCorpse(); 
        _pool.Release(gameObject);
    }

    protected abstract void SpawnCorpse();

    public void SetPool(ObjectPool<GameObject> pool)
    {
        _pool = pool;
    }

    public ObjectPool<GameObject> GetPool() => _pool;  

    public void OnObjectActivated()
    {
        IsDead = false;
        _state = EnemyState.WALK;
        _enemyHealth.ResetHealth();
    }

    protected IEnumerator StayInIdleForRandomTime()
    {
        float idleTime = Random.Range(4f, 15f);
        yield return new WaitForSeconds(idleTime);
        ChangeState(EnemyState.WALK);
    }

    protected IEnumerator CheckIfStuckCoroutine()
    {
        _lastPosition = transform.position;

        while (true)
        {
            yield return new WaitForSeconds(_stuckTimeThreshold);

            float distanceMoved = Vector2.Distance(_lastPosition, transform.position);

            if (distanceMoved < 2f && !_isStuck)
            {
                _isStuck = true;
                ChangeState(EnemyState.INVERTWALK);
                break;
            }

            _lastPosition = transform.position;
        }
    }

    public void SetGroundedState()
    {
        PlayerAction.Instance.SetIsGrounded(true);
    }
}