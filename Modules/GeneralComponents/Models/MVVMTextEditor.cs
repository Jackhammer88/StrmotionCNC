using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace GeneralComponents.Models
{
    public class MvvmTextEditor : TextEditor
    {
        public MvvmTextEditor()
        {
            LoadHighlightSyntax();
            TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        private void Caret_PositionChanged(object sender, System.EventArgs e)
        {
            //CurrentLine = TextArea.Caret.Line;
            SetValue(CurrentLineProperty, TextArea.Caret.Line - 1);
        }

        protected void LoadHighlightSyntax()
        {
            OpenHighLightFile("hl.xshd");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA3075:Небезопасная обработка DTD в формате XML", Justification = "<Ожидание>")]
        protected void OpenHighLightFile(string filename)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    using (var file = File.OpenRead($".\\Resources\\{filename}"))
                    using (var reader = new XmlTextReader(file))
                    {
                        SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
                catch (FileNotFoundException) { }
            }
        }
        private static void LineChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MvvmTextEditor editor)
            {
                double vertOffset = (editor.TextArea.TextView.DefaultLineHeight) * (uint)e.NewValue;
                editor.ScrollToVerticalOffset(vertOffset);
            }
        }

        public uint LineNumber
        {
            get { return (uint)GetValue(LineNumberProperty); }
            set { SetValue(LineNumberProperty, value); }
        }
        public static readonly DependencyProperty LineNumberProperty =
            DependencyProperty.Register("LineNumber", typeof(uint), typeof(MvvmTextEditor), new PropertyMetadata(LineChangedCallback));



        public int CurrentLine
        {
            get { return (int)GetValue(CurrentLineProperty); }
            set { SetValue(CurrentLineProperty, value); }
        }

        public static readonly DependencyProperty CurrentLineProperty =
            DependencyProperty.Register("CurrentLine", typeof(int), typeof(MvvmTextEditor), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
            });
    }
}
