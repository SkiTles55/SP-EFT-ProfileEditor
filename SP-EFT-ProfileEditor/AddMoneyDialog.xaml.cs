using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для AddMoneyDialog.xaml
    /// </summary>
    public partial class AddMoneyDialog : MetroWindow
    {
        public AddMoneyDialog()
        {
            InitializeComponent();
        }

        public string OkButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string ColorScheme { get; set; }
        public string Title { get; set; }

        public int MoneyCount { get { return (int)MoneyInput.Value; } }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            ThemeManager.Current.ChangeTheme(this, ColorScheme);
            OkButton.Content = OkButtonText;
            CancelButton.Content = CancelButtonText;
            DialogTitle.Text = Title;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
