using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerConfiguration : MonoBehaviour
{
    public static PlayerConfiguration Instance { get; private set; }

    private Rigidbody2D _rb;
    private CapsuleCollider2D _cpslCol;
    private PlayerAction _playerAction;
    private PlayerSwimAction _playerSwimAction;
    private SpriteRenderer _spriteRenderer; 
    [SerializeField] GameInput _gameInput;
    [SerializeField] private ScriptableRendererFeature _waterFullScreen;

    [Header("Spawn Positions")]
    private Vector2 _waterSpawnPos = new(-5, -6f);
    private Vector2 _vempaSpawnPos = new(-6, -2.2f);
    private Vector2 _defaultSpawnPos = new(-12, -6f);


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("There's more than one Instance of PlayerConfiguration!");
        }
        else { Instance = this; }

        _rb = GetComponent<Rigidbody2D>();
        _cpslCol = GetComponent<CapsuleCollider2D>();
        _playerAction = GetComponent<PlayerAction>();
        _playerSwimAction = GetComponent<PlayerSwimAction>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    public void DefaultConfig()
    {
        _waterFullScreen.SetActive(false);
        if(_rb != null)
        {
            _rb.gravityScale = _playerAction.GetGravityScaleAtStart();
            _rb.drag = _playerAction.GetDragAtStart(); 
        }

        _playerAction.enabled = true;
        _playerSwimAction.enabled = false;
        SetPlayerStartingPosition(_defaultSpawnPos);
        EnablePlayer(true);
    }

    public void WaterConfig()
    {
        _waterFullScreen.SetActive(true);
        if(_rb!= null)
        {
            _rb.gravityScale = 10;
            _rb.velocity = Vector2.zero;
        }
        _playerAction.enabled = false;
        _playerSwimAction.enabled = true;
        SetPlayerStartingPosition(_waterSpawnPos);
        EnablePlayer(true);
    }

    public void VempaConfig()
    {
        if (_rb != null)
        {
            _rb.gravityScale = _playerAction.GetGravityScaleAtStart();
            _rb.drag = _playerAction.GetDragAtStart();
            _rb.velocity = Vector2.zero;
        }
        Time.timeScale = 1f;

        SetPlayerStartingPosition(_vempaSpawnPos);
        EnablePlayer(true);
    }

    private void SetPlayerStartingPosition(Vector3 spawnPos)
    {
        float playerHeight = _cpslCol.bounds.extents.y;
        transform.position = spawnPos + new Vector3(0, playerHeight, 0);
    }

    public void EnablePlayer(bool onOff)
    {
        _spriteRenderer.enabled = onOff;
        _gameInput.enabled = onOff;
    }
}
