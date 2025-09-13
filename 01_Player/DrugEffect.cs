using UnityEngine;
using System.Collections;

public class DrugEffect : MonoBehaviour
{
    private float _timeScaleAtStart;
    private float _originalMass;
    private Rigidbody2D _rb;
    private PlayerAction _playerAction;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAction = GetComponent<PlayerAction>();
        _playerHealth = GetComponent<PlayerHealth>(); 
    }

    public void Cocaine(float boost, int damage, float interval, float duration)
    {
        if (_playerAction == null) return;

        _playerAction.IsGravityInvincible = true;
        _playerAction.RunSpeed += boost;
        _playerAction.ClimbSpeed += boost;
        _playerAction.JumpForce += boost;

        _playerHealth.TakeGradualDamage(damage, interval, duration);
        //Prende un damage di 1, ogni 2, per 10 secondi...

        StartCoroutine(CocaineEffectDuration(duration));
    }

    public void SpecialK(float KHoleDuration)
    {
        if (_playerAction == null) return;

        _playerAction.IsGravityInvincible = true;
        _originalMass = _rb.mass; 
        _rb.mass += 5; 
        _playerAction.RunSpeed *= 0.5f;  

        _timeScaleAtStart = Time.timeScale;
        StartCoroutine(KHoleEffectDuration(KHoleDuration));
    }

    private IEnumerator CocaineEffectDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        _playerAction.IsGravityInvincible = false;
        _playerAction.RunSpeed = _playerAction.GetRunSpeedAtStart();
        _playerAction.ClimbSpeed = _playerAction.GetClimbSpeedAtStart();
        _playerAction.JumpForce = _playerAction.GetJumpForceAtStart(); 
    }

    private IEnumerator KHoleEffectDuration(float duration)
    {
        Time.timeScale = 0.5f; 

        yield return new WaitForSecondsRealtime(duration);

        _playerAction.IsGravityInvincible = false;
        _playerAction.RunSpeed *= 2f; 

        _rb.mass = _originalMass;  

        Time.timeScale = _timeScaleAtStart;  
    }
}
