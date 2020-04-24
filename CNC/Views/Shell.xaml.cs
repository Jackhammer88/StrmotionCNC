using Infrastructure.WinAPI;
using System.Windows;

namespace CNC.Views
{
    /// <summary>
    /// Логика взаимодействия для Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
            ScreenSaverKiller.PreventSleep();
        }
    }
}
