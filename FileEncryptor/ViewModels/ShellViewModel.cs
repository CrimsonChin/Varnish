using FileEncryptor.Enumerations;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileEncryptor.ViewModels
{
    class ShellViewModel : BindableBase
    {
        private const string DefaultFolderName = "Select Folder";
        private const string FolderSelectionTitle = "Select The Folder To Process";

        private IFileService _fileService;
        private string _sourceFolderPath;
        private string _encryptedFolderPath;
        private string _password;
        private bool _isInvalidPassword;
        private string _verifyPassword;
        private ICryptographyService _cryptographyService;
        private int _processedFileCount;
        private int _totalFileCount;

        public ShellViewModel(IFileService fileService, ICryptographyService cryptographyService)
        {
            _fileService = fileService;
            _cryptographyService = cryptographyService;

            SelectSourceFolderPathCommand = new DelegateCommand(SelectSourceFolderPath);
            SelectOutputFolderPathCommand = new DelegateCommand(SelectOutputFolder);

            PasswordChangedCommand = new DelegateCommand<PasswordBox>(PasswordChanged);
            VerifyPasswordChangedCommand = new DelegateCommand<PasswordBox>(VerifyPasswordChanged);

            ProcessCommand = new DelegateCommand(Process, CanExecuteEncrypt);
            RemoveSelectedFilesCommand = new DelegateCommand(RemoveSelectedFiles);

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

            var ouputFolderName = string.Format("{0} Copy", Path.GetFileName(SourceFolderPath));

            DirectoryInfo parentFolderInfo = Directory.GetParent(SourceFolderPath);
            string parentFolder = parentFolderInfo != null ? parentFolderInfo.FullName : SourceFolderPath;
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
                FileViewModel fileViewModel = new FileViewModel(file);
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
        private string SelectFolder()
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
            get
            {
                return _isInvalidPassword;
            }

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

        public DelegateCommand ProcessCommand { get; set; }

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
                _cryptographyService.EncryptFile(file.FilePath, OuputFolderPath, _password);

                if (WipeSourceFile)
                {
                    File.Delete(file.FilePath);
                }

                ProcessedFileCount = ProcessedFileCount + 1;
                Debug.WriteLine(ProcessedFileCount);
            }

            if (Directory.GetFiles(SourceFolderPath).Count() == 0)
            {
                Directory.Delete(SourceFolderPath);
            }

            LoadFolderContents();
        }

        public int TotalFileCount
        {
            get
            {
                return FilesToBeProcessed.Any() ? _totalFileCount : 100;
            }

            set
            {
                SetProperty(ref _totalFileCount, value);
            }
        }

        public int ProcessedFileCount
        {
            get
            {
                return _processedFileCount;
            }

            set
            {
                SetProperty(ref _processedFileCount, value);
            }
        }
    }
}