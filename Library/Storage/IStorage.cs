namespace Library
{
    public interface IStorage
    {
        void Store(string key, string value);

        string Load(string key);
        bool IsValueExist(string value);
    }
}