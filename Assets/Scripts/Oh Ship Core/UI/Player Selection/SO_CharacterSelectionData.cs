using UnityEngine;

[CreateAssetMenu(fileName = "SO_CharacterSelectionData", menuName = "Scriptable Objects/Selectable Character Model Data")]
public class SO_CharacterSelectionData : ScriptableObject
{
    [Header("Insert character model for the player's selection here")]
    [SerializeField] private GameObject characterModelPrefab;
    
    public GameObject CharacterModelPrefab => characterModelPrefab;
}
