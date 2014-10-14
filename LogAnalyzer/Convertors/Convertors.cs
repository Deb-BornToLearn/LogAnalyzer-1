using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LogAnalyzer.Convertors
{
    public class TextBoxCollectionConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var textCollection = values[0] as ObservableCollection<string>;
            string fullText = string.Empty;

            if (textCollection != null && textCollection.Count > 0)
            {
              //  return textCollection.ToString();
                var sb = new StringBuilder();
                foreach (var line in textCollection)
                {
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
            else
             return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
