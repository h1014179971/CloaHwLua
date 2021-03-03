using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CYUtils;

public class FindReferences
{
    //private static string cfgSavePath = @"F:\UIReferences.txt";
    private static string cfgSavePath = System.IO.Path.Combine(Application.dataPath, "UIReferences.txt");

    [MenuItem("AtlasTool/查找未被使用的精灵图片")]
    static private void FindNoRefSprite()
    {
        string atlasPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(atlasPath) || atlasPath.Contains("Atlas") == false)
        {
            Debug.LogError("没有选中图集文件夹！");
            return;
        }
        List<string> allPicPath = new List<string>();
        string atlasFullPaht = System.IO.Path.Combine(Application.dataPath, atlasPath.Replace("Assets/", "") + "/");
        FileUtils.searchAllFiles(atlasFullPaht, allPicPath, new List<string> { ".jpg", ".png", ".tga" });
        if (allPicPath == null || allPicPath.Count == 0)
        {
            Debug.LogError("图片路径allPicPath为空！");
            return;
        }
        int count = 0;
        for (int i = 0; i < allPicPath.Count; i++)
        {
            string picPath = allPicPath[i].Replace(Application.dataPath, "Assets");
            Object selectRes = AssetDatabase.LoadAssetAtPath(picPath, typeof(Object)) as Object;
            int instanceId = selectRes.GetInstanceID();
            List<string> fileLst = new List<string>();
            string uiPath = System.IO.Path.Combine(Application.dataPath, "Resources/Prefabs/UI/");
            FileUtils.searchAllFiles(uiPath, fileLst, new List<string> { ".prefab", ".mat", ".asset" });
            int refCount = 0;
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                    string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                    foreach (var p in paths)
                    {
                        Object res = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                        if (res.GetInstanceID() == instanceId)
                        {
                            refCount++;
                            break;
                        }
                    }
                }
            }
            if (refCount == 0)
            {
                Debug.Log(picPath, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(allPicPath[i]), typeof(Object)));
            }
            count++;
            EditorUtility.DisplayProgressBar("Find Reference", picPath, count / allPicPath.Count);
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找结束-------------");
    }

    [MenuItem("AtlasTool/查找贴图被哪些特效引用")]
    static private void FindTextureEffectRef()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            Object selectRes = AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as Object;
            int instanceId = selectRes.GetInstanceID();
            List<string> fileLst = new List<string>();
            string subPath = "Resources/Prefabs/Effects/";
            string uipath = System.IO.Path.Combine(Application.dataPath, subPath);
            FileUtils.searchAllFiles(uipath, fileLst, new List<string> { ".prefab", ".mat", ".asset" });
            int i = 0;
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                    string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                    foreach (var p in paths)
                    {
                        Object res = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                        if (res.GetInstanceID() == instanceId)
                        {
                            Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(s), typeof(Object)));
                        }
                    }
                    EditorUtility.DisplayProgressBar("Find Reference", assetPath, i / fileLst.Count);
                    i++;
                }
            }
            fileLst.Clear();
            subPath = "Prefabs/Effects/";
            uipath = System.IO.Path.Combine(Application.dataPath, subPath);
            FileUtils.searchAllFiles(uipath, fileLst, new List<string> { ".prefab", ".mat", ".asset" });
            i = 0;
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                    string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                    foreach (var p in paths)
                    {
                        Object res = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                        if (res.GetInstanceID() == instanceId)
                        {
                            Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(s), typeof(Object)));
                        }
                    }
                    EditorUtility.DisplayProgressBar("Find Reference", assetPath, i / fileLst.Count);
                    i++;
                }
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("查找结束-------------");
        }
    }

    [MenuItem("Assets/查找图片引用预设", false, 4009)]
    static private void Find()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            Object selectRes = AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as Object;
            int instanceId = selectRes.GetInstanceID();
            List<string> fileLst = new List<string>();
            //string subPath = "Resources/Prefabs/UI/";
            //string uipath = System.IO.Path.Combine(Application.dataPath, subPath);
            string uipath = Application.dataPath;
            FileUtils.searchAllFiles(uipath, fileLst, new List<string> { ".prefab", ".mat", ".asset" });
            int i = 0;
            List<string> cfg = new List<string>();
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    //Debug.Log(go.name);
                    string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                    string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                    foreach (var p in paths)
                    {
                        Object res = AssetDatabase.LoadAssetAtPath(p, typeof(Object));
                        if (res.GetInstanceID() == instanceId)
                        {
                            Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(s), typeof(Object)));
                            cfg.Add(file);
                        }
                    }
                    EditorUtility.DisplayProgressBar("Find Reference", assetPath, i / fileLst.Count);
                    i++;
                }
            }
            writeTxt(cfg);
            EditorUtility.ClearProgressBar();            
            Debug.Log("查找匹结束-------------");
        }
    }

    [MenuItem("Assets/查找图片引用预设", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + System.IO.Path.GetFullPath(path).Replace(System.IO.Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    static private void writeTxt(List<string> cfg) {
        using (FileStream fs = new FileStream(cfgSavePath, FileMode.OpenOrCreate)) {
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default)) {
                for (int i = 0; i < cfg.Count; i++)
                {
                    sw.WriteLine(cfg[i]);
                }
            }
        }
    }

    [MenuItem("Assets/判断当前文件夹中是否有无效预制体", false, 4011)]
    public static void ThePrefabsEffective()
    {
        //递归当前选中的文件夹,找到可能变成无效的预制体
        List<string> file = new List<string>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string pathNew = path.Replace("Assets/", "");
        string uipath = System.IO.Path.Combine(Application.dataPath, pathNew);
        FileUtils.searchAllFiles(uipath, file, new List<string> { ".prefab", ".mat", ".asset", ".shader", ".fbx" });
        int i = 0;
        foreach (var f in file)
        {
            string fileFind = f.Replace(Application.dataPath, "Assets");
            Object selectRes = AssetDatabase.LoadAssetAtPath(fileFind, typeof(Object)) as Object;
            if (selectRes != null && string.IsNullOrEmpty(selectRes.name))
                Debug.LogError("无效预制体路径：" + fileFind, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(fileFind), typeof(Object)));
            i++;
            EditorUtility.DisplayProgressBar("Find Invalid Prefabs", fileFind, i / file.Count);
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找完成！！！");
    }
    [MenuItem("Assets/查找非正常引用预设", false, 4010)]
    public static void FindImproperPrefabs()
    {
        //递归Resources文件夹 将所有可能引用图片的对象拿出来
        List<string> fileLstRes = new List<string>();
        //string subPathRes = "XNRes/Resources";
        //string uipathRes = System.IO.Path.Combine(Application.dataPath, subPathRes); 
        string uipathRes = Application.dataPath;
        FileUtils.searchAllFiles(uipathRes, fileLstRes, new List<string> { ".prefab", ".mat", ".asset", ".shader" });
        //递归AssetBundle文件夹 将所有的图片拿出来
        List<string> fileLst = new List<string>();
        List<string> fileLstAb = new List<string>();
        //string subPath = "XNRes/AssetBundle";
        //string uipath = System.IO.Path.Combine(Application.dataPath, subPath);
        string uipath = Application.dataPath;
        FileUtils.searchAllFiles(uipath, fileLst, new List<string> { ".png", ".jpg" });
        FileUtils.searchAllFiles(uipath, fileLstAb, new List<string> { ".prefab", ".mat", ".asset" });
        //第一次找到Resources内引用了AssetBundle图片的对象
        List<string> findRes = new List<string>();
        int i = 0;

        foreach (var f in fileLstRes)
        {
            string fileRes = f.Replace(Application.dataPath, "Assets");
            string guid = AssetDatabase.AssetPathToGUID(fileRes);
            Object go = AssetDatabase.LoadAssetAtPath(fileRes, typeof(Object)) as Object;
            if (go != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                foreach (var p in paths)
                {
                    foreach (var fRes in fileLst)
                    {
                        string file = fRes.Replace(Application.dataPath, "Assets");
                        if (file == p)
                            findRes.Add(assetPath);
                    }
                    i++;
                    EditorUtility.DisplayProgressBar("Find Reference", assetPath, i / fileLst.Count);
                }
            }
        }
        i = 0;
        EditorUtility.ClearProgressBar();
        //第二次通过Resources中的对象找到AssetBundle中引用了这个对象的预设
        foreach (var ab in fileLstAb)
        {
            string fileAb = ab.Replace(Application.dataPath, "Assets");
            GameObject go = AssetDatabase.LoadAssetAtPath(fileAb, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });
                foreach (var p in paths)
                {
                    foreach (var find in findRes)
                    {
                        if (find == p)
                            Debug.LogError("引用了不正常预设的物体路径" + assetPath + "---------不正常预设路径" + find, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(assetPath), typeof(Object)));
                    }
                    i++;
                    EditorUtility.DisplayProgressBar("Find Reference", assetPath, i / fileLst.Count);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找结束！！！！");
    }
}
