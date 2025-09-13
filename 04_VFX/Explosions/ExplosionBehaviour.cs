using UnityEngine;

public class ExplosionBehaviour : MonoBehaviour
{
    [SerializeField] private int _explosionDamage;

    private Animator _animator;
    private CircleCollider2D _crclColl;
    private float _radiusSizeAtStart;
    private readonly float _radiusSizeMax = 2f;
    private float _offsetStartY; 
    private readonly float _radiusOffsetY = 2f;
    private float _animationLength;
    private float _timer = 0f; // Tracks elapsed time

    private void Awake()
    {
        _crclColl = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _radiusSizeAtStart = _crclColl.radius;
        _offsetStartY = _crclColl.offset.y;
        _animationLength = GetAnimationLength();

        // Automatically destroy when the animation finishes
        Invoke(nameof(WhenAnimationEndDestroy), _animationLength);
    }

    private void Update()
    {
        ChangeColliderRadiusSizeAndOffset();
    }

    private void ChangeColliderRadiusSizeAndOffset()
    {
        // Prevent timer overflow
        if (_timer > _animationLength)
        {
            _timer = 0f; // Reset if needed
        }

        // Update progress over time
        _timer += Time.deltaTime;

        // Normalize progress (0 to 1)
        float progress = Mathf.Clamp01(_timer / _animationLength);

        // Smoothly interpolate collider radius
        _crclColl.radius = Mathf.Lerp(_radiusSizeAtStart, _radiusSizeMax, progress);

        // Interpolate Y offset properly
        _crclColl.offset = new Vector2(_crclColl.offset.x, Mathf.Lerp(_offsetStartY, _radiusOffsetY, progress));
    }

    private void WhenAnimationEndDestroy()
    {
        Destroy(gameObject);
    }
    public void ResetColliderSizeAndOffset()
    {
        _crclColl.offset = new Vector2(_crclColl.offset.x, _offsetStartY);
        _crclColl.radius = _radiusSizeAtStart;
    }

    private float GetAnimationLength()
    {
        if (_animator.runtimeAnimatorController != null && _animator.runtimeAnimatorController.animationClips.Length > 0)
        {
            return _animator.runtimeAnimatorController.animationClips[0].length;
        }
        return 1f; // Default value if animation is missing
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(_explosionDamage);
            int gradualDamage = Mathf.CeilToInt(_explosionDamage * .3f);
            health.TakeGradualDamage(gradualDamage, 3, 20);
        }
    }
}
