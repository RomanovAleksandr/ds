namespace Library
{
    public interface IStorage
    {
        void StoreShardKey(string shardKey, string shardId);
        void Store(string shardKey, string key, string value);
        void StoreText(string shardKey, string key, string value);
        string Load(string shardKey, string key);
        bool IsValueExist(string value);
        string GetShardId(string shardKey);
    }
}