using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IFileService
    {
        IEnumerable<string> LoadFiles(string folderPath, HashSet<string> extensions);

        IEnumerable<string> LoadFiles(string folderPath, string extension);

        IEnumerable<string> LoadFiles(string folderPath);
    }
}