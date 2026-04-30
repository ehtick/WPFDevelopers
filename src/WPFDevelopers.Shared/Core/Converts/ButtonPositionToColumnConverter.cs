using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Converts
{
    
    public class ButtonPositionToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = (Position)value;
            return position == Position.Left ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ButtonPositionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            var position = (Position)value;
            var targetPosition = (Position)parameter;

            return position == targetPosition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
