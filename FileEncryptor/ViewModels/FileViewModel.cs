using Prism.Mvvm;
using System.IO;

namespace FileEncryptor.ViewModels
{
    class FileViewModel : BindableBase
    {
        private string _filePath;

        public FileViewModel(string filepath)
        {
            _filePath = filepath;
        }

        public bool IsSelected { get; set; }

        public string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }
    }
}
