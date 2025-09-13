using UnityEngine;
using System.Collections;
using System;

public class PlayerAttack : MonoBehaviour
{
    public event EventHandler OnShoot;
    public event EventHandler OnAttack;

    [Header("Melee Stats")]
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private Vector2 _attackOffset;
    [SerializeField] private LayerMask _attackMask;

    [Header("Weapon Stats")]
    public int Damage { get; set; }
    [SerializeField] private float _cooldown;
    [SerializeField] private int _weight;

    [Header("References")]
    private Rigidbody2D _rb;
    private AmmoAndStaminaManager _ammoAndStaminaManager;
    private PlayerBulletPool _playerBulletPool;

    // Attack action, set by weapon type (melee or gun)
    private Action _currentAttack;
    private bool _isAttacking = false;  //Flag to check if the player is attacking

    [SerializeField] private GameInput _gameInput; 

    private void OnEnable()
    {
        _gameInput.OnAttack += GameInput_OnAttack;
        _gameInput.OnAttackCanceled += GameInput_OnAttackCanceled;
    }

    private void OnDisable()
    {
        _gameInput.OnAttack -= GameInput_OnAttack;
        _gameInput.OnAttackCanceled -= GameInput_OnAttackCanceled;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerBulletPool = GetComponent<PlayerBulletPool>();
        _ammoAndStaminaManager = GetComponent<AmmoAndStaminaManager>();
    }

    private void GameInput_OnAttack(object sender, EventArgs e)
    {
        if (_isAttacking || PlayerAction.Instance.IsInteracting) return; // Prevent another attack during cooldown
        StartCoroutine(CoolDownCorroutine(_cooldown)); // Start cooldown
    }

    public void SetAttack(Action attackAction)
    {
        _currentAttack = attackAction;
    }

    private void GameInput_OnAttackCanceled(object sender, EventArgs e)
    {
        // Handle logic when attack input is canceled, such as resetting animations
    }

    private void GunAttack()
    {
        OnShoot?.Invoke(this, EventArgs.Empty);

        int bulletCount = _ammoAndStaminaManager.Ammo;

        if (bulletCount > 0)
        {
            //Processo un po' arzigogolato per cambiare il damage del proiettile
            Bullet bullet = _playerBulletPool.GetBulletPrefab();
            bullet.Damage= Damage;

            _playerBulletPool.ObjectPool.Get();  
            _ammoAndStaminaManager.DecreaseAmmo();  
        }
    }

    private void MeleeAttack()
    {
        OnAttack?.Invoke(this, EventArgs.Empty);

        PlayerAction player = PlayerAction.Instance;

        Vector3 position = player.transform.position;

        position += _attackOffset.x * player.transform.localScale.x * player.transform.right;
        position += player.transform.up * _attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(position, _attackRange, _attackMask);

        if (colInfo != null && colInfo.TryGetComponent(out IHealth enemyHealth))
        {
            enemyHealth.TakeDamage(Damage);  // Use the weapon's damage
        }
    }

    // This function is called by Inventory or Weapon system to set the weapon's stats
    public void SetWeaponStats(int damage, float cooldown, int weight)
    {
        Damage = damage;   // Set weapon damage from WeaponSO
        _cooldown = cooldown;     // Set weapon cooldown from WeaponSO
        _weight = weight;         // Set weapon weight from WeaponSO

    }

    public void SetWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            SetAttack(null);  
            return;
        }

        SetWeaponStats(weapon.BaseDamage, weapon.CoolDown, weapon.Weight);

        switch (weapon)
        {
            case FireWeaponSO:
                SetAttack(GunAttack);
                break;
            case MeleeWeaponSO:
                SetAttack(MeleeAttack);
                break;
        }
    }

    IEnumerator CoolDownCorroutine(float coolDownTimer)
    {
        _isAttacking = true;
        _rb.velocity = Vector2.zero; 
        _currentAttack?.Invoke(); 
        yield return new WaitForSeconds(coolDownTimer);
        _isAttacking = false; 
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        position += _attackOffset.x * transform.localScale.x * transform.right;
        position += transform.up * _attackOffset.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, _attackRange);
    }
}
