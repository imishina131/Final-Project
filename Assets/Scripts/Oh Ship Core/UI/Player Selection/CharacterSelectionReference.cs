using System.Collections.Generic;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class CharacterSelectionReference : PersistentService<ICharacterSelectionReference>, ICharacterSelectionReference
{
    private Dictionary<IPlayerController, SO_CharacterSelectionData> _characterSelectionDataSet = new();
  
    [Provide, UsedImplicitly] ICharacterSelectionReference GetTransitioner() => this;

    public void SetCharacterModelSelection(IPlayerController player, SO_CharacterSelectionData characterSelectionData)
    {
        _characterSelectionDataSet[player] = characterSelectionData;
    }

    public SO_CharacterSelectionData GetCharacterSelectionData(IPlayerController player)
    {
        return  _characterSelectionDataSet[player];
    }

    public void ClearSelections()
    {
        _characterSelectionDataSet.Clear();
    }
}
