using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

public class CharacterSelectorButton : Button
{
    [SerializeField] SO_CharacterSpecificData m_characterData;
    [SerializeField] public UnityEvent<int> OnCharacterSelectedSuccessfully = new();
    [SerializeField] public UnityEvent<int> OnCharacterSelectionFailed = new();
    [SerializeField] public UnityEvent<int> OnCharacterDeselected = new();
    [Inject] ICharacterSelectionDataHandler m_characterSelectionDataHandler;
    
    int m_ownerPlayerIndex = -1;
    bool IsLockedIn => m_ownerPlayerIndex != -1;

    public override void OnSubmit(BaseEventData eventData)
    {
        if (!TryGetPlayerInput(eventData, out PlayerInput playerInput)) return;
        if (IsLockedIn)
        {
            if (playerInput.playerIndex != m_ownerPlayerIndex) return;
            m_characterSelectionDataHandler.ClearCharacterSelectionData(m_ownerPlayerIndex);
            m_ownerPlayerIndex = -1;
            interactable = true;
            OnCharacterDeselected.Invoke(playerInput.playerIndex);
            return;
        }
        if (m_characterSelectionDataHandler.TrySetCharacterSelectionData(playerInput.playerIndex, m_characterData))
        {
            m_ownerPlayerIndex = playerInput.playerIndex;
            interactable = false;
            OnCharacterSelectedSuccessfully.Invoke(playerInput.playerIndex);
            Debug.Log("Successfully set character selection");
            base.OnSubmit(eventData);
        }
        else
        {
            OnCharacterSelectionFailed.Invoke(playerInput.playerIndex);
        }
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (IsLockedIn && TryGetPlayerInput(eventData, out PlayerInput playerInput) && playerInput.playerIndex == m_ownerPlayerIndex) return;
        base.OnMove(eventData);
    }
    
    static bool TryGetPlayerInput(BaseEventData eventData, out PlayerInput playerInput)
    {
        if (eventData.currentInputModule is InputSystemUIInputModule { } inputModule && inputModule.transform.root.GetComponent<PlayerInput>() is { } input)
        {
            playerInput = input;
            return true;
        }
        playerInput = null;
        return false;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(CharacterSelectorButton))]
public class CharacterSelectorButtonEditor : ButtonEditor
{
    SerializedProperty m_characterData;
    SerializedProperty m_onCharacterSelectedSuccessfully;
    SerializedProperty m_onCharacterSelectionFailed;
    SerializedProperty m_onCharacterDeselected;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_characterData = serializedObject.FindProperty("m_characterData");
        m_onCharacterSelectedSuccessfully = serializedObject.FindProperty("OnCharacterSelectedSuccessfully");
        m_onCharacterSelectionFailed = serializedObject.FindProperty("OnCharacterSelectionFailed");
        m_onCharacterDeselected = serializedObject.FindProperty("OnCharacterDeselected");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_characterData);
        EditorGUILayout.PropertyField(m_onCharacterSelectedSuccessfully);
        EditorGUILayout.PropertyField(m_onCharacterSelectionFailed);
        EditorGUILayout.PropertyField(m_onCharacterDeselected);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
#endif