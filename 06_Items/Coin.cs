using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int points = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out IWallet wallet))
        {
            wallet.ChangeBalance(points);
            Destroy(gameObject);
        }
    }
}
