using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Core;

namespace BatchRename.Converters
{
    public class PreviewRenameConverter : IValueConverter
    {
        public List<IRule> Rules { get; set; }

        public PreviewRenameConverter()
        {
            Rules = new List<IRule>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string origin = (string)value;

            string param = parameter as string;
            if (param.Equals("Success"))
            {
                return origin;
            }
            

            string newName = origin;

            foreach (var rule in Rules)
            {
                newName = rule.Rename(newName);
            }
            return newName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}