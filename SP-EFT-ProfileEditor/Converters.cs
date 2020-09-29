using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SP_EFT_ProfileEditor
{
    public class ExpToLevelConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            return Int32.Parse(value.ToString()) / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
    public class QuestEmptyBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value > 1) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
    public class RevertedQuestEmptyBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value > 1) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class PathBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;
            return ExtMethods.PathIsEftServerBase(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class ButtonBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return ExtMethods.PathIsEftServerBase(value.ToString()) && ExtMethods.ServerHaveProfiles(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class ProfileBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;
            if (!ExtMethods.ServerHaveProfiles(value.ToString())) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class ProfileSideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return new List<string>
            {
                value.ToString() + "_1",
                value.ToString() + "_2"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class ProfileLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            long exp = 0;
            int level = 0;
            for (int i = 0; i < ExpTable.Count(); i++)
            {
                if ((long)value < exp)
                    break;
                exp += ExpTable[i];
                level = i;
            }
            return level;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var st = value.ToString();
            if (string.IsNullOrEmpty(st))
                st = "0";
            int lvl = Int32.Parse(st);
            if (lvl > ExpTable.Count())
                lvl = ExpTable.Count();
            long exp = 0;
            for (int i = 0; i < lvl; i++)
                exp += ExpTable[i];
            return exp;
        }

        private List<long> ExpTable => new List<long>  { 0, 1000,
                2743,3999,5256,6494,7658,8851,10025,11098,12226,13336,16814,19924,23053,26283,29219,32045,34466,37044
                ,39162,41492,44002,46900,51490,56080,60670,65260,69850,74440,79030,83620,90964,98308,105652,112996,120340
                ,127684,135028,142372,149716,157060,167158,177256,187354,197452,207550,217648,227746,237844,247942,258040
                ,271810,285580,299350,313120,323450,362111,369536,386978,407174,430124,457664,494384,549464,622904,760604
                ,1036004,1449104,10000000 };
    }
}