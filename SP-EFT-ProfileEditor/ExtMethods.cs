using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SP_EFT_ProfileEditor
{
    class ExtMethods
    {
        public static List<long> ExpTable = new List<long>  { 0, 1000,
                2743,3999,5256,6494,7658,8851,10025,11098,12226,13336,16814,19924,23053,26283,29219,32045,34466,37044
                ,39162,41492,44002,46900,51490,56080,60670,65260,69850,74440,79030,83620,90964,98308,105652,112996,120340
                ,127684,135028,142372,149716,157060,167158,177256,187354,197452,207550,217648,227746,237844,247942,258040
                ,271810,285580,299350,313120,323450,362111,369536,386978,407174,430124,457664,494384,549464,622904,760604
                ,1036004,1449104,10000000 };

        public static bool PathIsEftServerBase(PEOptions options, string path = null)
        {
            if (string.IsNullOrEmpty(path)) path = options.EftServerPath;
            if (string.IsNullOrEmpty(path)) return false;
            if (!Directory.Exists(path)) return false;
            if (options.FilesList.Any(x => !File.Exists(Path.Combine(path, x.Value)))) return false;
            if (options.DirsList.Any(x => !Directory.Exists(Path.Combine(path, x.Value)))) return false;

            return true;
        }

        public static bool ServerHaveProfiles(PEOptions options)
        {
            if (string.IsNullOrEmpty(options.EftServerPath)) return false;
            if (!Directory.Exists(Path.Combine(options.EftServerPath, options.DirsList["dir_profiles"]))) return false;
            if (Directory.GetFiles(Path.Combine(options.EftServerPath, options.DirsList["dir_profiles"])).Count() < 1) return false;
            return true;
        }

        public static void Log(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Logs/";
            VerifyDir();
            string fileName = "log" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            try
            {
                StreamWriter file = new StreamWriter(path + fileName, true);
                file.WriteLine(DateTime.Now.ToString() + ": " + text);
                file.Close();
            }
            catch (Exception) { }

            void VerifyDir()
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                        dir.Create();
                }
                catch { }
            }
        }

        public static string generateNewId()
        {
            var getTime = DateTime.Now;
            Random rnd = new Random();
            var random = rnd.Next(100000000, 999999999).ToString();
            var retVal = $"I{getTime:MM}{getTime:dd}{getTime:HH}{getTime:mm}{getTime:ss}{random}";
            var sign = makeSign(24 - retVal.Count()).ToString();
            return retVal + sign;
        }

        private static string makeSign(int length)
        {
            var result = "";
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var charactersLength = characters.Count();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result += characters.ElementAt((int)Math.Floor(random.NextDouble() * charactersLength));
            }
            return result;
        }

        public static JObject RemoveNullAndEmptyProperties(JObject jObject)
        {
            while (jObject.Descendants().Any(jt => jt.Type == JTokenType.Property && (jt.Values().All(a => a.Type == JTokenType.Null) || !jt.Values().Any())))
                foreach (var jt in jObject.Descendants().Where(jt => jt.Type == JTokenType.Property && (jt.Values().All(a => a.Type == JTokenType.Null) || !jt.Values().Any())).ToArray())
                    jt.Remove();
            return jObject;
        }

        public static bool ProfileChanged(MainData data) => data.ProfileHash != 0 && data.ProfileHash != JsonConvert.SerializeObject(data.Character).ToString().GetHashCode();

        public static string GetWindowsCulture()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            switch (culture.Name)
            {
                case "de-DE":
                    return "ge";
                case "fr-FR":
                    return "fr";
                case "ru-RU":
                    return "ru";
                default:
                    return "en";
            }
        }

        public static void SetExpTable(List<long> value)
        {
            ExpTable = value;
        }
    }
}