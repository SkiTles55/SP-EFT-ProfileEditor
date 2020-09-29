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
            if (!Directory.Exists(Path.Combine(sptPath, "db"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"db\locales"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"user\configs"))) return false;

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
    }
}