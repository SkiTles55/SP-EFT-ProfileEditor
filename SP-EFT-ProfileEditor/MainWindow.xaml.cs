using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace SP_EFT_ProfileEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PEOptions Options { get; set; }
        Lang Lang = new Lang();

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSPT;

        //public static Dictionary<string, string> QuestNames { get; private set; } = new Dictionary<string, string>();
        //public static Dictionary<string, string> TraderNames { get; private set; } = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            /*
            var Profiles = Directory.GetDirectories(ServerPath + "\\user\\profiles");
            //check for many profiles
            AccountInfo Profile = JsonConvert.DeserializeObject<AccountInfo>(File.ReadAllText(Profiles.FirstOrDefault() + "\\character.json"));
            QuestNames = new Dictionary<string, string>();
            foreach (var qn in Directory.GetFiles(ServerPath + "\\db\\locales\\ru\\quest"))
                QuestNames.Add(Path.GetFileNameWithoutExtension(qn), JsonConvert.DeserializeObject<QuestLocale>(File.ReadAllText(qn)).name);
            TraderNames = new Dictionary<string, string>();
            foreach (var tn in  Directory.GetDirectories(ServerPath + "\\db\\assort"))
                TraderNames.Add(Path.GetFileName(tn), JsonConvert.DeserializeObject<TraderLocale>(File.ReadAllText(tn + "\\base.json")).nickname);
            foreach (var q in Profile.Quests)
            {
                try {
                    questsGrid.Items.Add(new QuestInfo { status = q.status, trader = TraderNames[q.qid], name = QuestNames[q.qid] });
                }
                catch { }
            //need to find names in db quests folder
            }
            */        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Options = PEOptions.Load();
            if (string.IsNullOrWhiteSpace(Options.Language))
                ChooseLanguage();
            Lang = Lang.Load(Options.Language);
            DataContext = Lang.locale;
            if (string.IsNullOrWhiteSpace(Options.EftServerPath) || !Directory.Exists(Options.EftServerPath))
                if (!ChooseEftServerDir())
                    System.Windows.Application.Current.Shutdown();
        }

        private bool ChooseEftServerDir()
        {
            folderBrowserDialogSPT = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = Lang.locale["server_select"],
                RootFolder = System.Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false
            };            
            bool pathOK = false;
            do
            {
                if (!string.IsNullOrWhiteSpace(Options.EftServerPath) && Directory.Exists(Options.EftServerPath))
                    folderBrowserDialogSPT.SelectedPath = Options.EftServerPath;
                if (folderBrowserDialogSPT.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;
                if (PathIsEftServerBase(folderBrowserDialogSPT.SelectedPath)) pathOK = true;
            } while (
                !pathOK &&
                (System.Windows.Forms.MessageBox.Show(Lang.locale["invalid_server_location_text"], Lang.locale["invalid_server_location_caption"], System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
            );

            if (pathOK)
            {
                Options.EftServerPath = folderBrowserDialogSPT.SelectedPath;
                Options.Save();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }

            return pathOK;
        }

        private bool PathIsEftServerBase(string sptPath)
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

        private void ChooseLanguage()
        {
            //Options.Language = LanguageSelectWindows.ShowDialog(this, AvailableLanguages, Options.Language);
            LanguageSelectWindows languageSelect = new LanguageSelectWindows();
            languageSelect.ShowDialog();
            Options.Language = languageSelect.SelectedLang;
            Options.Save();
        }
    }
}
