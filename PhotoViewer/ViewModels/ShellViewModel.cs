using Prism.Commands;
using Prism.Mvvm;
using Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PhotoViewer.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private ICryptographyService _cryptographyService;
        private ImageViewModel _selectedFile;
        private ImageSource _imageSource;
        private IFileService _fileService;
        private string _password;
        private DispatcherTimer _dispatcherTimer;
        private bool _isSlideShowPlaying;
        private string _folderPath;

        public ShellViewModel(ICryptographyService cryptographyService, IFileService fileService)
        {
            _cryptographyService = cryptographyService;
            _fileService = fileService;

            FolderContents = new ObservableCollection<ImageViewModel>();
            LoadFilesCommand = new DelegateCommand(LoadFiles, LoadFilesCommandCanExecute);
            PasswordChangedCommand = new DelegateCommand<PasswordBox>(PasswordChanged);
            StartSlideshowCommand = new DelegateCommand(StartSlideshow);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            var nextIndex = 0;

            if (SelectedFile != null)
            {
                var index = FolderContents.IndexOf(SelectedFile);

                nextIndex = index + 1 == FolderContents.Count
                    ? 0
                    : index + 1;
            }

            SelectedFile = FolderContents[nextIndex];
        }

        public DelegateCommand LoadFilesCommand { get; set; }

        public bool LoadFilesCommandCanExecute()
        {
            if (!Directory.Exists(FolderPath) || string.IsNullOrWhiteSpace(_password))
                return false;

            return true;
        }

        private void LoadFiles()
        {
            FolderContents.Clear();
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            foreach (var filePath in _fileService.LoadFiles(FolderPath, extensions))
            {
                // TODO move to factory
                byte[] image = _cryptographyService.DecryptFileToArray(filePath, _password);

                var imageViewModel = new ImageViewModel(filePath, image);
                FolderContents.Add(imageViewModel);
            }

            if (FolderContents.Count > 0)
            {
                SelectedFile = FolderContents[0];
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
            LoadFilesCommand.RaiseCanExecuteChanged();
        }

        public string FolderPath
        {
            get
            {
                return _folderPath;
            }

            set
            {
                SetProperty(ref _folderPath, value);
                LoadFilesCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ImageViewModel> FolderContents { get; set; }

        public ImageViewModel SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                SetProperty(ref _selectedFile, value);
                if (_selectedFile != null)
                {
                    ImageSource = _selectedFile.ImageSource;
                }
            }
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        public ICommand StartSlideshowCommand { get; set; }

        private void StartSlideshow()
        {
            if (!_isSlideShowPlaying)
            {
                _dispatcherTimer.Start();
                _isSlideShowPlaying = true;
            }
            else
            {
                _dispatcherTimer.Stop();
                _isSlideShowPlaying = false;
            }
        }
    }
}
