using UnityEngine;
using UnityEngine.Pool;

public interface IPoolableObject<T> where T : MonoBehaviour
{
    void SetPool(ObjectPool<T> pool);
    void OnObjectSpawned();
}

public interface IPoolableObject
{
    void SetPool(ObjectPool<GameObject> pool);
}