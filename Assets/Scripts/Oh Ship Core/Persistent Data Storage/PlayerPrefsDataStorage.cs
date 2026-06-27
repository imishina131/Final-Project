using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class PlayerPrefsDataStorage : MonoBehaviour, IPersistentDataStorage, IDependencyProvider
{
    readonly Dictionary<string, object> m_bindings = new();
    [UsedImplicitly, Provide]
    IPersistentDataStorage ProvideStorage() => this;
    void Start() => Load();
    public DataStorageBinding<T> GetOrCreateBinding<T>(string key, T defaultValue = default)
    {
        if (m_bindings.TryGetValue(key, out object existing)) return (DataStorageBinding<T>)existing;
        DataStorageBinding<T> binding = new(key, defaultValue);
        m_bindings[key] = binding;
        if (PlayerPrefs.HasKey(key)) binding.SetValueWithoutNotify(ReadFromPlayerPrefs<T>(key));
        binding.OnValueChanged += value => WriteToPlayerPrefs(key, value);
        return binding;
    }

    public void Load()
    {
        foreach ((string key, object obj) in m_bindings)
        {
            if (!PlayerPrefs.HasKey(key)) continue;
            switch (obj)
            {
                case DataStorageBinding<float> b:   b.SetValueWithoutNotify(PlayerPrefs.GetFloat(key));   break;
                case DataStorageBinding<int> b:     b.SetValueWithoutNotify(PlayerPrefs.GetInt(key));     break;
                case DataStorageBinding<string> b:  b.SetValueWithoutNotify(PlayerPrefs.GetString(key));  break;
            }
        }
    }

    public void Save()
    {
        foreach ((string key, object obj) in m_bindings)
        {
            switch (obj)
            {
                case DataStorageBinding<float> b:   PlayerPrefs.SetFloat(key, b.Value);   break;
                case DataStorageBinding<int> b:     PlayerPrefs.SetInt(key, b.Value);     break;
                case DataStorageBinding<string> b:  PlayerPrefs.SetString(key, b.Value);  break;
            }
        }

        PlayerPrefs.Save();
    }

    static T ReadFromPlayerPrefs<T>(string key) => default(T) switch
    {
        float  => (T)(object)PlayerPrefs.GetFloat(key),
        int    => (T)(object)PlayerPrefs.GetInt(key),
        string => (T)(object)PlayerPrefs.GetString(key),
        _ => throw new NotSupportedException($"PlayerPrefs does not support type {typeof(T)}")
    };

    static void WriteToPlayerPrefs<T>(string key, T value)
    {
        switch (value)
        {
            case float f:   PlayerPrefs.SetFloat(key, f);  break;
            case int i:     PlayerPrefs.SetInt(key, i);    break;
            case string s:  PlayerPrefs.SetString(key, s); break;
            default: throw new NotSupportedException($"PlayerPrefs does not support type {typeof(T)}");
        }
    }
}