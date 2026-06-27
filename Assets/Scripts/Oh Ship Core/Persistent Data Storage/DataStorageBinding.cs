using System;
[Serializable]
public class DataStorageBinding<T>
{
    public string Key { get; }
    T m_value;
    public T Value
    {
        get => m_value;
        set
        {
            m_value = value;
            OnValueChanged?.Invoke(m_value);
        }
    }
    public event Action<T> OnValueChanged;
    public DataStorageBinding(string key, T defaultValue = default)
    {
        Key = key;
        m_value = defaultValue;
    }
    public void SetValueWithoutNotify(T value) => m_value = value;
}