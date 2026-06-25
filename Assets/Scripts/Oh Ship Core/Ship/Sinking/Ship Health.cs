using System;
using System.Collections;
using System.Collections.Generic;
using MatrixUtils.Attributes;
using MatrixUtils.AudioSystem;
using MatrixUtils.DependencyInjection;
using MatrixUtils.GenericDatatypes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShipHealth : MonoBehaviour, IDamageable
{
    [Inject] INotificationMessenger m_notificationMessenger;
    [SerializeField] List<Transform> m_holePositions;
    RandomBag<Transform> m_availableHoles;
    [SerializeField] ShipHole m_holePrefab;
    IObjectPool<ShipHole> m_shipHoles;
    [SerializeField] Observer<float> m_fillPercentage = new(0);
    [SerializeField] float m_invulnerabilityTime = 3;
    [SerializeField] SoundData[] m_damageSounds;
    [SerializeField, RequiredField] WaterFillController m_waterFillController;
    [SerializeField] private CinemachineImpulseSource m_impulseSource;
    uint m_holeCount;
    bool m_isInvulnerable;
    bool m_warningEnabled;
    void Awake()
    {
        m_availableHoles = new(m_holePositions);
        m_impulseSource = GetComponent<CinemachineImpulseSource>();
        m_shipHoles = new ObjectPool<ShipHole>
        (
            createFunc: () =>
            {
                ShipHole newHole = Instantiate(m_holePrefab);
                m_waterFillController.OnWaterFillChanged.AddListener(fill => newHole.LeakEffect.SetFloat("Splash Plane", fill));
                return newHole;
            });
        m_fillPercentage.Notify();
        FindAnyObjectByType<Injector>().Inject(this);
    }
    void Update()
    {
        m_fillPercentage.Value = Mathf.Clamp01(m_holeCount > 0 ? m_fillPercentage +(m_holeCount * 0.005f * Time.deltaTime) : m_fillPercentage+ -0.05f * Time.deltaTime);
        switch (m_holeCount)
        {
            case > 0 when !m_warningEnabled:
                m_notificationMessenger.TryNotify("enable repair");
                m_warningEnabled = true;
                break;
            case <= 0 when m_warningEnabled:
                m_notificationMessenger.TryNotify("disable repair");
                m_warningEnabled = false;
                break;
        }
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
            m_impulseSource.GenerateImpulse();
            Debug.Log("Impulse");
            selectedHole.transform.SetParent(holeTransform.parent);
            selectedHole.transform.position = holeTransform.position;
            selectedHole.transform.rotation = holeTransform.rotation;
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
    #if UNITY_EDITOR
    [ContextMenu("Damage")]
    void DamageFromInspector()
    {
        Damage(1);
    }
    #endif
}