using System;
using UnityEngine;
using UnityEngine.Splines;

public class UnderbellyFollow : MonoBehaviour
{
    [SerializeField] private GameObject _playerSteamBoat;
    [SerializeField] private float yOffset;
    private Rigidbody m_rigidbody;


    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = _playerSteamBoat.transform.position;
        targetPosition.y -= yOffset; 
        m_rigidbody.MovePosition(targetPosition);
        //m_rigidbody.MoveRotation(_playerSteamBoat.transform.rotation);
    }
}
