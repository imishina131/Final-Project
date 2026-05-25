using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class TerrainCleanup : MonoBehaviour
{
    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    private void DeleteOldTerrain(GameObject terrain)
    {
        Destroy(terrain);
    }
}
