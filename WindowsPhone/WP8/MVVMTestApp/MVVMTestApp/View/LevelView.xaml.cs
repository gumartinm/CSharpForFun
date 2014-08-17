using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MVVMTestApp.View
{
    public partial class LevelView : UserControl
    {
        public LevelView()
        {
            InitializeComponent();
        }
    }


    public class BoolOpposite : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            bool b;

            if (bool.TryParse(s, out b))
            {
                return !b;
            }
            return false;
        }
    }
}