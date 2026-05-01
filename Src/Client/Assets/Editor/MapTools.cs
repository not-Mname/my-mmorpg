using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Common.Data;
using SkillBridge.Message;
using UnityEngine.AI;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")]//意思是按钮在菜单栏中显示为“Map Tools/Export Teleporters”
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene currentScene = EditorSceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (currentScene.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请保存场景", "确定");
            return;
        }

        List<TeleporterGameObject> teleporters = new List<TeleporterGameObject>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Resources/Levels/" + map.Value.Resource + ".unity";

            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.Log("场景文件不存在：" + sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterGameObject[] tg = GameObject.FindObjectsByType<TeleporterGameObject>(FindObjectsSortMode.None);
            foreach (var t in tg)
            {
                Debug.LogFormat("id : {0}", t.id);
                if (!DataManager.Instance.Teleporters.ContainsKey(t.id))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中的TeleporterGameObject未注册，ID为{1}", map.Key, t.id), "确定");
                    return;
                }
                TeleporterDefine define = DataManager.Instance.Teleporters[t.id];

                if (define.MapID != map.Key)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中的TeleporterGameObject与注册信息不匹配，ID为{1}", map.Key, t.id), "确定");
                    return;
                }

                define.Position = GameObjectTool.WorldToLogicN(t.transform.position);
                define.Direction = GameObjectTool.WorldToLogicN(t.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Resources/Levels/" + sceneName + ".unity", OpenSceneMode.Single);
        EditorUtility.DisplayDialog("提示", "导出成功", "确定");
    }

    [MenuItem("Map Tools/Export Spawn Points")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();
        Scene currentScene = EditorSceneManager.GetActiveScene();
        string name = currentScene.name;

        if (currentScene.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请保存场景", "确定");
            return;
        }

        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Resources/Levels/";
            sceneFile += map.Value.Resource + ".unity";

            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("场景文件不存在：{0}", sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Key))
                DataManager.Instance.SpawnPoints[map.Key] = new Dictionary<int, SpawnPointDefine>();
            SpawnPoint[] sps = GameObject.FindObjectsOfType<SpawnPoint>();
            foreach (var sp in sps)
            {
                if (!DataManager.Instance.SpawnPoints[map.Key].ContainsKey(sp.id))
                    DataManager.Instance.SpawnPoints[map.Key][sp.id] = new SpawnPointDefine();
                SpawnPointDefine define = DataManager.Instance.SpawnPoints[map.Key][sp.id];
                NVector3 position = GameObjectTool.WorldToLogicN(sp.transform.position);
                NVector3 direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
                define.Position = position;
                define.Direction = direction;
                define.ID = sp.id;
                define.MapID = map.Key;
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Resources/Levels/" + name + ".unity", OpenSceneMode.Single);
        EditorUtility.DisplayDialog("提示", "导出成功", "确定");
    }

    [MenuItem("Map Tools/Generate NavData")]
    public static void GenerateNavData()
    {
        Material red = new Material(Shader.Find("Particles/Alpha Blended"));
        red.color = Color.red;
        red.SetColor("_TintColor", Color.red);
        red.enableInstancing = true;
        GameObject go = GameObject.Find("MiniMapBoundBox");
        if(go != null)
        {
            GameObject root = new GameObject("Root");
            BoxCollider bound = go.AddComponent<BoxCollider>();
            float step = 1f;
            for(float x = bound.bounds.min.x; x < bound.bounds.max.x; x += step)
            {
                for(float z = bound.bounds.min.z; z < bound.bounds.max.z; z += step)
                {
                    for(float y = bound.bounds.min.y; y < bound.bounds.max.y + 5f; y += step)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        if(NavMesh.SamplePosition(pos, out hit, step, NavMesh.AllAreas))//检测这个点是否可以走
                        {
                            if (hit.hit)//如果可以走
                            {
                                //创建3维数组
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit" + hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform, true);
                                box.transform.position = hit.position;
                                box.transform.localScale = Vector3.one * 0.9f;
                            }
                        }
                    }
                }
            }
        }
    }
}
