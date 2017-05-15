using System.IO;

namespace FileEncyptor.ViewModels
{
    internal class FileViewModel
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
