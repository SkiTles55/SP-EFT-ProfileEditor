using System;
using System.Collections.Generic;
using System.IO;
using MahApps.Metro.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;
using MahApps.Metro.Controls.Dialogs;
using ControlzEx.Theming;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Lang Lang = new Lang();
        private BackgroundWorker LoadDataWorker;

        private List<Quest> Quests;

        Dictionary<string, string> Langs = new Dictionary<string, string>
        {
            ["en"] = "English",
            ["ru"] = "Русский",
            ["fr"] = "Français",
            ["ge"] = "Deutsch "
        };

        List<string> Sides = new List<string>
        {
            "Bear",
            "Usec"
        };

        private FolderBrowserDialog folderBrowserDialogSPT;

        //public static Dictionary<string, string> QuestNames { get; private set; } = new Dictionary<string, string>();
        //public static Dictionary<string, string> TraderNames { get; private set; } = new Dictionary<string, string>();
        
        //https://dev.offline-tarkov.com/sp-tarkov/server/src/branch/development/project/core/util/utility.js - generate id's
        //https://dev.offline-tarkov.com/sp-tarkov/server/src/branch/development/project/src/classes/profile.js - level calculate

        public MainWindow()
        {
            InitializeComponent();
            infotab_Side.ItemsSource = Sides;
            LoadDataWorker = new BackgroundWorker();
            LoadDataWorker.DoWork += LoadDataWorker_DoWork;
            LoadDataWorker.RunWorkerCompleted += LoadDataWorker_RunWorkerCompleted;
        }

        private void LoadDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Quests != null)
                questsGrid.ItemsSource = Quests;
        }

        private void LoadDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Quests = new List<Quest>();
            foreach (var TraderPath in Directory.GetDirectories(Path.Combine(Lang.options.EftServerPath, "db", "assort")))
            {
                if (Directory.Exists(Path.Combine(TraderPath, "quests")))
                {
                    foreach (var QuestInfo in Directory.GetFiles(Path.Combine(TraderPath, "quests")))
                    {
                        Quests.Add(new Quest { name = "just test", qid = Path.GetFileNameWithoutExtension(QuestInfo), trader = "just test" });
                    }
                }
            }
            /*
            QuestNames = new Dictionary<string, string>();
            foreach (var qn in Directory.GetFiles(ServerPath + "\\db\\locales\\ru\\quest"))
                QuestNames.Add(Path.GetFileNameWithoutExtension(qn), JsonConvert.DeserializeObject<QuestLocale>(File.ReadAllText(qn)).name);
            TraderNames = new Dictionary<string, string>();
            foreach (var tn in  Directory.GetDirectories(ServerPath + "\\db\\assort"))
                TraderNames.Add(Path.GetFileName(tn), JsonConvert.DeserializeObject<TraderLocale>(File.ReadAllText(tn + "\\base.json")).nickname);
            foreach (var q in Profile.Quests)
            {
                try {
                    questsGrid.Items.Add(new QuestInfo { status = q.status, trader = TraderNames[q.qid], name = QuestNames[q.qid] });
                }
                catch { }
            //need to find names in db quests folder
            }
            */
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool readyToLoad = false;
            Lang = Lang.Load();
            if (string.IsNullOrWhiteSpace(Lang.options.Language) || string.IsNullOrWhiteSpace(Lang.options.EftServerPath)
                || !Directory.Exists(Lang.options.EftServerPath) || !PathIsEftServerBase(Lang.options.EftServerPath)
                || string.IsNullOrWhiteSpace(Lang.options.DefaultProfile) || !Directory.Exists(Path.Combine(Lang.options.EftServerPath, "user\\profiles", Lang.options.DefaultProfile)) || !File.Exists(Path.Combine(Lang.options.EftServerPath, "user\\profiles", Lang.options.DefaultProfile, "character.json")))
                SettingsBorder.Visibility = Visibility.Visible;
            else
                readyToLoad = true;
            langSelectBox.ItemsSource = Langs;
            langSelectBox.SelectedItem = new KeyValuePair<string, string>(Lang.options.Language, Langs[Lang.options.Language]);
            if (!string.IsNullOrEmpty(Lang.options.ColorScheme))
            {
                ThemeManager.Current.ChangeTheme(this, Lang.options.ColorScheme);
            }
            foreach (var style in ThemeManager.Current.Themes.OrderBy(x => x.DisplayName))
            {
                var newItem = new AccentItem { Name = style.DisplayName, Color = style.PrimaryAccentColor.ToString(), Scheme = style.Name };
                StyleChoicer.Items.Add(newItem);
                if (ThemeManager.Current.DetectTheme(this).DisplayName == newItem.Name) StyleChoicer.SelectedItem = newItem;
            }
            DataContext = Lang;
            if (readyToLoad)
                LoadDataWorker.RunWorkerAsync();
        }

        private bool PathIsEftServerBase(string sptPath)
        {
            if (string.IsNullOrWhiteSpace(sptPath)) return false;
            if (!Directory.Exists(sptPath)) return false;
            if (!File.Exists(Path.Combine(sptPath, "Server.exe"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "db"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"db\items"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"db\locales"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"user\configs"))) return false;
            if (!File.Exists(Path.Combine(sptPath, @"user\configs\accounts.json"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"user\profiles"))) return false;

            return true;
        }

        private void langSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lang.options.Language = langSelectBox.SelectedValue.ToString();
            Lang.SaveOptions();
            Lang = Lang.Load();
            DataContext = Lang;
        }

        private async void serverSelect_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings
            {
                DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"]
            };
            folderBrowserDialogSPT = new FolderBrowserDialog
            {
                Description = Lang.locale["server_select"],
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false
            };
            bool pathOK = false;
            do
            {
                if (!string.IsNullOrWhiteSpace(Lang.options.EftServerPath) && Directory.Exists(Lang.options.EftServerPath))
                    folderBrowserDialogSPT.SelectedPath = Lang.options.EftServerPath;
                if (folderBrowserDialogSPT.ShowDialog() != System.Windows.Forms.DialogResult.OK) pathOK = false;
                if (PathIsEftServerBase(folderBrowserDialogSPT.SelectedPath)) pathOK = true;
            } while (
                !pathOK &&
                (await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["invalid_server_location_text"], MessageDialogStyle.AffirmativeAndNegative, dialogSettings) == MessageDialogResult.Affirmative)
            );

            if (pathOK)
            {
                Lang.options.EftServerPath = folderBrowserDialogSPT.SelectedPath;
                Lang.SaveOptions();
                Lang = Lang.Load();
                DataContext = Lang;
            }
        }

        private void profileSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lang.options.DefaultProfile = profileSelectBox.SelectedValue.ToString();
            Lang.SaveOptions();
            Lang = Lang.Load();
            DataContext = Lang;
        }

        private void StyleChoicer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccent = StyleChoicer.SelectedItem as AccentItem;
            if (selectedAccent.Name == ThemeManager.Current.DetectTheme(this).DisplayName) return;
            ThemeManager.Current.ChangeTheme(this, selectedAccent.Scheme);
            Lang.options.ColorScheme = selectedAccent.Scheme;
            Lang.SaveOptions();
            Lang = Lang.Load();
            DataContext = Lang;
        }

        private void TabSettingsClose_Click(object sender, RoutedEventArgs e) => SettingsBorder.Visibility = Visibility.Collapsed;

        private void Button_Click(object sender, RoutedEventArgs e) => SettingsBorder.Visibility = Visibility.Visible;

        private void test_Click(object sender, RoutedEventArgs e)
        {
            JsonSerializerSettings seriSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            Lang.Character.Info.LowerNickname = Lang.Character.Info.Nickname.ToLower();
            string json = JsonConvert.SerializeObject(Lang.Character, seriSettings);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testprofile.json"), json);
            System.Windows.Forms.MessageBox.Show("profile saved!");
        }

        private void infotab_Side_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!infotab_Voice.Items.Contains(infotab_Voice.SelectedItem))
                infotab_Voice.SelectedIndex = 0;
        }

        private void infotab_Level_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            Lang.Character.Info.Level = Convert.ToInt32(textBox.Text);
        }
    }
}