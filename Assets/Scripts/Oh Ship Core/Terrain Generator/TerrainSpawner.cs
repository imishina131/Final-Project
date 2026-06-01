using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TerrainSpawner : MonoBehaviour
{
    [SerializeField] SO_UsableTerrain terrainContainer;

    private SO_WaterPathingTerrain _currentTileSet;
    
    private Dictionary<string, GameObject> _terrainDictionary;
    
    private readonly TerrainEnvironmentSwaper _terrainSwaper = new TerrainEnvironmentSwaper();
    
    private readonly TerrainSelector _terrainSelector = new TerrainSelector();
    
    public List<GameObject> spawnedTerrains = new List<GameObject>();

    private string _currentTileKey = "0";
    
    private int _terrainContainerIndex = 0;
    
    private bool _terrainHasSwapped = false;

    void Awake()
    {
        InitializeTerrainSet(terrainContainer.terrainTileSets[_terrainContainerIndex]);
       // _terrainHasSwapped = true;
      //  StartCoroutine(ToggleBool());
    }

    void Start()
    {

        if (_terrainDictionary.TryGetValue(_currentTileKey, out GameObject terrain))
        {
            GameObject tile = Instantiate(terrain, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedTerrains.Add(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (spawnedTerrains.Count >= 4)
        {
            if (spawnedTerrains[0] != null)
            {
              
                Destroy(spawnedTerrains[0]);
                spawnedTerrains.RemoveAt(0);
            }
            
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            
            string currentTileKey = _terrainSelector.PickNextTile(_currentTileKey, _currentTileSet.terrainOptions);
            
            if (_terrainDictionary.TryGetValue(currentTileKey, out GameObject terrain))
            {
                Vector3 spawnLocation = spawnedTerrains[^1].transform.Find("Exit Point").position;
                GameObject tile = Instantiate(terrain, Vector3.zero, Quaternion.identity);
                tile.transform.position = spawnLocation - tile.transform.Find("Entry Point").position;
                spawnedTerrains.Add(tile);
                
                Debug.Log($"Spawned Terrain Key: {_currentTileKey}");
                _currentTileKey = currentTileKey;
                
                Debug.Log($"Spawned Terrain Key: {_currentTileKey}");
            }
        }
        
       // if (Time.time % 5 <= .1 && !_terrainHasSwapped)

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            _terrainContainerIndex++;
            SwapTerrainSet(terrainContainer.terrainTileSets[_terrainContainerIndex]);
            //_terrainHasSwapped = true;
           // StartCoroutine(ToggleBool());
        }
    }
    
    private void SwapTerrainSet(SO_WaterPathingTerrain newTerrainSet)
    {
        Debug.Log("Tiles Swap");
        
        _currentTileSet = _terrainSwaper.changeCurrentTerrainTiles(_currentTileSet, newTerrainSet);
        _terrainDictionary = _currentTileSet.possibleTiles;
        _currentTileKey = "0";
        
        
        if (_terrainDictionary.TryGetValue("0", out GameObject terrain))
        {
            Vector3 spawnLocation = spawnedTerrains[^1].transform.Find("Exit Point").position;
            GameObject tile = Instantiate(terrain, Vector3.zero, Quaternion.identity);
            tile.transform.position = spawnLocation - tile.transform.Find("Entry Point").position;
            spawnedTerrains.Add(tile);
        }
    }

    private void InitializeTerrainSet(SO_WaterPathingTerrain newTerrainSet)
    {
        _currentTileSet = _terrainSwaper.changeCurrentTerrainTiles(_currentTileSet, newTerrainSet);
        _terrainDictionary = _currentTileSet.possibleTiles;
        _currentTileKey = "0";
    }

    IEnumerator ToggleBool()
    {
        
        yield return new WaitForSeconds(1f);
        _terrainHasSwapped = !_terrainHasSwapped;
    }
}
