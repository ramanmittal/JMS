using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.Services
{
    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public TenantService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IEnumerable<string> GetTenantPaths()
        {
            return _applicationDbContext.Tenants.Select(x => x.JournalPath).ToList();
        }
    }
}
