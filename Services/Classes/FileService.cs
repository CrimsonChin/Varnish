using Services.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Services.Classes
{
    public class FileService : IFileService
    {
        IEnumerable<string> IFileService.LoadFiles(string folderPath)
        {
            return Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);
        }
    }
}
