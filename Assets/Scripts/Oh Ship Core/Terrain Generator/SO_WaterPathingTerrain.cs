using UnityEngine; 
using MatrixUtils.GenericDatatypes;

[System.Serializable]
public class TerrainOptions
{
    public string currentTileKey;
    public string[] tileOptions;
}


[CreateAssetMenu(fileName = "WaterPathData", menuName = "Scriptable Objects/Terrain")]
public class SO_WaterPathingTerrain : ScriptableObject
{
    public SerializableDictionary<string, GameObject> possibleTiles = new();
    public TerrainOptions[] terrainOptions;
    public string startingTileKey;
}
