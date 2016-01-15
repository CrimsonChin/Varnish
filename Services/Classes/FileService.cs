using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Services.Classes
{
    public class FileService : IFileService
    {
        public IEnumerable<string> LoadFiles(string folderPath, HashSet<string> extensions)
        {
            var result = new List<string>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories).Where(s => extensions.Contains(Path.GetExtension(s))))
            {
                result.Add(file);
            }

            return result;
        }

        public IEnumerable<string> LoadFiles(string folderPath, string extension)
        {
           return Directory.EnumerateFiles(folderPath, extension);
        }

        public IEnumerable<string> LoadFiles(string folderPath)
        {
            return Directory.EnumerateFiles(folderPath);
        }

        public IEnumerable<string> GetFlatListOfFoldersContainingImages(string folderPath)
        {
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };

            IList<string> directoriesWithImages = new List<string>();

            foreach (var directory in Directory.EnumerateDirectories(folderPath, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                bool isFolderVisible = (dirInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden;
                bool hasImageFiles = Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly).Where(s => extensions.Contains(Path.GetExtension(s))).Any();

                if (isFolderVisible && hasImageFiles)
                {
                    directoriesWithImages.Add(directory);
                }
            }

            return directoriesWithImages;
        }
    }
}
