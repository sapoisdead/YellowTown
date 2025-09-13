using UnityEngine;
using System.Collections;

public abstract class BaseHealth : MonoBehaviour, IHealth
{
    [SerializeField] protected int _health = 100;
    [SerializeField] protected int _maxHealth = 100;

    public bool IsAlive { get; set; } = true;
    public bool IsInvulnerable { get; set; } = false;
    protected Coroutine _gradualDamageCoroutine; // Per gestire la coroutine del danno graduale

    protected virtual int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, _maxHealth); }
    }

    protected virtual void Start()
    {
        ResetHealth();
    }

    public virtual void ResetHealth()
    {
        Health = _maxHealth;
        IsAlive = true;
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsInvulnerable || !IsAlive) return; // Evita danno se invulnerabile o morto
        ApplyDamage(damage);
    }

    public virtual void TakeGradualDamage(int damage, float interval, float duration)
    {
        if (!IsAlive) return; // Evita danno se l'entità è già morta

        // Ferma eventuali coroutine in corso prima di avviarne una nuova
        if (_gradualDamageCoroutine != null)
        {
            StopCoroutine(_gradualDamageCoroutine);
        }

        _gradualDamageCoroutine = StartCoroutine(GradualDamageCoroutine(damage, interval, duration));
    }

    protected virtual void ApplyDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0 && IsAlive)
        {
            Health = 0;
            IsAlive = false;
            Death();
        }
    }

    protected abstract void Death();

    public virtual void Heal(int amount)
    {
        if (!IsAlive) return; // Non curare un'entità morta

        Health += amount;
        if (Health > 0 && !IsAlive)
        {
            IsAlive = true; // Resuscita l'entità se curata
        }
    }

    public virtual void IncreaseMaxHealth(int amount)
    {
        _maxHealth += amount;
        Health = _maxHealth;
    }

    public int GetHealth()
    {
        return Health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    protected IEnumerator InvulnerabilityCoroutine(float duration = 1)
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(duration);
        IsInvulnerable = false;
    }

    protected virtual IEnumerator GradualDamageCoroutine(int damage, float interval, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (IsAlive)
            {
                ApplyDamage(damage); // Infliggi danno e controlla la morte

                if (!IsAlive) yield break; // Interrompi se l'entità è morta
            }
            else
            {
                yield break; // Ferma la coroutine se l'entità è morta
            }

            yield return new WaitForSeconds(interval); // Attendi l'intervallo
            elapsedTime += interval; // Aumenta il tempo trascorso
        }

        // Fine del danno graduale
        _gradualDamageCoroutine = null; // Resetta la variabile quando la coroutine è finita
    }
}
