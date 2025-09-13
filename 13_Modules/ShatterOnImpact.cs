using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DestructionTimer))]
public class ShatterOnImpact : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private GameObject _shatterPrefab;
    private readonly float _defaultAnimationDuration = 0.5f; 

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private float GetClipDurationByName(string clipName)
    {
        RuntimeAnimatorController controller = _animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length; 
            }
        }

        Debug.LogWarning("Animation clip not found: " + clipName);
        return _defaultAnimationDuration;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float breakAnimationDuration = GetClipDurationByName("Break");
        Vector2 contactPoint = collision.contacts[0].point; 
        StartCoroutine(PlayAnimationAndInstantiate(breakAnimationDuration, "Break", contactPoint));
    }

    private IEnumerator PlayAnimationAndInstantiate(float animationDuration, string animationName, Vector2 contactPoint)
    {

        _animator.Play(animationName);
 
        yield return new WaitForSeconds(animationDuration);

        if (_shatterPrefab != null && contactPoint != null)
        {
            Instantiate(_shatterPrefab, contactPoint, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
