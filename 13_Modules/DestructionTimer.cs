using UnityEngine;
using System;
using System.Collections;

public class DestructionTimer : MonoBehaviour
{
    public event EventHandler OnTimerEnd;
    [SerializeField] private float _destructionTimer = 3f;

    void Start()
    {
        StartCoroutine(SetDestructionTimer());
    }

    private IEnumerator SetDestructionTimer()
    {
        yield return new WaitForSeconds(_destructionTimer);
        OnTimerEnd?.Invoke(this, EventArgs.Empty);
    }
}
