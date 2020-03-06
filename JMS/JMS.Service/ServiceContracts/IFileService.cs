using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface IFileService
    {
        string SaveFile(Stream stream, string fileName);
        void RemoveFile(string fileName);
        string GetFile(string file);
    }
}
