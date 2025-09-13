using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private float _minExplosionForce = 1f; 
    [SerializeField] private float _maxExplosionForce = 3f; 

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); 
    }

    private void OnEnable()
    {
        Invoke(nameof(Explode), 0.3f);
    }

    public void Explode()
    {
        float angle = Random.Range(75f, 105f); //indica un angolo con ujn ampiezza di 30 gradi rivolto verso l'alto... quindi 0 (sinistra) 180 destra.
        float radians = angle * Mathf.Deg2Rad; // Convert degrees to radians

        // Calculate the explosion direction
        Vector2 explosionDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;

        // Apply random force to the debris
        _rb.AddForce(explosionDirection * Random.Range(_minExplosionForce, _maxExplosionForce), ForceMode2D.Impulse);
    }
}
