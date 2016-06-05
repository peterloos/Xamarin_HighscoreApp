namespace SimpleHighScoreApp.Converters
{
    using System;
    using System.Globalization;

    using Xamarin.Forms;
    public class StringToColorConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            String s = (String)value;

            if (String.IsNullOrEmpty(s))
                return Color.Default;

            return (s.ToLower().Equals("online")) ? Color.Green : Color.Red;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
