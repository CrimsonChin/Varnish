using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IFileService
    {
        IEnumerable<string> LoadFiles(string folderPath);
    }
}