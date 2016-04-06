using System.IO;
using Prism.Mvvm;

namespace FileEncyptor.ViewModels
{
    internal class FileViewModel : BindableBase
    {
        public FileViewModel(string filepath)
        {
            FilePath = filepath;
        }

        public bool IsSelected { get; set; }

        public string FileName => Path.GetFileName(FilePath);

        public string FilePath { get; }
    }
}
