using System; 
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : BaseAnimationHandler<PlayerAction, PlayerHealth>
{
    private PlayerAttack _playerAttack;
    private PlayerSwimAction _playerSwimAction; 
    private readonly float _shootingSpeed= 1.2f;

    private EventHandler _onIdleHandler;
    private EventHandler _onRunHandler;
    private EventHandler _onJumpHandler;
    private EventHandler _onClimbHandler;
    private EventHandler _onAttackHandler;
    private EventHandler _onShootHandler;
    private EventHandler _onHitHandler;
    private EventHandler _onFallHandler;
    private EventHandler _onSwimHandler;
    private EventHandler _onSwimIdleHandler;

    private void OnEnable()
    {
        if (_characterAction == null || _characterHealth == null || _playerAttack == null || _playerSwimAction == null)
        {
            Debug.LogWarning("Cannot subscribe to events, components not initialized yet!");
            return;
        }

        SubscribeToPlayerEvent();
    }

    private void OnDisable()
    {
        if (_characterAction == null || _characterHealth == null || _playerAttack == null || _playerSwimAction == null)
        {
            Debug.LogWarning("Cannot unsubscribe, some components are missing!");
            return;
        }
        UnsubscribeToPlayerEvent();
    }

    protected override void InitializeAnimationHashMap()
    {
       _animationStateToHashMap = new Dictionary<AnimationState, int>
        {
            {AnimationState.IDLE, Animator.StringToHash("Idle")},
            {AnimationState.RUN, Animator.StringToHash("Run")},
            {AnimationState.JUMP, Animator.StringToHash("Jump")},
            {AnimationState.CLIMB, Animator.StringToHash("Climb")},
            {AnimationState.ATTACK, Animator.StringToHash("Attack_Axe")},
            {AnimationState.SHOOT, Animator.StringToHash("Attack_Gun")},
            {AnimationState.HIT, Animator.StringToHash("Hit")},
            {AnimationState.FALL, Animator.StringToHash("Fall")},
            {AnimationState.PISS, Animator.StringToHash("Piss")},
            {AnimationState.UNMASK, Animator.StringToHash("Unmask")},
            {AnimationState.DEMON, Animator.StringToHash("Demonic_Idle")},
            {AnimationState.SWIM, Animator.StringToHash("Swim")},
            {AnimationState.SWIMIDLE, Animator.StringToHash("Swim_Idle")},
        };
    }

    protected override void InitializeAnimationLengthMap()
    {
        _animationHashToLengthMap = new Dictionary<int, float>();

        RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;
        for (int i = 0; i < runtimeAnimatorController.animationClips.Length; i++)
        {
            int clipHash = Animator.StringToHash(runtimeAnimatorController.animationClips[i].name);
            float clipLength = runtimeAnimatorController.animationClips[i].length;

            if (!_animationHashToLengthMap.ContainsKey(clipHash))
            {
                _animationHashToLengthMap.Add(clipHash, clipLength);
            }
        }
    }

    protected override void Awake()
    {
        _playerAttack = GetComponent<PlayerAttack>();
        _playerSwimAction = GetComponent<PlayerSwimAction>();
        base.Awake(); 
    }

    protected override void Start()
    {
        base.Start();
        SubscribeToPlayerEvent();
    }

    private void SubscribeToPlayerEvent()
    {
        _onIdleHandler = (sender, e) => PlayAnimation(AnimationState.IDLE);
        _onRunHandler = (sender, e) => PlayAnimation(AnimationState.RUN);
        _onJumpHandler = (sender, e) => PlayOneShotAnimation(AnimationState.JUMP);
        _onClimbHandler = (sender, e) => PlayAnimation(AnimationState.CLIMB);
        _onFallHandler = (sender, e) => PlayAnimation(AnimationState.FALL);
        _onHitHandler = (sender, e) => PlayOneShotAnimation(AnimationState.HIT);
        _onAttackHandler = (sender, e) => PlayOneShotAnimation(AnimationState.ATTACK);
        _onShootHandler = (sender, e) => PlayOneShotAnimation(AnimationState.SHOOT, AnimationState.IDLE, _shootingSpeed);
        _onSwimIdleHandler = (sender, e) => PlayAnimation(AnimationState.SWIMIDLE);
        _onSwimHandler = (sender, e) => PlayAnimation(AnimationState.SWIM);

        // Subscribe to CharacterAction Events
        _characterAction.OnIdle += _onIdleHandler;
        _characterAction.OnRun += _onRunHandler;
        _characterAction.OnJump += _onJumpHandler;
        _characterAction.OnClimb += _onClimbHandler;
        _characterAction.OnFall += _onFallHandler;

        // Subscribe to Health Events
        _characterHealth.OnTakeDamage += _onHitHandler;

        // Subscribe to Attack Events
        _playerAttack.OnAttack += _onAttackHandler;
        _playerAttack.OnShoot += _onShootHandler;

        // Subscribe to Swim Events
        _playerSwimAction.OnSwimIdle += _onSwimIdleHandler;
        _playerSwimAction.OnSwim += _onSwimHandler;
    }

    private void UnsubscribeToPlayerEvent()
    {
        // Unsubscribe from CharacterAction Events
        _characterAction.OnIdle -= _onIdleHandler;
        _characterAction.OnRun -= _onRunHandler;
        _characterAction.OnJump -= _onJumpHandler;
        _characterAction.OnClimb -= _onClimbHandler;
        _characterAction.OnFall -= _onFallHandler;

        // Unsubscribe from Health Events
        _characterHealth.OnTakeDamage -= _onHitHandler;

        // Unsubscribe from Attack Events
        _playerAttack.OnAttack -= _onAttackHandler;
        _playerAttack.OnShoot -= _onShootHandler;

        // Unsubscribe from Swim Events
        _playerSwimAction.OnSwimIdle -= _onSwimIdleHandler;
        _playerSwimAction.OnSwim -= _onSwimHandler;
    }

}
