using Infrastructure.WinAPI;
using System.Windows;

namespace CNC
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
