using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TrainerApp.Converter;

public class NameToFontConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        => value is string fontname ? new FontFamily(fontname) : (object)new FontFamily("Arial");

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => value is FontFamily font ? font.Source : (object)"Arial";
}
