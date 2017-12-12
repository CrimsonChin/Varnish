using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApp.Enumerations;
using Services.Classes;
using Services.Interfaces;

namespace ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Varnish File Encryptor");
			Console.WriteLine();

			try
			{
				var encryptionMode = SelectEncryptionMode();
				var directory = SelectDirectoryToOperateOn(encryptionMode);
				var files = GetFilesToBeEncrypted(directory);
				var password = ReadMaskedPassword();

				EncryptFiles(files, encryptionMode, directory, password);

				Console.WriteLine("All done.");
			}
			catch (Exception e)
			{
				Console.ResetColor();
				Console.WriteLine();
				Console.WriteLine(e.Message);
			}

			Console.WriteLine("Bye!");
			Console.ReadKey();
		}

		private static string SelectDirectoryToOperateOn(EncryptionMode encryptionMode)
		{
			Console.WriteLine();
			Console.WriteLine($"{encryptionMode} which directory: ");
			var dir = Console.ReadLine();
			return dir;
		}

		private static EncryptionMode SelectEncryptionMode()
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("What would you like to do?");
			Console.WriteLine("'1' Encode");
			Console.WriteLine("'2' Decode");
			Console.ResetColor();
			Console.Write("Select option: ");

			return GetCipherMode();
		}

		private static IEnumerable<string> GetFilesToBeEncrypted(string directory)
		{
			IFileService fileService = new FileService();
			var files = fileService.LoadFiles(directory);

			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var file in files)
			{
				Console.WriteLine(Path.GetFileName(file));
			}
			Console.ResetColor();

			return files;
		}

		private static void EncryptFiles(IEnumerable<string> files, EncryptionMode encryptionMode, string dir, string password)
		{
			ICryptographyService cryptographyService = new CryptographyService();

			foreach (var file in files)
			{
				switch (encryptionMode)
				{
					case EncryptionMode.Encrypt:
						{
							var path = $"{dir} enc";
							CreateDirectoryIfNotExists(path);
							cryptographyService.EncryptFile(file, path, password);
							break;

						}
					case EncryptionMode.Decrypt:
						{
							var path = $"{dir} dec";
							CreateDirectoryIfNotExists(path);
							cryptographyService.DecryptFile(file, path, password);
							break;
						}
				}
			}
		}

		private static void CreateDirectoryIfNotExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private static EncryptionMode GetCipherMode()
		{
			var input = Console.ReadKey();

			EncryptionMode userOption;
			var cipherOption = int.Parse(input.KeyChar.ToString());
			switch (cipherOption)
			{
				case 1:
					userOption = EncryptionMode.Encrypt;
					break;
				case 2:
					userOption = EncryptionMode.Decrypt;
					break;
				default:
					throw new InvalidOperationException("You had a choice of two and you've got it wrong.  Amazing.");
			}

			return userOption;
		}

		private static string ReadMaskedPassword()
		{
			Console.WriteLine("Enter password:");

			var password = string.Empty;
			ConsoleKeyInfo consoleKeyInfo;
			do
			{
				consoleKeyInfo = Console.ReadKey(true);

				if (consoleKeyInfo.Key != ConsoleKey.Backspace && consoleKeyInfo.Key != ConsoleKey.Enter)
				{
					password += consoleKeyInfo.KeyChar;
					Console.Write("*");
				}
				else if (consoleKeyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
				{
					password = password.Substring(0, password.Length - 1);
					Console.Write("\b \b");
				}
			}
			while (consoleKeyInfo.Key != ConsoleKey.Enter);

			Console.WriteLine();

			return password;
		}
	}
}
