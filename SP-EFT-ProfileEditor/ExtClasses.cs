using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SP_EFT_ProfileEditor
{
    public class Lang
    {
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
        public static Lang Load()
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Debug.Print($"Error loading languages: {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    System.Windows.Application.Current.Shutdown();
                }
            }
            if (!File.Exists(Path.Combine(path, "en.json")))
                CreateEnLoclae();
            if (!File.Exists(Path.Combine(path, "ru.json")))
                CreateRuLocale();
            if (!File.Exists(Path.Combine(path, "fr.json")))
                CreateFrLocale();
            if (!File.Exists(Path.Combine(path, "ge.json")))
                CreateGeLocale();
            PEOptions eOptions = PEOptions.Load();
            if (string.IsNullOrEmpty(eOptions.Language))
                eOptions.Language = "en";
            Lang lang = new Lang { locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(path, eOptions.Language + ".json"))), options = eOptions};
            if (!string.IsNullOrEmpty(eOptions.EftServerPath) && Directory.Exists(eOptions.EftServerPath))
            {
                lang.Profiles = Directory.GetDirectories(eOptions.EftServerPath + "\\user\\profiles").Where(x => File.Exists(x + "\\character.json")).Select(x => new DirectoryInfo(x).Name).ToList();
                if (lang.Profiles.Count > 0 && (string.IsNullOrEmpty(eOptions.DefaultProfile) || !lang.Profiles.Contains(eOptions.DefaultProfile)))
                    eOptions.DefaultProfile = lang.Profiles.FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(eOptions.EftServerPath) && !string.IsNullOrEmpty(eOptions.DefaultProfile))
                lang.Character = JsonConvert.DeserializeObject<Character>(File.ReadAllText(Path.Combine(eOptions.EftServerPath, "user\\profiles", eOptions.DefaultProfile, "character.json")));
            eOptions.Save();
            return lang;
        }

        public Dictionary<string, string> locale { get; set; }

        public PEOptions options { get; set; }

        public List<string> Profiles { get; set; }

        public Character Character { get; set; }

        static void CreateEnLoclae()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["button_yes"] = "Yes",
                ["button_no"] = "No",
                ["button_close"] = "Close",
                ["button_select"] = "Select",
                ["button_settings"] = "SETTINGS",
                ["server_select"] = "Select the SPTarkov Server directory.",
                ["invalid_server_location_caption"] = "Error",
                ["invalid_server_location_text"] = "The selected path does not seem to be a SPTarkov server location.",
                ["no_accounts"] = "Failed to get accounts. No accounts?!",
                ["tab_server_location_select"] = "Select",
                ["tab_info_title"] = "Information",
                ["tab_quests_title"] = "Quests",
                ["tab_settings_title"] = "Settings",
                ["tab_settings_lang"] = "Language",
                ["tab_settings_server"] = "SPTarkov Server directory",
                ["tab_settings_account"] = "Account",
                ["tab_settings_colorscheme"] = "Color scheme",
                ["tab_serversettings_title"] = "Server settings"
            };
            File.WriteAllText(Path.Combine(path, "en.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
        }

        static void CreateGeLocale()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["button_yes"] = "Ja",
                ["button_no"] = "Nein",
                ["button_close"] = "Schließen",
                ["button_select"] = "Wählen",
                ["button_settings"] = "EINSTELLUNGEN",
                ["server_select"] = "Wählen Sie das SPTarkov Server-Verzeichnis aus.",
                ["invalid_server_location_caption"] = "Error",
                ["invalid_server_location_text"] = "Der ausgewählte Pfad scheint kein SPTarkov-Serverstandort zu sein.",
                ["no_accounts"] = "Konten konnten nicht abgerufen werden. Keine Konten?!",
                ["tab_server_location_select"] = "Wählen",
                ["tab_info_title"] = "Information",
                ["tab_quests_title"] = "Quests",
                ["tab_settings_title"] = "Einstellungen",
                ["tab_settings_lang"] = "Sprache",
                ["tab_settings_server"] = "SPTarkov Server verzeichnis",
                ["tab_settings_account"] = "Konto",
                ["tab_settings_colorscheme"] = "Farbschema",
                ["tab_serversettings_title"] = "Server einstellungen"
            };
            File.WriteAllText(Path.Combine(path, "ge.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
        }

        static void CreateRuLocale()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["button_yes"] = "Да",
                ["button_no"] = "Нет",
                ["button_close"] = "Закрыть",
                ["button_select"] = "Выбрать",
                ["button_settings"] = "НАСТРОЙКИ",
                ["server_select"] = "Выберите папку с сервером SPTarkov.",
                ["invalid_server_location_caption"] = "Ошибка",
                ["invalid_server_location_text"] = "Сервер SPTarkov не найден. Попробовать снова?",
                ["no_accounts"] = "Не удалось получить аккаунты. Нет аккаунтов?!",
                ["tab_server_location_select"] = "Выбрать",
                ["tab_info_title"] = "Информация",
                ["tab_quests_title"] = "Квесты",
                ["tab_settings_title"] = "Настройки",
                ["tab_settings_lang"] = "Язык",
                ["tab_settings_server"] = "Каталог SPTarkov Server",
                ["tab_settings_account"] = "Аккаунт",
                ["tab_settings_colorscheme"] = "Цветовая схема",
                ["tab_serversettings_title"] = "Настройки сервера"
            };
            File.WriteAllText(Path.Combine(path, "ru.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
        }

        static void CreateFrLocale()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["button_yes"] = "Oui",
                ["button_no"] = "Non",
                ["button_close"] = "Fermer",
                ["button_select"] = "Sélectionner",
                ["button_settings"] = "PARAMÈTRES",
                ["server_select"] = "Sélectionnez le répertoire du serveur SPTarkov.",
                ["invalid_server_location_caption"] = "Erreur",
                ["invalid_server_location_text"] = "Le chemin sélectionné ne semble pas être un emplacement de serveur SPTarkov. Réessayer?",
                ["no_accounts"] = "Échec de l'obtention de comptes. Pas de comptes?!",
                ["tab_server_location_select"] = "Sélect",
                ["tab_info_title"] = "Information",
                ["tab_quests_title"] = "Quêtes",
                ["tab_settings_title"] = "Paramètres",
                ["tab_settings_lang"] = "Langue",
                ["tab_settings_server"] = "Répertoire du serveur SPTarkov",
                ["tab_settings_account"] = "Compte",
                ["tab_settings_colorscheme"] = "Schéma de couleur",
                ["tab_serversettings_title"] = "Paramètres du serveur"
            };
            File.WriteAllText(Path.Combine(path, "fr.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
        }
    }

    public class PEOptions
    {
        public static string PeoPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PEOptions.json");
        public static PEOptions Load()
        {
            if (!File.Exists(PeoPath))
                return new PEOptions();

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

        public string EftServerPath { get; set; }

        public string Language { get; set; }

        public string DefaultProfile { get; set; }

        public string ColorScheme { get; set; }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(PeoPath, json);
        }
    }

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

    public class AccentItem
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public string Scheme { get; set; }
    }
    /*
    public class AccountInfo
    {
        public string _id { get; set; }
        public string aid { get; set; }
        public string savage { get; set; }

        public Info Info { get; set; }
        public List<Quest> Quests { get; set; }
    }

    public class Info
    {
        public string Nickname { get; set; }
        public string Side { get; set; }
        public string Voice { get; set; }
        public string Experience { get; set; }
        public string GameVersion { get; set; }
    }

    public class Quest
    {
        public string qid { get; set; }
        public string status { get; set; }
    }

    public class QuestInfo
    {
        public string trader { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

    public class QuestLocale
    {
        public string name { get; set; }
    }

    public class TraderLocale
    {
        public string nickname { get; set; }
    }
    */
}
