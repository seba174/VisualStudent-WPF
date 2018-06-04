using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contracts;

namespace SyntaxHighlightPlugin2
{
    public class SyntaxHighlightPlugin2 : IPlugin
    {
        public string Name
        {
            get { return "ForForeachIfElseUsing Plugin"; }
        }

        public void Do(RichTextBox richTextBox)
        {
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "using", FontStyles.Normal, FontWeights.Normal, (Brush)Brushes.Red, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "foreach", FontStyles.Normal, FontWeights.Normal, (Brush)Brushes.Red, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "for", FontStyles.Normal, FontWeights.Normal, (Brush)Brushes.Red, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "if", FontStyles.Normal, FontWeights.Normal, (Brush)Brushes.Red, (Brush)Brushes.Transparent, 12.0);
            TextManipulation.FromTextPointer(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd, "else", FontStyles.Normal, FontWeights.Normal, (Brush)Brushes.Red, (Brush)Brushes.Transparent, 12.0);
        }
    }
}
