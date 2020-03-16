using System.Windows;
using System.Windows.Controls;

namespace GeneralComponents.Views
{
    /// <summary>
    /// Логика взаимодействия для Terminal.xaml
    /// </summary>
    public partial class Terminal : UserControl
    {
        public Terminal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TbSendCommand.Focus();
        }
    }
}
