using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CoinAnimationSelector : MonoBehaviour
{
    private Animator _animator;

    private readonly string[] _animations = { "Idle", "Idle2" };

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayRandomAnimation();
    }

    private void PlayRandomAnimation()
    {
        // Scegli un'animazione casuale dall'array
        int randomIndex = Random.Range(0, _animations.Length);
        _animator.Play(_animations[randomIndex]);
    }
}
