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
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Reflection;
using System.Threading.Tasks;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GlobalLang globalLang;
        private bool _shutdown;
        private FolderBrowserDialog folderBrowserDialogSPT;
        private MainData Lang = new MainData();
        private BackgroundWorker LoadDataWorker;
        private ProgressDialogController progressDialog;
        private string lastdata = null;
        private List<BackupFile> backups;
        private List<SkillInfo> commonSkills;
        private List<SkillInfo> masteringSkills;
        private List<CharacterHideoutArea> HideoutAreas;
        private List<TraderInfo> traderInfos;
        private List<Quest> Quests;
        private Dictionary<string, Item> itemsDB;
        private List<ExaminedItem> examinedItems;
        private BackgroundWorker SaveProfileWorker;
        private Dictionary<string, Dictionary<string, string>> ItemsForAdd;
        private ServerGlobals serverGlobals;
        private List<SuitInfo> Suits;

        private readonly string moneyRub = "5449016a4bdc2d6f028b456f";
        private readonly string moneyDol = "5696686a4bdc2da3298b456a";
        private readonly string moneyEur = "569668774bdc2da2298b4568";

        private readonly Dictionary<string, string> Langs = new Dictionary<string, string>
        {
            ["en"] = "English",
            ["ru"] = "Русский",
            ["fr"] = "Français",
            ["ge"] = "Deutsch "
        };

        private readonly List<string> BannedItems = new List<string>
        {
            "543be5dd4bdc2deb348b4569",
            "55d720f24bdc2d88028b456d",
            "557596e64bdc2dc2118b4571",
            "566965d44bdc2d814c8b4571",
            "566abbb64bdc2d144c8b457d",
            "5448f39d4bdc2d0a728b4568",
            "5943d9c186f7745a13413ac9",
            "5996f6cb86f774678763a6ca",
            "5996f6d686f77467977ba6cc",
            "5996f6fc86f7745e585b4de3",
            "5cdeb229d7f00c000e7ce174",
            "5d52cc5ba4b9367408500062"
        };


        public MainWindow()
        {
            InitializeComponent();
            infotab_Side.ItemsSource = new List<string> { "Bear", "Usec" };
            QuestsStatusesBox.ItemsSource = new List<string> { "Locked", "AvailableForStart", "Started", "Fail", "AvailableForFinish", "Success" };
            QuestsStatusesBox.SelectedIndex = 0;
            LoadDataWorker = new BackgroundWorker();
            LoadDataWorker.DoWork += LoadDataWorker_DoWork;
            LoadDataWorker.RunWorkerCompleted += LoadDataWorker_RunWorkerCompleted;
            SaveProfileWorker = new BackgroundWorker();
            SaveProfileWorker.DoWork += SaveProfileWorker_DoWork;
            SaveProfileWorker.RunWorkerCompleted += SaveProfileWorker_RunWorkerCompleted;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Title += string.Format(" {0}.{1}", version.Major, version.Minor);
        }

        private void LoadDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (traderInfos != null)
                merchantsGrid.ItemsSource = traderInfos;
            if (Quests != null)
                questsGrid.ItemsSource = Quests;
            if (HideoutAreas != null)
                hideoutGrid.ItemsSource = HideoutAreas;
            if (commonSkills != null)
                skillsGrid.ItemsSource = commonSkills;
            if (masteringSkills != null)
                masteringsGrid.ItemsSource = masteringSkills;
            if (backups != null)
                backupsGrid.ItemsSource = backups;
            if (examinedItems != null)
                examinedGrid.ItemsSource = examinedItems;
            RublesLabel.Text = Lang.characterInventory.Rubles.ToString();
            EurosLabel.Text = Lang.characterInventory.Euros.ToString();
            DollarsLabel.Text = Lang.characterInventory.Dollars.ToString();
            if (Lang.characterInventory.InventoryItems != null)
                stashGrid.ItemsSource = Lang.characterInventory.InventoryItems;
            if (ItemsForAdd != null)
                ItemCatSelector.ItemsSource = ItemsForAdd.OrderBy(x => x.Key);
            ItemCatSelector.SelectedIndex = 0;
            if (Suits != null)
                suitsGrid.ItemsSource = Suits;
            progressDialog.CloseAsync();
        }

        private void LoadDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            serverGlobals = JsonConvert.DeserializeObject<ServerGlobals>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "globals.json")));
            globalLang = JsonConvert.DeserializeObject<GlobalLang>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "locales", "global", Lang.options.Language + ".json")));
            itemsDB = new Dictionary<string, Item>();
            itemsDB = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "templates", "items.json")));
            if (Lang.Character.Quests != null)
            {
                Quests = new List<Quest>();
                Dictionary<string, QuestData> qData = JsonConvert.DeserializeObject<Dictionary<string, QuestData>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "templates", "quests.json")));
                List<Character.Character_Quests> forAdd = new List<Character.Character_Quests>();
                foreach (var qd in qData.Values)
                {
                    if (Lang.Character.Quests.Where(x => x.Qid == qd._id).Count() > 0)
                    {
                        var quest = Lang.Character.Quests.Where(x => x.Qid == qd._id).FirstOrDefault();
                        Quests.Add(new Quest { qid = quest.Qid, name = globalLang.Quests[quest.Qid].name, status = quest.Status, trader = globalLang.Traders[qd.traderId].Nickname });
                    }
                    else
                    {
                        forAdd.Add(new Character.Character_Quests { Qid = qd._id, Status = "Locked", CompletedConditions = new string[0], StartTime = 0, StatusTimers = new Dictionary<string, decimal>() });
                        Quests.Add(new Quest { qid = qd._id, name = globalLang.Quests[qd._id].name, status = "Locked", trader = globalLang.Traders[qd.traderId].Nickname });
                    }
                }
                if (forAdd.Count > 0)
                {
                    var temp = Lang.Character.Quests.ToList();
                    foreach (var add in forAdd)
                        temp.Add(add);
                    Lang.Character.Quests = temp.ToArray();
                }
            }
            if (Lang.Character.Skills != null)
            {
                commonSkills = new List<SkillInfo>();
                masteringSkills = new List<SkillInfo>();
                List<Character.Character_Skills.Character_Skill> forAdd = new List<Character.Character_Skills.Character_Skill>();
                foreach (var skill in Lang.Character.Skills.Common)
                {
                    if (globalLang.Interface.ContainsKey(skill.Id))
                        commonSkills.Add(new SkillInfo { progress = (int)skill.Progress, name = globalLang.Interface[skill.Id], id = skill.Id });
                }
                foreach (var md in serverGlobals.config.Mastering)
                {
                    string weapons = string.Empty;
                    foreach (var tmp in md.Templates.Where(x => globalLang.Templates.ContainsKey(x)))
                        weapons += globalLang.Templates[tmp].Name;
                    if (string.IsNullOrEmpty(weapons)) continue;
                    if (Lang.Character.Skills.Mastering.Where(x => x.Id == md.Name).Count() > 0)
                    {
                        var mastering = Lang.Character.Skills.Mastering.Where(x => x.Id == md.Name).FirstOrDefault();
                        masteringSkills.Add(new SkillInfo { progress = (int)mastering.Progress, name = weapons, id = mastering.Id, Max = md.Level3 });
                    }
                    else
                    {
                        forAdd.Add(new Character.Character_Skills.Character_Skill { Id = md.Name, Progress = 0 });
                        masteringSkills.Add(new SkillInfo { progress = 0, name = weapons, id = md.Name, Max = md.Level3 });
                    }
                }
                if (forAdd.Count > 0)
                {
                    var temp = Lang.Character.Skills.Mastering.ToList();
                    foreach (var add in forAdd)
                        temp.Add(add);
                    Lang.Character.Skills.Mastering = temp.ToArray();
                }
                Dispatcher.Invoke(() => { allmastering_exp.Maximum = serverGlobals.config.Mastering.OrderByDescending(x => x.Level3).First()?.Level3 ?? 1000; });
            }
            if (Lang.Character.TraderStandings != null)
            {
                traderInfos = new List<TraderInfo>();
                foreach (var mer in Lang.Character.TraderStandings)
                {
                    if (mer.Key == "ragfair") continue;
                    List<LoyaltyLevel> loyalties = new List<LoyaltyLevel>();
                    foreach (var lv in mer.Value.LoyaltyLevels)
                        loyalties.Add(new LoyaltyLevel { level = Int32.Parse(lv.Key) + 1, SalesSum = lv.Value.MinSalesSum + 1000, Standing = lv.Value.MinStanding + 0.01f });
                    traderInfos.Add(new TraderInfo { id = mer.Key, name = globalLang.Traders[mer.Key].Nickname, CurrentLevel = mer.Value.CurrentLevel, Levels = loyalties });
                }
            }
            if (Lang.Character.Hideout != null)
            {
                HideoutAreas = new List<CharacterHideoutArea>();
                var areas = JsonConvert.DeserializeObject<List<AreaInfo>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "hideout", "areas.json")));
                foreach (var area in areas)
                    HideoutAreas.Add(new CharacterHideoutArea { type = area.type, name = globalLang.Interface[$"hideout_area_{area.type}_name"], MaxLevel = area.stages.Count - 1, CurrentLevel = Lang.Character.Hideout.Areas.Where(x => x.Type == area.type).FirstOrDefault().Level, stages = area.stages });
            }
            if (Lang.Character.Encyclopedia != null)
            {
                examinedItems = new List<ExaminedItem>();
                foreach (var eItem in Lang.Character.Encyclopedia)
                    examinedItems.Add(new ExaminedItem { id = eItem.Key, name = globalLang.Templates[eItem.Key].Name });
            }
            Lang.characterInventory.Dollars = 0;
            Lang.characterInventory.Euros = 0;
            Lang.characterInventory.Rubles = 0;
            if (Lang.Character.Inventory != null)
            {
                Lang.characterInventory.InventoryItems = new List<InventoryItem>();
                foreach (var item in Lang.Character.Inventory.Items)
                {
                    if (item.Tpl == moneyDol) Lang.characterInventory.Dollars += (int)item.Upd.StackObjectsCount;
                    if (item.Tpl == moneyEur) Lang.characterInventory.Euros += (int)item.Upd.StackObjectsCount;
                    if (item.Tpl == moneyRub) Lang.characterInventory.Rubles += (int)item.Upd.StackObjectsCount;
                    if (item.ParentId == Lang.Character.Inventory.Stash)
                        Lang.characterInventory.InventoryItems.Add(new InventoryItem { id = item.Id, name = globalLang.Templates[item.Tpl].Name });
                }
            }
            ItemsForAdd = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in itemsDB.Where(x => x.Value.type == "Item" && x.Value.parent != null 
                && globalLang.Templates.ContainsKey(x.Value.parent) 
                && !x.Value.props.QuestItem && !BannedItems.Contains(x.Value.parent) && !BannedItems.Contains(x.Value.id)
                && x.Value.props.StackMaxSize == 1))
            {
                string cat = globalLang.Templates[item.Value.parent].Name;
                if (!ItemsForAdd.ContainsKey(cat))
                    ItemsForAdd.Add(cat, new Dictionary<string, string>());
                ItemsForAdd[cat].Add(item.Key, globalLang.Templates[item.Key].Name);
            }
            if (Lang.Character.Suits != null)
            {
                Suits = new List<SuitInfo>();
                foreach (var s in Directory.GetDirectories(Path.Combine(Lang.options.EftServerPath, "packages", "eft-database", "db", "traders")).Where(x => File.Exists(Path.Combine(x, "suits.json"))))
                {
                    var temp = JsonConvert.DeserializeObject<TraderSuit[]>(File.ReadAllText(Path.Combine(s, "suits.json")));
                    if (temp != null)
                    {
                        foreach (var suit in temp)
                            Suits.Add(new SuitInfo { Name = globalLang.Templates.ContainsKey(suit.suiteId) ? globalLang.Templates[suit.suiteId].Name : suit._id, ID = suit.suiteId, Bought = Lang.Character.Suits.Contains(suit.suiteId) });
                    }
                }
            }
            LoadBackups();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool readyToLoad = false;
            Lang = MainData.Load();
            if (string.IsNullOrWhiteSpace(Lang.options.Language) || string.IsNullOrWhiteSpace(Lang.options.EftServerPath)
                || !Directory.Exists(Lang.options.EftServerPath) || !ExtMethods.PathIsEftServerBase(Lang.options.EftServerPath)
                || string.IsNullOrWhiteSpace(Lang.options.DefaultProfile) || !File.Exists(Path.Combine(Lang.options.EftServerPath, "user\\profiles", Lang.options.DefaultProfile + ".json")))
                SettingsBorder.Visibility = Visibility.Visible;
            else
                readyToLoad = true;
            langSelectBox.ItemsSource = Langs;
            langSelectBox.SelectedItem = new KeyValuePair<string, string>(Lang.options.Language, Langs[Lang.options.Language]);
            if (!string.IsNullOrEmpty(Lang.options.ColorScheme))
                ThemeManager.Current.ChangeTheme(this, Lang.options.ColorScheme);
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
            progressDialog = await this.ShowProgressAsync(Lang.locale["progressdialog_title"], Lang.locale["progressdialog_caption"]);
            progressDialog.SetIndeterminate();
            LoadDataWorker.RunWorkerAsync();
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
            CheckPockets();
        }

        private void CheckPockets()
        {
            if (Lang.Character == null || Lang.Character.Inventory == null) return;
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
                if (ExtMethods.PathIsEftServerBase(folderBrowserDialogSPT.SelectedPath)) pathOK = true;
            } while (
                !pathOK &&
                (await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["invalid_server_location_text"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateHide = true, AnimateShow = true }) == MessageDialogResult.Affirmative)
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
            if (profileSelectBox.SelectedValue != null) Lang.options.DefaultProfile = profileSelectBox.SelectedValue.ToString();
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
            if (!string.IsNullOrEmpty(textBox.Text)) Lang.Character.Info.Level = Convert.ToInt32(textBox.Text);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            var comboBox = sender as System.Windows.Controls.ComboBox;
            Lang.Character.Quests.Where(x => x.Qid == ((Quest)comboBox.DataContext).qid).FirstOrDefault().Status = comboBox.SelectedItem.ToString();
        }

        private void merchantLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            var comboBox = sender as System.Windows.Controls.ComboBox;
            LoyaltyLevel level = (LoyaltyLevel)comboBox.SelectedItem;
            Lang.Character.TraderStandings[((TraderInfo)comboBox.DataContext).id].CurrentLevel = level.level;
            if (Lang.Character.TraderStandings[((TraderInfo)comboBox.DataContext).id].CurrentSalesSum < level.SalesSum) Lang.Character.TraderStandings[((TraderInfo)comboBox.DataContext).id].CurrentSalesSum = level.SalesSum;
            if (Lang.Character.TraderStandings[((TraderInfo)comboBox.DataContext).id].CurrentStanding < level.Standing) Lang.Character.TraderStandings[((TraderInfo)comboBox.DataContext).id].CurrentStanding = level.Standing;
        }

        private void hideoutarea_Level_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
                return;
            var slider = sender as Slider;
            Lang.Character.Hideout.Areas.Where(x => x.Type == ((CharacterHideoutArea)slider.DataContext).type).FirstOrDefault().Level = (int)slider.Value;
        }

        private void commonskill_exp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
                return;
            var slider = sender as Slider;
            var skill = (SkillInfo)slider.DataContext;
            if (Math.Abs(skill.progress - Lang.Character.Skills.Common.Where(x => x.Id == skill.id).FirstOrDefault().Progress) > 1)
                Lang.Character.Skills.Common.Where(x => x.Id == skill.id).FirstOrDefault().Progress = (float)slider.Value;
        }

        private void masteringskill_exp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
                return;
            var slider = sender as Slider;
            var skill = (SkillInfo)slider.DataContext;
            if (Math.Abs(skill.progress - Lang.Character.Skills.Mastering.Where(x => x.Id == skill.id).FirstOrDefault().Progress) > 1)
                Lang.Character.Skills.Mastering.Where(x => x.Id == skill.id).FirstOrDefault().Progress = (int)slider.Value <= skill.Max ? (int)slider.Value : skill.Max;
        }

        private void SuitBought_Checked(object sender, RoutedEventArgs e) => ProcessSuit(sender);

        private void SuitBought_Unchecked(object sender, RoutedEventArgs e) => ProcessSuit(sender);

        private void SuitsAcquireAll_Click(object sender, RoutedEventArgs e)
        {
            if (Lang.Character.Suits == null || Suits == null) return;
            foreach (var suit in Suits)
            {
                suit.Bought = true;
                if (!Lang.Character.Suits.Contains(suit.ID)) Lang.Character.Suits.Add(suit.ID);
            }
            suitsGrid.ItemsSource = null;
            suitsGrid.ItemsSource = Suits;
        }

        private void ProcessSuit(object sender)
        {
            if (!IsLoaded)
                return;
            var checkBox = sender as System.Windows.Controls.CheckBox;
            var suit = (SuitInfo)checkBox.DataContext;
            suit.Bought = checkBox.IsChecked == true ? true : false;
            switch (suit.Bought)
            {
                case true:
                    if (!Lang.Character.Suits.Contains(suit.ID)) Lang.Character.Suits.Add(suit.ID);
                    break;
                case false:
                    if (Lang.Character.Suits.Contains(suit.ID)) Lang.Character.Suits.Remove(suit.ID);
                    break;
            }
        }

        private void BigPocketsSwitcher_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            if (Lang.Character.Inventory.Items == null) return;
            if (Lang.Character.Inventory.Items.Where(x => x.Tpl == "557ffd194bdc2d28148b457f" || x.Tpl == "5af99e9186f7747c447120b8").Count() > 0)
                Lang.Character.Inventory.Items.Where(x => x.Tpl == "557ffd194bdc2d28148b457f" || x.Tpl == "5af99e9186f7747c447120b8").FirstOrDefault().Tpl = BigPocketsSwitcher.IsOn ? "5af99e9186f7747c447120b8" : "557ffd194bdc2d28148b457f";
        }

        private void QuestsStatusesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Lang.Character.Quests == null) return;
            foreach (var q in Lang.Character.Quests)
                q.Status = QuestsStatusesBox.SelectedItem.ToString();
            LoadData();
        }

        private async void ResetProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["reloadprofiledialog_title"], Lang.locale["reloadprofiledialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateHide = true, AnimateShow = true }) == MessageDialogResult.Affirmative)
            {
                SaveAndReload();
                LoadData();
            }
        }

        private void HideoutMaximumButton_Click(object sender, RoutedEventArgs e)
        {
            if (HideoutAreas == null) return;
            foreach (var a in Lang.Character.Hideout.Areas)
                a.Level = HideoutAreas.Where(x => x.type == a.Type).FirstOrDefault().MaxLevel;
            LoadData();
        }

        private void ExamineAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (examinedItems == null) return;
            foreach (var item in itemsDB.Where(x => x.Value.parent != null && x.Value.type == "Item" && !x.Value.props.ExaminedByDefault && examinedItems.Where(c => c.id == x.Key).Count() < 1))
                if (globalLang.Templates.ContainsKey(item.Key))
                    Lang.Character.Encyclopedia.Add(item.Key, true);
            LoadData();
        }

        private void SkillsExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (commonSkills == null) return;
            foreach (var exp in Lang.Character.Skills.Common.Where(x => globalLang.Interface.ContainsKey(x.Id)))
                exp.Progress = (float)allskill_exp.Value;
            LoadData();
        }

        private void MasteringsExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (masteringSkills == null) return;
            foreach (var ms in Lang.Character.Skills.Mastering)
                ms.Progress = (int)allmastering_exp.Value <= masteringSkills.Where(x => x.id == ms.Id).FirstOrDefault().Max ? (int)allmastering_exp.Value : masteringSkills.Where(x => x.id == ms.Id).FirstOrDefault().Max;
            LoadData();
        }

        private void MerchantsMaximumButton_Click(object sender, RoutedEventArgs e)
        {
            if (traderInfos == null) return;
            foreach (var tr in Lang.Character.TraderStandings)
            {
                var max = tr.Value.LoyaltyLevels.Last();
                tr.Value.CurrentLevel = Int32.Parse(max.Key) + 1;
                if (tr.Value.CurrentSalesSum < max.Value.MinSalesSum + 1000) tr.Value.CurrentSalesSum = max.Value.MinSalesSum + 1000;
                if (tr.Value.CurrentStanding < max.Value.MinStanding + 0.01f) tr.Value.CurrentStanding = max.Value.MinStanding + 0.01f;
            }
            LoadData();
        }

        private async void SaveProfileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await progressDialog.CloseAsync();
            if (e.Error != null)
            {
                await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], e.Error.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
                ExtMethods.Log($"SaveProfileButton_Click | {e.Error.Message}");
            }
            else
                await this.ShowMessageAsync(Lang.locale["saveprofiledialog_title"], Lang.locale["saveprofiledialog_caption"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateHide = true, AnimateShow = true });
            SaveAndReload();
            LoadData();
        }

        private void SaveProfileWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            JsonSerializerSettings seriSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string profilepath = Path.Combine(Lang.options.EftServerPath, "user", "profiles", Lang.options.DefaultProfile + ".json");
            JObject jobject = JObject.Parse(File.ReadAllText(profilepath));
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Nickname"] = Lang.Character.Info.Nickname;
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["LowerNickname"] = Lang.Character.Info.Nickname.ToLower();
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Side"] = Lang.Character.Info.Side;
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Voice"] = Lang.Character.Info.Voice;
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Level"] = Lang.Character.Info.Level;
            jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Experience"] = Lang.Character.Info.Experience;
            var Stash = jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items").ToObject<Character.Character_Inventory.Character_Inventory_Item[]>();
            var iDs = Lang.Character.Inventory.Items.Select(b => b.Id).ToList();
            List<JToken> ForRemove = new List<JToken>();
            for (int index = 0; index < Stash.Count(); index++)
            {
                var probe = Stash[index];
                if (probe.Tpl == "557ffd194bdc2d28148b457f" || probe.Tpl == "5af99e9186f7747c447120b8")
                {
                    Dispatcher.Invoke(() => jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items")[index]["_tpl"] = BigPocketsSwitcher.IsOn ? "5af99e9186f7747c447120b8" : "557ffd194bdc2d28148b457f");
                    continue;
                }
                if (!iDs.Contains(probe.Id))
                {
                    foreach (var obj in jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items").ToList())
                    {
                        if (obj.ToObject<Character.Character_Inventory.Character_Inventory_Item>().Id == probe.Id)
                            ForRemove.Add(obj);
                    }
                }
            }
            foreach (var j in ForRemove)
                j.Remove();
            foreach (var item in Lang.Character.Inventory.Items.Where(x => !Stash.Select(b => b.Id).ToList().Contains(x.Id)))
                jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items").LastOrDefault().AddAfterSelf(ExtMethods.RemoveNullAndEmptyProperties(JObject.FromObject(item)));
            for (int index = 0; index < Lang.Character.Skills.Common.Count(); ++index)
            {
                var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Common")[index].ToObject<Character.Character_Skills.Character_Skill>();
                jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Common")[index]["Progress"] = Lang.Character.Skills.Common.Where(x => x.Id == probe.Id).FirstOrDefault().Progress;
            }
            for (int index = 0; index < Lang.Character.Hideout.Areas.Count(); ++index)
            {
                var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Hideout").SelectToken("Areas")[index].ToObject<Character.Character_Hideout_Areas>();
                int probeLevel = Lang.Character.Hideout.Areas.Where(x => x.Type == probe.Type).FirstOrDefault().Level;
                var startLevel = Int32.Parse(jobject.SelectToken("characters")["pmc"].SelectToken("Hideout").SelectToken("Areas")[index]["level"].ToString());
                jobject.SelectToken("characters")["pmc"].SelectToken("Hideout").SelectToken("Areas")[index]["level"] = probeLevel;
                if (probeLevel > 0 && probeLevel > startLevel)
                {
                    for (int i = startLevel; i <= probeLevel; i++)
                    {
                        var areaBonuses = HideoutAreas.Where(x => x.type == probe.Type).FirstOrDefault().stages[i.ToString()];
                        if (areaBonuses != null)
                        {
                            var BonusesList = areaBonuses.SelectToken("bonuses").ToObject<List<JToken>>();
                            if (BonusesList != null && BonusesList.Count() > 0)
                            {
                                foreach (var t in BonusesList)
                                {
                                    var bonus = t.ToObject<Character.Character_Bonuses>();
                                    if (bonus != null)
                                    {
                                        switch (bonus.Type)
                                        {
                                            case "StashSize":
                                                ChangeStash(bonus.TemplateId);
                                                break;
                                            case "MaximumEnergyReserve":
                                                jobject.SelectToken("characters")["pmc"].SelectToken("Health").SelectToken("Energy")["Maximum"] = 110;
                                                break;
                                            default:
                                                break;
                                        }
                                        jobject.SelectToken("characters")["pmc"].SelectToken("Bonuses").LastOrDefault().AddAfterSelf(JObject.FromObject(t));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var tr in Lang.Character.TraderStandings)
            {
                jobject.SelectToken("characters")["pmc"].SelectToken("TraderStandings").SelectToken(tr.Key)["currentLevel"] = Lang.Character.TraderStandings[tr.Key].CurrentLevel;
                jobject.SelectToken("characters")["pmc"].SelectToken("TraderStandings").SelectToken(tr.Key)["currentSalesSum"] = Lang.Character.TraderStandings[tr.Key].CurrentSalesSum;
                jobject.SelectToken("characters")["pmc"].SelectToken("TraderStandings").SelectToken(tr.Key)["currentStanding"] = Lang.Character.TraderStandings[tr.Key].CurrentStanding;
                if (Lang.Character.TraderStandings[tr.Key].CurrentLevel > 1)
                    jobject.SelectToken("characters")["pmc"].SelectToken("TraderStandings").SelectToken(tr.Key)["display"] = true;
            }
            if (Lang.Character.Quests.Count() > 0)
            {
                var questsObject = jobject.SelectToken("characters")["pmc"].SelectToken("Quests").ToObject<Character.Character_Quests[]>();
                for (int i = 0; i < questsObject.Count(); i++)
                {
                    var quest = jobject.SelectToken("characters")["pmc"].SelectToken("Quests")[i].ToObject<Character.Character_Quests>();
                    if (quest != null)
                        jobject.SelectToken("characters")["pmc"].SelectToken("Quests")[i]["status"] = Lang.Character.Quests.Where(x => x.Qid == quest.Qid).FirstOrDefault().Status;
                }
                if (questsObject.Count() > 0)
                {
                    foreach (var quest in Lang.Character.Quests)
                    {
                        if (questsObject.Where(x => x.Qid == quest.Qid).Count() < 1)
                            jobject.SelectToken("characters")["pmc"].SelectToken("Quests").LastOrDefault().AddAfterSelf(JObject.FromObject(quest));
                    }
                }
                else
                    jobject.SelectToken("characters")["pmc"].SelectToken("Quests").Replace(JToken.FromObject(Lang.Character.Quests));
            }
            if (Lang.Character.Skills.Mastering.Count() > 0)
            {
                var masteringObject = jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering").ToObject<Character.Character_Skills.Character_Skill[]>();
                for (int i = 0; i < masteringObject.Count(); i++)
                {
                    var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering")[i].ToObject<Character.Character_Skills.Character_Skill>();
                    if (probe != null)
                        jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering")[i]["Progress"] = Lang.Character.Skills.Mastering.Where(x => x.Id == probe.Id).FirstOrDefault().Progress;
                }
                if (masteringObject.Count() > 0)
                {
                    foreach (var master in Lang.Character.Skills.Mastering)
                    {
                        if (masteringObject.Where(x => x.Id == master.Id).Count() < 1)
                            jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering").LastOrDefault().AddAfterSelf(JObject.FromObject(master));
                    }
                }
                else
                    jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering").Replace(JToken.FromObject(Lang.Character.Skills.Mastering));
            }
            jobject.SelectToken("characters")["pmc"].SelectToken("Encyclopedia").Replace(JToken.FromObject(Lang.Character.Encyclopedia));
            jobject.SelectToken("suits").Replace(JToken.FromObject(Lang.Character.Suits.ToArray()));
            DateTime now = DateTime.Now;
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups")))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups"));
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile)))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile));
            string backupfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile, $"{Lang.options.DefaultProfile}-backup-{now:dd-MM-yyyy-HH-mm-ss}.json");
            File.Copy(profilepath, backupfile, true);
            string json = JsonConvert.SerializeObject(jobject, seriSettings);
            File.WriteAllText(profilepath, json);

            void ChangeStash(string tpl)
            {
                if (!string.IsNullOrEmpty(tpl))
                {
                    for (int index = 0; index < Stash.Count(); index++)
                    {
                        var probe = Stash[index];
                        if (probe.Id == Lang.Character.Inventory.Stash)
                        {
                            jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items")[index]["_tpl"] = tpl;
                        }
                    }
                }
            }
        }

        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            progressDialog = await this.ShowProgressAsync(Lang.locale["progressdialog_title"], Lang.locale["saveprofiledialog_title"]);
            progressDialog.SetIndeterminate();
            SaveProfileWorker.RunWorkerAsync();
        }

        private void LoadBackups()
        {
            backups = new List<BackupFile>();
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups")) && Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile)))
            {
                foreach (var bk in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile)).Where(x => x.Contains("backup")).OrderByDescending(i => i))
                {
                    try { backups.Add(new BackupFile { Path = bk, date = DateTime.ParseExact(Path.GetFileNameWithoutExtension(bk).Remove(0, Lang.options.DefaultProfile.Count() + 8), "dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd.MM.yyyy HH:mm:ss") }); }
                    catch (Exception ex)
                    { ExtMethods.Log($"LoadBackups | {ex.GetType().Name}: {ex.Message}"); }
                }
            }
            if (!LoadDataWorker.IsBusy)
            {
                backupsGrid.ItemsSource = null;
                backupsGrid.ItemsSource = backups;
            }
        }

        private async void backupRemove_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["removebackupdialog_title"], Lang.locale["removebackupdialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateShow = true, AnimateHide = true }) == MessageDialogResult.Affirmative)
            {
                try
                {
                    var button = sender as System.Windows.Controls.Button;
                    File.Delete(((BackupFile)button.DataContext).Path);
                }
                catch (Exception ex)
                {
                    ExtMethods.Log($"backupRemove_Click | {ex.GetType().Name}: {ex.Message}");
                    await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
                }                
                LoadBackups();
            }
        }

        private async void backupRestore_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["restorebackupdialog_title"], Lang.locale["restorebackupdialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateShow = true, AnimateHide = true }) == MessageDialogResult.Affirmative)
            {
                try
                {
                    var button = sender as System.Windows.Controls.Button;
                    File.Copy(((BackupFile)button.DataContext).Path, Path.Combine(Lang.options.EftServerPath, "user\\profiles", Lang.options.DefaultProfile + ".json"), true);
                    File.Delete(((BackupFile)button.DataContext).Path);
                }
                catch (Exception ex)
                {
                    ExtMethods.Log($"backupRestore_Click | {ex.GetType().Name}: {ex.Message}");
                    await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
                } 
                SaveAndReload();
                LoadData();
            }
        }

        private async void StashItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["removestashitem_title"], Lang.locale["removestashitem_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateShow = true, AnimateHide = true }) == MessageDialogResult.Affirmative)
            {
                var button = sender as System.Windows.Controls.Button;
                var TargetItem = Lang.Character.Inventory.Items.Where(x => x.Id == ((InventoryItem)button.DataContext).id).FirstOrDefault();
                if (TargetItem != null)
                {
                    var items = Lang.Character.Inventory.Items.ToList();
                    var toDo = new List<Character.Character_Inventory.Character_Inventory_Item>();
                    toDo.Add(TargetItem);
                    while (toDo.Count() > 0)
                    {
                        if (toDo.ElementAt(0) != null)
                        {
                            foreach (var item in Lang.Character.Inventory.Items.Where(x => x.ParentId == toDo.ElementAt(0).Id))
                            {
                                toDo.Add(item);
                            }
                        }
                        items.Remove(toDo.ElementAt(0));
                        toDo.Remove(toDo.ElementAt(0));
                    }
                    Lang.Character.Inventory.Items = items.ToArray();
                    LoadData();
                }                
            }
        }

        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if ( await this.ShowMessageAsync(Lang.locale["removestashitem_title"], Lang.locale["removestashitems_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"], AnimateShow = true, AnimateHide = true }) == MessageDialogResult.Affirmative)
            {
                var items = Lang.Character.Inventory.Items.ToList();
                foreach (var Titem in Lang.characterInventory.InventoryItems.ToArray())
                {
                    var TargetItem = Lang.Character.Inventory.Items.Where(x => x.Id == (Titem.id)).FirstOrDefault();
                    if (TargetItem != null)
                    {
                        var toDo = new List<Character.Character_Inventory.Character_Inventory_Item>();
                        toDo.Add(TargetItem);
                        while (toDo.Count() > 0)
                        {
                            if (toDo.ElementAt(0) != null)
                            {
                                foreach (var item in Lang.Character.Inventory.Items.Where(x => x.ParentId == toDo.ElementAt(0).Id))
                                {
                                    toDo.Add(item);
                                }
                            }
                            items.Remove(toDo.ElementAt(0));
                            toDo.Remove(toDo.ElementAt(0));
                        }
                    }
                }
                Lang.Character.Inventory.Items = items.ToArray();
                LoadData();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void AddMoneyDialog(string moneytpl)
        {
            AddMoneyDialog AddMoneys = new AddMoneyDialog
            {
                OkButtonText = Lang.locale["saveprofiledialog_ok"],
                CancelButtonText = Lang.locale["button_close"],
                Owner = this,
                ColorScheme = ThemeManager.Current.DetectTheme(this).Name,
                MoneyTitle = Lang.locale["tab_stash_dialogmoney"]
            };
            if (AddMoneys.ShowDialog() == true)
            {
                AddMoney(moneytpl, AddMoneys.MoneyCount);
            }
        }

        private void ShowRublesAddDialog(object sender, RoutedEventArgs e) => AddMoneyDialog(moneyRub);

        private void ShowEurosAddDialog(object sender, RoutedEventArgs e) => AddMoneyDialog(moneyEur);

        private void ShowDollarsAddDialog(object sender, RoutedEventArgs e) => AddMoneyDialog(moneyDol);

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            //может быть доделаю позже. выдача предметов со стаками, выдача аптечек
            if (ItemIdSelector.SelectedValue == null) return;
            var item = itemsDB[ItemIdSelector.SelectedValue.ToString()];
            var Stash = getPlayerStashSlotMap();
            List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location> locations = new List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location>();
            int FreeSlots = 0;
            int Amount = Convert.ToInt32(ItemAddAmount.Text);
            for (int y = 0; y < Stash.GetLength(0); y++)
                for (int x = 0; x < Stash.GetLength(1); x++)
                    if (Stash[y, x] == 0)
                    {
                        FreeSlots++;
                        locations.Add(new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location { X = x, Y = y, R = "Horizontal" });
                    }
            int tempslots = item.props.Width * item.props.Height * Amount;
            if (FreeSlots < tempslots)
                await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["tab_stash_noslots"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
            else
            {
                List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location> NewItemsLocations = new List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location>();
                foreach (var slot in locations)
                {
                    if (item.props.Width == 1 && item.props.Height == 1)
                        NewItemsLocations.Add(slot);
                    else
                    {
                        int size = 0;
                        for (int y = 0; y < item.props.Height; y++)
                        {
                            if (slot.X + item.props.Width < Stash.GetLength(1) && slot.Y + y < Stash.GetLength(0))
                                for (int z = slot.X; z < slot.X + item.props.Width; z++)
                                    if (Stash[slot.Y + y, z] == 0) size++;
                        }
                        if (size == item.props.Width * item.props.Height)
                        {
                            for (int y = 0; y < item.props.Height; y++)
                            {
                                for (int z = slot.X; z < slot.X + item.props.Width; z++)
                                    Stash[slot.Y + y, z] = 1;
                            }
                            NewItemsLocations.Add(slot);
                        }
                        if (NewItemsLocations.Count == Amount) break;
                        size = 0;
                        for (int y = 0; y < item.props.Width; y++)
                        {
                            if (slot.X + item.props.Height < Stash.GetLength(1) && slot.Y + y < Stash.GetLength(0))
                                for (int z = slot.X; z < slot.X + item.props.Height; z++)
                                    if (Stash[slot.Y + y, z] == 0) size++;
                        }
                        if (size == item.props.Width * item.props.Height)
                        {
                            for (int y = 0; y < item.props.Width; y++)
                            {
                                for (int z = slot.X; z < slot.X + item.props.Height; z++)
                                    Stash[slot.Y + y, z] = 1;
                            }
                            NewItemsLocations.Add(new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location { X = slot.X, Y = slot.Y, R = "Vertical" });
                        }
                    }
                    if (NewItemsLocations.Count == Amount) break;
                }
                if (NewItemsLocations.Count == Amount)
                {
                    List<string> iDs = Lang.Character.Inventory.Items.Select(x => x.Id).ToList();
                    string id = iDs.Last();
                    var items = Lang.Character.Inventory.Items.ToList();
                    for (int i = 0; i < NewItemsLocations.Count; i++)
                    {
                        while (iDs.Contains(id))
                            id = ExtMethods.generateNewId();
                        iDs.Add(id);
                        items.Add(new Character.Character_Inventory.Character_Inventory_Item { 
                            Id = id, ParentId = Lang.Character.Inventory.Stash, 
                            SlotId = "hideout", Tpl = item.id, 
                            Location = new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location { R = NewItemsLocations[i].R, X = NewItemsLocations[i].X, Y = NewItemsLocations[i].Y, IsSearched = true }, 
                            Upd = new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Upd { StackObjectsCount = item.props.StackMaxSize } });
                    }
                    Lang.Character.Inventory.Items = items.ToArray();
                    LoadData();
                }
                else
                {
                    await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["tab_stash_noslots"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
                }
            }
        }

        private async void AddMoney(string tpl, int count)
        {
            var mItem = itemsDB[tpl];
            var Stash = getPlayerStashSlotMap();
            List<string> iDs = Lang.Character.Inventory.Items.Select(x => x.Id).ToList();
            List<Character.Character_Inventory.Character_Inventory_Item> items = Lang.Character.Inventory.Items.ToList();
            List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location> locations = new List<Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location>();
            int FreeSlots = 0;
            for (int y = 0; y < Stash.GetLength(0); y++)
                for (int x = 0; x < Stash.GetLength(1); x++)
                    if (Stash[y,x] == 0)
                    {
                        FreeSlots++;
                        locations.Add(new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Location { X = x, Y = y, R = "Horizontal" });
                    }
            int stacksForGive = 1;
            if (count > mItem.props.StackMaxSize)
            {
                stacksForGive = count / mItem.props.StackMaxSize;
                if (stacksForGive * mItem.props.StackMaxSize < count) stacksForGive++;
            }
            if (FreeSlots >= stacksForGive)
            {
                string id = iDs.Last();
                for (int c = 0; c < stacksForGive; c++)
                {
                    if (count <= 0) break;
                    while (iDs.Contains(id))
                        id = ExtMethods.generateNewId();
                    iDs.Add(id);
                    items.Add(new Character.Character_Inventory.Character_Inventory_Item { Id = id, Location = locations.First(), ParentId = Lang.Character.Inventory.Stash, Tpl = tpl, Upd = new Character.Character_Inventory.Character_Inventory_Item.Character_Inventory_Item_Upd { StackObjectsCount = count > mItem.props.StackMaxSize ? mItem.props.StackMaxSize : count }, SlotId = "hideout" });
                    locations.Remove(locations.First());
                    count -= mItem.props.StackMaxSize;
                }
                Lang.Character.Inventory.Items = items.ToArray();
                LoadData();
            }
            else
            {
                await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["tab_stash_noslots"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
            }
        }        

        private int[,] getPlayerStashSlotMap()
        {
            var ProfileStash = Lang.Character.Inventory.Items.Where(x => x.Id == Lang.Character.Inventory.Stash).FirstOrDefault();
            var stashTPL = itemsDB[ProfileStash.Tpl].props.Grids.First();
            var stashX = (stashTPL.gridProps.cellsH != 0) ? stashTPL.gridProps.cellsH : 10;
            var stashY = (stashTPL.gridProps.cellsV != 0) ? stashTPL.gridProps.cellsV : 66;
            var Stash2D = new int[stashY, stashX];

            foreach (var item in Lang.Character.Inventory.Items.Where(x => x.ParentId == Lang.Character.Inventory.Stash))
            {
                if (item.Location == null)
                    continue;
                var tmpSize = getSizeByInventoryItemHash(item);
                int iW = tmpSize.Key;
                int iH = tmpSize.Value;
                int fH = item.Location.R == "Vertical" ? iW : iH;
                int fW = item.Location.R == "Vertical" ? iH : iW;
                for (int y = 0; y < fH; y++)
                {
                    try
                    {
                        for (int z = item.Location.X; z < item.Location.X + fW; z++)
                            Stash2D[item.Location.Y + y, z] = 1;
                    }
                    catch (Exception ex)
                    {
                        ExtMethods.Log($"[OOB] for item with id { item.Id}; Error message: {ex.Message}");
                    }
                }
            }

            return Stash2D;
        }
        private KeyValuePair<int, int> getSizeByInventoryItemHash(Character.Character_Inventory.Character_Inventory_Item itemtpl)
        {
            List<Character.Character_Inventory.Character_Inventory_Item> toDo = new List<Character.Character_Inventory.Character_Inventory_Item>();
            toDo.Add(itemtpl);
            var tmpItem = itemsDB[itemtpl.Tpl];
            var rootItem = Lang.Character.Inventory.Items.Where(x => x.ParentId == itemtpl.Id).FirstOrDefault();
            var FoldableWeapon = tmpItem.props.Foldable;
            var FoldedSlot = tmpItem.props.FoldedSlot;

            var SizeUp = 0;
            var SizeDown = 0;
            var SizeLeft = 0;
            var SizeRight = 0;

            var ForcedUp = 0;
            var ForcedDown = 0;
            var ForcedLeft = 0;
            var ForcedRight = 0;
            var outX = tmpItem.props.Width;
            var outY = tmpItem.props.Height;
            if (rootItem != null)
            {
                var skipThisItems = new List<string> { "5448e53e4bdc2d60728b4567", "566168634bdc2d144c8b456c", "5795f317245977243854e041" };
                var rootFolded = rootItem.Upd != null && rootItem.Upd.Foldable != null && rootItem.Upd.Foldable.Folded;

                if (FoldableWeapon && string.IsNullOrEmpty(FoldedSlot) && rootFolded)
                    outX -= tmpItem.props.SizeReduceRight;

                if (!skipThisItems.Contains(tmpItem.parent))
                {
                    while (toDo.Count() > 0)
                    {
                        if (toDo.ElementAt(0) != null)
                        {
                            foreach (var item in Lang.Character.Inventory.Items.Where(x => x.ParentId == toDo.ElementAt(0).Id))
                            {
                                if (!item.SlotId.Contains("mod_"))
                                    continue;
                                toDo.Add(item);
                                var itm = itemsDB[item.Tpl];
                                var childFoldable = itm.props.Foldable;
                                var childFolded = item.Upd != null && item.Upd.Foldable != null && item.Upd.Foldable.Folded;
                                if (FoldableWeapon && FoldedSlot == item.SlotId && (rootFolded || childFolded))
                                    continue;
                                else if (childFoldable && rootFolded && childFolded)
                                    continue;
                                if (itm.props.ExtraSizeForceAdd)
                                {
                                    ForcedUp += itm.props.ExtraSizeUp;
                                    ForcedDown += itm.props.ExtraSizeDown;
                                    ForcedLeft += itm.props.ExtraSizeLeft;
                                    ForcedRight += itm.props.ExtraSizeRight;
                                }
                                else
                                {
                                    SizeUp = (SizeUp < itm.props.ExtraSizeUp) ? itm.props.ExtraSizeUp : SizeUp;
                                    SizeDown = (SizeDown < itm.props.ExtraSizeDown) ? itm.props.ExtraSizeDown : SizeDown;
                                    SizeLeft = (SizeLeft < itm.props.ExtraSizeLeft) ? itm.props.ExtraSizeLeft : SizeLeft;
                                    SizeRight = (SizeRight < itm.props.ExtraSizeRight) ? itm.props.ExtraSizeRight : SizeRight;
                                }
                            }
                        }
                        toDo.Remove(toDo.ElementAt(0));
                    }                    
                }
            }

            return new KeyValuePair<int, int>(outX + SizeLeft + SizeRight + ForcedLeft + ForcedRight, outY + SizeUp + SizeDown + ForcedUp + ForcedDown);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (Int32.TryParse(textBox.Text, out int money))
            {
                if (money < 1) textBox.Text = "1";
            }
            else
            {
                textBox.Text = Int32.MaxValue.ToString();
            }
        }

        private void HideWarningButton_Click(object sender, RoutedEventArgs e) => ItemsAddWarning.Visibility = ItemsAddWarning.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (e.Cancel)
                return;
            
            if (ExtMethods.ProfileChanged(Lang)  && _shutdown == false)
            {
                e.Cancel = true;
                Dispatcher.BeginInvoke(new Action(async () => await ConfirmShutdown()));
            }
        }

        private async Task ConfirmShutdown()
        {
            var mySettings = new MetroDialogSettings { AffirmativeButtonText = Lang.locale["button_quit"], NegativeButtonText = Lang.locale["button_cancel"], AnimateShow = true, AnimateHide = true };
            var result = await this.ShowMessageAsync(Lang.locale["app_quit"], Lang.locale["reloadprofiledialog_caption"], MessageDialogStyle.AffirmativeAndNegative, mySettings);
            _shutdown = result == MessageDialogResult.Affirmative;
            if (_shutdown)
                System.Windows.Application.Current.Shutdown();
        }
    }
}