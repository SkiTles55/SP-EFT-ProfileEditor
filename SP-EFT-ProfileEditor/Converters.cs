using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SP_EFT_ProfileEditor
{
    public class PathBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;

            if (string.IsNullOrWhiteSpace(value.ToString())) return Visibility.Visible;
            if (!Directory.Exists(value.ToString())) return Visibility.Visible;
            if (!File.Exists(Path.Combine(value.ToString(), "Server.exe"))) return Visibility.Visible;
            if (!Directory.Exists(Path.Combine(value.ToString(), "db"))) return Visibility.Visible;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"db\items"))) return Visibility.Visible;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"db\locales"))) return Visibility.Visible;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"user\configs"))) return Visibility.Visible;
            if (!File.Exists(Path.Combine(value.ToString(), @"user\configs\accounts.json"))) return Visibility.Visible;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"user\profiles"))) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ButtonBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            if (string.IsNullOrWhiteSpace(value.ToString())) return false;
            if (!Directory.Exists(value.ToString())) return false;
            if (!File.Exists(Path.Combine(value.ToString(), "Server.exe"))) return false;
            if (!Directory.Exists(Path.Combine(value.ToString(), "db"))) return false;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"db\items"))) return false;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"db\locales"))) return false;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"user\configs"))) return false;
            if (!File.Exists(Path.Combine(value.ToString(), @"user\configs\accounts.json"))) return false;
            if (!Directory.Exists(Path.Combine(value.ToString(), @"user\profiles"))) return false;
            if (Directory.GetDirectories(value.ToString() + "\\user\\profiles").Where(x => File.Exists(x + "\\character.json")).Count() < 1) return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ProfileBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;
            if (Directory.GetDirectories(value.ToString() + "\\user\\profiles").Where(x => File.Exists(x + "\\character.json")).Count() < 1) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ProfileSideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return new List<string>
            {
                value.ToString() + "_1",
                value.ToString() + "_2"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ProfileLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            long exp = 0;
            int level = 0;
            for (int i = 0; i < ExpTable.Count(); i++)
            {
                if ((long)value < exp)
                {
                    break;
                }

                //Lang.Character.Info.Level = i;
                exp += ExpTable[i];
                level = i;
            }
            return level;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private List<long> ExpTable
        {
            get => new List<long>
        {
            0,
                1000,
                2743,3999,5256,6494,7658,8851,10025,11098,12226,13336,16814,19924,23053,26283,29219,32045,34466,37044
                ,39162,41492,44002,46900,51490,56080,60670,65260,69850,74440,79030,83620,90964,98308,105652,112996,120340
                ,127684,135028,142372,149716,157060,167158,177256,187354,197452,207550,217648,227746,237844,247942,258040
                ,271810,285580,299350,313120,323450,362111,369536,386978,407174,430124,457664,494384,549464,622904,760604
                ,1036004,1449104,10000000
        };
        }
    }
}