using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для LanguageSelectWindows.xaml
    /// </summary>
    /// 

    public partial class LanguageSelectWindows : Window
    {
        public string SelectedLang { get; set; }

        Dictionary<string, string> Langs = new Dictionary<string, string>
        {
            ["en"] = "English",
            ["ru"] = "Русский",
            ["fr"] = "Français",
            ["ge"] = "Deutsch "
        };

        public LanguageSelectWindows()
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(MainWindow.Options.Language))
                SelectedLang = "en";
            else
                SelectedLang = MainWindow.Options.Language;
            langSelectBox.ItemsSource = Langs;
            langSelectBox.SelectedItem = new KeyValuePair<string, string>(SelectedLang, Langs[SelectedLang]);
        }

        private void langSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedLang = langSelectBox.SelectedValue.ToString();
        }

        private void buttonSelects_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
