using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contracts;

namespace SyntaxHighlightPlugin
{
    public class SyntaxHighlightPlugin : IPlugin
    {
        public string Name
        {
            get { return "PublicPrivateGetSet Plugin"; }
        }

        public void Do(RichTextBox richTextBox)
        {
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "public", FontStyles.Normal, FontWeights.Bold, (Brush)Brushes.Blue, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "private", FontStyles.Normal, FontWeights.Bold, (Brush)Brushes.Blue, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "get", FontStyles.Normal, FontWeights.Bold, (Brush)Brushes.Blue, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "set", FontStyles.Normal, FontWeights.Bold, (Brush)Brushes.Blue, (Brush)Brushes.Transparent, 12.0);
        }
    }
}
