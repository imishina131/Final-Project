using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MenuButtonSelection : IPlayerSelection
{
    public Transform Transform { get; }
    public bool AllowsMultipleSelectors { get; } = true;
    
    [SerializeField] private UnityEvent addSelector;
    
    public bool TryAddSelector(IPlayerControllable selector)
    {
        addSelector.Invoke();
        return true;
    }

    public bool TryRemoveSelector(IPlayerControllable selector)
    {
        return false;
    }

    public bool IsSelectedBy(IPlayerControllable selector)
    {
        return false;
    }

    public bool IsAvailableTo(IPlayerControllable selector)
    {
        return true;
    }
}
