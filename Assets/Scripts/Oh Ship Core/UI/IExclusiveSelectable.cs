using UnityEngine.EventSystems;

public interface IExclusiveSelectable
{
    bool IsLockedFor(BaseInputModule module);
}