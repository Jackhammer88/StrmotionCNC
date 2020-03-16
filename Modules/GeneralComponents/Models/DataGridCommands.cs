using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeneralComponents.Models
{
    public static class DataGridCommands
    {
        public static readonly DependencyProperty DataGridDoubleClickProperty =
          DependencyProperty.RegisterAttached("DataGridDoubleClickCommand", typeof(ICommand), typeof(DataGridCommands),
                            new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDataGridDoubleClickEvent)));

        public static ICommand GetDataGridDoubleClickCommand(DependencyObject depObj)
        {
            if (depObj == null) throw new NullReferenceException();
            return (ICommand)depObj.GetValue(DataGridDoubleClickProperty);
        }

        public static void SetDataGridDoubleClickCommand(DependencyObject depObj, ICommand value)
        {
            if (depObj == null) throw new NullReferenceException();
            depObj.SetValue(DataGridDoubleClickProperty, value);
        }

        public static void AttachOrRemoveDataGridDoubleClickEvent(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            if (depObj is DataGrid dataGrid)
            {

                if (args.OldValue == null && args.NewValue != null)
                {
                    dataGrid.MouseDoubleClick += ExecuteDataGridDoubleClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    dataGrid.MouseDoubleClick -= ExecuteDataGridDoubleClick;
                }
            }
        }

        private static void ExecuteDataGridDoubleClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand)obj.GetValue(DataGridDoubleClickProperty);
            if (cmd != null)
            {
                if (cmd.CanExecute(obj))
                {
                    cmd.Execute(obj);
                }
            }
        }

    }
}
