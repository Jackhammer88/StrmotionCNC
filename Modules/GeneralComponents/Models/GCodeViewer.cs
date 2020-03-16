using System;
using System.Linq;
using System.Windows;

namespace GeneralComponents.Models
{
    public class GCodeViewer : MvvmTextEditor
    {
        public GCodeViewer()
        {
            IsReadOnly = true;
            //Options.HighlightCurrentLine = true;
        }
        #region ProgramText property
        public string ProgramText
        {
            get { return (string)GetValue(ProgramTextProperty); }
            set { SetValue(ProgramTextProperty, value); }
        }

        public static readonly DependencyProperty ProgramTextProperty =
            DependencyProperty.Register("ProgramText", typeof(string), typeof(GCodeViewer), new PropertyMetadata(TextChangedCallback));

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GCodeViewer viewer)
            {
                viewer.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();
                viewer.Document.Text = e.NewValue.ToString();
            }
        }
        #endregion
        #region SelectedLine
        public int SelectedLine
        {
            get { return (int)GetValue(SelectedLineProperty); }
            set { SetValue(SelectedLineProperty, value); }
        }

        public static readonly DependencyProperty SelectedLineProperty =
            DependencyProperty.Register("SelectedLine", typeof(int), typeof(GCodeViewer), new PropertyMetadata(SelectedLineCallback));

        private static void SelectedLineCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GCodeViewer viewer)
            {
                var offset = viewer.Document.IndexOf($"N{(int)e.NewValue}", 0, viewer.Text.Length, StringComparison.OrdinalIgnoreCase);
                var line = viewer.Text.Take(offset).Count(c => c == '\n') + 1;
                viewer.TextArea.Caret.Line = line;
            }
        }
        #endregion
        #region HighlightLine


        public bool HighlightLine
        {
            get { return (bool)GetValue(HighlightLineProperty); }
            set { SetValue(HighlightLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightLineProperty =
            DependencyProperty.Register("HighlightLine", typeof(bool), typeof(GCodeViewer), new PropertyMetadata(OnHighlightLineChanged));

        private static void OnHighlightLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GCodeViewer viewer)
            {
                viewer.Options.HighlightCurrentLine = (bool)e.NewValue;
            }
        }


        #endregion
    }
}
