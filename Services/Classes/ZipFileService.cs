using Services.Interfaces;
using System.IO;
using System.IO.Compression;

namespace Services.Classes
{
    public class ZipFileService : IZipFileService
    {
        public void CreateZipFile(string sourcePath, string zipPath)
        {
            ZipFile.CreateFromDirectory(sourcePath, zipPath);
        }

        public void ExtractZipFile(string zipPath, string destinationPath)
        {
            ZipFile.ExtractToDirectory(zipPath, destinationPath);
        }

        public void PrepareZipFile(string sourcePath, string zipPath)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in Directory.EnumerateFiles(sourcePath))
                    {
                        // manually add files
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }

                using (var fileStream = new FileStream(zipPath, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        public byte[] ZipFileArray(string sourcePath, string zipPath)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in Directory.EnumerateFiles(sourcePath))
                    {
                        // manually add files
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
