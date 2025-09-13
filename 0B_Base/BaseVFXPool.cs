using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseVFXPool : MonoBehaviour
{
    protected ObjectPool<GameObject> _fxPool;
    [SerializeField] protected GameObject _fxPrefab;
    [SerializeField] protected float _fxReturnTime = 5f; 

    protected virtual void Start()
    {
        _fxPool = new ObjectPool<GameObject>(CreateEffect, OnTakeEffect, OnReturnEffect, OnDestroyEffect, true, 40, 60);
    }

    protected virtual GameObject CreateEffect()
    {
        GameObject effect = Instantiate(_fxPrefab);
        effect.SetActive(false);
        return effect;
    }

    protected virtual void OnTakeEffect(GameObject fx)
    {
        fx.SetActive(true);
    }

    protected virtual void OnReturnEffect(GameObject fx)
    {
        fx.SetActive(false);
    }

    protected virtual void OnDestroyEffect(GameObject fx)
    {
        Destroy(fx);
    }

    public abstract void SpawnEffect(Vector2 position);

protected virtual IEnumerator ReturnToPoolTimer(GameObject fx, bool fade)
{
    if (fade)
    {
        SpriteRenderer spriteRenderer = fx.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break; // Evita errori se il componente è stato distrutto

        Color spriteColor = spriteRenderer.color;
        float startAlpha = spriteColor.a;

        for (float t = 0.0f; t < _fxReturnTime; t += Time.deltaTime)
        {
            float normalizedTime = t / _fxReturnTime;
            spriteColor.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            if (spriteRenderer != null) // Controllo extra
                spriteRenderer.color = spriteColor;
            yield return null;
        }
    }
    else
    {
        yield return new WaitForSeconds(_fxReturnTime);
    }

    if (fx != null && fx.activeInHierarchy) 
    {
        _fxPool.Release(fx);
    }
}



    public void ReleaseEffect(GameObject fx)
    {
        _fxPool.Release(fx);
    }

    #region UtilityMethod

    protected void ResetFXSpriteAlpha(GameObject fx)
    {
        SpriteRenderer spriteRenderer = fx.GetComponent<SpriteRenderer>();
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1; // Reset dell'alpha
        spriteRenderer.color = spriteColor;
    }

    protected void ResetFXVelocity(GameObject fx)
    { 
        Rigidbody2D rb= fx.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    protected Vector2 GetSpriteLowBoundPosition(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Vector2 lowBoundPosition = new(spriteRenderer.bounds.center.x, spriteRenderer.bounds.min.y);
        return lowBoundPosition;
    }

    #endregion

}



