// Add to a new file named Converters.cs in the GhChatbot.UI namespace
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Glab.C_AI.Chatbot.Converters
{
    public class Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiThinkVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return Visibility.Collapsed;

            // First value: IsThinkingHidden (bool) == True
            if (values[0] is bool isThinkingHidden && isThinkingHidden)
            {
                return Visibility.Collapsed;
            }

            // Second value: Content (string) 
            if (values[1] is string content && content.Contains("<think>") && content.Contains("</think>"))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? new SolidColorBrush(Colors.AliceBlue) : new SolidColorBrush(Colors.WhiteSmoke);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUserMessage)
            {
                return isUserMessage ? Brushes.Blue : Brushes.Gray;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThinkVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content && content.Contains("<think>") && content.Contains("</think>"))
            {
                return Visibility.Visible; // Show the border if <think> tags are present
            }
            return Visibility.Collapsed; // Hide the border if no <think> tags are present
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThinkContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content && content.Contains("<think>") && content.Contains("</think>"))
            {
                int startIndex = content.IndexOf("<think>") + "<think>".Length;
                int endIndex = content.IndexOf("</think>");
                return content.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return null; // Return null to ensure no content is displayed
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MainContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string content)
            {
                if (content.Contains("<think>") && content.Contains("</think>"))
                {
                    int thinkStartIndex = content.IndexOf("<think>");
                    int thinkEndIndex = content.IndexOf("</think>") + "</think>".Length;

                    // Get content before <think>
                    string beforeThink = content.Substring(0, thinkStartIndex);

                    // Get content after </think>
                    string afterThink = "";
                    if (thinkEndIndex < content.Length)
                    {
                        afterThink = content.Substring(thinkEndIndex);
                    }

                    return (beforeThink + afterThink).Trim();
                }
                return content;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
