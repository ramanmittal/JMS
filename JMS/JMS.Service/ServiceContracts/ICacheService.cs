using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ICacheService
    {
        void SetValue(string key, object value, string journalPath = null);
        object GetValue(string key, string journalPath = null);

        void DeleteValue(string key, string journalPath = null);
        void ClearJournalCache(string journalPath);
        string GetJournalName(string journalPath);
        string GetJournalTitle(string journalPath);
        string GetJournalLogo(string journalPath);
    }
}
