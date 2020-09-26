using System.IO;

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
            if (!Directory.Exists(Path.Combine(sptPath, @"db\items"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"db\locales"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"user\configs"))) return false;
            if (!File.Exists(Path.Combine(sptPath, @"user\configs\accounts.json"))) return false;
            if (!Directory.Exists(Path.Combine(sptPath, @"user\profiles"))) return false;

            return true;
        }
    }
}