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

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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
        private GlobalLang globalLang;
        private Dictionary<string, Item> itemsDB;
        private List<ExaminedItem> examinedItems;

        private static string moneyRub = "5449016a4bdc2d6f028b456f";
        private static string moneyDol = "5696686a4bdc2da3298b456a";
        private static string moneyEur = "569668774bdc2da2298b4568";

        private Dictionary<string, string> Langs = new Dictionary<string, string>
        {
            ["en"] = "English",
            ["ru"] = "Русский",
            ["fr"] = "Français",
            ["ge"] = "Deutsch "
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
            progressDialog.CloseAsync();
        }

        private void LoadDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            globalLang = JsonConvert.DeserializeObject<GlobalLang>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "db", "locales", "global_" + Lang.options.Language + ".json")));
            itemsDB = new Dictionary<string, Item>();
            itemsDB = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "db", "templates", "items.json")));
            if (Lang.Character.Quests != null)
            {
                Quests = new List<Quest>();
                List<QuestData> qData = JsonConvert.DeserializeObject<List<QuestData>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "db", "templates", "quests.json")));
                foreach (var qd in qData)
                {
                    if (Lang.Character.Quests.Where(x => x.Qid == qd._id).Count() > 0)
                    {
                        var quest = Lang.Character.Quests.Where(x => x.Qid == qd._id).FirstOrDefault();
                        Quests.Add(new Quest { qid = quest.Qid, name = globalLang.Quests[quest.Qid].name, status = quest.Status, trader = globalLang.Traders[qd.traderId].Nickname });
                    }
                }
            }
            traderInfos = new List<TraderInfo>();
            foreach (var mer in Lang.Character.TraderStandings)
            {
                if (mer.Key == "ragfair") continue;
                List<LoyaltyLevel> loyalties = new List<LoyaltyLevel>();
                foreach (var lv in mer.Value.LoyaltyLevels)
                    loyalties.Add(new LoyaltyLevel { level = Int32.Parse(lv.Key) + 1, SalesSum = lv.Value.MinSalesSum + 1000, Standing = lv.Value.MinStanding + 0.01f });
                traderInfos.Add(new TraderInfo { id = mer.Key, name = globalLang.Traders[mer.Key].Nickname, CurrentLevel = mer.Value.CurrentLevel, Levels = loyalties });
            }
            HideoutAreas = new List<CharacterHideoutArea>();
            var areas = JsonConvert.DeserializeObject<List<AreaInfo>>(File.ReadAllText(Path.Combine(Lang.options.EftServerPath, "db", "hideout", "areas.json")));
            foreach (var area in areas)
                HideoutAreas.Add(new CharacterHideoutArea { type = area.type, name = globalLang.Interface[$"hideout_area_{area.type}_name"], MaxLevel = area.stages.Count - 1, CurrentLevel = Lang.Character.Hideout.Areas.Where(x => x.Type == area.type).FirstOrDefault().Level });
            commonSkills = new List<SkillInfo>();
            foreach (var skill in Lang.Character.Skills.Common)
            {
                if (globalLang.Interface.ContainsKey(skill.Id))
                    commonSkills.Add(new SkillInfo { progress = (int)skill.Progress, name = globalLang.Interface[skill.Id], id = skill.Id });
            }
            masteringSkills = new List<SkillInfo>();
            foreach (var skill in Lang.Character.Skills.Mastering)
                masteringSkills.Add(new SkillInfo { progress = (int)skill.Progress, name = skill.Id, id = skill.Id });
            //need load mastering from "C:\EFTSingle-Aki\Server\db\others\globals.json" Mastering
            examinedItems = new List<ExaminedItem>();
            foreach (var eItem in Lang.Character.Encyclopedia)
                examinedItems.Add(new ExaminedItem { id = eItem.Key, name = globalLang.Templates[eItem.Key].Name });
            Lang.characterInventory.Dollars = 0;
            Lang.characterInventory.Euros = 0;
            Lang.characterInventory.Rubles = 0;
            foreach (var item in Lang.Character.Inventory.Items)
            {
                if (item.Tpl == moneyDol) Lang.characterInventory.Dollars += (int)item.Upd.StackObjectsCount;
                if (item.Tpl == moneyEur) Lang.characterInventory.Euros += (int)item.Upd.StackObjectsCount;
                if (item.Tpl == moneyRub) Lang.characterInventory.Rubles += (int)item.Upd.StackObjectsCount;
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
                if (ExtMethods.PathIsEftServerBase(folderBrowserDialogSPT.SelectedPath)) pathOK = true;
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
            Lang.Character.Skills.Common.Where(x => x.Id == ((SkillInfo)slider.DataContext).id).FirstOrDefault().Progress = (float)slider.Value;
        }

        private void masteringskill_exp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
                return;
            var slider = sender as Slider;
            Lang.Character.Skills.Mastering.Where(x => x.Id == ((SkillInfo)slider.DataContext).id).FirstOrDefault().Progress = (int)slider.Value;
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
            if (await this.ShowMessageAsync(Lang.locale["reloadprofiledialog_title"], Lang.locale["reloadprofiledialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"] }) == MessageDialogResult.Affirmative)
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
            foreach (var item in itemsDB.Where(x => !x.Value.props.ExaminedByDefault && examinedItems.Where(c => c.id == x.Key).Count() < 1))
                if (globalLang.Templates.ContainsKey(item.Key))
                    Lang.Character.Encyclopedia.Add(item.Key, true);
            LoadData();
        }

        private void SkillsExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (commonSkills == null) return;
            foreach (var exp in Lang.Character.Skills.Common)
                exp.Progress = (float)allskill_exp.Value;
            LoadData();
        }

        private void MasteringsExpButton_Click(object sender, RoutedEventArgs e)
        {
            if (masteringSkills == null) return;
            foreach (var ms in Lang.Character.Skills.Mastering)
                ms.Progress = (int)allmastering_exp.Value;
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

        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            JsonSerializerSettings seriSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            };
            try
            {
                string profilepath = Path.Combine(Lang.options.EftServerPath, "user", "profiles", Lang.options.DefaultProfile + ".json");
                JObject jobject = JObject.Parse(File.ReadAllText(profilepath));
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Nickname"] = Lang.Character.Info.Nickname;
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["LowerNickname"] = Lang.Character.Info.Nickname.ToLower();
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Side"] = Lang.Character.Info.Side;
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Voice"] = Lang.Character.Info.Voice;
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Level"] = Lang.Character.Info.Level;
                jobject.SelectToken("characters")["pmc"].SelectToken("Info")["Experience"] = Lang.Character.Info.Experience;
                for (int index = 0; index < Lang.Character.Inventory.Items.Count(); ++index)
                {
                    var count = jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items").ToObject<Character.Character_Inventory.Character_Inventory_Item[]>();
                    if (index < count.Count())
                    {
                        var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items")[index].ToObject<Character.Character_Inventory.Character_Inventory_Item>();
                        if (probe.Tpl == "557ffd194bdc2d28148b457f" || probe.Tpl == "5af99e9186f7747c447120b8")
                        {
                            jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items")[index]["_tpl"] = BigPocketsSwitcher.IsOn ? "5af99e9186f7747c447120b8" : "557ffd194bdc2d28148b457f";
                        }
                    }
                    else
                        jobject.SelectToken("characters")["pmc"].SelectToken("Inventory").SelectToken("items").Last().AddAfterSelf(ExtMethods.RemoveNullAndEmptyProperties(JObject.FromObject(Lang.Character.Inventory.Items[index])));
                }
                for (int index = 0; index < Lang.Character.Skills.Common.Count(); ++index)
                {
                    var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Common")[index].ToObject<Character.Character_Skills.Character_Skill>();
                    jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Common")[index]["Progress"] = Lang.Character.Skills.Common.Where(x => x.Id == probe.Id).FirstOrDefault().Progress;
                }
                for (int index = 0; index < Lang.Character.Hideout.Areas.Count(); ++index)
                {
                    var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Hideout").SelectToken("Areas")[index].ToObject<Character.Character_Hideout_Areas>();
                    jobject.SelectToken("characters")["pmc"].SelectToken("Hideout").SelectToken("Areas")[index]["level"] = Lang.Character.Hideout.Areas.Where(x => x.Type == probe.Type).FirstOrDefault().Level;
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
                    for (int index = 0; index < Lang.Character.Quests.Count(); ++index)
                    {
                        var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Quests")[index].ToObject<Character.Character_Quests>();
                        jobject.SelectToken("characters")["pmc"].SelectToken("Quests")[index]["status"] = Lang.Character.Quests.Where(x => x.Qid == probe.Qid).FirstOrDefault().Status;
                    }
                }
                for (int index = 0; index < Lang.Character.Skills.Mastering.Count(); ++index)
                {
                    var probe = jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering")[index].ToObject<Character.Character_Skills.Character_Skill>();
                    jobject.SelectToken("characters")["pmc"].SelectToken("Skills").SelectToken("Mastering")[index]["Progress"] = Lang.Character.Skills.Mastering.Where(x => x.Id == probe.Id).FirstOrDefault().Progress;
                }
                jobject.SelectToken("characters")["pmc"].SelectToken("Encyclopedia").Replace(JToken.FromObject(Lang.Character.Encyclopedia));
                DateTime now = DateTime.Now;
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups")))
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups"));
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile)))
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile));
                string backupfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", Lang.options.DefaultProfile, $"{Lang.options.DefaultProfile}-backup-{now:dd-MM-yyyy-HH-mm-ss}.json");
                File.Copy(profilepath, backupfile, true);
                string json = JsonConvert.SerializeObject(jobject, seriSettings);
                File.WriteAllText(profilepath, json);
                await this.ShowMessageAsync(Lang.locale["saveprofiledialog_title"], Lang.locale["saveprofiledialog_caption"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"] });
            }
            catch (Exception ex)
            {
                ExtMethods.Log($"SaveProfileButton_Click | {ex.GetType().Name}: {ex.Message}");
                await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"] });
            }
            LoadBackups();
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
            if (await this.ShowMessageAsync(Lang.locale["removebackupdialog_title"], Lang.locale["removebackupdialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"] }) == MessageDialogResult.Affirmative)
            {
                try
                {
                    var button = sender as System.Windows.Controls.Button;
                    File.Delete(((BackupFile)button.DataContext).Path);
                }
                catch (Exception ex)
                {
                    ExtMethods.Log($"backupRemove_Click | {ex.GetType().Name}: {ex.Message}");
                    await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"] });
                }                
                LoadBackups();
            }
        }

        private async void backupRestore_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync(Lang.locale["restorebackupdialog_title"], Lang.locale["restorebackupdialog_caption"], MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative, AffirmativeButtonText = Lang.locale["button_yes"], NegativeButtonText = Lang.locale["button_no"] }) == MessageDialogResult.Affirmative)
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
                    await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"] });
                } 
                SaveAndReload();
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

        private void ShowRublesAddDialog(object sender, RoutedEventArgs e)
        {
            AddMoneyDialog AddRoubles = new AddMoneyDialog
            {
                OkButtonText = Lang.locale["saveprofiledialog_ok"],
                CancelButtonText = Lang.locale["button_close"],
                Owner = this,
                ColorScheme = Lang.options.ColorScheme,
                Title = Lang.locale["tab_stash_dialogmoney"]
            };
            if (AddRoubles.ShowDialog() == true)
            {
                AddMoney("5449016a4bdc2d6f028b456f", AddRoubles.MoneyCount);
            }
        }

        private void ShowEurosAddDialog(object sender, RoutedEventArgs e)
        {
            AddMoneyDialog AddRoubles = new AddMoneyDialog
            {
                OkButtonText = Lang.locale["saveprofiledialog_ok"],
                CancelButtonText = Lang.locale["button_close"],
                Owner = this,
                ColorScheme = Lang.options.ColorScheme,
                Title = Lang.locale["tab_stash_dialogmoney"]
            };
            if (AddRoubles.ShowDialog() == true)
            {
                AddMoney("569668774bdc2da2298b4568", AddRoubles.MoneyCount);
            }
        }

        private void ShowDollarsAddDialog(object sender, RoutedEventArgs e)
        {
            AddMoneyDialog AddRoubles = new AddMoneyDialog
            {
                OkButtonText = Lang.locale["saveprofiledialog_ok"],
                CancelButtonText = Lang.locale["button_close"],
                Owner = this,
                ColorScheme = Lang.options.ColorScheme,
                Title = Lang.locale["tab_stash_dialogmoney"]
            };
            if (AddRoubles.ShowDialog() == true)
            {
                AddMoney("5696686a4bdc2da3298b456a", AddRoubles.MoneyCount);
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
                await this.ShowMessageAsync(Lang.locale["invalid_server_location_caption"], Lang.locale["tab_stash_noslots"], MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = Lang.locale["saveprofiledialog_ok"] });
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
    }
}