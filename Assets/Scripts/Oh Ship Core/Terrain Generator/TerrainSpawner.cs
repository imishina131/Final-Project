using UnityEngine;
using UnityEngine.InputSystem;
public class TerrainSpawner : MonoBehaviour
{
    [SerializeField] SO_WaterPathingTerrain usableTerrain;
    //[SerializeField] bool useCustomSpawnLocation;
    
    void Start()
    {
        
        
        
        /*if (!useCustomSpawnLocation)
        {
            Instantiate(testObject, new Vector3(0,0,0), Quaternion.identity);
        }
        else
        {
            Instantiate(testObject, new Vector3(0, 0, 0), Quaternion.identity);
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Spawning Terrain");
            GameObject testObject = usableTerrain.waterPaths[Random.Range(0, usableTerrain.waterPaths.Length)];
            Instantiate(testObject, transform.position, transform.rotation);
        }
    }
    
    /*private void OnValidate()
    {
        if (useCustomSpawnLocation)
        {
            SpawnCustomLocationMarker();
        }
        else
        {
            
            RemoveCustomLocationMarker();
            
        }
    }

    private void RemoveCustomLocationMarker()
    {
        throw new System.NotImplementedException();
    }

    private void SpawnCustomLocationMarker()
    {
        throw new System.NotImplementedException();
    }*/
}
