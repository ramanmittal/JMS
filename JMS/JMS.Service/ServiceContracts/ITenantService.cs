using JMS.Entity.Entities;
using JMS.ViewModels.Journals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ITenantService
    {
        IEnumerable<string> GetTenantPaths();
        IEnumerable<Tenant> GetTenants(int? pageIndex, int? pagesize);
        long GetTenantsCount();
        void CreateTenant(CreateJournalModel model, Stream stream, string journalLogo);
        Tenant GetTenant(long id);
        bool ValidateTenantPath(string JournalPath);
        bool ValidateTenantPath(string JournalPath, long? JournalId);
        void EditTenant(EditJournalModel createJournalModel, Stream stream, string fileName);
        void DeleteTenant(long id);
    }
}
