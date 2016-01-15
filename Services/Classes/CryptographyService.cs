using Services.Interfaces;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services.Classes
{
    public class CryptographyService : ICryptographyService
    {
        private byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static byte[] HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            return SHA256.Create().ComputeHash(passwordBytes);
        }

        public void EncryptFile(string plainTextFile, string outputPath, string password)
        {
            byte[] bytesToBeEncrypted = File.ReadAllBytes(plainTextFile);
            byte[] passwordBytes = HashPassword(password);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string filename = Path.GetFileName(plainTextFile);
            string fileEncrypted = Path.Combine(outputPath, filename);

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }

        public void DecryptFile(string fileEncrypted, string outputFilePath, string password)
        {
            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = HashPassword(password);
            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string filename = Path.GetFileName(fileEncrypted);
            string file = Path.Combine(outputFilePath, filename);

            File.WriteAllBytes(file, bytesDecrypted);
        }

        public byte[] DecryptFileToArray(string fileEncrypted, string password)
        {
            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = HashPassword(password);

            return AES_Decrypt(bytesToBeDecrypted, passwordBytes);
        }

        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream memorySteam = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(memorySteam, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cryptoStream.Close();
                    }
                    encryptedBytes = memorySteam.ToArray();
                }
            }

            return encryptedBytes;
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(memoryStream, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cryptoStream.Close();
                    }
                    decryptedBytes = memoryStream.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
