using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseProjectile<T> : MonoBehaviour, IPoolableObject<T> where T : BaseProjectile<T>, IDamageable
{
    public int Damage { get; set; } = 10; 
    [SerializeField] protected float _projectileSpeed = 20f;
    [SerializeField] protected float _projectileDestroyTime = 2f;
    [SerializeField] protected float _projectileLaunchAngle = 0.4f;

    protected Rigidbody2D _rb;
    protected ObjectPool<T> _pool;
    protected bool _isReleased;  

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        _isReleased = false; 
        OnObjectSpawned();
        StartCoroutine(DeactivateProjectileAfterTime());
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent(out IHealth health))
        {
            health.TakeDamage(Damage);
        };

        ReleaseProjectile();
    }

    public void SetPool(ObjectPool<T> pool)
    {
        _pool = pool;
    }

    public abstract void OnObjectSpawned();

    protected virtual IEnumerator DeactivateProjectileAfterTime()
    {
        yield return new WaitForSeconds(_projectileDestroyTime);
        if (!_isReleased)  // Verifica che non sia già stato rilasciato
        {
            ReleaseProjectile();
        }
    }

    protected void ReleaseProjectile()
    {
        if (!_isReleased)  // Verifica che non sia già stato rilasciato
        {
            _isReleased = true;  
            _pool.Release(this as T);  // Rilascia l'oggetto nella pool
        }
    }

}
