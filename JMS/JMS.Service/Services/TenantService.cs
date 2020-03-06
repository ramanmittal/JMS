using JMS.Entity.Data;
using JMS.Service.ServiceContracts;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using JMS.Entity.Entities;
using JMS.ViewModels.Journals;
using System.IO;
using Microsoft.Extensions.Configuration;
using JMS.Service.Settings;
using System;
using Microsoft.EntityFrameworkCore;

namespace JMS.Service.Services
{
    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFileService _fileService;
        public readonly IConfiguration _configuration;
        public TenantService(ApplicationDbContext applicationDbContext, IFileService fileService, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _fileService = fileService;
            _configuration = configuration;
        }
        public IEnumerable<string> GetTenantPaths()
        {
            return _applicationDbContext.Tenants.IgnoreQueryFilters().Select(x => x.JournalPath).ToList();
        }
        public IEnumerable<Tenant> GetTenants(int? pageIndex, int? pagesize)
        {
            var list = new List<Tenant>();
            if (pageIndex.HasValue && pagesize.HasValue)
            {
                list = _applicationDbContext.Tenants.Skip((pageIndex.Value - 1) * pagesize.Value).Take(pagesize.Value).ToList();
            }
            else
            {
                list = _applicationDbContext.Tenants.ToList();
            }
            return list;
        }
        public long GetTenantsCount()
        {
            return _applicationDbContext.Tenants.Count();
        }

        public void CreateTenant(CreateJournalModel model, Stream stream, string journalLogo)
        {
            var tenant = new Tenant { JournalName = model.JournalName, JournalPath = model.JournalPath, JournalTitle = model.JournalTitle, IsDisabled = !model.IsActive };
            _applicationDbContext.Tenants.Add(tenant);
            var path = _fileService.SaveFile(stream, journalLogo);
            tenant.JournalLogo = path;
            _applicationDbContext.SaveChanges();
        }
        public Tenant GetTenant(long id)
        {
            return _applicationDbContext.Tenants.SingleOrDefault(x => x.Id == id);
        }
        public bool ValidateTenantPath(string JournalPath)
        {
            var paths = (List<string>)GetTenantPaths();
            paths.Add(_configuration[JMSSetting.DefaultTenant]);
            return !paths.Contains(JournalPath, StringComparer.OrdinalIgnoreCase);
        }
        public bool ValidateTenantPath(string JournalPath, long? JournalId)
        {
            if (JournalId.HasValue)
            {
                var paths = _applicationDbContext.Tenants.IgnoreQueryFilters().Where(x => x.Id != JournalId.Value).Select(x => x.JournalPath).ToList();
                paths.Add(_configuration[JMSSetting.DefaultTenant]);
                return !paths.Contains(JournalPath, StringComparer.OrdinalIgnoreCase);
            }
            return ValidateTenantPath(JournalPath);
        }
        public void EditTenant(EditJournalModel model, Stream stream, string fileName)
        {
            var tenant = _applicationDbContext.Tenants.Single(x => x.Id == model.JournalId);
            tenant.IsDisabled = !model.IsActive;
            tenant.JournalName = model.JournalName;
            tenant.JournalTitle = model.JournalTitle;
            if (stream != null && !string.IsNullOrEmpty(fileName))
            {
                _fileService.RemoveFile(tenant.JournalLogo);
                tenant.JournalLogo = _fileService.SaveFile(stream, fileName);
            }
            _applicationDbContext.SaveChanges();
        }
        public void DeleteTenant(long id)
        {
            var deleted = _applicationDbContext.Tenants.Single(x => x.Id==id);
            _applicationDbContext.Tenants.Remove(deleted);
            _applicationDbContext.SaveChanges();
        }
    }
}
