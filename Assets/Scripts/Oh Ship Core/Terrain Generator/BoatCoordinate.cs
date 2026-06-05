using System;
using UnityEngine;
using UnityEngine.Events;

public class BoatCoordinate : MonoBehaviour
{
    [SerializeField] private float rayCastLength = 100f;
    private RaycastHit _hit;
    private GameObject _currentTerrainTile;
    private GameObject _lastTerrainTile;

    private int _allowedLayerMasks;

    public GameObject CurrentTerrainTile => _currentTerrainTile;
    public static event Action OnTerrainTileChange;
    void Awake()
    {
        DisableChildCollidersFromRayCast();
    }
    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, Vector3.down * rayCastLength, Color.red);
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out _hit, rayCastLength, _allowedLayerMasks))
        {
            _currentTerrainTile = _hit.collider.gameObject;
            if (_currentTerrainTile != _lastTerrainTile)
            {
                _lastTerrainTile = _currentTerrainTile;
                OnTerrainTileChange?.Invoke();
            }
        }
    }

    private void DisableChildCollidersFromRayCast()
    {
        int ignoreLayers = 1 << gameObject.layer;
        Collider[] childrenColliders = GetComponentsInChildren<Collider>();
        
        foreach (Collider childCollider in childrenColliders)
        {
            ignoreLayers |= (1 << childCollider.gameObject.layer);
        }
        
        _allowedLayerMasks = ~ignoreLayers;
    }

    public bool ShipHasChangedTerrainTiles()
    {
        return true;
    }
}
