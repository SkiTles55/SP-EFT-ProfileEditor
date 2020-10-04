using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        public string MoneyTitle { get; set; }

        public int MoneyCount { get { return Convert.ToInt32(MoneyInput.Text); } }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            ThemeManager.Current.ChangeTheme(this, ColorScheme);
            OkButton.Content = OkButtonText;
            CancelButton.Content = CancelButtonText;
            DialogTitle.Text = MoneyTitle;
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

        private void MoneyInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (Int32.TryParse(textBox.Text, out int money))
            {
                if (money < 1) textBox.Text = "1";
            }  
            else
            {
                textBox.Text = Int32.MaxValue.ToString();
            }
        }
    }
}
