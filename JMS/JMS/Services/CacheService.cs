using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using JMS.Service.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
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
        private readonly IFileService _fileService;
        private const string journalLogoKey = "journalLogo";
        private const string journalNameKey = "journalName";
        private const string journalTitleKey = "journalTitle";
        public CacheService(IMemoryCache cache, IFileService fileService, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _cache = cache;
            _fileService = fileService;
            _configuration = configuration;
        }
        public void DeleteValue(string key, string journalPath = null)
        {
            if (!string.IsNullOrEmpty(journalPath))
            {
                var dict = GetJournalValues(journalPath);
                if (dict.ContainsKey(key))
                {
                    object value;
                    dict.TryRemove(key,out value);
                }
            }
            else
            {
                _cache.Remove(key);
            }
        }

        public object GetValue(string key, string journalPath = null)
        {
            if (!string.IsNullOrEmpty(journalPath))
            {
                var dict = GetJournalValues(journalPath);
                if (dict.ContainsKey(key))
                {
                    return dict[key];
                }
                else
                {
                    var setting = _applicationDbContext.JournalSettings.FirstOrDefault(x => x.Tenant.JournalPath == journalPath && x.Key == key);
                    object value = null;
                    if (setting == null)
                    {
                        value = _configuration[key];
                    }
                    else
                    {
                        value = setting.Value;
                    }
                    SetValue(key, value, journalPath);
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

        public void SetValue(string key, object value, string journalPath = null)
        {
            if (!string.IsNullOrEmpty(journalPath))
            {
                var dict = GetJournalValues(journalPath);
                dict[key] = value;
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        private ConcurrentDictionary<string, object> GetJournalValues(string journalPath)
        {
            ConcurrentDictionary<string, object> dict = null;
            if (!_cache.TryGetValue(journalPath, out dict))
            {
                dict = new ConcurrentDictionary<string, object>();
                _cache.Set(journalPath, dict);
            }
            return dict;
        }
        public string GetJournalLogo(string journalPath)
        {
            var logo = GetValue(journalLogoKey, journalPath);
            if (logo == null)
            {
                var journal = _applicationDbContext.Tenants.First(x => x.JournalPath == journalPath);
                logo = journal.JournalLogo;
                SetValue(journalLogoKey, journal.JournalLogo, journalPath);
            }
            return (string)logo;
        }
        public string GetJournalTitle(string journalPath)
        {
            var title = GetValue(journalTitleKey, journalPath);
            if (title == null)
            {
                var journal = _applicationDbContext.Tenants.First(x => x.JournalPath == journalPath);
                title = journal.JournalTitle;
                SetValue(journalTitleKey, journal.JournalTitle, journalPath);
            }
            return (string)title;
        }
        public string GetJournalName(string journalPath)
        {
            var name = GetValue(journalNameKey, journalPath);
            if (name == null)
            {
                var journal = _applicationDbContext.Tenants.First(x => x.JournalPath == journalPath);
                name = journal.JournalName;
                SetValue(journalNameKey, journal.JournalName, journalPath);
            }
            return (string)name;
        }
        public void ClearJournalCache(string journalPath)
        {
            GetJournalValues(journalPath).Clear();
        }

        public string GetSystemLogo()
        {
            string value = null;
            if (!_cache.TryGetValue(JMSSetting.SystemLogo, out value))
            {
                var setting = _applicationDbContext.SystemSettings.FirstOrDefault(x => x.Key == JMSSetting.SystemLogo);
                if (setting == null)
                {
                    value = _configuration[JMSSetting.SystemLogo];                    
                }
                else
                {
                    value = setting.Value;
                    value = _fileService.GetFile(value);
                }
                SetValue(JMSSetting.SystemLogo, value);
            }
            return value;
        }
    }
}
