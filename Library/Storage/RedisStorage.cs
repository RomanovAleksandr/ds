using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Library
{
    public class RedisStorage : IStorage
    {
        private IDatabase _db;
        private IServer _server;
        private readonly Dictionary<string, IDatabase> connections;
        private readonly string _textSetKey = "textSetKey";

        public RedisStorage()
        {
            connections = new Dictionary<string, IDatabase>();
            connections.Add(Constants.SHARD_RUS,
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User)).GetDatabase());
            connections.Add(Constants.SHARD_EU,
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User)).GetDatabase());
            connections.Add(Constants.SHARD_OTHER,
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User)).GetDatabase());    
            
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            _db = redis.GetDatabase();
            _server = redis.GetServer("localhost", 6379);
        }

        public void StoreShardKey(string shardKey, string shardId)
        {
            _db.StringSet(shardKey, shardId);
        }
        public void Store(string shardKey, string key, string value)
        {
            GetConnection(GetShardId(shardKey)).StringSet(key , value);
        }
        public void StoreText(string shardKey, string key, string value)
        {
            Store(shardKey, key, value);
            StoreToSet(shardKey, value);
        }

        public string Load(string shardKey, string key)
        {
            return GetConnection(GetShardId(shardKey)).StringGet(key);
        }

        public bool IsValueExist(string value)
        {
            foreach (KeyValuePair<string, IDatabase> connection in connections)
            {
                if (connection.Value.SetContains(_textSetKey, value))
                {
                    return true;
                }
            }
            return false;
        }

        private IDatabase GetConnection(string shardId)
        {
            return connections[shardId];
        }

        public string GetShardId(string shardKey)
        {
            return _db.StringGet(shardKey);
        }
        public void StoreToSet(string shardKey, string value)
        {
            GetConnection(GetShardId(shardKey)).SetAdd(_textSetKey, value);
        }
    }
}