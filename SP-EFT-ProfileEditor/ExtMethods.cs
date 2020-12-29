using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace SP_EFT_ProfileEditor
{
    class ExtMethods
    {
        public static bool PathIsEftServerBase(string sptPath)
        {
            if (string.IsNullOrWhiteSpace(sptPath)) return false;
            if (!Directory.Exists(sptPath)) return false;
            if (!File.Exists(Path.Combine(sptPath, "Server.exe"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data", "Server"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data", "Server", "eft-database"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data", "Server", "eft-database", "db"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data", "Server", "eft-database", "db", "locales"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "Aki_Data", "Server", "eft-database", "db", "templates"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, "user"))) return false;

            return true;
        }

        public static bool ServerHaveProfiles(string sptPath)
        {
            if (!Directory.Exists(Path.Combine(sptPath, @"user\profiles"))) return false;
            if (Directory.GetFiles(Path.Combine(sptPath, @"user\profiles")).Count() < 1) return false;
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
    }
}