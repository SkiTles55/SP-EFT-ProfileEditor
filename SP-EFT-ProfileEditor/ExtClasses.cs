using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SP_EFT_ProfileEditor
{
    public class AccentItem
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Scheme { get; set; }
    }

    public class Quest
    {
        public string qid { get; set; }
        public string trader { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

    public class TraderLocale
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string Nickname { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }

    public class QuestLocale
    {
        public string name { get; set; }
        public string description { get; set; }
        public string location { get; set; }
    }
    
    public class AreaInfo
    {
        public int type { get; set; }
        public Dictionary<string, object> stages { get; set; }
    }
    
    public class CharacterHideoutArea
    {
        public int type { get; set; }
        public string name { get; set; }
        public int MaxLevel { get; set; }
        public int CurrentLevel { get; set; }
    }

    public class SkillInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public int progress { get; set; }
    }

    public class TraderInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public int CurrentLevel { get; set; }

        public List<LoyaltyLevel> Levels { get; set; }
    }

    public class LoyaltyLevel
    {
        public int level { get; set; }
        public long SalesSum { get; set; }
        public float Standing { get; set; }
    }

    public class BackupFile
    {
        public string Path { get; set; }
        public string date { get; set; }
    }
}