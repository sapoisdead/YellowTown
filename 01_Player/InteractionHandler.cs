using UnityEngine;
using System;
using System.Collections.Generic;

public class InteractionHandler : MonoBehaviour
{
    public event EventHandler OnInteractionAvailable;
    public event EventHandler OnInteractionUnavailable;

    public event Action<float> OnDrugTaken;
    public event Action<int> OnTakeDamage;
    public event Action<IInventoryItem> OnItemPickedUp;

    [SerializeField] private GameInput _gameInput;

    private readonly List<IInteractable> _interactables = new(); // Handle overlapping interactables
    private IInteractable _currentInteractable;

    private void OnEnable()
    {
        _gameInput.OnInteract += GameInput_OnInteract;
        _gameInput.OnStopInteract += GameInput_OnStopInteract;
    }

    private void OnDisable()
    {
        _gameInput.OnInteract -= GameInput_OnInteract;
        _gameInput.OnStopInteract -= GameInput_OnStopInteract;
    }

    private void GameInput_OnInteract(object sender, EventArgs e)
    {
        if (_currentInteractable != null)
        {
            PlayerAction.Instance.IsInteracting = true;
            _currentInteractable.Interact();
        }
    }

    private void GameInput_OnStopInteract(object sender, EventArgs e)
    {
        if (_currentInteractable != null)
        {
            PlayerAction.Instance.IsInteracting = false;
            _currentInteractable.StopInteract();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out IConsumable consumable))
        {
            float _dose = consumable.OnInteract();
            OnDrugTaken?.Invoke(_dose);
            return;
        }

        if (collider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            int _damage = damageable.Damage;
            OnTakeDamage?.Invoke(_damage);
            return;
        }

        if (collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if (!_interactables.Contains(interactable))
            {
                _interactables.Add(interactable);
            }
            _currentInteractable = interactable;
            OnInteractionAvailable?.Invoke(this, EventArgs.Empty);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            _interactables.Remove(interactable);

            if (_currentInteractable == interactable)
            {
                _currentInteractable = _interactables.Count > 0 ? _interactables[^1] : null;
                if (_currentInteractable == null)
                {
                    OnInteractionUnavailable?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            int _damage = damageable.Damage;
            OnTakeDamage?.Invoke(_damage);
            return;
        }
        else if (collision.gameObject.TryGetComponent(out IInventoryItem inventoryItem))
        {
            OnItemPickedUp?.Invoke(inventoryItem);
            inventoryItem.OnInteract();
            return;
        }
    }
}
