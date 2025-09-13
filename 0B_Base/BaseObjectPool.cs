using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    public ObjectPool<T> ObjectPool { get; set; }

    [SerializeField] protected T _objectPrefab; 
    [SerializeField] protected Transform _spawnPosition; 

    protected void Start()
    {
        ObjectPool = new ObjectPool<T>(InstantiateObject, OnTakeObjectFromPool, OnReturnObjectToPool, OnDestroyObject, true, 30, 60);
    }

    protected T InstantiateObject()
    {
        T obj = Instantiate(_objectPrefab, _spawnPosition.position, Quaternion.identity);
   

        if (obj is IPoolableObject<T> poolableObj)
        {
            poolableObj.SetPool(ObjectPool);
        }
        return obj;
    }

    protected void OnTakeObjectFromPool(T obj)
    {
        obj.transform.SetPositionAndRotation(_spawnPosition.position, Quaternion.identity);

        if (obj is IPoolableObject<T> poolableObj)
        {
            poolableObj.OnObjectSpawned();
            
        }
        obj.gameObject.SetActive(true);
    }

    protected void OnReturnObjectToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    protected void OnDestroyObject(T obj)
    {
        Destroy(obj.gameObject);
    }

    public T GetBulletPrefab() => _objectPrefab;
}


