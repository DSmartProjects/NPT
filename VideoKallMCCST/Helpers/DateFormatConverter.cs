using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace VideoKallMCCST.Helpers
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            DateTime dt = DateTime.Parse(value.ToString());
            return dt.ToString(Constants.US_DATE_MM_DD_YYYY, CultureInfo.CreateSpecificCulture(Constants.US_DATE_FORMATE));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
