using JMS.Service.ServiceContracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMS.Service.Services
{
    public class LocalFileService: IFileService
    {
        private readonly string _imgDirectoryPath;
        private readonly string _imgDirectoryVirtaulPath;
        public LocalFileService(string imgDirectoryPath, string imgDirectoryVirtaulPath)
        {
            _imgDirectoryPath = imgDirectoryPath;
            _imgDirectoryVirtaulPath = imgDirectoryVirtaulPath;
        }
        public string SaveFile(Stream stream, string fileName) {
            var extenstion = fileName.Split('.').Count() > 1 ? fileName.Split('.').ElementAt(1) : "";
            var uiquename = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(extenstion))
            {
                uiquename = uiquename + "." + extenstion;
            }
            var path = Path.Combine(_imgDirectoryPath, uiquename);
            using (Stream file = File.Create(path))
            {
                CopyStream(stream, file);
            }
            return uiquename;
        }
        public void RemoveFile(string fileName) {
            var path = Path.Combine(_imgDirectoryPath, fileName);
            File.Delete(path);
        }
        public string GetFile(string file)
        {
            return _imgDirectoryVirtaulPath + file;
        }
        public byte[] GetFileBytes(string file)
        {
            return File.ReadAllBytes(Path.Combine(_imgDirectoryPath, file)) ;
        }
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
