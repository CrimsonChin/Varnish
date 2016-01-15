using Prism.Unity;
using System.Windows;
using Microsoft.Practices.Unity;
using PhotoViewer.Views;
using Services.Interfaces;
using Services.Classes;

namespace PhotoViewer.Classes
{
    class Bootstrapper : UnityBootstrapper 
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            Container.RegisterType<IFileService, FileService>();
            Container.RegisterType<ICryptographyService, CryptographyService>();
            Container.RegisterType<IZipFileService, ZipFileService>();

            base.ConfigureContainer();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }
    }
}