using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace SP_EFT_ProfileEditor
{
    public class MainData
    {
        private static string LangPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
        private static string PeoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PEOptions.json");
        public static MainData Load()
        {
            if (!Directory.Exists(LangPath))
                Directory.CreateDirectory(LangPath);
            if (!File.Exists(Path.Combine(LangPath, "en.json")))
                File.WriteAllText(Path.Combine(LangPath, "en.json"), JsonConvert.SerializeObject(EN, Formatting.Indented));
            if (!File.Exists(Path.Combine(LangPath, "ru.json")))
                File.WriteAllText(Path.Combine(LangPath, "ru.json"), JsonConvert.SerializeObject(RU, Formatting.Indented));
            if (!File.Exists(Path.Combine(LangPath, "fr.json")))
                File.WriteAllText(Path.Combine(LangPath, "fr.json"), JsonConvert.SerializeObject(FR, Formatting.Indented));
            if (!File.Exists(Path.Combine(LangPath, "ge.json")))
                File.WriteAllText(Path.Combine(LangPath, "ge.json"), JsonConvert.SerializeObject(GE, Formatting.Indented));
            if (!File.Exists(Path.Combine(LangPath, "ch.json")))
                File.WriteAllText(Path.Combine(LangPath, "ch.json"), JsonConvert.SerializeObject(CH, Formatting.Indented));
            if (!File.Exists(Path.Combine(LangPath, "es.json")))
                File.WriteAllText(Path.Combine(LangPath, "es.json"), JsonConvert.SerializeObject(ES, Formatting.Indented));
            PEOptions eOptions = CreateOptions();
            if (string.IsNullOrEmpty(eOptions.Language))
            {
                var WLang = ExtMethods.GetWindowsCulture();
                if (WLang == "en" || WLang == "ru" || WLang == "fr" || WLang == "ge" || WLang == "ch" || WLang == "es")
                    eOptions.Language = WLang;
                else
                    eOptions.Language = "en";
            }
            Dictionary<string, string> Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(LangPath, eOptions.Language + ".json")));
            Dictionary<string, string> Eth = new Dictionary<string, string>();
            switch (eOptions.Language)
            {
                case "en":
                    Eth = EN;
                    break;
                case "ge":
                    Eth = GE;
                    break;
                case "ru":
                    Eth = RU;
                    break;
                case "fr":
                    Eth = FR;
                    break;
                case "ch":
                    Eth = CH;
                    break;
                case "es":
                    Eth = ES;
                    break;
            }                
            bool needReSave = false;
            foreach (var lc in Eth)
            {
                if (!Locale.ContainsKey(lc.Key))
                {
                    Locale.Add(lc.Key, lc.Value);
                    needReSave = true;
                }
            }
            if (needReSave)
                File.WriteAllText(Path.Combine(LangPath, $"{eOptions.Language}.json"), JsonConvert.SerializeObject(Locale, Formatting.Indented));
            MainData lang = new MainData { locale = Locale, options = eOptions, characterInventory = new CharacterInventory { Rubles = 0, Euros = 0, Dollars = 0 }, gridFilters = new GridFilters() };
            try
            {
                if (!string.IsNullOrEmpty(eOptions.EftServerPath) && !ExtMethods.PathIsEftServerBase(eOptions))
                    eOptions.EftServerPath = null;
                if (!string.IsNullOrEmpty(eOptions.DefaultProfile) && !string.IsNullOrEmpty(eOptions.EftServerPath) && !File.Exists(Path.Combine(eOptions.EftServerPath, eOptions.DirsList["dir_profiles"], eOptions.DefaultProfile + ".json")))
                    eOptions.DefaultProfile = null;
                if (!string.IsNullOrEmpty(eOptions.EftServerPath) && ExtMethods.ServerHaveProfiles(eOptions))
                {
                    lang.Profiles = Directory.GetFiles(Path.Combine(eOptions.EftServerPath, eOptions.DirsList["dir_profiles"])).Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
                    if (lang.Profiles.Count > 0 && (string.IsNullOrEmpty(eOptions.DefaultProfile) || !lang.Profiles.Contains(eOptions.DefaultProfile)))
                        eOptions.DefaultProfile = lang.Profiles.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(eOptions.EftServerPath) && !string.IsNullOrEmpty(eOptions.DefaultProfile))
                {
                    var Pr = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(Path.Combine(eOptions.EftServerPath, eOptions.DirsList["dir_profiles"], eOptions.DefaultProfile + ".json")));
                    lang.Character = Pr.characters?.pmc;
                    if (lang.Character == null) lang.Character = new Character();
                    lang.Character.Suits = Pr.suits?.ToList();
                    lang.Character.WeaponPresets = Pr.weaponbuilds;
                }
                if (lang.Character != null && lang.Character.Info != null && lang.Character.Inventory != null && lang.Character.TraderStandings != null && lang.Character.Skills != null)
                    lang.ProfileHash = JsonConvert.SerializeObject(lang.Character).ToString().GetHashCode();
            }
            catch (Exception ex)
            {
                DialogManager.ShowMessageAsync(Application.Current.MainWindow as MetroWindow, lang.locale["invalid_server_location_caption"], $"{ex.GetType().Name}: {ex.Message}", MessageDialogStyle.Affirmative, new MetroDialogSettings { AffirmativeButtonText = lang.locale["saveprofiledialog_ok"], AnimateShow = true, AnimateHide = true });
                ExtMethods.Log($"Error loading profile: {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            return lang;
        }

        private static PEOptions CreateOptions()
        {
            if (!File.Exists(PeoPath))
                return new PEOptions 
                { 
                    DirsList = new Dictionary<string, string> 
                    {
                        ["dir_globals"] = "Aki_Data\\Server\\database\\locales\\global",
                        ["dir_traders"] = "Aki_Data\\Server\\database\\traders",
                        ["dir_profiles"] = "user\\profiles"
                    },
                    FilesList = new Dictionary<string, string>
                    {
                        ["file_globals"] = "Aki_Data\\Server\\database\\globals.json",
                        ["file_items"] = "Aki_Data\\Server\\database\\templates\\items.json",
                        ["file_quests"] = "Aki_Data\\Server\\database\\templates\\quests.json",
                        ["file_usec"] = "Aki_Data\\Server\\database\\bots\\types\\usec.json",
                        ["file_bear"] = "Aki_Data\\Server\\database\\bots\\types\\bear.json",
                        ["file_areas"] = "Aki_Data\\Server\\database\\hideout\\areas.json",
                        ["file_serverexe"] = "Server.exe"
                    }
                };

            string json = File.ReadAllText(PeoPath);
            PEOptions peo = null;

            try
            {
                peo = JsonConvert.DeserializeObject<PEOptions>(json);
            }
            catch (Exception ex)
            {
                Debug.Print($"Error loading PEOptions.json: {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return new PEOptions();
            }

            return peo;
        }

        public void SaveOptions()
        {
            string json = JsonConvert.SerializeObject(options, Formatting.Indented);
            File.WriteAllText(PeoPath, json);
            Debug.Print($"Options writed to file!");
        }

        public Dictionary<string, string> locale { get; set; }

        public PEOptions options { get; set; }

        public List<string> Profiles { get; set; }

        public Character Character { get; set; }
 
        public CharacterInventory characterInventory { get; set; }

        public GridFilters gridFilters { get; set; }

        public int ProfileHash { get; set; }

        static Dictionary<string, string> EN => new Dictionary<string, string>()
        {
            ["button_yes"] = "Yes",
            ["button_no"] = "No",
            ["button_close"] = "Close",
            ["button_select"] = "Select",
            ["button_settings"] = "SETTINGS",
            ["button_saveprofile"] = "SAVE PROFILE",
            ["button_reloadprofile"] = "RESET CHANGES",
            ["progressdialog_title"] = "Please wait ...",
            ["progressdialog_caption"] = "Loading data",
            ["reloadprofiledialog_title"] = "Resetting changes",
            ["reloadprofiledialog_caption"] = "Are you sure? All changes will be lost",
            ["server_select"] = "Select the SPTarkov Server directory.",
            ["invalid_server_location_caption"] = "Error",
            ["invalid_server_location_text"] = "The selected path does not seem to be a SPTarkov server location.",
            ["no_accounts"] = "Failed to get accounts. No accounts?!",
            ["tab_server_location_select"] = "Select",
            ["tab_info_title"] = "Information",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "Nickname",
            ["tab_info_side"] = "Side",
            ["tab_info_voice"] = "Voice",
            ["tab_info_level"] = "Level",
            ["tab_info_experience"] = "Experience",
            ["tab_info_gameversion"] = "Game Version",
            ["tab_info_pockets"] = "Pockets",
            ["tab_info_head"] = "Head",
            ["tab_merchants_title"] = "Merchants",
            ["tab_merchants_name"] = "Name",
            ["tab_merchants_level"] = "Level",
            ["tab_merchants_enabled"] = "Enabled",
            ["tab_quests_title"] = "Quests",
            ["tab_quests_trader"] = "Trader",
            ["tab_quests_name"] = "Name",
            ["tab_quests_status"] = "Status",
            ["tab_quests_editallbutton"] = "Execute",
            ["tab_quests_markall"] = "Mark all quests:",
            ["tab_quests_nodata"] = "No data to display. Go into the game and run at least one quest",
            ["tab_settings_title"] = "Settings",
            ["tab_settings_lang"] = "Language",
            ["tab_settings_server"] = "SPTarkov Server directory",
            ["tab_settings_account"] = "Account",
            ["tab_settings_colorscheme"] = "Color scheme",
            ["tab_hideout_title"] = "Hideout",
            ["tab_hideout_area"] = "Area",
            ["tab_hideout_level"] = "Level",
            ["tab_hideout_maximumbutton"] = "Set everything to maximum",
            ["tab_skills_title"] = "Skills",
            ["tab_skills_skill"] = "Skill",
            ["tab_skills_exp"] = "Experience",
            ["tab_skills_setall"] = "Set experience for all skills:",
            ["tab_mastering_title"] = "Weapon mastering",
            ["tab_mastering_weapon"] = "Weapon",
            ["tab_mastering_exp"] = "Experience",
            ["tab_mastering_setall"] = "Set experience for all weapons:",
            ["tab_mastering_nodata"] = "No data to display. Play at least one match",
            ["tab_examineditems_title"] = "Examined items",
            ["tab_examineditems_item"] = "Item",
            ["tab_examineditems_exallbutton"] = "Examine all",
            ["tab_stash_title"] = "Stash",
            ["tab_stash_money"] = "Money",
            ["tab_stash_items"] = "Items",
            ["tab_stash_dialogmoney"] = "Enter the amount of money you want to add",
            ["tab_stash_noslots"] = "Not enough free slots",
            ["tab_stash_category"] = "Category",
            ["tab_stash_amount"] = "Amount",
            ["tab_stash_add"] = "Add",
            ["tab_stash_remove"] = "Remove all",
            ["tab_stash_fir"] = "Item found in raid",
            ["tab_backups_title"] = "Backups",
            ["tab_backups_date"] = "Date",
            ["tab_backups_actions"] = "Actions",
            ["tab_backups_restore"] = "Restore",
            ["tab_backups_remove"] = "Remove",
            ["tab_about_title"] = "About",
            ["tab_about_text"] = "Program for editing player profile on the SPTarkov server",
            ["tab_about_developer"] = "Developer:",
            ["tab_about_latestversion"] = "Latest version:",
            ["tab_about_support"] = "Support the developer:",
            ["saveprofiledialog_title"] = "Saving a profile",
            ["saveprofiledialog_caption"] = "Profile saved successfully",
            ["saveprofiledialog_ok"] = "OK",
            ["removebackupdialog_title"] = "Deleting a backup",
            ["removebackupdialog_caption"] = "Are you sure want to delete this backup?",
            ["restorebackupdialog_title"] = "Restoring a backup",
            ["restorebackupdialog_caption"] = "Are you sure want to restore this backup?",
            ["removestashitem_title"] = "Removing an item",
            ["removestashitem_caption"] = "Are you sure you want to delete this item?",
            ["removestashitems_caption"] = "Are you sure you want to delete all items?",
            ["tab_stash_warningtitle"] = "The stash edit function can damage the profile. Use at your own risk.",
            ["tab_stash_moditems"] = "The stash contains items added by mods.\nFunctions for adding money and items are disabled.\nTo use them, remove these items, or move them to the container.",
            ["tab_stash_warningbutton"] = "I understood",
            ["profile_empty"] = "There is no data to display. The profile is empty. Log into the game under this profile and try again.",
            ["app_quit"] = "Quit application?",
            ["button_quit"] = "Quit",
            ["button_cancel"] = "Cancel",
            ["tab_clothing_title"] = "Clothing",
            ["tab_clothing_acquired"] = "Acquired",
            ["tab_clothing_acquireall"]= "Acquire all",
            ["server_runned"] = "The server you selected is currently running. Shut down the server and restart the program.",
            ["update_avialable"] = "Update available",
            ["update_caption"] = "A new version of the program is available. Open the download page?",
            ["tab_presets_title"] = "Presets",
            ["tab_presets_export"] = "Export",
            ["tab_presets_import"] = "Import",
            ["tab_presets_wrongfile"] = "This file does not contain the weapon assembly",
            ["removepresetdialog_title"] = "Deleting a preset",
            ["removepresetdialog_caption"] = "Are you sure want to delete this preset?",
            ["tab_presets_ergonomics"] = "Ergonomics",
            ["tab_preset_recoilup"] = "Recoil Up",
            ["tab_preset_recoilback"] = "Recoil Back",
            ["message_duplicateditems"] = "The profile contains items with the same ID. Do you want to fix it? The profile will be automatically saved with the creation of a backup."
        };

        static Dictionary<string, string> GE => new Dictionary<string, string>
        {
            ["button_yes"] = "Ja",
            ["button_no"] = "Nein",
            ["button_close"] = "Schließen",
            ["button_select"] = "Wählen",
            ["button_settings"] = "EINSTELLUNGEN",
            ["button_saveprofile"] = "PROFIL SICHERN",
            ["button_reloadprofile"] = "ÄNDERUNGEN ÄNDERN",
            ["progressdialog_title"] = "Bitte warten ...",
            ["progressdialog_caption"] = "Daten laden",
            ["reloadprofiledialog_title"] = "Änderungen zurücksetzen",
            ["reloadprofiledialog_caption"] = "Bist du sicher? Alle Änderungen gehen verloren",
            ["server_select"] = "Wählen Sie das SPTarkov Server-Verzeichnis aus.",
            ["invalid_server_location_caption"] = "Error",
            ["invalid_server_location_text"] = "Der ausgewählte Pfad scheint kein SPTarkov-Serverstandort zu sein.",
            ["no_accounts"] = "Konten konnten nicht abgerufen werden. Keine Konten?!",
            ["tab_server_location_select"] = "Wählen",
            ["tab_info_title"] = "Information",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "Spitzname",
            ["tab_info_side"] = "Seite",
            ["tab_info_voice"] = "Abstimmung",
            ["tab_info_level"] = "Niveau",
            ["tab_info_experience"] = "Erfahrung",
            ["tab_info_gameversion"] = "Spielversion",
            ["tab_info_pockets"] = "Hosentaschen",
            ["tab_info_head"] = "Kopf",
            ["tab_merchants_title"] = "Händler",
            ["tab_merchants_name"] = "Name",
            ["tab_merchants_level"] = "Niveau",
            ["tab_merchants_enabled"] = "Aktiviert",
            ["tab_quests_title"] = "Quests",
            ["tab_quests_trader"] = "Händler",
            ["tab_quests_name"] = "Name",
            ["tab_quests_status"] = "Status",
            ["tab_quests_editallbutton"] = "Ausführen",
            ["tab_quests_markall"] = "Markiere alle quests:",
            ["tab_quests_nodata"] = "Keine Daten zum Anzeigen. Geh ins Spiel und führe mindestens eine Quest aus",
            ["tab_settings_title"] = "Einstellungen",
            ["tab_settings_lang"] = "Sprache",
            ["tab_settings_server"] = "SPTarkov Server verzeichnis",
            ["tab_settings_account"] = "Konto",
            ["tab_settings_colorscheme"] = "Farbschema",
            ["tab_hideout_title"] = "Unterschlupf",
            ["tab_hideout_area"] = "Bereich",
            ["tab_hideout_level"] = "Niveau",
            ["tab_hideout_maximumbutton"] = "Stellen Sie alles auf Maximum",
            ["tab_skills_title"] = "Fähigkeiten",
            ["tab_skills_skill"] = "Fähigkeit",
            ["tab_skills_exp"] = "Erfahrung",
            ["tab_skills_setall"] = "Erfahrung für alle Fähigkeiten festlegen:",
            ["tab_mastering_title"] = "Waffenbeherrschung",
            ["tab_mastering_weapon"] = "Waffe",
            ["tab_mastering_exp"] = "Erfahrung",
            ["tab_mastering_setall"] = "Setze Erfahrung für alle Waffen:",
            ["tab_mastering_nodata"] = "Keine Daten verfügbar. Spiele mindestens ein Match",
            ["tab_examineditems_title"] = "Untersuchte Gegenstände",
            ["tab_examineditems_item"] = "Artikel",
            ["tab_examineditems_exallbutton"] = "Untersuche alle",
            ["tab_stash_title"] = "Versteck",
            ["tab_stash_money"] = "Geld",
            ["tab_stash_items"] = "Artikel",
            ["tab_stash_dialogmoney"] = "Geben Sie den Geldbetrag ein, den Sie hinzufügen möchten",
            ["tab_stash_noslots"] = "Nicht genug freie Slots",
            ["tab_stash_category"] = "Kategorie",
            ["tab_stash_amount"] = "Menge",
            ["tab_stash_add"] = "Hinzufügen",
            ["tab_stash_remove"] = "Alles entfernen",
            ["tab_stash_fir"] = "Im Raid gefundener Gegenstand",
            ["tab_backups_title"] = "Backups",
            ["tab_backups_date"] = "Datum",
            ["tab_backups_actions"] = "Aktionen",
            ["tab_backups_restore"] = "Restaurieren",
            ["tab_backups_remove"] = "Entfernen",
            ["tab_about_title"] = "Über",
            ["tab_about_text"] = "Programm zum Bearbeiten des Spielerprofils auf dem SPTarkov-Server",
            ["tab_about_developer"] = "Entwickler:",
            ["tab_about_latestversion"] = "Letzte Version:",
            ["tab_about_support"] = "Unterstützen Sie den Entwickler:",
            ["saveprofiledialog_title"] = "Profil speichern",
            ["saveprofiledialog_caption"] = "Profil erfolgreich gespeichert",
            ["saveprofiledialog_ok"] = "OK",
            ["removebackupdialog_title"] = "Backup löschen",
            ["removebackupdialog_caption"] = "Möchten Sie dieses Backup wirklich löschen?",
            ["restorebackupdialog_title"] = "Backup wiederherstellen",
            ["restorebackupdialog_caption"] = "Möchten Sie dieses Backup wirklich wiederherstellen?",
            ["removestashitem_title"] = "УEinen Gegenstand entfernen",
            ["removestashitem_caption"] = "Möchten Sie diesen Artikel wirklich löschen?",
            ["removestashitems_caption"] = "Möchten Sie wirklich alle Elemente löschen?",
            ["tab_stash_warningtitle"] = "Die Stash-Bearbeitungsfunktion kann das Profil beschädigen. Benutzung auf eigene Gefahr.",
            ["tab_stash_moditems"] = "Der Stash enthält Gegenstände, die von Mods hinzugefügt wurden.\nFunktionen zum Hinzufügen von Geld und Gegenständen sind deaktiviert.\nUm sie zu verwenden, entfernen Sie diese Elemente oder verschieben Sie sie in den Container.",
            ["tab_stash_warningbutton"] = "Ich habe verstanden",
            ["profile_empty"] = "Es sind keine Daten anzuzeigen. Das Profil ist leer. Melde dich unter diesem Profil im Spiel an und versuche es erneut.",
            ["app_quit"] = "Bewerbung beenden?",
            ["button_quit"] = "Verlassen",
            ["button_cancel"] = "Stornieren",
            ["tab_clothing_title"] = "Kleidung",
            ["tab_clothing_acquired"] = "Erworben",
            ["tab_clothing_acquireall"] = "Erwerben Sie alle",
            ["server_runned"] = "Der von Ihnen ausgewählte Server wird derzeit ausgeführt. Fahren Sie den Server herunter und starten Sie das Programm neu.",
            ["update_avialable"] = "Update verfügbar",
            ["update_caption"] = "Eine neue Version des Programms ist verfügbar. Download-Seite öffnen?",
            ["tab_presets_title"] = "Vorlagen",
            ["tab_presets_export"] = "Export",
            ["tab_presets_import"] = "Importieren",
            ["tab_presets_wrongfile"] = "Diese Datei enthält nicht die Waffenbaugruppe",
            ["removepresetdialog_title"] = "Voreinstellung löschen",
            ["removepresetdialog_caption"] = "Möchten Sie diese Voreinstellung wirklich löschen?",
            ["tab_presets_ergonomics"] = "Ergonomie",
            ["tab_preset_recoilup"] = "Vertikaler Rückstoß",
            ["tab_preset_recoilback"] = "Horizontaler Rückstoß",
            ["message_duplicateditems"] = "Das Profil enthält Elemente mit derselben ID. Möchten Sie das Problem beheben? Das Profil wird beim Erstellen eines Backups automatisch gespeichert."
        };

        static Dictionary<string, string> RU => new Dictionary<string, string>
        {
            ["button_yes"] = "Да",
            ["button_no"] = "Нет",
            ["button_close"] = "Закрыть",
            ["button_select"] = "Выбрать",
            ["button_settings"] = "НАСТРОЙКИ",
            ["button_saveprofile"] = "СОХРАНИТЬ ПРОФИЛЬ",
            ["button_reloadprofile"] = "СБРОСИТЬ ИЗМЕНЕНИЯ",
            ["progressdialog_title"] = "Пожалуйста подождите ...",
            ["progressdialog_caption"] = "Идет загрузка данных",
            ["reloadprofiledialog_title"] = "Сброс изменений",
            ["reloadprofiledialog_caption"] = "Вы уверены? Все изменения будут потеряны",
            ["server_select"] = "Выберите папку с сервером SPTarkov.",
            ["invalid_server_location_caption"] = "Ошибка",
            ["invalid_server_location_text"] = "Сервер SPTarkov не найден. Попробовать снова?",
            ["no_accounts"] = "Не удалось получить аккаунты. Нет аккаунтов?!",
            ["tab_server_location_select"] = "Выбрать",
            ["tab_info_title"] = "Информация",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "Никнейм",
            ["tab_info_side"] = "Сторона",
            ["tab_info_voice"] = "Голос",
            ["tab_info_level"] = "Уровень",
            ["tab_info_experience"] = "Опыт",
            ["tab_info_gameversion"] = "Версия игры",
            ["tab_info_pockets"] = "Карманы",
            ["tab_info_head"] = "Голова",
            ["tab_merchants_title"] = "Торговцы",
            ["tab_merchants_name"] = "Имя",
            ["tab_merchants_level"] = "Уровень",
            ["tab_merchants_enabled"] = "Включен",
            ["tab_quests_title"] = "Квесты",
            ["tab_quests_trader"] = "Торговец",
            ["tab_quests_name"] = "Название",
            ["tab_quests_status"] = "Статус",
            ["tab_quests_editallbutton"] = "Выполнить",
            ["tab_quests_markall"] = "Отметить все квесты:",
            ["tab_quests_nodata"] = "Нет данных для отображения. Зайдите в игру и запустите хотя бы один квест",
            ["tab_settings_title"] = "Настройки",
            ["tab_settings_lang"] = "Язык",
            ["tab_settings_server"] = "Каталог SPTarkov Server",
            ["tab_settings_account"] = "Аккаунт",
            ["tab_settings_colorscheme"] = "Цветовая схема",
            ["tab_hideout_title"] = "Убежище",
            ["tab_hideout_area"] = "Зона",
            ["tab_hideout_level"] = "Уровень",
            ["tab_hideout_maximumbutton"] = "Установить все на максимум",
            ["tab_skills_title"] = "Умения",
            ["tab_skills_skill"] = "Умение",
            ["tab_skills_exp"] = "Опыт",
            ["tab_skills_setall"] = "Установить опыт для всех умений:",
            ["tab_mastering_title"] = "Владение оружием",
            ["tab_mastering_weapon"] = "Оружие",
            ["tab_mastering_exp"] = "Опыт",
            ["tab_mastering_setall"] = "Установить опыт для всего оружия:",
            ["tab_mastering_nodata"] = "Нет данных. Сыграйте хотя бы один матч",
            ["tab_examineditems_title"] = "Изученные предметы",
            ["tab_examineditems_item"] = "Предмет",
            ["tab_examineditems_exallbutton"] = "Изучить все",
            ["tab_stash_title"] = "Схрон",
            ["tab_stash_items"] = "Предметы",
            ["tab_stash_money"] = "Деньги",
            ["tab_stash_dialogmoney"] = "Введите сумму денег, которую хотите добавить",
            ["tab_stash_noslots"] = "Недостаточно свободных слотов",
            ["tab_stash_category"] = "Категория",
            ["tab_stash_amount"] = "Количество",
            ["tab_stash_add"] = "Добавить",
            ["tab_stash_remove"] = "Удалить все",
            ["tab_stash_fir"] = "Предмет найденный в рейде",
            ["tab_backups_title"] = "Бэкапы",
            ["tab_backups_date"] = "Дата",
            ["tab_backups_actions"] = "Действия",
            ["tab_backups_restore"] = "Восстановить",
            ["tab_backups_remove"] = "Удалить",
            ["tab_about_title"] = "О программе",
            ["tab_about_text"] = "Программа для редактирования профиля игрока на сервере SPTarkov",
            ["tab_about_developer"] = "Разработчик:",
            ["tab_about_latestversion"] = "Последняя версия:",
            ["tab_about_support"] = "Поддержать разработчика:",
            ["saveprofiledialog_title"] = "Сохранение профиля",
            ["saveprofiledialog_caption"] = "Профиль успешно сохранен",
            ["saveprofiledialog_ok"] = "OK",
            ["removebackupdialog_title"] = "Удаление бэкапа",
            ["removebackupdialog_caption"] = "Вы действительно хотите удалить этот бэкап?",
            ["restorebackupdialog_title"] = "Восстановление бэкапа",
            ["restorebackupdialog_caption"] = "Вы действительно хотите восстановить этот бэкап?",
            ["removestashitem_title"] = "Удаление предмета",
            ["removestashitem_caption"] = "Вы действительно хотите удалить этот предмет?",
            ["removestashitems_caption"] = "Вы действительно хотите удалить все предметы?",
            ["tab_stash_warningtitle"] = "Функция редактирования схрона может повредить профиль. Используйте на свой страх и риск.",
            ["tab_stash_moditems"] = "Схрон содержит предметы добавленные модами.\nФункции добавления денег и предметов отключены.\nЧто бы воспользоваться ими удалите эти предметы, или переместите их в контейнер.",
            ["tab_stash_warningbutton"] = "Я понял",
            ["profile_empty"] = "Нет данных для отображения. Профиль пустой. Зайдите в игру под этим профилем и попробуйте снова.",
            ["app_quit"] = "Выйти из приложения?",
            ["button_quit"] = "Выход",
            ["button_cancel"] = "Отмена",
            ["tab_clothing_title"] = "Одежда",
            ["tab_clothing_acquired"] = "Приобретено",
            ["tab_clothing_acquireall"] = "Получить все",
            ["server_runned"] = "Выбранный вами сервер запущен в данный момент. Выключите сервер и перезапустите программу.",
            ["update_avialable"] = "Доступно обновление",
            ["update_caption"] = "Доступна новая версия программы. Открыть страницу загрузки?",
            ["tab_presets_title"] = "Сборки",
            ["tab_presets_export"] = "Экспорт",
            ["tab_presets_import"] = "Импорт",
            ["tab_presets_wrongfile"] = "Этот файл не содержит сборку оружия",
            ["removepresetdialog_title"] = "Удаление сборки",
            ["removepresetdialog_caption"] = "Вы действительно хотите удалить эту сборку?",
            ["tab_presets_ergonomics"] = "Эргономика",
            ["tab_preset_recoilup"] = "Вертикальная отдача",
            ["tab_preset_recoilback"] = "Горизонтальная отдача",
            ["message_duplicateditems"] = "В профиле есть предметы с одинаковыми ID. Вы хотите это исправить? Профиль будет автоматически сохранен с созданием бэкапа."
        };

        static Dictionary<string, string> FR => new Dictionary<string, string>
        {
            ["button_yes"] = "Oui",
            ["button_no"] = "Non",
            ["button_close"] = "Fermer",
            ["button_select"] = "Sélectionner",
            ["button_settings"] = "PARAMÈTRES",
            ["button_saveprofile"] = "ENREGISTRER LE PROFIL",
            ["button_reloadprofile"] = "RÉINITIALISER LES MODIFICATIONS",
            ["progressdialog_title"] = "Veuillez patienter ...",
            ["progressdialog_caption"] = "Chargement des données",
            ["reloadprofiledialog_title"] = "Réinitialiser les modifications",
            ["reloadprofiledialog_caption"] = "Êtes-vous sûr? Tous les changements seront perdus",
            ["server_select"] = "Sélectionnez le répertoire du serveur SPTarkov.",
            ["invalid_server_location_caption"] = "Erreur",
            ["invalid_server_location_text"] = "Le chemin sélectionné ne semble pas être un emplacement de serveur SPTarkov. Réessayer?",
            ["no_accounts"] = "Échec de l'obtention de comptes. Pas de comptes?!",
            ["tab_server_location_select"] = "Sélect",
            ["tab_info_title"] = "Information",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "Surnom",
            ["tab_info_side"] = "Côté",
            ["tab_info_voice"] = "Voter",
            ["tab_info_level"] = "Niveau",
            ["tab_info_experience"] = "Expérience",
            ["tab_info_gameversion"] = "Version du jeu",
            ["tab_info_pockets"] = "Poches",
            ["tab_info_head"] = "Tête",
            ["tab_merchants_title"] = "Marchands",
            ["tab_merchants_name"] = "Nom",
            ["tab_merchants_level"] = "Niveau",
            ["tab_merchants_enabled"] = "Activé",
            ["tab_quests_title"] = "Quêtes",
            ["tab_quests_trader"] = "Commerçant",
            ["tab_quests_name"] = "Nom",
            ["tab_quests_status"] = "Statut",
            ["tab_quests_editallbutton"] = "Exécuter",
            ["tab_quests_markall"] = "Marquez toutes les quêtes:",
            ["tab_quests_nodata"] = "Aucune donnée à afficher. Entrez dans le jeu et lancez au moins une quête",
            ["tab_settings_title"] = "Paramètres",
            ["tab_settings_lang"] = "Langue",
            ["tab_settings_server"] = "Répertoire du serveur SPTarkov",
            ["tab_settings_account"] = "Compte",
            ["tab_settings_colorscheme"] = "Schéma de couleur",
            ["tab_hideout_title"] = "Planque",
            ["tab_hideout_area"] = "Zone",
            ["tab_hideout_level"] = "Niveau",
            ["tab_hideout_maximumbutton"] = "Réglez tout au maximum",
            ["tab_skills_title"] = "Compétence",
            ["tab_skills_skill"] = "Compétence",
            ["tab_skills_exp"] = "Expérience",
            ["tab_skills_setall"] = "Définissez l'expérience pour toutes les compétences:",
            ["tab_mastering_title"] = "Maîtrise des armes",
            ["tab_mastering_weapon"] = "Arme",
            ["tab_mastering_exp"] = "Expérience",
            ["tab_mastering_setall"] = "Définissez l'expérience pour toutes les armes:",
            ["tab_mastering_nodata"] = "Aucune donnée à afficher. Jouez au moins un match",
            ["tab_examineditems_title"] = "Articles examinés",
            ["tab_examineditems_item"] = "Article",
            ["tab_examineditems_exallbutton"] = "Tout examiner",
            ["tab_stash_title"] = "Réserve",
            ["tab_stash_money"] = "Argent",
            ["tab_stash_items"] = "Articles",
            ["tab_stash_dialogmoney"] = "Entrez le montant que vous souhaitez ajouter",
            ["tab_stash_noslots"] = "Pas assez d'emplacements gratuits",
            ["tab_stash_category"] = "Catégorie",
            ["tab_stash_amount"] = "Montant",
            ["tab_stash_add"] = "Ajouter",
            ["tab_stash_remove"] = "Enlever tout",
            ["tab_stash_fir"] = "Objet trouvé en raid",
            ["tab_backups_title"] = "Sauvegardes",
            ["tab_backups_date"] = "Date",
            ["tab_backups_actions"] = "Actions",
            ["tab_backups_restore"] = "Restaurer",
            ["tab_backups_remove"] = "Retirer",
            ["tab_about_title"] = "Sur",
            ["tab_about_text"] = "Programme d'édition du profil du joueur sur le serveur SPTarkov",
            ["tab_about_developer"] = "Développeur:",
            ["tab_about_latestversion"] = "Dernière version:",
            ["tab_about_support"] = "Soutenez le développeur:",
            ["saveprofiledialog_title"] = "Enregistrer un profil",
            ["saveprofiledialog_caption"] = "Profil enregistré avec succès",
            ["saveprofiledialog_ok"] = "OK",
            ["removebackupdialog_title"] = "Supprimer une sauvegarde",
            ["removebackupdialog_caption"] = "Voulez-vous vraiment supprimer cette sauvegarde?",
            ["restorebackupdialog_title"] = "Restaurer une sauvegarde",
            ["restorebackupdialog_caption"] = "Voulez-vous vraiment restaurer cette sauvegarde?",
            ["removestashitem_title"] = "Supprimer un élément",
            ["removestashitem_caption"] = "Êtes-vous sûr de bien vouloir supprimer cet élément?",
            ["removestashitems_caption"] = "Voulez-vous vraiment supprimer tous les éléments?",
            ["tab_stash_warningtitle"] = "La fonction d'édition de cache peut endommager le profil. À utiliser à vos risques et périls.",
            ["tab_stash_moditems"] = "La réserve contient des éléments ajoutés par les mods.\nLes fonctions d'ajout d'argent et d'objets sont désactivées.\nPour les utiliser, supprimez ces éléments ou déplacez-les vers le conteneur.",
            ["tab_stash_warningbutton"] = "J'ai compris",
            ["profile_empty"] = "Il n'y a aucune donnée à afficher. Le profil est vide. Connectez-vous au jeu sous ce profil et réessayez.",
            ["app_quit"] = "Quitter l'application?",
            ["button_quit"] = "Quitter",
            ["button_cancel"] = "Annuler",
            ["tab_clothing_title"] = "Vêtements",
            ["tab_clothing_acquired"] = "Acquise",
            ["tab_clothing_acquireall"] = "Acquérir tout",
            ["server_runned"] = "Le serveur que vous avez sélectionné est en cours d’exécution. Arrêtez le serveur et redémarrez le programme.",
            ["update_avialable"] = "Mise à jour disponible",
            ["update_caption"] = "Une nouvelle version du programme est disponible. Ouvrez la page de téléchargement?",
            ["tab_presets_title"] = "Configurations",
            ["tab_presets_export"] = "Exportation",
            ["tab_presets_import"] = "Importer",
            ["tab_presets_wrongfile"] = "Ce fichier ne contient pas l'assemblage d'arme",
            ["removepresetdialog_title"] = "Supprimer un préréglage",
            ["removepresetdialog_caption"] = "Voulez-vous vraiment supprimer ce préréglage?",
            ["tab_presets_ergonomics"] = "Ergonomie",
            ["tab_preset_recoilup"] = "Recul vertical",
            ["tab_preset_recoilback"] = "Recul horizontal",
            ["message_duplicateditems"] = "Le profil contient des éléments avec le même ID. Voulez-vous le réparer? Le profil sera automatiquement sauvegardé avec la création d'une sauvegarde."
        };

        //Chinese, thx https://sns.oddba.cn/author/40234
        static Dictionary<string, string> CH => new Dictionary<string, string>
        {
            ["button_yes"] = "是",
            ["button_no"] = "否",
            ["button_close"] = "关闭",
            ["button_select"] = "选择",
            ["button_settings"] = "设定",
            ["button_saveprofile"] = "保存配置文件",
            ["button_reloadprofile"] = "重置更改",
            ["progressdialog_title"] = "请稍候 ...",
            ["progressdialog_caption"] = "正在加载数据",
            ["reloadprofiledialog_title"] = "重置更改",
            ["reloadprofiledialog_caption"] = "确定放弃保存吗？所有更改都将丢失",
            ["server_select"] = "选择SPTarkov服务器目录",
            ["invalid_server_location_caption"] = "错误",
            ["invalid_server_location_text"] = "所选路径似乎不是SPTarkov服务器位置。",
            ["no_accounts"] = "无法获取帐户。没有帐户？!",
            ["tab_server_location_select"] = "选择",
            ["tab_info_title"] = "信息",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "昵称",
            ["tab_info_side"] = "阵营",
            ["tab_info_voice"] = "语音",
            ["tab_info_level"] = "等级",
            ["tab_info_experience"] = "经验",
            ["tab_info_gameversion"] = "游戏版本",
            ["tab_info_pockets"] = "口袋",
            ["tab_info_head"] = "头部",
            ["tab_merchants_title"] = "商人",
            ["tab_merchants_name"] = "名称",
            ["tab_merchants_level"] = "等级",
            ["tab_merchants_enabled"] = "Enabled",
            ["tab_quests_title"] = "任务",
            ["tab_quests_trader"] = "商人",
            ["tab_quests_name"] = "名称",
            ["tab_quests_status"] = "状态",
            ["tab_quests_editallbutton"] = "执行",
            ["tab_quests_markall"] = "标记所有任务：",
            ["tab_quests_nodata"] = "无数据可显示。请先进入游戏并接受至少一个任务",
            ["tab_settings_title"] = "设置",
            ["tab_settings_lang"] = "语言",
            ["tab_settings_server"] = "SPTarkov服务器目录",
            ["tab_settings_account"] = "帐户",
            ["tab_settings_colorscheme"] = "配色方案",
            ["tab_hideout_title"] = "藏身处",
            ["tab_hideout_area"] = "区域",
            ["tab_hideout_level"] = "等级",
            ["tab_hideout_maximumbutton"] = "将所有内容设置为最大",
            ["tab_skills_title"] = "技能",
            ["tab_skills_skill"] = "技能",
            ["tab_skills_exp"] = "经验",
            ["tab_skills_setall"] = "设置所有技能的经验：",
            ["tab_mastering_title"] = "武器精通",
            ["tab_mastering_weapon"] = "武器",
            ["tab_mastering_exp"] = "经验",
            ["tab_mastering_setall"] = "设置所有武器的使用经验：",
            ["tab_mastering_nodata"] = "无数据可显示。至少玩一场比赛",
            ["tab_examineditems_title"] = "检视物品",
            ["tab_examineditems_item"] = "物品",
            ["tab_examineditems_exallbutton"] = "全部检视",
            ["tab_stash_title"] = "仓库物品",
            ["tab_stash_money"] = "金钱",
            ["tab_stash_items"] = "项目",
            ["tab_stash_dialogmoney"] = "输入要添加的金额",
            ["tab_stash_noslots"] = "可用插槽不足",
            ["tab_stash_category"] = "类别",
            ["tab_stash_amount"] = "金额",
            ["tab_stash_add"] = "添加",
            ["tab_stash_remove"] = "全部删除",
            ["tab_stash_fir"] = "在战局中找到",
            ["tab_backups_title"] = "备份",
            ["tab_backups_date"] = "日期",
            ["tab_backups_actions"] = "动作",
            ["tab_backups_restore"] = "还原",
            ["tab_backups_remove"] = "删除",
            ["tab_about_title"] = "关于",
            ["tab_about_text"] = "用于在SPTarkov服务器上编辑存档文件的程序",
            ["tab_about_developer"] = "开发人员：",
            ["tab_about_latestversion"] = "最新版本：",
            ["tab_about_support"] = "支持开发人员：",
            ["saveprofiledialog_title"] = "保存个人资料",
            ["saveprofiledialog_caption"] = "配置文件成功保存",
            ["saveprofiledialog_ok"] = "好的",
            ["removebackupdialog_title"] = "正在删除备份",
            ["removebackupdialog_caption"] = "确定要删除此备份吗？",
            ["restorebackupdialog_title"] = "正在还原备份",
            ["restorebackupdialog_caption"] = "您确定要还原此备份吗？",
            ["removestashitem_title"] = "正在移除物品",
            ["removestashitem_caption"] = "确定要删除此项目吗？",
            ["removestashitems_caption"] = "确定要删除所有项目吗？",
            ["tab_stash_warningtitle"] = "仓库物品编辑功能可能会损坏配置文件。使用后果自负。",
            ["tab_stash_moditems"] = "储藏室包含mods添加的项目。\n用于添加金钱和项目的功能被禁用。\n若要使用它们，请删除这些项目，或将它们移动到容器中。",
            ["tab_stash_warningbutton"] = "我了解",
            ["profile_empty"] = "没有数据可显示。配置文件为空。在此配置文件下登录游戏，然后重试。",
            ["app_quit"] = "要退出应用程序吗？",
            ["button_quit"] = "退出",
            ["button_cancel"] = "取消",
            ["tab_clothing_title"] = "服装",
            ["tab_clothing_acquired"] = "获取",
            ["tab_clothing_acquireall"] = "全部获取",
            ["server_runned"] = "您选择的服务器当前正在运行。关闭服务器并重新启动程序。",
            ["update_avialable"] = "可用更新",
            ["update_caption"] = "新版本的程序可用。打开下载页？",
            ["tab_presets_title"] = "预设",
            ["tab_presets_export"] = "导出",
            ["tab_presets_import"] = "导入",
            ["tab_presets_wrongfile"] = "此文件不包含武器组件",
            ["removepresetdialog_title"] = "删除预设",
            ["removepresetdialog_caption"] = "确实要删除此预设吗？",
            ["tab_presets_ergonomics"] = "人机工效",
            ["tab_preset_recoilup"] = "增加后坐力",
            ["tab_preset_recoilback"] = "减小后坐力",
            ["message_duplicateditems"] = "配置文件包含具有相同ID的项。是否要修复它？配置文件将随着备份的创建而自动保存。"
        };

        //Spanish, thx https://hub.sp-tarkov.com/user/8265-darkfd95/

        static Dictionary<string, string> ES => new Dictionary<string, string>
        {
            ["button_yes"] = "Si",
            ["button_no"] = "No",
            ["button_close"] = "Cerrar",
            ["button_select"] = "Seleccionar",
            ["button_settings"] = "CONFIGURACION",
            ["button_saveprofile"] = "GUARDAR PERFIL",
            ["button_reloadprofile"] = "RESTABLECER CAMBIOS",
            ["progressdialog_title"] = "Porfavor espere ...",
            ["progressdialog_caption"] = "Cargando datos",
            ["reloadprofiledialog_title"] = "Restablecer cambios",
            ["reloadprofiledialog_caption"] = "Estas seguro? Todos los cambios se perderan",
            ["server_select"] = "Selecciona el directorio de SPTarkov Server.",
            ["invalid_server_location_caption"] = "Error",
            ["invalid_server_location_text"] = "La ruta seleccionada no parece ser una ubicación del SPTarkov server.",
            ["no_accounts"] = "Failed to get accounts. No accounts?!",
            ["tab_server_location_select"] = "Select",
            ["tab_info_title"] = "Informacion",
            ["tab_info_id"] = "ID",
            ["tab_info_nickname"] = "Nickname",
            ["tab_info_side"] = "Faccion",
            ["tab_info_voice"] = "Voz",
            ["tab_info_level"] = "Nivel",
            ["tab_info_experience"] = "Experiencia",
            ["tab_info_gameversion"] = "Version del juego",
            ["tab_info_pockets"] = "Bolsillos",
            ["tab_info_head"] = "Cabeza",
            ["tab_merchants_title"] = "Comerciantes",
            ["tab_merchants_name"] = "Name",
            ["tab_merchants_level"] = "Level",
            ["tab_merchants_enabled"] = "Habilitado",
            ["tab_quests_title"] = "Misiones",
            ["tab_quests_trader"] = "Traficante",
            ["tab_quests_name"] = "Nombre",
            ["tab_quests_status"] = "Estado",
            ["tab_quests_editallbutton"] = "Ejecutar",
            ["tab_quests_markall"] = "Marcar todas las misiones:",
            ["tab_quests_nodata"] = "No hay informacion para mostrar. Ve dentro del juego e inicia una mision",
            ["tab_settings_title"] = "Opciones",
            ["tab_settings_lang"] = "Language",
            ["tab_settings_server"] = "Directorio de SPTarkov Server",
            ["tab_settings_account"] = "Cuenta",
            ["tab_settings_colorscheme"] = "Esquema de color",
            ["tab_hideout_title"] = "Refugio",
            ["tab_hideout_area"] = "Area",
            ["tab_hideout_level"] = "Nivel",
            ["tab_hideout_maximumbutton"] = "Estableser todo al maximo",
            ["tab_skills_title"] = "Habilidades",
            ["tab_skills_skill"] = "Habilidad",
            ["tab_skills_exp"] = "Experiencia",
            ["tab_skills_setall"] = "Establecer la experiencia para todas las habilidades:",
            ["tab_mastering_title"] = "Maestria de armas",
            ["tab_mastering_weapon"] = "Arma",
            ["tab_mastering_exp"] = "Experiencia",
            ["tab_mastering_setall"] = "Establecer la experiencia para todas las armas:",
            ["tab_mastering_nodata"] = "No hay informacion para mostrar. Juega al menos un raid",
            ["tab_examineditems_title"] = "Objetos examinados",
            ["tab_examineditems_item"] = "Objetos",
            ["tab_examineditems_exallbutton"] = "Examinar todo",
            ["tab_stash_title"] = "Alijo",
            ["tab_stash_money"] = "Dineto",
            ["tab_stash_items"] = "Objetos",
            ["tab_stash_dialogmoney"] = "Ingrece la cantidad de dinero a agregar",
            ["tab_stash_noslots"] = "No hay suficiente espacio",
            ["tab_stash_category"] = "Categoria",
            ["tab_stash_amount"] = "Cantidad",
            ["tab_stash_add"] = "Agregar",
            ["tab_stash_remove"] = "Quitar todo",
            ["tab_stash_fir"] = "Objetos found in raid(encontrados en incurcion)",
            ["tab_backups_title"] = "Respaldo",
            ["tab_backups_date"] = "Fecha",
            ["tab_backups_actions"] = "Acciones",
            ["tab_backups_restore"] = "Restablecer",
            ["tab_backups_remove"] = "Quitar",
            ["tab_about_title"] = "Acerca de",
            ["tab_about_text"] = "El programa para editar el perfil del jugador de SPTarkov server",
            ["tab_about_developer"] = "Desarrollador:",
            ["tab_about_latestversion"] = "Ultima version:",
            ["tab_about_support"] = "Ayuda al desarrollador:",
            ["saveprofiledialog_title"] = "Guardando un perfil",
            ["saveprofiledialog_caption"] = "El perfil se a guardado exitosamente",
            ["saveprofiledialog_ok"] = "OK",
            ["removebackupdialog_title"] = "Eliminando backup",
            ["removebackupdialog_caption"] = "Estas seguro que deseas eliminar este backup?",
            ["restorebackupdialog_title"] = "Restableciendo backup",
            ["restorebackupdialog_caption"] = "Estas seguro que deseas restablecer este backup?",
            ["removestashitem_title"] = "Eliminando un objeto",
            ["removestashitem_caption"] = "Estas seguro que deseas eliminar este objeto?",
            ["removestashitems_caption"] = "Estas seguro que deseas eliminar este todos los objetos?",
            ["tab_stash_warningtitle"] = "El editor de alijo puede dañar el perfil del jugador. Utilizar bajo su propio riesgo.",
            ["tab_stash_moditems"] = "El alijo contiene objetos agregados con mods.\nLa funcion de Agregado de dinero y objetos esta deshabilitada.\nPara usarla, remueve estos objetos, o moevelos a los contenedores.",
            ["tab_stash_warningbutton"] = "He entendido",
            ["profile_empty"] = "No hay informacion para mostrar. El prefil esta vacio. Inicia sesion en el juego con este perfil y vuelva a intentar.",
            ["app_quit"] = "Salir de la aplicacion?",
            ["button_quit"] = "Salir",
            ["button_cancel"] = "Cancelar",
            ["tab_clothing_title"] = "Vestimenta",
            ["tab_clothing_acquired"] = "Adquirido",
            ["tab_clothing_acquireall"] = "Adquirir Todo",
            ["server_runned"] = "El servidor selecionado se elcuentra en ejecucion. Apague el servidor y reinicie el programa.",
            ["update_avialable"] = "Actualizacion disponible",
            ["update_caption"] = "Nueva version del programa esta disponible. Abrir pagina de descarga?",
            ["tab_presets_title"] = "Presets",
            ["tab_presets_export"] = "Exportar",
            ["tab_presets_import"] = "Importar",
            ["tab_presets_wrongfile"] = "Este archivo no contiene el ensamble del arma",
            ["removepresetdialog_title"] = "Eliminar preset",
            ["removepresetdialog_caption"] = "Estas seguro que deseas eliminar este preset?",
            ["tab_presets_ergonomics"] = "Ergonomia",
            ["tab_preset_recoilup"] = "Retroceso Vertical",
            ["tab_preset_recoilback"] = "Retroceso Horizontal",
            ["message_duplicateditems"] = "El perfil contien objetos con el mismo ID. Desea repararlo? El perfil sera guardado automaticamente con la creacion de un backup."
        };
    }

    public class PEOptions
    {
        public string EftServerPath { get; set; }

        public string Language { get; set; }

        public string DefaultProfile { get; set; }

        public string ColorScheme { get; set; }

        public Dictionary<string, string> DirsList { get; set; }

        public Dictionary<string, string> FilesList { get; set; }
    }
}
