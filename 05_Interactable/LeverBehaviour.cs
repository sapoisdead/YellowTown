using UnityEngine;
using System;

[RequireComponent(typeof(LeverAnimation))]
public class LeverBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _coins;
    [SerializeField] private LayerMask _playerLayerMask;

    private PlayerHealth _playerHealth;
    private LeverAnimation _leverAnimation;
    private readonly int _damage = 10;
    private bool _isClose = true;

    private void OnDisable()
    {
        if (_leverAnimation != null)
        {
            _leverAnimation.OnLeverActioned -= HandleCoinsActivation;
        }
    }

    private void Awake()
    {
        _leverAnimation = GetComponent<LeverAnimation>();
    }

    private void Start()
    {
        if (PlayerConfiguration.Instance.TryGetComponent(out PlayerHealth playerHealth))
        {
            _playerHealth = playerHealth; 
        }

        _leverAnimation.OnLeverActioned += HandleCoinsActivation;
    }

    public void Interact()
    {
        if (_isClose)
        {
            _leverAnimation.PlayLeverAnimationAndEvent(LeverAnimationState.Open, LeverAnimationState.IdleOpen);
            _isClose = !_isClose;
        }
        else
        {
            _leverAnimation.PlayLeverAnimationAndEvent(LeverAnimationState.Close, LeverAnimationState.IdleClose);
            _isClose = !_isClose;
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (_isClose)
            {
                _leverAnimation.PlayLeverAnimation(LeverAnimationState.IdleCloseSelected);
            }
            else
            {
                _leverAnimation.PlayLeverAnimation(LeverAnimationState.IdleOpenSelected);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((_playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            if (_isClose)
            {
                _leverAnimation.PlayLeverAnimation(LeverAnimationState.IdleClose);
            }
            else
            {
                _leverAnimation.PlayLeverAnimation(LeverAnimationState.IdleOpen);
            }
        }
    }

    private void HandleCoinsActivation(object sender, EventArgs e)
    {
        if (_coins == null)
        {
            Debug.LogWarning("Gameobject not assigned in the Inspector");
            return;
        }
        _coins.SetActive(!_coins.activeSelf);
        _playerHealth.TakeDamage(_damage);
        StopInteract();
    }

    public void StopInteract()
    {
        PlayerAction.Instance.IsInteracting = false;
    }
}
