using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExclusiveButton : Button, IExclusiveSelectable
{
    BaseInputModule m_currentOwner;
    public bool IsLockedFor(BaseInputModule module) => m_currentOwner != null && m_currentOwner != module;
    public override void OnSelect(BaseEventData eventData)
    {
        BaseInputModule selectingModule = eventData.currentInputModule;
        if (IsLockedFor(selectingModule))
        {
            if (eventData is not AxisEventData axisEventData) return;
            Selectable next = FindNextAvailable(axisEventData.moveDir, selectingModule);
            if (next != null) EventSystem.current.SetSelectedGameObject(next.gameObject, eventData);
            return;
        }
        m_currentOwner = selectingModule;
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (eventData.currentInputModule == m_currentOwner) m_currentOwner = null;
        base.OnDeselect(eventData);
    }
    
    Selectable FindNextAvailable(MoveDirection dir, BaseInputModule requestingModule)
    {
        HashSet<Selectable> visited = new() { this };
        Selectable current = this;
        while (true)
        {
            Selectable next = dir switch
            {
                MoveDirection.Left => current.FindSelectableOnLeft(),
                MoveDirection.Right => current.FindSelectableOnRight(),
                MoveDirection.Up => current.FindSelectableOnUp(),
                MoveDirection.Down => current.FindSelectableOnDown(),
                _ => null
            };
            if (next == null || !visited.Add(next)) return null;
            if (next is not IExclusiveSelectable lockable || !lockable.IsLockedFor(requestingModule)) return next;
            current = next;
        }
    }
}