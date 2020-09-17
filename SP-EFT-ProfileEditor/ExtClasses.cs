using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SP_EFT_ProfileEditor
{
    public class Lang
    {
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
        public static Lang Load(string locale)
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
            //create other langs
            Lang lang = new Lang { locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(path, locale + ".json"))) };
            return lang;
        }

        public Dictionary<string, string> locale { get; set; }

        static void CreateEnLoclae()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["server_select"] = "Select the SPTarkov Server directory. If you cancel this, application will close.",
                ["invalid_server_location_caption"] = "Error",
                ["invalid_server_location_text"] = "The selected path does not seem to be a SPTarkov server location. Try again? If you press No, application will close.",
                ["tab_info_title"] = "Information",
                ["tab_quests_title"] = "Quests",
                ["tab_settings_title"] = "Settings"
            };
            File.WriteAllText(Path.Combine(path, "en.json"), JsonConvert.SerializeObject(locale));
        }

        static void CreateRuLocale()
        {
            Dictionary<string, string> locale = new Dictionary<string, string>
            {
                ["server_select"] = "Выберите папку с сервером SPTarkov. Приложение закроется если вы нажмете отмену.",
                ["invalid_server_location_caption"] = "Ошибка",
                ["invalid_server_location_text"] = "Сервер SPTarkov не найден. Попробовать снова? Приложение закроется если вы нажмете Нет.",
                ["tab_info_title"] = "Информация",
                ["tab_quests_title"] = "Квесты",
                ["tab_settings_title"] = "Настройки"
            };
            File.WriteAllText(Path.Combine(path, "ru.json"), JsonConvert.SerializeObject(locale));
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
                return new PEOptions { Language = "en" };
            }

            return peo;
        }

        public string EftServerPath { get; set; }

        public string Language { get; set; }

        public string DefaultProfile { get; set; }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(PeoPath, json);
        }
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