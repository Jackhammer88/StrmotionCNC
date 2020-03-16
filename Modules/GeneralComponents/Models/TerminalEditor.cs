using System;
using System.Windows;
using System.Windows.Input;

namespace GeneralComponents.Models
{
    public class TerminalEditor : MvvmTextEditor
    {
        public TerminalEditor()
        {

        }

        protected new void LoadHighlightSyntax()
        {
            OpenHighLightFile("terminal.xshd");
        }


        public static readonly DependencyProperty CanEditProperty = DependencyProperty.Register("CanEdit", typeof(bool), typeof(TerminalEditor), new PropertyMetadata(default(bool)));
        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null) throw new NullReferenceException();
            if (!CanEdit)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                return;
            }
            base.OnKeyDown(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e == null) throw new NullReferenceException();
            e.Handled = !CanEdit;
            base.OnPreviewTextInput(e);
        }

    }
}
