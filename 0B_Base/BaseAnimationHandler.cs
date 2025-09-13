using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    IDLE,
    WALK,
    RUN,
    JUMP,
    CLIMB,
    PRE_ATTACK,
    ATTACK,
    SHOOT,
    HIT,
    FALL,
    DEATH,
    DEATH_GROUND,
    DEATH_FINAL,
    PISS,
    UNMASK,
    DEMON,
    SWIM,
    SWIMIDLE,
    JUMPUP,
    JUMPDOWN,
}

[RequireComponent(typeof(Animator))]
public abstract class BaseAnimationHandler<A, H> : MonoBehaviour, IAnimation
    where A : MonoBehaviour, IAction
    where H : MonoBehaviour, IHealth
{
    [Header("Animation Settings")]
    protected Animator _animator;
    protected A _characterAction;
    protected H _characterHealth; 
    protected Dictionary<AnimationState, int> _animationStateToHashMap;
    protected Dictionary<int, float> _animationHashToLengthMap;
    protected float _animatorSpeedAtStart;

    public bool IsPerformingAction { get; set; } //NON CANCELLARE UTILIZZATO DA PLAYER.

    protected virtual void Awake()
    {
        _characterAction = GetComponent<A>();
        _characterHealth = GetComponent<H>();
        if (_characterAction == null)
        {
            Debug.LogError("Component implementing IAction not found!");
        }
        if (_characterHealth == null)
        {
            Debug.LogError("Component implementing IHealth not found!");
        }

        _animator = GetComponent<Animator>();
        InitializeAnimationHashMap();
        InitializeAnimationLengthMap();
    }

    protected virtual void Start()
    {
        _animatorSpeedAtStart = GetAnimatorSpeed();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    protected abstract void InitializeAnimationHashMap();
    protected abstract void InitializeAnimationLengthMap();


    #region Animation Handling

    protected void PlayAnimation(AnimationState state)
    {
        if (_animationStateToHashMap.TryGetValue(state, out int animationHash))
        {
            _animator.Play(animationHash);
        }
        else
        {
            Debug.LogWarning($"Animation state {state} not found in the dictionary.");
        }
    }

    protected virtual void PlayOneShotAnimation(AnimationState state, AnimationState returnToState = AnimationState.IDLE, float speedMultiplier = 1)
    {
        ChangeAnimatorSpeed(speedMultiplier);
        _characterAction.IsPerformingOneShotAction = true; //set flag 
        PlayAnimation(state);
        StopAllCoroutines();
        StartCoroutine(ResetOneShotTrigger(GetAnimationClipLength(state), returnToState));
    }

    protected virtual void PlayDeathAnimation(AnimationState state = AnimationState.DEATH)
    {
        _characterAction.IsPerformingOneShotAction = true; 
        PlayAnimation(state);
        StopAllCoroutines();
        _characterAction.IsDead = true;

        StartCoroutine(TriggerDeath(GetAnimationClipLength(state)));
    }

    #endregion

    #region Animation Utilities

    protected float GetAnimatorSpeed()
    {
        return _animator.speed;
    }

    protected void ChangeAnimatorSpeed(float speedMultiplier)
    {
        _animator.speed = speedMultiplier;
    }

    protected void ResetAnimatorSpeed()
    {
        _animator.speed = _animatorSpeedAtStart;
    }

    protected float GetAnimationClipLength(AnimationState animationName)
    {
        if (_animationStateToHashMap.TryGetValue(animationName, out int animationHash))
        {
            if (_animationHashToLengthMap.TryGetValue(animationHash, out float clipLength))
            {
                return clipLength;
            }
        }
        return 0.5f;
    }

    #endregion

    #region Coroutines

    protected virtual IEnumerator ResetOneShotTrigger(float originalDuration, AnimationState returnToState)
    {
        float adjustedDuration = originalDuration / GetAnimatorSpeed();
        yield return new WaitForSeconds(adjustedDuration);
        _characterAction.IsPerformingOneShotAction = false; //set flag for enemies

        ResetAnimatorSpeed();

        PlayAnimation(returnToState);
    }

    protected virtual IEnumerator TriggerDeath(float originalDuration)
    {
        float adjustedDuration = originalDuration / GetAnimatorSpeed();
        yield return new WaitForSeconds(adjustedDuration);
        IsPerformingAction = false;
        _characterAction.ChangeStateToDeath();
    }

    #endregion
}
