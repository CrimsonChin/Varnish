namespace Services.Interfaces
{
    public interface ICryptographyService
    {
        void EncryptFile(string plainTextFile, string outputPath, string password);

        void DecryptFile(string fileEncrypted, string outputFilePath, string password);

        byte[] DecryptFileToArray(string fileEncrypted, string password);
    }
}