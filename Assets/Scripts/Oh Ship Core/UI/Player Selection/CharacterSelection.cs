using System;
using UnityEngine;
[Serializable]
public class CharacterSelection : IPlayerSelection
{
    [field:SerializeField] public Transform Transform { get; private set; }
    
    [SerializeField] private SO_CharacterSelectionData characterPrefab;
    public bool AllowsMultipleSelectors => false;

    public IPlayerControllable m_currentSelector;

    public bool TryAddSelector(IPlayerControllable selector)
    {
        if (m_currentSelector != null) return false;
        m_currentSelector = selector;
        return true;
    }

    public bool TryRemoveSelector(IPlayerControllable selector)
    {
        if (m_currentSelector == selector)return false;
        m_currentSelector = null;
        return true;
    }
    
    public bool IsSelectedBy(IPlayerControllable selector) => m_currentSelector == selector;
    public bool IsAvailableTo(IPlayerControllable selector)=> m_currentSelector == null || m_currentSelector == selector;
    
    public bool IsConfirmed => m_currentSelector != null;
    
    public SO_CharacterSelectionData CharacterData => characterPrefab;
}