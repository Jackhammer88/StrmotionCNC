using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace CNC.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public ShellViewModel()
        {
            ShowWindowCommand = new DelegateCommand<RoutedEventArgs>(ShowWindowExecute);
        }

        private void ShowWindowExecute(RoutedEventArgs e)
        {
            if (e.Source is Window mainWindow)
            {
                mainWindow.Activate();
            }
        }

        public DelegateCommand<RoutedEventArgs> ShowWindowCommand { get; }
    }
}

