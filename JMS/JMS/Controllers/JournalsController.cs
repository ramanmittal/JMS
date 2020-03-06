using JMS.Models.Journals;
using JMS.Service.ServiceContracts;
using JMS.Setting;
using JMS.ViewModels.Journals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JMS.Controllers
{
    [Authorize(Roles = RoleName.SystemAdmin)]
    public class JournalsController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly IFileService _fileService;
        public JournalsController(ITenantService tenantService, IFileService fileService)
        {
            _tenantService = tenantService;
            _fileService = fileService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            var model = new JMS.Models.Journals.CreateJournalModel();
            model.IsActive = true;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Journals.CreateJournalModel model)
        {
            if (ModelState.IsValid)
            {
                _tenantService.CreateTenant(new ViewModels.Journals.CreateJournalModel
                {
                    IsActive = model.IsActive,
                    JournalName = model.JournalName,
                    JournalPath = model.JournalPath,
                    JournalTitle = model.JournalTitle
                }, model.Logo.OpenReadStream(), model.Logo.FileName);
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(long id)
        {
            var journal = _tenantService.GetTenant(id);
            var journalModel = new Models.Journals.EditJournalModel
            {
                JournalId = id,
                IsActive = !journal.IsDisabled.GetValueOrDefault(),
                JournalName = journal.JournalName,
                JournalTitle = journal.JournalTitle,
                LogoPath = _fileService.GetFile(journal.JournalLogo)
            };
            return View(journalModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Models.Journals.EditJournalModel model)
        {
            if (ModelState.IsValid)
            {
                _tenantService.EditTenant(new ViewModels.Journals.EditJournalModel
                {
                    JournalId=model.JournalId,
                    IsActive = model.IsActive,
                    JournalName = model.JournalName,
                    JournalTitle = model.JournalTitle
                }, model.Logo?.OpenReadStream(), model.Logo?.FileName);
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            _tenantService.DeleteTenant(id);
            return RedirectToAction("Index");
        }
    }
}