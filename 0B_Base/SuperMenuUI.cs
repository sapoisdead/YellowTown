using UnityEngine;
using TMPro;
using System;

public abstract class SuperMenuUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _titleText;
    [SerializeField] protected PauseMenuUI _pauseMenuUI;
    [SerializeField] protected GameInput _gameInput;

    protected int _index;

    protected virtual void OnEnable()
    {
        _gameInput.OnCycleSelect += HandleCycleSelection;
        _gameInput.OnBack += GameInput_OnBack;
        _titleText.text = UpdateText();
    }

    protected virtual void OnDisable()
    {
        _gameInput.OnCycleSelect -= HandleCycleSelection;
        _gameInput.OnBack -= GameInput_OnBack;
    }

    protected abstract string UpdateText();

    protected abstract void HandleCycleSelection(float direction); 

    protected virtual void GameInput_OnBack(object sender, EventArgs e)
    {
        _pauseMenuUI.ShowMainMenu();
    }
}
