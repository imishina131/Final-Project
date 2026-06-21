using System;
using UnityEngine;

public class CollisionDamager : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.transform.root.GetComponentInChildren<IDamageable>();
        damageable?.Damage(1);
    }
}
