using System.Collections.Generic;
using StackExchange.Redis;

namespace Redis.Models.Home
{
    public class IndexModel
    {
        public string SessionStartTime { get; internal set; }
        public IEnumerable<(RedisKey Key, RedisValue? Value)> CacheItems { get; internal set; }
    }
}
