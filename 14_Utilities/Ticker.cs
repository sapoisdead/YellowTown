using UnityEngine;

public class Ticker : MonoBehaviour
{
    [SerializeField] private float _tickFrequency = 0.2f;

    private float _tickTimer;

    public delegate void TickAction();

    public static event TickAction OnTickAction; 


    private void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= _tickFrequency)
        {
            _tickTimer = 0;
            OnTickAction?.Invoke();
        }
    }
}
