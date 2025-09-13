using UnityEngine;
using System; 

public class FloatEffect : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _initialPosition;

    [SerializeField] private float _fluctuationSpeed = 1f;
    [SerializeField] private float _fluctuationRange = 0.5f;
    [SerializeField] private DestructionTimer _destructionTimer;

    private bool _isFloating = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _initialPosition = _rb.position;

        if (_destructionTimer != null)
        {
            _destructionTimer.OnTimerEnd += HandleTimerEnd;
        }
    }

    private void OnDisable()
    {
        if (_destructionTimer != null)
        {
            _destructionTimer.OnTimerEnd -= HandleTimerEnd;
        }
    }

    private void HandleTimerEnd(object sender, EventArgs e)
    {
        _isFloating = false; 
    }

    private void FixedUpdate()
    {
        if (!_isFloating) return;

        float newYPosition = _initialPosition.y + Mathf.Sin(Time.time * _fluctuationSpeed) * _fluctuationRange;
        _rb.MovePosition(new Vector2(_initialPosition.x, newYPosition));
    }
}
