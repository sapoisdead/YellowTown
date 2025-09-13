using UnityEngine;
using System;

public abstract class BaseInteractableAnimation : MonoBehaviour
{
    protected Animator _animator;
    protected InteractionHandler _interactionHandler;

    protected int _idleHash;
    protected int _interactableHash;

    protected virtual void OnDisable()
    {
        if (_interactionHandler != null)
        {
            _interactionHandler.OnInteractionAvailable -= OnInteractionAvailable;
            _interactionHandler.OnInteractionUnavailable -= OnInteractionUnavailable;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_interactionHandler != null)
        {
            _interactionHandler.OnInteractionAvailable -= OnInteractionAvailable;
            _interactionHandler.OnInteractionUnavailable -= OnInteractionUnavailable;
        }
    }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError($"Animator component missing on {gameObject.name}");
        }
        AnimationToHash();
    }

    protected virtual void Start()
    {
        if (PlayerAction.Instance.TryGetComponent(out InteractionHandler handler))
        {
            _interactionHandler = handler;
            _interactionHandler.OnInteractionAvailable += OnInteractionAvailable;
            _interactionHandler.OnInteractionUnavailable += OnInteractionUnavailable;
        }
        else
        {
            Debug.LogError($"InteractionHandler not found on PlayerAction: {PlayerAction.Instance.name ?? "null"}.");
        }
    }

    protected virtual void AnimationToHash()
    {
        _idleHash = Animator.StringToHash("Idle");
        _interactableHash = Animator.StringToHash("Interactable");
    }

    protected virtual void OnInteractionAvailable(object sender, EventArgs e)
    {
        PlayInteractAnimation();
    }

    protected virtual void OnInteractionUnavailable(object sender, EventArgs e)
    {
        PlayIdle();
    }

    protected void PlayInteractAnimation()
    {
        _animator.Play(_interactableHash);
    }

    protected void PlayIdle()
    {
        _animator.Play(_idleHash);
    }
}
