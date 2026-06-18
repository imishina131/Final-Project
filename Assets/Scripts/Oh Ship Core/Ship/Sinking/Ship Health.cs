using System.Collections;
using System.Collections.Generic;
using MatrixUtils.AudioSystem;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ShipHealth : MonoBehaviour, IDamageable
{
    [SerializeField] List<Transform> m_holePositions;
    RandomBag<Transform> m_availableHoles;
    [SerializeField] ShipHole m_holePrefab;
    IObjectPool<ShipHole> m_shipHoles;
    [SerializeField] Observer<float> m_fillPercentage = new(0);
    [SerializeField] float m_invulnerabilityTime = 3;
    [SerializeField] SoundData[] m_damageSounds;
    uint m_holeCount;
    bool m_isInvulnerable;
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

        if (m_fillPercentage >= 1f - .1f)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    /// <inheritdoc/>
    public bool Damage(uint amount)
    {
        if (m_isInvulnerable) return false;
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
            selectedHole.transform.SetParent(holeTransform.parent);
            selectedHole.gameObject.SetActive(true);
        }
        if(m_damageSounds.Length > 0) SoundManager.Instance?.CreateSound().WithSoundData(m_damageSounds[Random.Range(0, m_damageSounds.Length)]).WithRandomPitch().Play();
        StartCoroutine(StartInvulnerability());
        return true;
    }

    IEnumerator StartInvulnerability()
    {
        m_isInvulnerable = true;
        yield return new WaitForSeconds(m_invulnerabilityTime);
        m_isInvulnerable = false;
    }
    // void OnGUI()
    // {
    //     if (GUI.Button(new Rect(10, 10, 150, 40), "Deal Damage"))
    //         Damage(1);
    // }
}