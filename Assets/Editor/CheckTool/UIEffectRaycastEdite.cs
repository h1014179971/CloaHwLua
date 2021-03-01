using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CYUtils;
using System.IO;

public class UIEffectRaycastEdite
{
    [MenuItem("Assets/批量移除UI特效成就Raycast", false, 4004)]
    public static void RaycastEdite()
    {
        List<string> fileLst = new List<string>();
        string subPath = "XNRes/AssetBundle/Prefabs/Effect/";
        string path = System.IO.Path.Combine(Application.dataPath, subPath);

        FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
        foreach (string s in fileLst)
        {
            string file = s.Replace(Application.dataPath, "Assets");
            GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
            //Debug.Log(go.name);
            FindImage(go);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        Debug.Log("批量移除UI特效成就Raycast修改完成");
    }

    private static void FindImage(GameObject go)
    {
        if (go != null)
        {
            Image[] components = go.GetComponents<Image>();
            Image c = null;
            for (int i = 0; i < components.Length; i++)
            {
                c = components[i];
                if (c != null && c.raycastTarget)
                {
                    c.raycastTarget = false;
                    EditorUtility.SetDirty(go);
                }
            }
            foreach (Transform t in go.transform)
            {
                FindImage(t.gameObject);
            }
        }

    }
}
