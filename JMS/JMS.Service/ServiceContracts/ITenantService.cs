using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ITenantService
    {
        public IEnumerable<string> GetTenantPaths();
    }
}
