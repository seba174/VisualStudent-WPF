using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Contracts
{
    public class TextManipulation
    {
        public static void FromTextPointer(TextPointer startPointer, TextPointer endPointer, string keyword, FontStyle fontStyle, FontWeight fontWeight, Brush foreground, Brush background, double fontSize)
        {
            TextManipulation.FromTextPointer(startPointer, endPointer, keyword, fontStyle, fontWeight, foreground, background, fontSize, (string)null);
        }

        public static void FromTextPointer(TextPointer startPointer, TextPointer endPointer, string keyword, FontStyle fontStyle, FontWeight fontWeight, Brush foreground, Brush background, double fontSize, string newString)
        {
            if (startPointer == null) throw new ArgumentNullException(nameof(startPointer));
            if (endPointer == null) throw new ArgumentNullException(nameof(endPointer));
            if (string.IsNullOrEmpty(keyword)) throw new ArgumentNullException(keyword);

            TextRange text = new TextRange(startPointer, endPointer);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    int index = textInRun.IndexOf(keyword);
                    if (index != -1)
                    {
                        TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                        TextRange selection = new TextRange(selectionStart, selectionEnd);

                        if (!string.IsNullOrEmpty(newString))
                            selection.Text = newString;

                        selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                        selection.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
                        selection.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
                        selection.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
                        selection.ApplyPropertyValue(TextElement.BackgroundProperty, background);
                    }
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }
}
