using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class BaseMenuUI : SuperMenuUI
{
    [SerializeField] protected Button[] _buttons;

    protected override void OnEnable()
    {
        base.OnEnable();
        _gameInput.OnSelect += ClickButton;
        SelectButton(_index); // Select the first button on enable
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _gameInput.OnSelect -= ClickButton;
    }

    protected override void HandleCycleSelection(float direction)
    {
        if (_buttons == null || _buttons.Length == 0) return;

        // Deselect current button
        _buttons[_index].OnPointerExit(null);

        // Update index with wrapping
        _index = (_index + (int)direction + _buttons.Length) % _buttons.Length;

        // Select the new button
        SelectButton(_index);
    }

    protected void SelectButton(int index)
    {
        // Deselect all buttons
        foreach (Button button in _buttons)
        {
            button.OnPointerExit(null);
        }

        _buttons[index].OnPointerEnter(null);
    }

    protected void ClickButton(object sender, EventArgs e)
    {
            // Invoke the click event on the currently selected button
            _buttons[_index].onClick.Invoke();
    }
}
