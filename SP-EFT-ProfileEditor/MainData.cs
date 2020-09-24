using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SP_EFT_ProfileEditor
{
    public class MainData
    {
        private static string LangPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
        private static string PeoPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PEOptions.json");
        public static MainData Load()
        {
            if (!Directory.Exists(LangPath))
                Directory.CreateDirectory(LangPath);
            if (!File.Exists(Path.Combine(LangPath, "en.json")))
                CreateEnLoclae();
            if (!File.Exists(Path.Combine(LangPath, "ru.json")))
                CreateRuLocale();
            if (!File.Exists(Path.Combine(LangPath, "fr.json")))
                CreateFrLocale();
            if (!File.Exists(Path.Combine(LangPath, "ge.json")))
                CreateGeLocale();
            PEOptions eOptions = CreateOptions();
            if (string.IsNullOrEmpty(eOptions.Language))
                eOptions.Language = "en";
            MainData lang = new MainData { locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(LangPath, eOptions.Language + ".json"))), options = eOptions };
            if (!string.IsNullOrEmpty(eOptions.EftServerPath) && Directory.Exists(eOptions.EftServerPath))
            {
                lang.Profiles = Directory.GetDirectories(eOptions.EftServerPath + "\\user\\profiles").Where(x => File.Exists(x + "\\character.json")).Select(x => new DirectoryInfo(x).Name).ToList();
                if (lang.Profiles.Count > 0 && (string.IsNullOrEmpty(eOptions.DefaultProfile) || !lang.Profiles.Contains(eOptions.DefaultProfile)))
                    eOptions.DefaultProfile = lang.Profiles.FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(eOptions.EftServerPath) && !string.IsNullOrEmpty(eOptions.DefaultProfile))
                lang.Character = JsonConvert.DeserializeObject<Character>(File.ReadAllText(Path.Combine(eOptions.EftServerPath, "user\\profiles", eOptions.DefaultProfile, "character.json")));
            return lang;
        }

        private static PEOptions CreateOptions()
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

        static void CreateEnLoclae()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
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
                ["tab_info_bigpockets"] = "Big pockets",
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
                ["tab_serversettings_title"] = "Server settings"
            };
            File.WriteAllText(Path.Combine(LangPath, "en.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
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
                ["tab_info_bigpockets"] = "Große Taschen",
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
                ["tab_serversettings_title"] = "Server einstellungen"
            };
            File.WriteAllText(Path.Combine(LangPath, "ge.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
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
                ["tab_info_bigpockets"] = "Большие карманы",
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
                ["tab_serversettings_title"] = "Настройки сервера"
            };
            File.WriteAllText(Path.Combine(LangPath, "ru.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
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
                ["tab_info_bigpockets"] = "Grandes poches",
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
                ["tab_serversettings_title"] = "Paramètres du serveur"
            };
            File.WriteAllText(Path.Combine(LangPath, "fr.json"), JsonConvert.SerializeObject(locale, Formatting.Indented));
        }
    }

    public class PEOptions
    {
        public string EftServerPath { get; set; }

        public string Language { get; set; }

        public string DefaultProfile { get; set; }

        public string ColorScheme { get; set; }
    }
}