namespace Services.Interfaces
{
    public interface IZipFileService
    {
        void CreateZipFile(string sourcePath, string zipPath);

        void ExtractZipFile(string zipPath, string destinationPath);

        void PrepareZipFile(string sourcePath, string zipPath);
    }
}