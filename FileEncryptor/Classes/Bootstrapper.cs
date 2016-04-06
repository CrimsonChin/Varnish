using System.Windows;
using FileEncryptor.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Services.Classes;
using Services.Interfaces;

namespace FileEncyptor.Classes
{
    internal class Bootstrapper : UnityBootstrapper 
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

            base.ConfigureContainer();
        }
    }
}