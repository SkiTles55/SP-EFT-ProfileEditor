using System;
using System.Collections.Generic;
using System.IO;
using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Newtonsoft.Json;
using MahApps.Metro.Controls.Dialogs;
using ControlzEx.Theming;
using System.ComponentModel;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainData Lang = new MainData();
        private BackgroundWorker LoadDataWorker;
        private ProgressDialogController progressDialog;
        private string lastdata = null;

        private List<Quest> Quests;
        private Dictionary<string, TraderLocale> TradersLocales;
        public Dictionary<string, QuestLocale> QuestsLocales;

        private Dictionary<string, string> Langs = new Dictionary<string, string>
        {
            ["en"] = "English",
            ["ru"] = "Русский",
            ["fr"] = "Français",
            ["ge"] = "Deutsch "
        };

        private FolderBrowserDialog folderBrowserDialogSPT;
        
        //https://dev.offline-tarkov.com/sp-tarkov/server/src/branch/development/project/core/util/utility.js - generate id's

        public MainWindow()
        {
            InitializeComponent();
            infotab_Side.ItemsSource = new List<string> { "Bear", "Usec" };
            QuestsStatusesBox.ItemsSource = new List<string> { "Locked", "AvailableForStart", "Started", "Fail", "AvailableForFinish", "Success" };
            QuestsStatusesBox.SelectedIndex = 0;
            LoadDataWorker = new BackgroundWorker();
            LoadDataWorker.DoWork += LoadDataWorker_DoWork;
            LoadDataWorker.RunWorkerCompleted += LoadDataWorker_RunWorkerCompleted;
        }

        private void LoadDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Quests != null)
                questsGrid.ItemsSource = Quests;
            progressDialog.CloseAsync();
        }

        private void LoadDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Lang.Character.Quests != null)
            {
                TradersLocales = new Dictionary<string, TraderLocale>();
                foreach (var trader in Directory.GetFiles(Path.Combine(Lang.options.EftServerPath, "db", "locales", Lang.options.Language, "trading")))
                    TradersLocales.Add(Path.GetFileNameWithoutExtension(trader), JsonConvert.DeserializeObject<TraderLocale>(File.ReadAllText(trader)));
                QuestsLocales = new Dictionary<string, QuestLocale>();
                foreach (var quest in Directory.GetFiles(Path.Combine(Lang.options.EftServerPath, "db", "locales", Lang.options.Language, "quest")))
                    QuestsLocales.Add(Path.GetFileNameWithoutExtension(quest), JsonConvert.DeserializeObject<QuestLocale>(File.ReadAllText(quest)));
                Quests = new List<Quest>();
                foreach (var TraderPath in Directory.GetDirectories(Path.Combine(Lang.options.EftServerPath, "db", "assort")))
                {
                    if (Directory.Exists(Path.Combine(TraderPath, "quests")))
                    {
                        foreach (var QuestInfo in Directory.GetFiles(Path.Combine(TraderPath, "quests")))
                        {
                            string qid = Path.GetFileNameWithoutExtension(QuestInfo);
                            var quest = Lang.Character.Quests.Where(x => x.Qid == qid).FirstOrDefault();
                            if (quest != null)
                                Quests.Add(new Quest { qid = qid, name = QuestsLocales[qid].name, status = quest.Status, trader = TradersLocales[Path.GetFileName(TraderPath)].Nickname });
                        }
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool readyToLoad = false;
            Lang = MainData.Load();
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
            CheckPockets();
            if (readyToLoad)
            {
                lastdata = Lang.options.EftServerPath + Lang.options.Language + Lang.Character.Aid;
                LoadData();
            }
        }

        private async void LoadData()
        {
            //progressDialog = await this.ShowProgressAsync(Lang.locale["progressdialog_title"], Lang.locale["progressdialog_caption"]);
            //progressDialog.SetIndeterminate();
            //LoadDataWorker.RunWorkerAsync();
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
            if (!IsLoaded)
                return;
            Lang.options.Language = langSelectBox.SelectedValue.ToString();
            SaveAndReload();
        }

        private void SaveAndReload()
        {
            Lang.SaveOptions();
            Lang = MainData.Load();
            DataContext = Lang;
            questsGrid.ItemsSource = null;
            questsGrid.ItemsSource = Lang.Character.Quests;
            CheckPockets();
        }

        private void CheckPockets()
        {
            if (Lang.Character == null) return;
            if (Lang.Character.Inventory.Items.Where(x => x.Tpl == "557ffd194bdc2d28148b457f").Count() > 0) 
                BigPocketsSwitcher.IsOn = false;
            if (Lang.Character.Inventory.Items.Where(x => x.Tpl == "5af99e9186f7747c447120b8").Count() > 0)
                BigPocketsSwitcher.IsOn = true;
        }

        private async void serverSelect_Click(object sender, RoutedEventArgs e)
        {
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
                (await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["invalid_server_location_text"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"] }) == MessageDialogResult.Affirmative)
            );

            if (pathOK)
            {
                Lang.options.EftServerPath = folderBrowserDialogSPT.SelectedPath;
                SaveAndReload();
            }
        }

        private void profileSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            Lang.options.DefaultProfile = profileSelectBox.SelectedValue.ToString();
            SaveAndReload();
        }

        private void StyleChoicer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            var selectedAccent = StyleChoicer.SelectedItem as AccentItem;
            if (selectedAccent.Name == ThemeManager.Current.DetectTheme(this).DisplayName) return;
            ThemeManager.Current.ChangeTheme(this, selectedAccent.Scheme);
            Lang.options.ColorScheme = selectedAccent.Scheme;
            SaveAndReload();
        }

        private void TabSettingsClose_Click(object sender, RoutedEventArgs e)
        {
            SettingsBorder.Visibility = Visibility.Collapsed;
            if (lastdata != Lang.options.EftServerPath + Lang.options.Language + Lang.Character.Aid)
            {
                LoadData();
                lastdata = Lang.options.EftServerPath + Lang.options.Language + Lang.Character.Aid;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => SettingsBorder.Visibility = Visibility.Visible;

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
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
            if (!IsLoaded)
                return;
            if (!infotab_Voice.Items.Contains(infotab_Voice.SelectedItem))
                infotab_Voice.SelectedIndex = 0;
        }

        private void infotab_Level_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            Lang.Character.Info.Level = Convert.ToInt32(textBox.Text);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            var comboBox = sender as System.Windows.Controls.ComboBox;
            Lang.Character.Quests.Where(x => x.Qid == ((Character.Character_Quests)comboBox.DataContext).Qid).FirstOrDefault().Status = comboBox.SelectedItem.ToString();
        }

        private void BigPocketsSwitcher_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            if (Lang.Character.Inventory.Items == null) return;
            if (Lang.Character.Inventory.Items.Where(x => x.Tpl == "557ffd194bdc2d28148b457f" || x.Tpl == "5af99e9186f7747c447120b8").Count() > 0)
            {
                Lang.Character.Inventory.Items.Where(x => x.Tpl == "557ffd194bdc2d28148b457f" || x.Tpl == "5af99e9186f7747c447120b8").FirstOrDefault().Tpl = BigPocketsSwitcher.IsOn ? "5af99e9186f7747c447120b8" : "557ffd194bdc2d28148b457f";
            }
        }

        private void QuestsStatusesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Lang.Character.Quests == null) return;
            foreach (var q in Lang.Character.Quests)
                q.Status = QuestsStatusesBox.SelectedItem.ToString();
            questsGrid.ItemsSource = null;
            questsGrid.ItemsSource = Lang.Character.Quests;
        }

        private async void ResetProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["reloadprofiledialog_caption"], Lang.locale["reloadprofiledialog_title"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"] }) == MessageDialogResult.Affirmative)
            {
                SaveAndReload();
                LoadData();
            }
        }
    }
}