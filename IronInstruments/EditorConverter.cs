using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using ICSharpCode.AvalonEdit.Document;

namespace IronInstruments
{
    class EditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TextDocument)
            {
                return value;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TextDocument)
            {
                return value;
            }
            return Binding.DoNothing;
        }
    }
}

