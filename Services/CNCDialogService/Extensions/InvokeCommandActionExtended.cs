using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CNCDialogService.Extensions
{
    public class InvokeCommandActionExtended : TriggerAction<DependencyObject>
    {



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandActionExtended), new PropertyMetadata(null));



        public ICommand CommandParameter
        {
            get { return (ICommand)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(ICommand), typeof(InvokeCommandActionExtended), new PropertyMetadata(null));
        private bool handled;

        protected override void Invoke(object parameter)
        {
            if (parameter != null && parameter is SelectionChangedEventArgs addedItems && addedItems.AddedItems.Count >= 1 && !handled)
            {
                handled = true;
                Command?.Execute((parameter as SelectionChangedEventArgs).AddedItems[0]);
                handled = false;
            }
        }
    }
}
