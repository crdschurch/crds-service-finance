using System;
using System.Collections.Generic;
using System.Text;

namespace Pushpay.Cache
{
    public interface ICacheService
    {
        T GetOrSet<T>(string key, TimeSpan offset, Func<T> fun);
        T Set<T>(string key, T value, TimeSpan offset);
        T Get<T>(T key);
    }
}
