using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BatchRename
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
