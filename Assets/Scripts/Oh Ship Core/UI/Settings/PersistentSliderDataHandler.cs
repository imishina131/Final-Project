using System;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

public class PersistentSliderDataHandler : MonoBehaviour
{
    [SerializeField] string m_key;
    [SerializeField] float m_defaultValue;
    [SerializeField] Slider m_slider;
    [UsedImplicitly, Inject]
    void OnInjected(IPersistentDataStorage persistentDataStorage)
    {
        m_binding = persistentDataStorage.GetOrCreateBinding(m_key, m_defaultValue);
        m_slider.value = m_binding.Value;
        m_binding.OnValueChanged += value => m_slider.SetValueWithoutNotify(value);
        m_slider.onValueChanged.AddListener(value => m_binding.Value = value);
    }
    DataStorageBinding<float> m_binding;
}
