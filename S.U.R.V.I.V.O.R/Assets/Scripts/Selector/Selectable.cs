using System;
using UnityEngine;
using UnityEngine.Events;

public class Selectable: MonoBehaviour
{
    [SerializeField] private UnityEvent onSelected = new UnityEvent();
    [SerializeField] private UnityEvent onDeselected = new UnityEvent();
    public void Selected() => onSelected.Invoke();
    public void Deselected() => onDeselected.Invoke();

    private void Awake()
    {
        Selector.Units.Add(this);
    }

    private void OnDestroy()
    {
        Selector.Units.Remove(this);
    }
}