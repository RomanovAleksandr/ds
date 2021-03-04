using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private IDatabase _db;
        private IServer _server;

        public RedisStorage()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            _db = redis.GetDatabase();
            _server = redis.GetServer("localhost", 6379);
        }

        public void Store(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public string Load(string key)
        {
            return _db.StringGet(key);
        }

        public bool IsValueExist(string value)
        {
            foreach(var key in _server.Keys())
            {
                if(value == _db.StringGet(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}