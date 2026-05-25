using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TerrainSpawner : MonoBehaviour
{
    [SerializeField] SO_WaterPathingTerrain usableTerrain;
    //[SerializeField] bool useCustomSpawnLocation;
    
    public static Action<GameObject> OnCleanupTerrain;
    //private int terrainCount = 0;
    private List<GameObject> _spawnedTerrains = new List<GameObject>();
    
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
        if (_spawnedTerrains.Count >= 3)
        {
           // OnCleanupTerrain?.Invoke(oldTerrain);
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Spawning Terrain");
            GameObject currentTerrain = usableTerrain.waterPaths[Random.Range(0, usableTerrain.waterPaths.Length)];
            Instantiate(currentTerrain, transform.position, transform.rotation);
            
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
