using System;
using UnityEngine;

public class Wallet : MonoBehaviour, IWallet
{
    [SerializeField] private int _balance = 0;
    private readonly int _maxBalance = 100000;

    public int Balance
    {
        get => _balance;
        private set
        {
            int clampedValue = Mathf.Clamp(value, 0, _maxBalance);

            if (_balance != clampedValue) // Attiva solo se il valore cambia
            {
                _balance = clampedValue;
                OnBalanceChanged?.Invoke(_balance); // Assicura che l'evento si attivi sempre

                if (_balance == 0)
                    OnBalanceEmpty?.Invoke();
            }
        }
    }

    public event Action<int> OnBalanceChanged;
    public event Action OnBalanceEmpty;

    public void ChangeBalance(int points)
    {
        Balance += points; // Usa la proprietà Balance per applicare i limiti e attivare eventi
    }

    public bool HasEnoughPoints(int points)
    {
        return _balance >= points;
    }

    public bool IsBroke()
    {
        return _balance == 0;
    }
}
