using UnityEngine;
using System;

public enum InputMapping
{
    Player,
    UIMenu,
    Dialogue,
}

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public event EventHandler OnMovePressed; 
    public event EventHandler OnJump;
    public event EventHandler OnAttack;
    public event EventHandler OnInteract;
    public event EventHandler OnStopInteract;
    public event EventHandler OnAttackCanceled;
    public event EventHandler OnCycleSelectItemUIL;
    public event EventHandler OnCycleSelectItemUIR;
    public event EventHandler OnDropSelectedItem;
    public event EventHandler OnUseSelectedItem;
    public event EventHandler OnPauseMenu;
    public Action<float> OnVolumeChange;
    public Action<float> OnCycleSelect;
    public event EventHandler OnSelect;
    public event EventHandler OnBack; 

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.UI.Enable();
        playerInputActions.Dialogue.Enable();
        playerInputActions.UIPauseMenu.Enable();

        playerInputActions.Player.MovePrsd.performed += MovePrsd_performed;
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Attack.performed += Attack_performed;
        playerInputActions.Player.Attack.canceled += Attack_canceled;
        playerInputActions.Dialogue.Interact.performed += Interact_performed;
        playerInputActions.Dialogue.StopInteraction.performed += StopInteraction_performed;

        playerInputActions.Player.CycleSelectItemR.performed += CycleSelectItemR_performed;
        playerInputActions.Player.CycleSelectItemL.performed += CycleSelectItemL_performed;
        playerInputActions.Player.DropSelectedItem.performed += DropSelectedItem_performed;
        playerInputActions.Player.UseSelectedItem.performed += UseSelectedItem_performed;

        playerInputActions.UI.PauseMenu.performed += PauseMenu_performed;

        playerInputActions.UIPauseMenu.Volume.performed += Volume_performed;
        playerInputActions.UIPauseMenu.CycleSelect.performed += CycleSelect_performed;
        playerInputActions.UIPauseMenu.Select.performed += Select_performed;
        playerInputActions.UIPauseMenu.Back.performed += Back_performed;
    }

    //Dialogue INPUTs
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void StopInteraction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnStopInteract?.Invoke(this, EventArgs.Empty);
    }

    //Player INPUTs
    private void MovePrsd_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMovePressed?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        return inputVector;

    }

    private void CycleSelectItemR_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCycleSelectItemUIR?.Invoke(this, EventArgs.Empty);
    }

    private void CycleSelectItemL_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCycleSelectItemUIL?.Invoke(this, EventArgs.Empty);
    }

    private void UseSelectedItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnUseSelectedItem?.Invoke(this, EventArgs.Empty);
    }

    private void DropSelectedItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDropSelectedItem?.Invoke(this, EventArgs.Empty);
    }

    //UI

    private void PauseMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseMenu?.Invoke(this, EventArgs.Empty);
    }

    //SoundMenu

    private void Volume_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnVolumeChange?.Invoke(GetVolumeChange()); 
    }

    private void CycleSelect_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCycleSelect?.Invoke(GetSelectionDirection());
    }

    public float GetVolumeChange()
    {
        float volumeChange;
        volumeChange = playerInputActions.UIPauseMenu.Volume.ReadValue<float>();
        return volumeChange; 
    }

    public float GetSelectionDirection()
    {
        float selectionDirection;
        selectionDirection = playerInputActions.UIPauseMenu.CycleSelect.ReadValue<float>();

        return Mathf.Sign(selectionDirection); 
    }

    private void Select_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSelect?.Invoke(this, EventArgs.Empty);
    }

    private void Back_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnBack?.Invoke(this, EventArgs.Empty);
    }

    //Disable - Enable InputMapping HelperMethods

    public void DisableInputMapping(InputMapping inputMapping)
    {
        switch (inputMapping)
        {
            case InputMapping.Player:
                playerInputActions.Player.Disable();
                break;

            case InputMapping.UIMenu:
                playerInputActions.UIPauseMenu.Disable();
                playerInputActions.Dialogue.Enable();
                playerInputActions.Player.Enable();
                playerInputActions.UI.Enable();
                break;

            case InputMapping.Dialogue:
                playerInputActions.Dialogue.Disable();
                playerInputActions.Player.Enable();
                break;
        }
    }

    public void EnableInputMapping(InputMapping inputMapping)
    {
        switch (inputMapping)
        {
            case InputMapping.Player:
                playerInputActions.Player.Enable();
                break;

            case InputMapping.UIMenu:
                playerInputActions.UIPauseMenu.Enable();
                playerInputActions.Dialogue.Disable();
                playerInputActions.Player.Disable();
                playerInputActions.UI.Disable();
                break;

            case InputMapping.Dialogue:
                playerInputActions.Dialogue.Enable();
                playerInputActions.Player.Disable();
                break;
        }
    }
}
