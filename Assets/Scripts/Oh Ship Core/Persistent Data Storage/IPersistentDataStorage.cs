public interface IPersistentDataStorage
{
    DataStorageBinding<T> GetOrCreateBinding<T>(string key, T defaultValue = default);
    void Load();
    void Save();
}