using Asset;
using AssetBundleFramework;
using Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Utilities;

public class DataManager : Singleton<DataManager>
{
    public static bool Editor;
    public string DataPath;
    public Dictionary<int, MapDefine> Maps = null;
    public Dictionary<int, CharacterDefine> Characters = null;
    public Dictionary<int, TeleporterDefine> Teleporters = null;
    public Dictionary<int, NPCDefine> NPCs = null;
    public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
    public Dictionary<int, Dictionary<int, SpawnRuleDefine>> SpawnRules = null;
    public Dictionary<int, ItemDefine> Items = null;
    public Dictionary<int, ShopDefine> Shops = null;
    public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
    public Dictionary<int, EquipDefine> Equips = null;
    public Dictionary<int, QuestDefine> Quests = null;
    public Dictionary<int, RideDefine> Rides = null;
    public Dictionary<int, Dictionary<int, SkillDefine>> Skills = null;
    public Dictionary<int, BuffDefine> Buffs = null;

    /// <summary>
    /// 配置注册列表，由 GameEntry 遍历使用
    /// </summary>
    public List<(string name, string fileName, Action<string> setter)> Configs { get; private set; }

    public DataManager()
    {
        if (Editor)
        {
            this.DataPath = "Data/";
        }
        else
        {
            this.DataPath = $"Assets/AssetBundle/Data/";
        }

        this.Configs = new()
        {
            ("MapDefine",       "MapDefine.txt",       json => this.Maps          =     JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json)),
            ("CharacterDefine", "CharacterDefine.txt", json => this.Characters    =     JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json)),
            ("TeleporterDefine","TeleporterDefine.txt",json => this.Teleporters   =     JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json)),
            ("NPCDefine",       "NPCDefine.txt",       json => this.NPCs          =     JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json)),
            ("SpawnPointDefine","SpawnPointDefine.txt",json => this.SpawnPoints   =     JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json)),
            ("SpawnRuleDefine", "SpawnRuleDefine.txt", json => this.SpawnRules    =     JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json)),
            ("ItemDefine",      "ItemDefine.txt",      json => this.Items         =     JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json)),
            ("ShopDefine",      "ShopDefine.txt",      json => this.Shops         =     JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json)),
            ("ShopItemDefine",  "ShopItemDefine.txt",  json => this.ShopItems     =     JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json)),
            ("EquipDefine",     "EquipDefine.txt",     json => this.Equips        =     JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json)),
            ("QuestDefine",     "QuestDefine.txt",     json => this.Quests        =     JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json)),
            ("RideDefine",      "RideDefine.txt",      json => this.Rides         =     JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json)),
            ("SkillDefine",     "SkillDefine.txt",     json => this.Skills        =     JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SkillDefine>>>(json)),
            ("BuffDefine",      "BuffDefine.txt",      json => this.Buffs         =     JsonConvert.DeserializeObject<Dictionary<int, BuffDefine>>(json)),
        };

        LogHelper.Log("DataManager > DataManager()", LogUser.DataManager);
    }

    /// <summary>
    /// 从 AssetBundle 同步加载单个配置文件，返回 json 字符串（真机模式）
    /// </summary>
    public string LoadJsonFromBundle(string fileName)
    {
        IResource res = Resloader.Instance.LoadAssetSync(this.DataPath + fileName);
        return res.GetAsset<TextAsset>().text;
    }

    /// <summary>
    /// 从文件系统读取单个配置文件，返回 json 字符串（编辑器模式）
    /// </summary>
    public string LoadJsonFromFile(string fileName)
    {
        return File.ReadAllText(this.DataPath + fileName);
    }

#if UNITY_EDITOR

    public void Load()
    {
        this.DataPath = "Data/";
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "NPCDefine.txt");
        this.NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

        json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

        json = File.ReadAllText(this.DataPath + "SpawnRuleDefine.txt");
        this.SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "RideDefine.txt");
        this.Rides = JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json);

        json = File.ReadAllText(this.DataPath + "SkillDefine.txt");
        this.Skills = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SkillDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "BuffDefine.txt");
        this.Buffs = JsonConvert.DeserializeObject<Dictionary<int, BuffDefine>>(json);
    }

    public void SaveTeleporters()
    {
        string json = JsonConvert.SerializeObject(this.Teleporters, Formatting.Indented);
        Debug.Log(json);
        File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
    }

    public void SaveSpawnPoints()
    {
        string json = JsonConvert.SerializeObject(this.SpawnPoints, Formatting.Indented);
        File.WriteAllText(this.DataPath + "SpawnPointDefine.txt", json);
    }
#endif
}
