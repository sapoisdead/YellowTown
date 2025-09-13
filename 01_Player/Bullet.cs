using UnityEngine;

public class Bullet : BaseProjectile<Bullet>, IPoolableObject<Bullet>, IDamageable
{

    [SerializeField] private float _launchAngle = 0.4f;
    public override void OnObjectSpawned()
    {
        _rb.velocity = Vector2.zero;
        Vector2 direction = new (PlayerAction.Instance.transform.localScale.x, _launchAngle);
        transform.localScale = new Vector3(direction.x, 1, 1); 
        _rb.AddForce(direction * _projectileSpeed, ForceMode2D.Impulse);
    }
}
