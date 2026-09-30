using SmartFactory.Enums;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartFactory.Converters
{
    public class PeriodButtonBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not DashboardPeriod selectedPeriod)
                return new SolidColorBrush(Color.FromRgb(85, 85, 85));

            if (parameter is not string periodText ||
                !Enum.TryParse<DashboardPeriod>(periodText, out var buttonPeriod))
                return new SolidColorBrush(Color.FromRgb(85, 85, 85));

            return selectedPeriod == buttonPeriod
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