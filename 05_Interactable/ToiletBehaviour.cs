using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ToiletBehaviour : BaseInteractableAnimation,IInteractable
{
    [SerializeField] private SceneField _sceneField;
    [SerializeField] private BoxCollider2D _toiletOpenTrigger;
    private int _flushHash;
    private int _toiletCloseHash;
    private Coroutine _loadBonusLevelCorroutine;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _toiletSfx;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (_loadBonusLevelCorroutine != null)
        {
            Debug.LogWarning("Interaction already in progress.");
            return;
        }
        PlayerConfiguration.Instance.EnablePlayer(false);
        _loadBonusLevelCorroutine = StartCoroutine(WaitAnimationEnd());
    }

    public void StopInteract()
    {
        PlayerAction.Instance.IsInteracting = false; 
    }

    protected override void OnInteractionAvailable(object sender, EventArgs e)
    {
        base.OnInteractionAvailable(sender, e);
        _toiletOpenTrigger.enabled = true;
    }

    protected override void OnInteractionUnavailable(object sender, EventArgs e)
    {
        _animator.Play(_toiletCloseHash);
        _toiletOpenTrigger.enabled = false;
    }

    protected override void AnimationToHash()
    {
        base.AnimationToHash();
        _flushHash = Animator.StringToHash("Flush");
        _toiletCloseHash = Animator.StringToHash("ToiletClose");
    }

    private IEnumerator WaitAnimationEnd()
    {
        _animator.Play(_flushHash);
        _audioSource.PlayOneShot(_toiletSfx); 
        yield return new WaitForSeconds(GetClipLengthByHash(_flushHash));
        StopInteract();
        LevelManager.LoadScene(_sceneField);
    }

    private float GetClipLengthByHash(int animationHash)
    {
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == animationHash)
            {
                return clip.length;
            }
        }
        Debug.LogWarning($"Animation clip with hash {animationHash} not found. Returning default length.");
        return 1f; // Default fallback length
    }

}
