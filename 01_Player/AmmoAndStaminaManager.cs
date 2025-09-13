using UnityEngine;
using System; 

public class AmmoAndStaminaManager : MonoBehaviour
{
    public event Action<int> OnAmmoChanged;

    public int Ammo
    {
        get => _ammo;
        set {
                int clampedValue = Mathf.Clamp(value, 0, _maxAmmo);
                if (_ammo != clampedValue)  // Only trigger event if ammo changes
                {
                    _ammo = clampedValue;
                    OnAmmoChanged?.Invoke(_ammo); // Invoke event with new ammo value
                }
            }
    }

    [SerializeField] private int _ammoAtStart = 30;
    [SerializeField] private int _maxAmmo = 30;
    private int _ammo;

    private float _coolDownTimer;
    private float _staminaTimer;

    private void Start()
    {
        _ammo = _ammoAtStart;
    }

    private void Update()
    {
        if (_coolDownTimer > 0)
        {
            _coolDownTimer -= Time.deltaTime;
            if (_coolDownTimer <= 0)
            {
                CooldownFinished();
            }
        }

        if (_staminaTimer > 0)
        {
            _staminaTimer -= Time.deltaTime;
            if (_staminaTimer <= 0)
            {
                StaminaRecoveryFinished();
            }
        }
    }

    // Ammo management
    public void DecreaseAmmo() => Ammo--; 
    public void RefillAmmo(int amount) => Ammo += amount; 

    // Cooldown timer management
    public void ResetCoolDownTimer(float time) => _coolDownTimer = time;
    private void CooldownFinished()
    {
   
    }

    // Stamina timer management
    public void ResetStaminaTimer(float time) => _staminaTimer = time;
    private void StaminaRecoveryFinished()
    {

    }
}
