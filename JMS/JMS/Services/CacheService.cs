using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        public CacheService(IMemoryCache cache, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _cache = cache;
            _configuration = configuration;
        }
        public void DeleteValue(string key, long? journalId = null)
        {
            if (journalId.HasValue)
            {
                var dict = GetJournalValues(journalId.Value);
                if (dict.ContainsKey(key))
                {
                    dict.Remove(key);
                }
            }
            else
            {
                _cache.Remove(key);
            }
        }

        public object GetValue(string key, long? journalId = null)
        {
            if (journalId.HasValue)
            {
                var dict = GetJournalValues(journalId.Value);
                if (dict.ContainsKey(key))
                {
                    return dict[key];
                }
                else
                {
                    var setting = _applicationDbContext.JournalSettings.FirstOrDefault(x => x.TenantId == journalId && x.Key == key);
                    object value = null;
                    if (setting == null)
                    {
                        value = _configuration[key];
                    }
                    else
                    {
                        value = setting.Value;
                    }
                    SetValue(key, value, journalId.Value);
                    return value;
                }
            }
            else
            {
                object value = null;
                if (!_cache.TryGetValue(key, out value))
                {
                    var setting = _applicationDbContext.SystemSettings.FirstOrDefault(x => x.Key == key);
                    if (setting == null)
                    {
                        value = _configuration[key];
                    }
                    else
                    {
                        value = setting.Value;
                    }
                    SetValue(key, value);
                }
                return value;
            }
        }

        public void SetValue(string key, object value, long? journalId = null)
        {
            if (journalId.HasValue)
            {
                var dict = GetJournalValues(journalId.Value);
                if (dict.ContainsKey(key))
                {
                    dict[key] = value;
                }
                else
                {
                    dict.Add(key, value);
                }
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        private Dictionary<string, object> GetJournalValues(long journalId)
        {
            Dictionary<string, object> dict = null;
            if (!_cache.TryGetValue(journalId, out dict))
            {
                dict = new Dictionary<string, object>();
                _cache.Set(journalId, dict);
            }
            return dict;
        }
    }
}
