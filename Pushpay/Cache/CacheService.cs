using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Pushpay.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(T key)
        {
            return _cache.Get<T>(key);
        }

        public T GetOrSet<T>(string key, TimeSpan offset, Func<T> fun)
        {
            T value;
            if (!_cache.TryGetValue(key, out value))
            {
                value = Set(key, fun(), offset);
            }
            return value;

        }

        public T Set<T>(string key, T value, TimeSpan offset)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(offset);

            return _cache.Set<T>(key, value, cacheEntryOptions);
        }
    }
}

