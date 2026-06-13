using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterSelectionHandler : MonoBehaviour, IMenuHandler, IDependencyProvider
{
    [SerializeReference, ClassSelector] List<IPlayerSelection> m_playerSelections;
    [Provide, UsedImplicitly] IMenuHandler Provide() => this;
    
    [Inject] ICharacterSelectionReference m_characterSelectionReference;

    [SerializeField] private UnityEvent m_sceneChange;
    private void Start()
    {
        FindAnyObjectByType<Injector>().Inject(this);
        m_characterSelectionReference.ClearSelections();
        Debug.Log("Cleared Character Selections");
    }

    public bool TryConfirmSelection(IPlayerControllable selector, IPlayerSelection target)
    {
        Debug.Log("TryConfirmSelection called");

       if (target == null) return false;
        
       if(!target.TryAddSelector(selector)) return false;

       if (target is not CharacterSelection characterSelection) return false;
       m_characterSelectionReference.SetCharacterModelSelection(selector.GetActivePlayerController(), characterSelection.CharacterData);
       int confirmedCount = 0;
       foreach (var c in m_playerSelections)
       {
           if (c is CharacterSelection cs && cs.IsConfirmed)
           {
               confirmedCount++;
               Debug.Log("Confirmed " + confirmedCount);
           }
              
       }

       if (confirmedCount == 2)
       {
           Debug.Log("Characters confirms");
           SceneManager.LoadScene("Build Scene");
       }

       return true;
    }
    
    
    
    public bool TryCancelSelection(IPlayerControllable selector, IPlayerSelection current) => current?.TryRemoveSelector(selector) ?? false;

    public bool TryGetNextAvailableSelection(
        IPlayerControllable selector,
        IPlayerSelection currentSelection,
        Vector2 direction,
        out IPlayerSelection nextSelection)
    {
        nextSelection = null;
        if (m_playerSelections.Count == 0) return false;
        int currentIndex = m_playerSelections.IndexOf(currentSelection);
        int step = direction.x > 0f ? 1 : -1;
        int start = currentIndex == -1 ? 1 : Mathf.Clamp(currentIndex + step, 0, m_playerSelections.Count - 1);
        for (int i = start; i >= 0 && i < m_playerSelections.Count; i += step)
        {
            IPlayerSelection candidate = m_playerSelections[i];
            if (candidate == currentSelection) break;
            if (!candidate.AllowsMultipleSelectors && !candidate.IsAvailableTo(selector)) continue;
            nextSelection = candidate;
            
            return true;
        }
        return false;
    }

    public IPlayerSelection GetDefaultSelectionZone()
    {
        return m_playerSelections[1];
    }
}