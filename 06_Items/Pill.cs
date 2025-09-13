using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pill : MonoBehaviour, IConsumable
{
    Rigidbody2D _rb;
    [SerializeField] private float _dose = 5f;
    [SerializeField, Tooltip("Destroy Delay Time")] private float _ddt = 0.5f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public float OnInteract()
    {
        _rb.AddForce(Vector2.up * 360f);
        Destroy(gameObject, _ddt);
        return _dose;
    }
}
