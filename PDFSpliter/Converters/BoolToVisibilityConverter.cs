using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PDFSplitter.Converters
{
    /// <summary>
    /// Converter for bool to visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value != null && (bool) value;

            return boolValue ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
