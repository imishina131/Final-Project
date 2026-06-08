using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SO_CoalData", menuName = "Scriptable Objects/Coal Inputs")]
public class SO_CoalData : ScriptableObject
{
    
   [SerializeField] private InputData[] possibleInputs;
    
    public InputData[] PossibleInputs => possibleInputs;
    
   
    
}

[System.Serializable] 
public class InputData
{
   [SerializeField] private string inputName;
    public string InputName => inputName;
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}
