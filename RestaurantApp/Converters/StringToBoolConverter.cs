using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace RestaurantApp.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        // Support class to convert a string to a boolean value for visibility purposes
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the string is not null or whitespace, return true (Visible)
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
