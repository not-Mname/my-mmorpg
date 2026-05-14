using Asset;
using AssetBundleFramework;
using Common.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
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
        LogHelper.Log("DataManager > DataManager()", LogUser.DataManager);
    }

    public IEnumerator Load(UI.Common.ProgressBar progressBar, TextMeshProUGUI progressText)
    {
        float step = 100f / 14f;
        progressText.text = "加载配置数据...";
        progressBar.SetData(100, 0, 1);
        var wait = new WaitForSeconds(0.1f);

        IResource res = Resloader.Instance.LoadAssetSync(this.DataPath + "MapDefine.txt");
        string json = res.GetAsset<TextAsset>().text;
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "CharacterDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "TeleporterDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "NPCDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "SpawnPointDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "SpawnRuleDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "ItemDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "ShopDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "ShopItemDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "EquipDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "QuestDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "RideDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Rides = JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "SkillDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Skills = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SkillDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        res = Resloader.Instance.LoadAssetSync(this.DataPath + "BuffDefine.txt");
        json = res.GetAsset<TextAsset>().text;
        this.Buffs = JsonConvert.DeserializeObject<Dictionary<int, BuffDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressBar.UpdateProgress();
    }

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

    public IEnumerator LoadDataEditor(UI.Common.ProgressBar progressBar, TextMeshProUGUI progressText)
    {
        float step = 100f / 14f;
        progressBar.SetData(100, 0, 5);

        var wait = new WaitForSeconds(0.1f);

        progressText.text = "加载 MapDefine...";
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 CharacterDefine...";
        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 TeleporterDefine...";
        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 SpawnPointDefine...";
        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 NPCDefine...";
        json = File.ReadAllText(this.DataPath + "NPCDefine.txt");
        this.NPCs = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 ItemDefine...";
        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 ShopDefine...";
        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 ShopItemDefine...";
        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 EquipDefine...";
        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 QuestDefine...";
        json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 SpawnRuleDefine...";
        json = File.ReadAllText(this.DataPath + "SpawnRuleDefine.txt");
        this.SpawnRules = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnRuleDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 RideDefine...";
        json = File.ReadAllText(this.DataPath + "RideDefine.txt");
        this.Rides = JsonConvert.DeserializeObject<Dictionary<int, RideDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 SkillDefine...";
        json = File.ReadAllText(this.DataPath + "SkillDefine.txt");
        this.Skills = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SkillDefine>>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressText.text = "加载 BuffDefine...";
        json = File.ReadAllText(this.DataPath + "BuffDefine.txt");
        this.Buffs = JsonConvert.DeserializeObject<Dictionary<int, BuffDefine>>(json);
        progressBar.CurrentValue += step;
        yield return wait;

        progressBar.UpdateProgress();
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

}
