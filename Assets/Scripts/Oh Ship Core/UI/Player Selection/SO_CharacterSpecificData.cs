using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_CharacterSpecificData", menuName = "Scriptable Objects/Selectable Character Model Data")]
public class SO_CharacterSpecificData : ScriptableObject
{
    [FormerlySerializedAs("characterModelPrefab")]
    [Header("Insert character model for the player's selection here")]
    [SerializeField] GameObject m_characterModelPrefab;
    [SerializeField] LayerMask m_characterPostEffectLayer;
    
    public GameObject CharacterModelPrefab => m_characterModelPrefab;
    public LayerMask CharacterPostEffectLayer => m_characterPostEffectLayer;
}
