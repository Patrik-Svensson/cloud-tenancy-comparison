using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using WebApplication.Models;
using WebApplication.DAL;

public class TenantCache : ICache
{
    private static MemoryCache memoryCache
    {
        get
        {
            return MemoryCache.Default;
        }
    }

    private readonly ITenantIdProvider _tenantIdProvider;

    public TenantCache(ITenantIdProvider tenantIdProvider)
    {
        _tenantIdProvider = tenantIdProvider;
    }

    void ICache.Set(string key, object value, DateTimeOffset absoluteExpiration)
    {
        string tenantKey = String.Concat(key, _tenantIdProvider.TenantId());
        memoryCache.Add(tenantKey, value, absoluteExpiration);
    }

    object ICache.Get(string key)
    {
        string tenantKey = String.Concat(key, _tenantIdProvider.TenantId());
        return memoryCache.Get(tenantKey);
    }
}
