using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using FileEncyptor.Classes;
using FileEncyptor.Enumerations;
using Microsoft.WindowsAPICodePack.Dialogs;
using Services.Interfaces;

namespace FileEncyptor.ViewModels
{
    internal class ShellViewModel : ViewModelBase
    {
        private const string DefaultFolderName = "Select Folder";
        private const string FolderSelectionTitle = "Select The Folder To Process";

        private readonly IFileService _fileService;
        private readonly ICryptographyService _cryptographyService;

        private string _sourceFolderPath;
        private string _encryptedFolderPath;
        private string _password;
        private bool _isInvalidPassword;
        private string _verifyPassword;
        private int _processedFileCount;
        private int _totalFileCount;

        public ShellViewModel(IFileService fileService, ICryptographyService cryptographyService)
        {
            _fileService = fileService;
            _cryptographyService = cryptographyService;

            SelectSourceFolderPathCommand = new RelayCommand(SelectSourceFolderPath);
            SelectOutputFolderPathCommand = new RelayCommand(SelectOutputFolder);

            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
            VerifyPasswordChangedCommand = new RelayCommand<PasswordBox>(VerifyPasswordChanged);

            ProcessCommand = new RelayCommand(Process, CanExecuteEncrypt);
            RemoveSelectedFilesCommand = new RelayCommand(RemoveSelectedFiles);

            FilesToBeProcessed = new ObservableCollection<FileViewModel>();
            Mode = EncryptionMode.Encrypt;
        }

        public EncryptionMode Mode { get; set; }

        public ObservableCollection<FileViewModel> FilesToBeProcessed { get; }

        public ICommand SelectSourceFolderPathCommand { get; set; }

        private void SelectSourceFolderPath()
        {
            SourceFolderPath = SelectFolder();

            if (!Directory.Exists(SourceFolderPath))
            {
                return;
            }

            var ouputFolderName = $"{Path.GetFileName(SourceFolderPath)} Copy";

            var parentFolderInfo = Directory.GetParent(SourceFolderPath);
            var parentFolder = parentFolderInfo?.FullName ?? SourceFolderPath;
            OuputFolderPath = Path.Combine(parentFolder, ouputFolderName);

            LoadFolderContents();
        }

        private void LoadFolderContents()
        {
            if (!Directory.Exists(SourceFolderPath))
            {
                return;
            }

            FilesToBeProcessed.Clear();
            foreach (var file in _fileService.LoadFiles(SourceFolderPath))
            {
                var fileViewModel = new FileViewModel(file);
                FilesToBeProcessed.Add(fileViewModel);
            }

            TotalFileCount = FilesToBeProcessed.Count();
        }

        public ICommand SelectOutputFolderPathCommand { get; set; }

        private void SelectOutputFolder()
        {
            OuputFolderPath = SelectFolder();
        }

        //TODO move to service
        private static string SelectFolder()
        {
            var filename = string.Empty;

            var dialog = new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = false,
                AllowNonFileSystemItems = false,
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                IsFolderPicker = true,
                DefaultFileName = DefaultFolderName,
                Title = FolderSelectionTitle,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                filename = dialog.FileName;
            }

            return filename;
        }

        public string SourceFolderPath
        {
            get { return _sourceFolderPath; }

            set
            {
                SetProperty(ref _sourceFolderPath, value);
                ProcessCommand.RaiseCanExecuteChanged();
            }
        }

        public string OuputFolderPath
        {
            get { return _encryptedFolderPath; }

            set
            {
                SetProperty(ref _encryptedFolderPath, value);
                ProcessCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsInvalidPassword
        {
            get { return _isInvalidPassword; }

            set
            {
                SetProperty(ref _isInvalidPassword, value);
                ProcessCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand PasswordChangedCommand { get; set; }

        private void PasswordChanged(PasswordBox obj)
        {
            if (obj == null)
            {
                return;
            }

            _password = obj.Password;
            ValidatePassword();
        }

        public ICommand VerifyPasswordChangedCommand { get; set; }

        private void VerifyPasswordChanged(PasswordBox obj)
        {
            if (obj == null)
            {
                return;
            }

            _verifyPassword = obj.Password;
            ValidatePassword();
        }

        private void ValidatePassword()
        {
            IsInvalidPassword = (_verifyPassword != _password);
        }

        public ICommand RemoveSelectedFilesCommand { get; set; }

        private void RemoveSelectedFiles()
        {
            foreach (var file in FilesToBeProcessed.Reverse())
            {
                if (file.IsSelected)
                {
                    FilesToBeProcessed.Remove(file);
                }
            }
        }

        public bool WipeSourceFile { get; set; }

        public RelayCommand ProcessCommand { get; set; }

        private bool CanExecuteEncrypt()
        {
            return !(string.IsNullOrWhiteSpace(SourceFolderPath)
                     || string.IsNullOrWhiteSpace(OuputFolderPath)
                     || string.IsNullOrWhiteSpace(_password)
                     || string.IsNullOrWhiteSpace(_verifyPassword)
                     || IsInvalidPassword);
        }

        private void Process()
        {
            if (!Directory.Exists(OuputFolderPath))
            {
                Directory.CreateDirectory(OuputFolderPath);
            }

            ProcessedFileCount = 0;
            foreach (var file in FilesToBeProcessed)
            {
                if (Mode == EncryptionMode.Encrypt)
                {
                    _cryptographyService.EncryptFile(file.FilePath, OuputFolderPath, _password);
                }
                else
                {
                    _cryptographyService.DecryptFile(file.FilePath, OuputFolderPath, _password);
                }


                if (WipeSourceFile)
                {
                    File.Delete(file.FilePath);
                }

                ProcessedFileCount = ProcessedFileCount + 1;
                Debug.WriteLine(ProcessedFileCount);
            }

            if (!Directory.GetFiles(SourceFolderPath).Any())
            {
                Directory.Delete(SourceFolderPath);
            }

            LoadFolderContents();
        }

        public int TotalFileCount
        {
            get { return FilesToBeProcessed.Any() ? _totalFileCount : 100; }

            set { SetProperty(ref _totalFileCount, value); }
        }

        public int ProcessedFileCount
        {
            get { return _processedFileCount; }

            set { SetProperty(ref _processedFileCount, value); }
        }
    }
}