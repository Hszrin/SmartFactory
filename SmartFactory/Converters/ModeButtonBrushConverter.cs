using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartFactory.Converters
{
    public class ModeButtonBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not bool isChart)
                return new SolidColorBrush(Color.FromRgb(85, 85, 85));

            bool buttonIsChart =
                parameter?.ToString() == "Chart";

            return isChart == buttonIsChart
                ? new SolidColorBrush(Color.FromRgb(21, 101, 192))
                : new SolidColorBrush(Color.FromRgb(85, 85, 85));
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}