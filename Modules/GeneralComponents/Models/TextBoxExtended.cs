using System.Windows.Controls;

namespace GeneralComponents.Models
{
    public class TextBoxExtended : TextBox
    {
        public TextBoxExtended()
        {
            this.GotFocus += TextBoxExtended_GotFocus;
        }

        private void TextBoxExtended_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SelectAll();
        }
    }
}
