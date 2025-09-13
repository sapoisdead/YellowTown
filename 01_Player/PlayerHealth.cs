using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : BaseHealth
{
    public event EventHandler OnDeath;
    public event EventHandler OnTakeDamage;
    public event EventHandler OnTakeGradualDamage;
    public event EventHandler OnEndGradualDamage; 
    public event EventHandler OnHeal; 
    public event EventHandler OnIncreaseMaxHealth;
    public event EventHandler OnActivateInvulnerability;

    private InteractionHandler _interactionHandler;

    private void OnEnable()
    {
        _interactionHandler.OnTakeDamage += InteractionHandler_OnTakeDamage;
    }

    private void OnDisable()
    {
        _interactionHandler.OnTakeDamage -= InteractionHandler_OnTakeDamage;
    }

    private void Awake()
    {
        _interactionHandler = GetComponent<InteractionHandler>();
    }

    private void InteractionHandler_OnTakeDamage(int damage)
    {
        TakeDamage(damage);
    }

    public override void TakeDamage(int damage)
    {
        OnTakeDamage?.Invoke(this, EventArgs.Empty);
        base.TakeDamage(damage);
    }

    protected override void Death()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        OnHeal?.Invoke(this, EventArgs.Empty);
    }

    public override void IncreaseMaxHealth(int amount)
    {
        base.IncreaseMaxHealth(amount);
        OnIncreaseMaxHealth?.Invoke(this, EventArgs.Empty);
    }

    private void ActivateInvulnerability(float duration)
    {
        OnActivateInvulnerability?.Invoke(this, EventArgs.Empty);
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    protected override IEnumerator GradualDamageCoroutine(int damage, float interval, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (IsAlive)
            {
                OnTakeGradualDamage?.Invoke(this, EventArgs.Empty);

                ApplyDamage(damage); 

                if (!IsAlive) yield break;
            }
            else
            {
                yield break; 
            }

            yield return new WaitForSeconds(interval); 
            elapsedTime += interval; 
        }

        _gradualDamageCoroutine = null;
        OnEndGradualDamage?.Invoke(this, EventArgs.Empty);
    }
}
