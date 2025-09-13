using UnityEngine;
using System.Collections;

public class MayorBrugnaroLevelLogic : MonoBehaviour
{
    [Header("SerializeFields")]
    [SerializeField] private BrugnaroHealth _brugnaroHealth;
    [SerializeField] private GameObject _deathBrugnaro;
    [SerializeField] private BoxCollider2D _wallLimit;
    [SerializeField] private GameObject _visualObstacle;

    [SerializeField] private GameObject[] _items;
    private GameObject _spawnedItem; 
    private float _spawnDelay = 20f;
    private Coroutine _spawner;


    private void OnEnable()
    {
        _brugnaroHealth.OnDeath += BrugnaroHealth_OnDeath; 
    }

    private void OnDisable()
    {
        _brugnaroHealth.OnDeath -= BrugnaroHealth_OnDeath;
    }

    private void Start()
    {
        _spawner = StartCoroutine(SpawnAtTime());
    }

    private void BrugnaroHealth_OnDeath(object sender, System.EventArgs e)
    {
        Vector2 spawnPos = _brugnaroHealth.gameObject.transform.position;
        Instantiate(_deathBrugnaro, spawnPos, Quaternion.identity);
        _wallLimit.isTrigger = true;
        _visualObstacle.SetActive(false);
        if (_spawner != null) StopCoroutine(_spawner);
        if (_spawnedItem != null) Destroy(_spawnedItem);

    }

    private IEnumerator SpawnAtTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnDelay);
            if(_spawnedItem != null)Destroy(_spawnedItem);

            int _casualItem = Random.Range(0, _items.Length);
            int _casualX = Random.Range(-16, 2);
            Vector2 spawnPos = new(_casualX, -6f);

            _spawnedItem = Instantiate(_items[_casualItem], spawnPos, Quaternion.identity);
        }
    }

}
