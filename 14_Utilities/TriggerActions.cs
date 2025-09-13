using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerActions : MonoBehaviour
{
    private BoxCollider2D _boxColl;

    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private BusBehaviour _objToChange;
    [SerializeField] private Vector2 _nextPos;
    [SerializeField] private Vector2 _previousPos;

    private void Awake()
    {
        _boxColl = GetComponent<BoxCollider2D>();
        _boxColl.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0 && _objToChange != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            HandleTriggerDirection(direction);
        }
    }

    private void HandleTriggerDirection(Vector2 dir)
    {
        float tolerance = 0.1f;

        if (dir.x < -tolerance)
        {
            _objToChange.SetNewStartPosition(_nextPos);
        }
        else if (dir.x > tolerance)
        {
            _objToChange.SetNewStartPosition(_previousPos);
        }
    }
}
