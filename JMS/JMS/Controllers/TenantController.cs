using System.Linq;
using JMS.Models.Journals;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class TenantController : Controller
    {
        public readonly ITenantService _tenantService;
        public readonly IFileService _fileService;
        public readonly IConfiguration _configuration;
        public TenantController(ITenantService tenantService, IConfiguration configuration, IFileService fileService)
        {
            _tenantService = tenantService;
            _fileService = fileService;
            _configuration = configuration;
        }
        public IActionResult Tenants(int? pageIndex, int? pagesize)
        {
            var tenants = _tenantService.GetTenants(pageIndex, pagesize);
            var journalGridModelList = tenants.Select(x => new JournalGridRow
            {
                Active = !x.IsDisabled.GetValueOrDefault(),
                JournalName = x.JournalName,
                JournalTitle = x.JournalTitle,
                Path = x.JournalPath,
                Id = x.Id,
                Logo = _fileService.GetFile(x.JournalLogo)
            });
            return Json(new JxGridModel<JournalGridRow> { data = journalGridModelList, itemsCount = _tenantService.GetTenantsCount() });
        }
        public IActionResult ValidateTenantPath(string JournalPath,long? JournalId)
        {
            if (!_tenantService.ValidateTenantPath(JournalPath, JournalId))
            {
                return Json(data: Messages.TenantNotAvailiable);
            }
            return Json(data: true);
        }      
        
    }
}