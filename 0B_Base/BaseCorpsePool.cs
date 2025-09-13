using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseCorpsePool : MonoBehaviour
{
    public ObjectPool<GameObject> _deadBodyPool;
    [SerializeField] protected GameObject _deadBodyPrefab;
    [SerializeField] protected float _deadBodyReturnTime = 20f;
    protected Coroutine returnCoroutine; 

    protected virtual void Start()
    {
        _deadBodyPool = new ObjectPool<GameObject>(CreateDeadBody, OnTakeDeadBody, OnReturnDeadBody, OnDestroyDeadBody, true, 40, 60);
    }

    protected virtual GameObject CreateDeadBody()
    {
        GameObject deadBody = Instantiate(_deadBodyPrefab);
        deadBody.SetActive(false);
        return deadBody;
    }

    protected virtual void OnTakeDeadBody(GameObject deadBody)
    {
        deadBody.SetActive(true);
    }

    protected virtual void OnReturnDeadBody(GameObject deadBody)
    {
        StopCoroutine(returnCoroutine);
        deadBody.SetActive(false);
    }

    protected virtual void OnDestroyDeadBody(GameObject deadBody)
    {
        StopCoroutine(returnCoroutine);
        Destroy(deadBody);
    }

    public abstract void SpawnDeadBody(Transform enemyTransform);

    protected virtual IEnumerator ReturnToPoolTimer(GameObject deadBody, bool isDecomposing = false)
    {
        if (isDecomposing)
        {
            //get animator and play animation. 
        }
        else
        {
            yield return new WaitForSeconds(_deadBodyReturnTime);
        }

        // Ensure the object is still active before releasing it
        if (deadBody.activeInHierarchy)
        {
            _deadBodyPool.Release(deadBody);
        }
    }
}
