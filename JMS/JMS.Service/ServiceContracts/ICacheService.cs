using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ICacheService
    {
        void SetValue(string key, object value, long? journalId = null);
        object GetValue(string key, long? journalId = null);

        void DeleteValue(string key, long? journalId = null);
    }
}
