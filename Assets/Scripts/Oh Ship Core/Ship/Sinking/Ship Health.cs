using System.Collections.Generic;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Pool;

public class ShipHealth : MonoBehaviour, IDamageable
{
    [SerializeField] List<Transform> m_holePositions;
    RandomBag<Transform> m_availableHoles;
    [SerializeField] ShipHole m_holePrefab;
    IObjectPool<ShipHole> m_shipHoles;
    [SerializeField] Observer<float> m_fillPercentage = new(0);
    uint m_holeCount;
    void Awake()
    {
        m_availableHoles = new(m_holePositions);
        m_shipHoles = new ObjectPool<ShipHole>
        (
            createFunc: () => Instantiate(m_holePrefab)
        );
        m_fillPercentage.Notify();
    }
    void Update()
    {
        m_fillPercentage.Value = Mathf.Clamp01(m_holeCount > 0 ? m_fillPercentage +(m_holeCount * 0.005f * Time.deltaTime) : m_fillPercentage+ -0.05f * Time.deltaTime);
    }
    /// <inheritdoc/>
    public bool Damage(uint amount)
    {
        for (uint i = 0; i < amount; i++)
        {
            if (m_availableHoles.Count == 0) return false;
            m_holeCount++;
            Transform holeTransform = m_availableHoles.Take();
            ShipHole selectedHole = m_shipHoles.Get();
            selectedHole.Initialize(() =>
            {
                m_shipHoles.Release(selectedHole);
                m_holeCount--;
                m_availableHoles.Return(holeTransform);
                selectedHole.gameObject.SetActive(false);
            });
            selectedHole.transform.position = holeTransform.position;
            selectedHole.transform.rotation = holeTransform.rotation;
            selectedHole.gameObject.SetActive(true);
        }
        return true;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 40), "Deal Damage"))
            Damage(1);
    }
}