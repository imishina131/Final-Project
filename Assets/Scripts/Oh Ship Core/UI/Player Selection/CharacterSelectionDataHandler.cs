using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionDataHandler : PersistentService<ICharacterSelectionDataHandler>, ICharacterSelectionDataHandler
{
    [SerializeField] private string m_characterSelectionSceneName;
    readonly Dictionary<int, SO_CharacterSpecificData> m_characterSelectionDataSet = new();
    readonly HashSet<SO_CharacterSpecificData> m_selectedCharacters = new();
    [Provide, UsedImplicitly] ICharacterSelectionDataHandler GetTransitioner() => this;
    [Inject] ISceneTransitioner m_sceneTransitioner;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool TrySetCharacterSelectionData(int playerIndex, SO_CharacterSpecificData characterSpecificData)
    {
        Debug.Log($"TrySetCharacterSelectionData called for player: {playerIndex}, data: {characterSpecificData?.name}");
        if (m_selectedCharacters.Contains(characterSpecificData)) return false;
        m_characterSelectionDataSet[playerIndex] = characterSpecificData;
        bool result = m_selectedCharacters.Add(characterSpecificData);
        Debug.Log($"TrySetCharacterSelectionData called for player: {playerIndex}, data: {characterSpecificData?.name}");
        return result;
    }
    public bool HasSelection(int playerIndex) => m_characterSelectionDataSet.ContainsKey(playerIndex);
    public bool TryGetCharacterSelectionData(int playerIndex, out SO_CharacterSpecificData characterSpecificData) => m_characterSelectionDataSet.TryGetValue(playerIndex, out characterSpecificData);
    public bool ClearCharacterSelectionData(int playerIndex) => m_characterSelectionDataSet.Remove(playerIndex, out SO_CharacterSpecificData data) && m_selectedCharacters.Remove(data);
    public SO_CharacterSpecificData GetCharacterSelectionData(int player) => m_characterSelectionDataSet[player];

    public void ClearSelections()
    {
        Debug.Log($"ClearSelections called, stack: {System.Environment.StackTrace}");
        m_characterSelectionDataSet.Clear();
        m_selectedCharacters.Clear();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name}, characterSelectionSceneName: {m_characterSelectionSceneName}, match: {scene.name == m_characterSelectionSceneName}");
        if (scene.name == m_characterSelectionSceneName)
        {
            ClearSelections();
        }
    }
}
