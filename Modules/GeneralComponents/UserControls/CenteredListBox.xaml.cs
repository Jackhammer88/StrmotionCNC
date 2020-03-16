using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CNCCenteredListBox
{
    /// <summary>
    /// Логика взаимодействия для CenteredListBox.xaml
    /// </summary>
    public partial class CenteredListBox : UserControl
    {
        public CenteredListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IDictionary<int, string>), typeof(CenteredListBox), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourceChanged)));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения", Justification = "<Ожидание>")]
        public IDictionary<int, string> ItemsSource
        {
            get { return (IDictionary<int, string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as CenteredListBox;
            if (uc != null)
            {
                uc.CurrentItemsSourceChanged(uc);
            }
        }
        private void CurrentItemsSourceChanged(CenteredListBox uc)
        {
            uc.listBox.ItemsSource = uc.ItemsSource;
        }



        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CenteredListBox), new PropertyMetadata(-1, new PropertyChangedCallback(SelectedIndexChanged)));
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as CenteredListBox;
            if (uc != null)
            {
                var pairs = uc.ItemsSource.Where(p => p.Key == uc.SelectedIndex);
                if (pairs.Any())
                    uc.listBox.SelectedIndex = uc.listBox.Items.IndexOf(pairs.First());
            }
        }


        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(int), typeof(CenteredListBox), new PropertyMetadata(5));
        public int VerticalOffset
        {
            get { return (int)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }


        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register("TextFontSize", typeof(int), typeof(CenteredListBox), new PropertyMetadata(20));
        public int TextFontSize
        {
            get { return (int)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int oldIndex;
            object item = null;
            if (listBox.SelectedIndex < VerticalOffset)
                oldIndex = listBox.Items.IndexOf(listBox.SelectedItem);
            else
                oldIndex = listBox.Items.IndexOf(listBox.SelectedItem) + VerticalOffset;
            if (listBox.Items.Count > 0 && oldIndex > 0 && listBox.Items.Count > oldIndex)
                item = listBox.Items[oldIndex];
            if (item != null)
                listBox.ScrollIntoView(item);
            if (listBox.SelectedIndex == 46)
                MessageBox.Show(string.Empty);
        }
    }
}
