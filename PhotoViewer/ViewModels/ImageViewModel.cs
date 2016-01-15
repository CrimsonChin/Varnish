using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoViewer.ViewModels
{
    public class ImageViewModel
    {
        private string _filePath;
        private BitmapImage _image;

        public ImageViewModel(string filePath, byte[] imageData)
        {
            _filePath = filePath;
            _image = LoadImage(imageData);
            ImageSource = _image;
        }

        private void SetupImageInfo()
        {
            throw new NotImplementedException();
        }

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

        public ImageSource ImageSource { get; set; }

        // TODO move into own class
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            var image = new BitmapImage();
            using (var memoryStream = new MemoryStream(imageData))
            {
                memoryStream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        public double Width
        {
            get
            {
                return _image.Width;
            }
        }

        public double Height
        {
            get
            {
                return _image.Height;
            }
        }

        public string Type
        {
            get
            {
                return Path.GetExtension(FilePath);
            }
        }

        public long FileSize
        {
            get
            {
                FileInfo fileInfo = new FileInfo(_filePath);
                return fileInfo.Length;
            }
        }
    }
}
