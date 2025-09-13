using UnityEngine;

public abstract class BaseCorpseBehaviour<T> : MonoBehaviour, IHealth, IWalkable where T : BaseCorpsePool
{
    public int DamageAmount { get; set; } = 0;
    [SerializeField] protected int _cryticalAmount = 20;
    [SerializeField] protected int _coinAmount = 5; 
    protected Animator _animator;

    protected void Awake()
    {
        _animator = GetComponent<Animator>(); 
    }

    public void ResetDamageAmount()
    {
        DamageAmount = 0;
    }
    public abstract void TakeDamage(int damage);

    public void SetGroundedState()
    {
        PlayerAction.Instance.SetIsGrounded(true);
    }
}