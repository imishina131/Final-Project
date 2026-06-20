using UnityEngine;

[CreateAssetMenu(fileName = "SO_CookableFoodData", menuName = "Scriptable Objects/Food Data")]
public class SO_CookableFoodData : ScriptableObject
{
    [SerializeField] private SerializableDictionary<CookState, float> cookTimeThresholds;
}
