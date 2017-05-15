using System.Windows;
using FileEncyptor.ViewModels;
using Services.Classes;
using Services.Interfaces;

namespace FileEncyptor.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
			IFileService fileService = new FileService();
			ICryptographyService cryptographyService = new CryptographyService();
			DataContext = new ShellViewModel(fileService, cryptographyService);

            InitializeComponent();
        }
    }
}
