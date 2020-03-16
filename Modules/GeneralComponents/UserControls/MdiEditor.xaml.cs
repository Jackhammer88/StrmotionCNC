using ICSharpCode.AvalonEdit;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GeneralComponents.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MdiEditor.xaml
    /// </summary>
    public partial class MdiEditor : UserControl
    {
        public MdiEditor()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ProgramTextProperty = DependencyProperty.Register("ProgramText", typeof(string), typeof(MdiEditor), new PropertyMetadata(default(string), new PropertyChangedCallback(ProgramTextChanged)));
        public string ProgramText
        {
            get { return (string)GetValue(ProgramTextProperty); }
            set
            {
                SetValue(ProgramTextProperty, value);
            }
        }

        private static void ProgramTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as MdiEditor;
            if (uc != null)
            {
                //uc.Texteditor1.Document.Text = e.NewValue.ToString();
            }
        }

        private void Texteditor1_TextChanged(object sender, EventArgs e)
        {
            var avalon = sender as TextEditor;
            ProgramText = avalon.Document.Text;
        }
    }
}
