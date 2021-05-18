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

        [JsonProperty("Customization")]
        public Character_Customization Customization { get; set; }

        [JsonProperty("Inventory")]
        public Character_Inventory Inventory { get; set; }

        [JsonProperty("Skills")]
        public Character_Skills Skills { get; set; }

        [JsonProperty("Encyclopedia")]
        public Dictionary<string, bool> Encyclopedia { get; set; }

        [JsonProperty("Hideout")]
        public Character_Hideout Hideout { get; set; }

        [JsonProperty("Bonuses")]
        public Character_Bonuses[] Bonuses { get; set; }

        [JsonProperty("Quests")]
        public Character_Quests[] Quests { get; set; }

        [JsonProperty("TraderStandings")]
        public Dictionary<string, Character_TraderStandings> TraderStandings { get; set; }

        public List<string> Suits { get; set; }
        public Dictionary<string, WeaponPreset> WeaponPresets { get; set; }


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

            [JsonProperty("GameVersion")]
            public string GameVersion { get; set; }
        }

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

                    [JsonProperty("StackObjectsCount")]
                    public long? StackObjectsCount { get; set; }

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
                        public double HpResource { get; set; }
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
        }

        public class Character_Skills
        {
            [JsonProperty("Common")]
            public Character_Skill[] Common { get; set; }

            [JsonProperty("Mastering")]
            public Character_Skill[] Mastering { get; set; }

            public class Character_Skill
            {
                [JsonProperty("Id")]
                public string Id { get; set; }

                [JsonProperty("Progress")]
                public float Progress { get; set; }
            }
        }

        public class Character_Hideout
        {

            [JsonProperty("Areas")]
            public Character_Hideout_Areas[] Areas { get; set; }
        }

        public class Character_Hideout_Areas
        {
            [JsonProperty("type")]
            public int Type { get; set; } // 0,

            [JsonProperty("level")]
            public int Level { get; set; } // 3,
        }

        public class Character_Bonuses
        {
            [JsonProperty("value")]
            public int Value { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; } // "5d8b89addfc57a648453e89d",

            [JsonProperty("type")]
            public string Type { get; set; } // "StashSize",

            [JsonProperty("templateId")]
            public string TemplateId { get; set; } // "566abbc34bdc2d92178b4576"
        }

        public class Character_Quests
        {
            [JsonProperty("qid")]
            public string Qid { get; set; } // "59674eb386f774539f14813a",

            [JsonProperty("startTime")]
            public long StartTime { get; set; }

            [JsonProperty("completedConditions")]
            public string[] CompletedConditions { get; set; }

            [JsonProperty("statusTimers")]
            public Dictionary<string, decimal> StatusTimers { get; set; }

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
            }
        }

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