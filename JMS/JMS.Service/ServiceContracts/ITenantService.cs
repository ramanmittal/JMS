using JMS.Entity.Entities;
using JMS.ViewModels.Journals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface ITenantService
    {
        IEnumerable<string> GetTenantPaths();
        IEnumerable<Tenant> GetTenants(int? pageIndex, int? pagesize);
        long GetTenantsCount();
        Task CreateTenant(CreateJournalModel model, Stream stream, string journalLogo);
        Tenant GetTenant(long id, params Expression<Func<Tenant, object>>[] includes);
        bool ValidateTenantPath(string JournalPath);
        bool ValidateTenantPath(string JournalPath, long? JournalId);
        void EditTenant(EditJournalModel createJournalModel, Stream stream, string fileName);
        void DeleteTenant(long id);

        void SaveTenantAdmin(EditJournalAdminModel editJournalAdminModel);
        Task CreateTenantAdmin(CreateJournalAdminModel model);
        Task DeleteTenantAdmin(long userId);
    }
}
