using JMS.ViewModels.SystemAdmin;
using System.IO;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface ISystemService
    {
        Task InitializeSystem();
        SystemSettingViewModel GetSystemSettings();
        void SetSystemSetting(SystemSettingViewModel systemSettingViewModel, Stream stream, string fileName);
    }
}