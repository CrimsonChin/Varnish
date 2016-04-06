using Services.Interfaces;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services.Classes
{
    public class CryptographyService : ICryptographyService
    {
        #region Implementation of ICryptographyService

        private readonly byte[] _saltBytes = {1, 2, 3, 4, 5, 6, 7, 8};

        public void EncryptFile(string plainTextFile, string outputPath, string password)
        {
            var bytesToBeEncrypted = File.ReadAllBytes(plainTextFile);
            var passwordBytes = HashPassword(password);
            var bytesEncrypted = AesEncrypt(bytesToBeEncrypted, passwordBytes);

            var filename = Path.GetFileName(plainTextFile);
            var fileEncrypted = Path.Combine(outputPath, filename);

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }

        public void DecryptFile(string fileEncrypted, string outputFilePath, string password)
        {
            var bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            var passwordBytes = HashPassword(password);
            var bytesDecrypted = AesDecrypt(bytesToBeDecrypted, passwordBytes);

            var filename = Path.GetFileName(fileEncrypted);
            var file = Path.Combine(outputFilePath, filename);

            File.WriteAllBytes(file, bytesDecrypted);
        }

        #endregion

        #region Private Methods

        private static byte[] HashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return SHA256.Create().ComputeHash(passwordBytes);
        }

        private byte[] AesEncrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            return Cipher(true, bytesToBeEncrypted, passwordBytes);
        }

        private byte[] AesDecrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            return Cipher(false, bytesToBeDecrypted, passwordBytes);
        }

        private byte[] Cipher(bool isEncrypt, byte[] fileBytes, byte[] passwordBytes)
        {
            byte[] decryptedBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, _saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize/8);
                    aes.IV = key.GetBytes(aes.BlockSize/8);

                    aes.Mode = CipherMode.CBC;

                    var cryptoTransform = isEncrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
                    using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(fileBytes, 0, fileBytes.Length);
                        cryptoStream.Close();
                    }
                    decryptedBytes = memoryStream.ToArray();
                }
            }

            return decryptedBytes;
        }

        #endregion
    }
}
