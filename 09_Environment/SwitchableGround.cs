using UnityEngine;

public class SwitchableGround : MonoBehaviour, IWalkable, IObstacle
{
    private BoxCollider2D _bxColl;
    [SerializeField] private LayerMask _enemyLayer; 

    private void Awake()
    {
        _bxColl = GetComponent<BoxCollider2D>(); 
    }

    public void SetGroundedState()
    {
        PlayerAction.Instance.SetIsGrounded(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _enemyLayer) != 0)
        {
            _bxColl.enabled = false; // Disattiva il collider
        }

    }
}
