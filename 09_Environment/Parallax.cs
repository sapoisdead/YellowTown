using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPos;
    private Transform _virtualCamera;
    [SerializeField] private float parallaxFx;

    void Start()
    {
        _virtualCamera = PersistentObjects.GetMainCameraTransform();
        if (_virtualCamera == null) return; 
        startPos = transform.position.x;
    }

    void Update()
    {
        float distance = (_virtualCamera.position.x * parallaxFx);
        transform.position = new Vector2(startPos + distance, transform.position.y);
    }
}
