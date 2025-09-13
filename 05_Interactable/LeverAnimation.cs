using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public enum LeverAnimationState
{
    IdleOpen,
    IdleClose,
    IdleOpenSelected,
    IdleCloseSelected,
    Open,
    Close,
}

public class LeverAnimation : MonoBehaviour
{
    public event EventHandler OnLeverActioned;

    private Animator _animator;
    private Dictionary<LeverAnimationState, int> _leverAnimationStateToHashMap;
    private Dictionary<int, float> _leverHashToLenghtMap;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        InitializeAnimationHashMap();
        InitializeAnimationLengthMap();

    }

    private void InitializeAnimationHashMap()
    {
        _leverAnimationStateToHashMap = new Dictionary<LeverAnimationState, int>
        {
            {LeverAnimationState.IdleOpen, Animator.StringToHash("LeverIdleOpen")},
            {LeverAnimationState.IdleClose, Animator.StringToHash("LeverIdleClose")},
            {LeverAnimationState.IdleOpenSelected, Animator.StringToHash("LeverIdleOpenSelected")},
            {LeverAnimationState.IdleCloseSelected, Animator.StringToHash("LeverIdleCloseSelected")},
            {LeverAnimationState.Open, Animator.StringToHash("LeverOpen")},
            {LeverAnimationState.Close, Animator.StringToHash("LeverClose")},
        };
    }

    private void InitializeAnimationLengthMap()
    {
        _leverHashToLenghtMap = new Dictionary<int, float>();

        RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;
        for (int i = 0; i < runtimeAnimatorController.animationClips.Length; i++)
        {
            int clipHash = Animator.StringToHash(runtimeAnimatorController.animationClips[i].name);
            float clipLength = runtimeAnimatorController.animationClips[i].length;

            if (!_leverHashToLenghtMap.ContainsKey(clipHash))
            {
                _leverHashToLenghtMap.Add(clipHash, clipLength);
            }
        }
    }

    public void PlayLeverAnimation(LeverAnimationState state)
    {
        if (_leverAnimationStateToHashMap.TryGetValue(state, out int animationHash))
        {
            _animator.Play(animationHash);
        }
        else
        {
            Debug.LogWarning($"Animation state {state} not found in the dictionary.");
        }
    }

    public void PlayLeverAnimationAndEvent(LeverAnimationState state, LeverAnimationState exitState)
    {
        float clipLength = GetAnimationClipLength(state);

        if (_leverAnimationStateToHashMap.TryGetValue(state, out int animationHash))
        {
            _animator.Play(animationHash);
            StartCoroutine(TriggerNextAction(clipLength, exitState));
        }
        else
        {
            Debug.LogWarning($"Animation state {state} not found in the dictionary.");
        }
    }

    private float GetAnimationClipLength(LeverAnimationState state)
    {
        if (_leverAnimationStateToHashMap.TryGetValue(state, out int animationHash))
        {
            if (_leverHashToLenghtMap.TryGetValue(animationHash, out float clipLength))
            {
                return clipLength;
            }
        }
        return 0.5f;
    }

    private IEnumerator TriggerNextAction(float clipLength, LeverAnimationState exitState)
    {
        yield return new WaitForSeconds(clipLength);        
        OnLeverActioned?.Invoke(this, EventArgs.Empty);
        PlayLeverAnimation(exitState);
    }
}
