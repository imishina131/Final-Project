using UnityEngine;

public interface ICharacterSelectionReference
{
    void SetCharacterModelSelection(IPlayerController player, SO_CharacterSelectionData characterSelectionData);
    
    SO_CharacterSelectionData GetCharacterSelectionData(IPlayerController player);

    public void ClearSelections();
}
