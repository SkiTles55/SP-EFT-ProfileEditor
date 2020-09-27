using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static SP_EFT_ProfileEditor.Character.Character_Inventory.Character_Inventory_Item;

namespace SP_EFT_ProfileEditor
{
    public class Character
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("aid")]
        public string Aid { get; set; }

        [JsonProperty("savage")]
        public string Savage { get; set; }

        [JsonProperty("Info")]
        public Character_Info Info { get; set; }

        //[JsonProperty("Customization")]
        //public Character_Customization Customization { get; set; }

        //[JsonProperty("Health")]
        //public Character_Health Health { get; set; }

        [JsonProperty("Inventory")]
        public Character_Inventory Inventory { get; set; }

        [JsonProperty("Skills")]
        public Character_Skills Skills { get; set; }

        //[JsonProperty("Stats")]
        //public Character_Stats Stats { get; set; }

        [JsonProperty("Encyclopedia")]
        public Dictionary<string, bool> Encyclopedia { get; set; }

        //[JsonProperty("ConditionCounters")]
        //public Character_ConditionCounters ConditionCounters { get; set; }

        //[JsonProperty("BackendCounters")]
        //public Dictionary<string, IdQidAndValue> BackendCounters { get; set; }

        //[JsonProperty("InsuredItems")]
        //public TidAndItemId[] InsuredItems { get; set; }

        [JsonProperty("Hideout")]
        public Character_Hideout Hideout { get; set; }

        //[JsonProperty("Bonuses")]
        //public Character_Bonuses[] Bonuses { get; set; }

        //[JsonProperty("Notes")]
        //public Character_Notes Notes { get; set; }

        [JsonProperty("Quests")]
        public Character_Quests[] Quests { get; set; }

        [JsonProperty("TraderStandings")]
        public Dictionary<string, Character_TraderStandings> TraderStandings { get; set; }

        //[JsonProperty("RagfairInfo")]
        //public Character_RagfairInfo RagfairInfo { get; set; }

        //[JsonProperty("WishList")]
        //public string[] WishList { get; set; } // item ids


        // Subnode classes

        public class Character_Info
        {
            [JsonProperty("Nickname")]
            public string Nickname { get; set; }

            [JsonProperty("LowerNickname")]
            public string LowerNickname { get; set; }

            [JsonProperty("Side")]
            public string Side { get; set; }

            [JsonProperty("Voice")]
            public string Voice { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("Experience")]
            public long Experience { get; set; }

            //[JsonProperty("RegistrationDate")]
            //public long RegistrationDate { get; set; }

            [JsonProperty("GameVersion")]
            public string GameVersion { get; set; }
            /*
            [JsonProperty("AccountType")]
            public int AccountType { get; set; }
            
            [JsonProperty("MemberCategory")]
            public string MemberCategory { get; set; }

            [JsonProperty("lockedMoveCommands")]
            public bool LockedMoveCommands { get; set; }

            [JsonProperty("SavageLockTime")]
            public long SavageLockTime { get; set; }

            [JsonProperty("LastTimePlayedAsSavage")]
            public long LastTimePlayedAsSavage { get; set; }

            [JsonProperty("Settings")]
            public Character_Info_Settings Settings { get; set; }

            [JsonProperty("NeedWipe")]
            public bool NeedWipe { get; set; }

            [JsonProperty("GlobalWipe")]
            public bool GlobalWipe { get; set; }

            [JsonProperty("NicknameChangeDate")]
            public long NicknameChangeDate { get; set; }

            [JsonProperty("Bans")]
            public string[] Bans { get; set; }

            public class Character_Info_Settings
            {
                [JsonProperty("Role")]
                public string Role { get; set; }

                [JsonProperty("BotDifficulty")]
                public string BotDifficulty { get; set; }

                [JsonProperty("Experience")]
                public long Experience { get; set; }
            }*/
        }
        /*
        public class Character_Customization
        {
            [JsonProperty("Head")]
            public string Head { get; set; }

            [JsonProperty("Body")]
            public string Body { get; set; }

            [JsonProperty("Feet")]
            public string Feet { get; set; }

            [JsonProperty("Hands")]
            public string Hands { get; set; }
        }
        
        public class Character_Health
        {
            [JsonProperty("Hydration")]
            public CurrAndMaxF Hydration { get; set; }

            [JsonProperty("Energy")]
            public CurrAndMaxF Energy { get; set; }

            [JsonProperty("BodyParts")]
            public Character_Health_BodyParts BodyParts { get; set; }

            [JsonProperty("UpdateTime")]
            public long UpdateTime { get; set; }

            public class Character_Health_BodyParts
            {
                [JsonProperty("Head")]
                public Character_Health_BodyParts_Part Head { get; set; }

                [JsonProperty("Chest")]
                public Character_Health_BodyParts_Part Chest { get; set; }

                [JsonProperty("Stomach")]
                public Character_Health_BodyParts_Part Stomach { get; set; }

                [JsonProperty("LeftArm")]
                public Character_Health_BodyParts_Part LeftArm { get; set; }

                [JsonProperty("RightArm")]
                public Character_Health_BodyParts_Part RightArm { get; set; }

                [JsonProperty("LeftLeg")]
                public Character_Health_BodyParts_Part LeftLeg { get; set; }

                [JsonProperty("RightLeg")]
                public Character_Health_BodyParts_Part RightLeg { get; set; }

                public class Character_Health_BodyParts_Part
                {
                    [JsonProperty("Health")]
                    public CurrAndMaxF Health { get; set; }

                    [JsonProperty("Effects")]
                    public Character_Health_BodyParts_Part_Effects Effects { get; set; }

                    public class Character_Health_BodyParts_Part_Effects
                    {
                        [JsonProperty("BreakPart")]
                        public Character_Health_BodyParts_Part_Effects_Breakpart Breakpart { get; set; }

                        public class Character_Health_BodyParts_Part_Effects_Breakpart
                        {
                            [JsonProperty("Time")]
                            public int Time { get; set; }
                        }
                    }
                }
            }
        }*/

        public class Character_Inventory
        {
            [JsonProperty("items")]
            public Character_Inventory_Item[] Items { get; set; }

            [JsonProperty("equipment")]
            public string Equipment { get; set; }

            [JsonProperty("stash")]
            public string Stash { get; set; }

            [JsonProperty("questRaidItems")]
            public string QuestRaidItems { get; set; }

            [JsonProperty("questStashItems")]
            public string QuestStashItems { get; set; }

            //[JsonProperty("fastPanel")]
            //public Character_Inventory_FastPanel FastPanel { get; set; }

            public class Character_Inventory_Item
            {
                [JsonProperty("_id")]
                public string Id { get; set; }

                [JsonProperty("_tpl")]
                public string Tpl { get; set; }

                [JsonProperty("parentId")]
                public string ParentId { get; set; }

                [JsonProperty("slotId")]
                public string SlotId { get; set; } // "main", "hideout", "1", "2", ...

                [JsonProperty("location")]
                public Character_Inventory_Item_Location Location { get; set; }

                [JsonProperty("upd")]
                public Character_Inventory_Item_Upd Upd { get; set; }

                [JsonConverter(typeof(LocationJsonConverter))]
                public class Character_Inventory_Item_Location
                {
                    [JsonIgnore]
                    public int? SimpleNumber { get; set; } = null;

                    [JsonProperty("x")]
                    public int X { get; set; }

                    [JsonProperty("y")]
                    public int Y { get; set; }

                    [JsonProperty("r")]
                    public string R { get; set; } // "Horizontal", "Vertical" or missing

                    [JsonProperty("isSearched")]
                    public bool? IsSearched { get; set; } //test
                }

                public class Character_Inventory_Item_Upd
                {
                    //private int soc = 1;

                    //[JsonProperty("StackObjectsCount")]
                    //public int? StackObjectsCount { get => soc; set => soc = value ?? 1; }

                    [JsonProperty("StackObjectsCount")]
                    public int? StackObjectsCount { get; set; }

                    /// <summary>
                    /// Found in raid?
                    /// </summary>
                    [JsonProperty("SpawnedInSession")]
                    public bool SpawnedInSession { get; set; }

                    public bool ShouldSerializeSpawnedInSession()
                    {
                        return SpawnedInSession;
                    }

                    [JsonProperty("Resource")]
                    public Character_Inventory_Item_Upd_Resource Resource { get; set; }

                    [JsonProperty("Key")]
                    public Character_Inventory_Item_Upd_Key Key { get; set; }

                    [JsonProperty("Foldable")]
                    public Character_Inventory_Item_Upd_Foldable Foldable { get; set; }

                    [JsonProperty("FoodDrink")]
                    public Character_Inventory_Item_Upd_FoodDrink FoodDrink { get; set; }

                    [JsonProperty("Lockable")]
                    public Character_Inventory_Item_Upd_Lockable Lockable { get; set; } //test

                    [JsonProperty("Map")]
                    public Character_Inventory_Item_Upd_Map Map { get; set; } //test

                    [JsonProperty("Repairable")]
                    public Character_Inventory_Item_Upd_Repairable Repairable { get; set; }

                    [JsonProperty("Sight")]
                    public Character_Inventory_Item_Upd_Sight Sight { get; set; }

                    [JsonProperty("MedKit")]
                    public Character_Inventory_Item_Upd_MedKit MedKit { get; set; }

                    [JsonProperty("FireMode")]
                    public Character_Inventory_Item_Upd_FireMode FireMode { get; set; }

                    [JsonProperty("Light")]
                    public Character_Inventory_Item_Upd_Light Light { get; set; }

                    [JsonProperty("Tag")]
                    public Character_Inventory_Item_Upd_Tag Tag { get; set; }

                    [JsonProperty("Togglable")]
                    public Character_Inventory_Item_Upd_Togglable Togglable { get; set; }

                    [JsonProperty("Dogtag")]
                    public Character_Inventory_Item_Upd_Dogtag Dogtag { get; set; }

                    [JsonProperty("FaceShield")]
                    public Character_Inventory_Item_Upd_FaceShield FaceShield { get; set; }

                    public class Character_Inventory_Item_Upd_Resource
                    {
                        [JsonProperty("Value")]
                        public float Value { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_Key
                    {
                        [JsonProperty("NumberOfUsages")]
                        public int NumberOfUsages { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_Foldable
                    {
                        [JsonProperty("Folded")]
                        public bool Folded { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_FoodDrink
                    {
                        [JsonProperty("HpPercent")]
                        public int HpPercent { get; set; } // 1..63
                    }

                    public class Character_Inventory_Item_Upd_Lockable
                    {
                        [JsonProperty("Locked")]
                        public bool Locked { get; set; } // true, false //test
                    }

                    public class Character_Inventory_Item_Upd_Map
                    {
                        [JsonProperty("Markers", DefaultValueHandling = DefaultValueHandling.Include)] //test
                        public object[] Markers { get; set; }  //test
                    }

                    public class Character_Inventory_Item_Upd_Repairable
                    {
                        [JsonProperty("MaxDurability")]
                        public float MaxDurability { get; set; }

                        [JsonProperty("Durability")]
                        public float Durability { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_Sight
                    {
                        [JsonProperty("ScopesCurrentCalibPointIndexes")]
                        public int[] ScopesCurrentCalibPointIndexes { get; set; }

                        [JsonProperty("ScopesSelectedModes")]
                        public int[] ScopesSelectedModes { get; set; }

                        [JsonProperty("SelectedSightMode")] //test SelectedScope
                        public int SelectedSightMode { get; set; }

                        [JsonProperty("SelectedScope")] //test SelectedScope
                        public int SelectedScope { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_MedKit
                    {
                        [JsonProperty("HpResource")]
                        public int HpResource { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_FireMode
                    {
                        [JsonProperty("FireMode")]
                        public string FireMode { get; set; } // "fullauto", "single"
                    }

                    public class Character_Inventory_Item_Upd_Light
                    {
                        [JsonProperty("IsActive")]
                        public bool IsActive { get; set; }

                        [JsonProperty("SelectedMode")]
                        public int SelectedMode { get; set; } // 0..2
                    }

                    public class Character_Inventory_Item_Upd_Tag
                    {
                        [JsonProperty("Color")]
                        public int Color { get; set; } // 0..?

                        [JsonProperty("Name")]
                        public string Name { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_Togglable
                    {
                        [JsonProperty("On")]
                        public bool On { get; set; }
                    }

                    public class Character_Inventory_Item_Upd_Dogtag
                    {
                        [JsonProperty("Nickname")]
                        public string Nickname { get; set; }

                        [JsonProperty("Side")]
                        public string Side { get; set; } // "Bear", "Used"

                        [JsonProperty("Level")]
                        public int Level { get; set; }

                        [JsonProperty("Time")]
                        public DateTime Time { get; set; }

                        [JsonProperty("Status")]
                        public string Status { get; set; } // "Killed by"

                        [JsonProperty("KillerName")]
                        public string KillerName { get; set; }

                        [JsonProperty("WeaponName")]
                        public string WeaponName { get; set; } // "5447a9cd4bdc2dbd208b4567 Name" (id, propertyname?)
                    }

                    public class Character_Inventory_Item_Upd_FaceShield
                    {
                        [JsonProperty("Hits")]
                        public int Hits { get; set; }

                        [JsonProperty("HitSeed")]
                        public int HitSeed { get; set; } // 64?!
                    }
                }
            }
            /*
            public class Character_Inventory_FastPanel 
            {
                [JsonProperty("Item4")] //test
                public string Item4 { get; set; }

                [JsonProperty("Item5")]
                public string Item5 { get; set; }

                [JsonProperty("Item6")]
                public string Item6 { get; set; }

                [JsonProperty("Item7")]
                public string Item7 { get; set; }

                [JsonProperty("Item8")]
                public string Item8 { get; set; }

                [JsonProperty("Item9")]
                public string Item9 { get; set; }

                [JsonProperty("Item0")]
                public string Item0 { get; set; }
            }*/
        }

        public class Character_Skills
        {
            [JsonProperty("Common")]
            public Character_Skills_Common[] Common { get; set; }

            [JsonProperty("Mastering")]
            public Character_Skills_Mastering[] Mastering { get; set; }

            //[JsonProperty("Bonuses")]
            //public object Bonuses { get; set; } // ?!

            //[JsonProperty("Points")]
            //public int Points { get; set; } // 0?

            public class Character_Skills_Common
            {
                [JsonProperty("Id")]
                public string Id { get; set; }

                [JsonProperty("Progress")]
                public float Progress { get; set; }

                //[JsonProperty("PointsEarnedDuringSession")]
                //public float PointsEarnedDuringSession { get; set; }

                //[JsonProperty("LastAccess")]
                //public long LastAccess { get; set; }
            }

            public class Character_Skills_Mastering
            {
                [JsonProperty("Id")]
                public string Id { get; set; }

                [JsonProperty("Progress")]
                public float Progress { get; set; }
            }
        }
        /*
        public class Character_Stats
        {
            [JsonProperty("SessionCounters")]
            public Character_Stats_Counters SessionCounters { get; set; }

            [JsonProperty("OverallCounters")]
            public Character_Stats_Counters OverallCounters { get; set; }

            [JsonProperty("SessionExperienceMult")]
            public float SessionExperienceMult { get; set; } // 1.5

            [JsonProperty("SurvivorClass")]
            public string SurvivorClass { get; set; } //test

            [JsonProperty("TotalInGameTime")]
            public long? TotalInGameTime { get; set; } // 0, null //test

            [JsonProperty("ExperienceBonusMult")]
            public float ExperienceBonusMult { get; set; } // 1

            [JsonProperty("TotalSessionExperience")]
            public float TotalSessionExperience { get; set; } // 0

            [JsonProperty("LastSessionDate")]
            public long LastSessionDate { get; set; } // 1598814193

            [JsonProperty("Aggressor")]
            public Character_Stats_Aggressor Aggressor { get; set; }

            [JsonProperty("DroppedItems")]
            public Character_Stats_DroppedItems[] DroppedItems { get; set; }

            [JsonProperty("FoundInRaidItems")]
            public Character_Stats_FoundInRaidItems[] FoundInRaidItems { get; set; }

            [JsonProperty("Victims")]
            public Character_Stats_Victims[] Victims { get; set; }

            [JsonProperty("CarriedQuestItems")]
            public string[] CarriedQuestItems { get; set; } // item ids

            [JsonProperty("DamageHistory")]
            public Character_Stats_DamageHistory DamageHistory { get; set; }

            public class Character_Stats_Counters
            {
                [JsonProperty("Items")]
                public Character_Stats_Counters_Items[] Items { get; set; }

                public class Character_Stats_Counters_Items
                {
                    [JsonProperty("Key")]
                    public string[] Key { get; set; }

                    [JsonProperty("Value")]
                    public int Value { get; set; }
                }
            }

            public class Character_Stats_Aggressor
            {
                [JsonProperty("Id")]
                public string Id { get; set; }

                [JsonProperty("Name")]
                public string Name { get; set; }

                [JsonProperty("Side")]
                public string Side { get; set; } // "Savage", ...?

                [JsonProperty("BodyPart")]
                public string BodyPart { get; set; } // "LeftLeg", ...

                [JsonProperty("HeadSegment")]
                public string HeadSegment { get; set; } // "Top", ...

                [JsonProperty("WeaponName")]
                public string WeaponName { get; set; } // "56dee2bdd2720bc8328b4567 ShortName" - (id propname?)

                [JsonProperty("Category")]
                public string Category { get; set; } // "Default"
            }

            public class Character_Stats_DroppedItems
            {
                [JsonProperty("QuestId")]
                public string QuestId { get; set; }

                [JsonProperty("ItemId")]
                public string ItemId { get; set; }

                [JsonProperty("ZoneId")]
                public string ZoneId { get; set; }
            }

            public class Character_Stats_FoundInRaidItems
            {
                // what goes here?
            }

            public class Character_Stats_Victims
            {
                [JsonProperty("Name")]
                public string Name { get; set; }

                [JsonProperty("Side")]
                public string Side { get; set; } // "Bear", "Usec", "Savage"

                [JsonProperty("Time")]
                public string Time { get; set; } // "00:01:17.5130000"

                [JsonProperty("Level")]
                public int Level { get; set; }

                [JsonProperty("BodyPart")]
                public string BodyPart { get; set; } // "Head", "Chest"

                [JsonProperty("Weapon")]
                public string Weapon { get; set; } // "5447a9cd4bdc2dbd208b4567 ShortName" (id propertyname)
            }

            public class Character_Stats_DamageHistory
            {
                [JsonProperty("LethalDamagePart")]
                public string LethalDamagePart { get; set; } // "Common",

                [JsonProperty("LethalDamage")]
                public Character_Stats_DamageHistory_LethalDamage LethalDamage { get; set; } // null,

                [JsonProperty("BodyParts")]
                public Character_Stats_DamageHistory_BodyParts BodyParts { get; set; }

                [JsonProperty("TotalInGameTime")]
                public long? TotalInGameTime { get; set; } // 0, null //test

                [JsonProperty("SurvivorClass")]
                public string SurvivorClass { get; set; } // "Neutralizer"

                public class Character_Stats_DamageHistory_LethalDamage
                {
                    [JsonProperty("Amount")]
                    public float Amount { get; set; } // 8.00003052

                    [JsonProperty("Type")]
                    public string Type { get; set; } // "Bloodloss", "Bullet"

                    [JsonProperty("SourceId")]
                    public string SourceId { get; set; } // null, ammo id

                    [JsonProperty("OverDamageFrom")]
                    public string OverDamageFrom { get; set; } // null ??

                    [JsonProperty("Blunt")]
                    public bool Blunt { get; set; }

                    [JsonProperty("ImpactsCount")]
                    public int ImpactsCount { get; set; } // > 0
                }

                public class Character_Stats_DamageHistory_BodyParts
                {
                    [JsonProperty("Head")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] Head { get; set; }

                    [JsonProperty("Chest")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] Chest { get; set; }

                    [JsonProperty("Stomach")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] Stomach { get; set; }

                    [JsonProperty("LeftArm")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] LeftArm { get; set; }

                    [JsonProperty("RightArm")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] RightArm { get; set; }

                    [JsonProperty("LeftLeg")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] LeftLeg { get; set; }

                    [JsonProperty("RightLeg")]
                    public Character_Stats_DamageHistory_BodyParts_Damage[] RightLeg { get; set; }

                    public class Character_Stats_DamageHistory_BodyParts_Damage
                    {
                        [JsonProperty("Amount")]
                        public float Amount { get; set; } // 8.00003052

                        [JsonProperty("Type")]
                        public string Type { get; set; } // "Bloodloss", "Bullet"

                        [JsonProperty("SourceId")]
                        public string SourceId { get; set; } // null, ammo id

                        [JsonProperty("OverDamageFrom")]
                        public string OverDamageFrom { get; set; } // null ??

                        [JsonProperty("Blunt")]
                        public bool Blunt { get; set; }

                        [JsonProperty("ImpactsCount")]
                        public int ImpactsCount { get; set; } // > 0
                    }
                }
            }
        }*/
        /*
        public class Character_ConditionCounters
        {
            [JsonProperty("Counters")]
            public IdAndValue[] Counters { get; set; }
        }*/

        public class Character_Hideout
        {
            //[JsonProperty("Production")]
            //public object Production { get; set; } // ?!

            [JsonProperty("Areas")]
            public Character_Hideout_Areas[] Areas { get; set; }
        }

        public class Character_Hideout_Areas
        {
            [JsonProperty("type")]
            public int Type { get; set; } // 0,

            [JsonProperty("level")]
            public int Level { get; set; } // 3,
            /*
            [JsonProperty("active")]
            public bool Active { get; set; } // true,

            [JsonProperty("passiveBonusesEnabled")]
            public bool PassiveBonusesEnabled { get; set; } // true,

            [JsonProperty("completeTime")]
            public int CompleteTime { get; set; } // 0,

            [JsonProperty("constructing")]
            public bool Constructing { get; set; } // false,

            [JsonProperty("slots")]
            public Character_Hideout_Areas_Slots[] Slots { get; set; } // []

            public class Character_Hideout_Areas_Slots
            {
                [JsonProperty("item")] //notwork
                public Character_Inventory.Character_Inventory_Item[] Item { get; set; }
            }*/
        }
        /*
        public class Character_Bonuses
        {
            [JsonProperty("value")]
            public int Value { get; set; } // 2 
            public bool ShouldSerializeValue() => Type != "StashSize";

            [JsonProperty("passive")]
            public bool Passive { get; set; } // true
            public bool ShouldSerializePassive() => Type != "StashSize";

            [JsonProperty("production")]
            public bool Production { get; set; } // false
            public bool ShouldSerializeProduction() => Type != "StashSize";

            [JsonProperty("visible")]
            public bool Visible { get; set; } // true
            public bool ShouldSerializeVisible() => Type != "StashSize";

            [JsonProperty("filter")]
            public string[] Filter { get; set; } // [ "id" ],
            public bool ShouldSerializeFilter() => /*(Type != "StashSize") &&*/ //(Filter != null);
        /*
            [JsonProperty("id")]
            public string Id { get; set; } // "5d8b89addfc57a648453e89d",
            public bool ShouldSerializeId() => /*(Type != "StashSize") &&*/ //(Id != null);
        /*
            [JsonProperty("icon")]
            public string Icon { get; set; } // "/files/Hideout/icon_hideout_fuelslots.png",
            public bool ShouldSerializeIcon() => /*(Type != "StashSize") &&*/ //(Icon != null);
        /*
            [JsonProperty("skillType")]
            public string SkillType { get; set; } // "Physical",
            public bool ShouldSerializeSkillType() => /*(Type != "StashSize") &&*/ //(SkillType != null);
            /*
            [JsonProperty("type")]
            public string Type { get; set; } // "StashSize",

            [JsonProperty("templateId")]
            public string TemplateId { get; set; } // "566abbc34bdc2d92178b4576"
        }*/
        /*
        public class Character_Notes
        {
            [JsonProperty("Notes")]
            public Character_Notes_Item[] Notes { get; set; }

            public class Character_Notes_Item
            {
                [JsonProperty("Time")]
                public long Time { get; set; } // 1598992000,

                [JsonProperty("Text")]
                public string Text { get; set; } // "A sample note"
            }
        }*/

        public class Character_Quests
        {
            [JsonProperty("qid")]
            public string Qid { get; set; } // "59674eb386f774539f14813a",

            //[JsonProperty("startTime")]
            //public long StartTime { get; set; } // 1596824527,

            //[JsonProperty("completedConditions")]
            //public string[] CompletedConditions { get; set; } // [
                                                              //	"59a926c386f7747bbc027ac8",
                                                              //	"",
                                                              //	"5968929e86f7740d121082d3",
                                                              //	"59674fe586f7744f4e358aa2",
                                                              //	"5977784486f774285402cf52"
                                                              //],

            //[JsonProperty("statusTimers")]
            //public Dictionary<string, decimal> StatusTimers { get; set; } // {
                                                                          //	"1": 1596824523.379,
                                                                          //	"2": 1596826424.135,
                                                                          //	"3": 1596827807.145,
                                                                          //	"4": 1598813900.368
                                                                          //},

            [JsonProperty("status")]
            public string Status { get; set; } // "Success", "Locked", "Started"
        }

        public class Character_TraderStandings
        {
            [JsonProperty("currentLevel")]
            public int CurrentLevel { get; set; } // 4,

            [JsonProperty("currentSalesSum")]
            public long CurrentSalesSum { get; set; } // 3474588,

            [JsonProperty("currentStanding")]
            public float CurrentStanding { get; set; } // 2.52,

            //[JsonProperty("NextLoyalty")]
            //public object NextLoyalty { get; set; } // null,
            //public bool ShouldSerializeNextLoyalty() => true;

            [JsonProperty("loyaltyLevels")]
            public Dictionary<string, Character_TraderStandings_LoyaltyLevels> LoyaltyLevels { get; set; }

            [JsonProperty("display")]
            public bool Display { get; set; } // true

            public class Character_TraderStandings_LoyaltyLevels
            {
                [JsonProperty("minLevel")]
                public int MinLevel { get; set; } // 22,

                [JsonProperty("minSalesSum")]
                public long MinSalesSum { get; set; } // 1500000,

                [JsonProperty("minStanding")]
                public float MinStanding { get; set; } // 0.35

                //[JsonProperty("heal_price_coef")]
                //public float heal_price_coef { get; set; } // test //serialize only if trader id is '54cb57776803fa99248b456e'
            }
        }
        /*
        public class Character_RagfairInfo
        {
            [JsonProperty("rating")]
            public float Rating { get; set; } // 0.2,

            [JsonProperty("isRatingGrowing")]
            public bool IsRatingGrowing { get; set; } // true,

            [JsonProperty("offers")]
            public string[] Offers { get; set; } // [] maybe offer type..
        }*/


        // Helpers

        public class CurrAndMaxI
        {
            [JsonProperty("Current")]
            public int Current { get; set; }

            [JsonProperty("Maximum")]
            public int Maximum { get; set; }
        }

        public class CurrAndMaxF
        {
            [JsonProperty("Current")]
            public float Current { get; set; }

            [JsonProperty("Maximum")]
            public float Maximum { get; set; }

            public static CurrAndMaxF operator +(CurrAndMaxF a, CurrAndMaxF b)
            {
                return new CurrAndMaxF { Current = a.Current + b.Current, Maximum = a.Maximum + b.Maximum };
            }
        }
        /*
        public class IdAndValue
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("value")]
            public int Value { get; set; }
        }*/

        public class IdQidAndValue
        {
            [JsonProperty("id")]
            public string Id { get; set; } // "596737cb86f77463a8115efd",

            [JsonProperty("qid")]
            public string Qid { get; set; } // "5936d90786f7742b1420ba5b",

            [JsonProperty("value")]
            public int Value { get; set; } // 2
        }

        public class TidAndItemId
        {
            [JsonProperty("tid")]
            public string Tid { get; set; } // Trader ID

            [JsonProperty("itemId")]
            public string ItemId { get; set; }
        }
    }

    public class LocationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                JObject jo = JObject.Load(reader);

                Character_Inventory_Item_Location loc = new Character_Inventory_Item_Location();
                serializer.Populate(jo.CreateReader(), loc);

                return loc;
            }
            else if (reader.TokenType == JsonToken.Integer)
                return new Character_Inventory_Item_Location { SimpleNumber = Convert.ToInt32(reader.Value) };
            else
                return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Character_Inventory_Item_Location ciil)
            {
                if (ciil.SimpleNumber.HasValue)
                    writer.WriteValue(ciil.SimpleNumber.Value);
                else
                    serializer.Serialize(writer, new { x = ciil.X, y = ciil.Y, r = ciil.R, isSearched = ciil.IsSearched });
            }
            else
                throw new NotImplementedException();
        }
    }
}