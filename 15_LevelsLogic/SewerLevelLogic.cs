using UnityEngine;

public class SewerLevelLogic : MonoBehaviour
{
    private PlayerHealth _playerHealth;
    private readonly int _damage = 10;
    public float OxygenLevel { get; set; }
    public float MaxOxygenLevel { get; set; } = 60f;

    private void Start()
    {
        OxygenLevel = MaxOxygenLevel; 

        if (PlayerConfiguration.Instance.TryGetComponent(out PlayerHealth playerHealth))
        {
            _playerHealth = playerHealth;
        }
    }

    private void Update()
    {
        OxygenLevel -= Time.deltaTime;

        if (OxygenLevel <= 0)
        {
            _playerHealth.TakeDamage(_damage);
        }
    }
}


