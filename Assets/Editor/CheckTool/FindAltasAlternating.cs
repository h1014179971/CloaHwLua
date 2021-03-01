using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CYUtils;
using UIEditor;
using UnityEngine.UI;

public class FindAltasAlternating
{
    [MenuItem("Assets/查找引用图集超过2个(选中文件夹)", false, 4011)]
    static private void FindThis()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        int errorCount = 0;
        int prefabCount = 0;
        for (int i = 0; i < objs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            if (!path.EndsWith(".prefab")) continue;
            prefabCount++;
            string file = path.Replace(Application.dataPath, "Assets");
            GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                //Debug.Log(go.name);
                string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                int counter = GetRefCount(assetPath);
                if (counter > 2)
                {
                    errorCount++;
                    Debug.LogError(assetPath + "图集引用了" + counter + "个图集", AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(assetPath), typeof(Object)));
                }
            }
        }
        if (prefabCount <= 0)
        {
            Debug.LogError("当前选中文件夹没有预设,是否选中了正确的文件夹？");
        }
        else
        {
            Debug.Log("查找结束----总共预设:" + prefabCount + "个, <color=red>" + errorCount + "</color>预设引用了2个以上图集");
        }
    }

    [MenuItem("Assets/查找引用图集超过2个的UI预设", false, 4010)]
    static private void Find()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            Object selectRes = AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as Object;
            int instanceId = selectRes.GetInstanceID();
            List<string> fileLst = new List<string>();
            string subPath = "XNRes/AssetBundle/Prefabs/UI/";
            string uipath = System.IO.Path.Combine(Application.dataPath, subPath);
            FileUtils.searchAllFiles(uipath, fileLst, new List<string> { ".prefab" });
            int i = 0;
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    //Debug.Log(go.name);
                    string assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
                    int counter = GetRefCount(assetPath);
                    if (counter > 2)
                    {
                        Debug.LogError(assetPath + "图集引用了" + counter + "个图集", AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(assetPath), typeof(Object)));
                    }
                }
                EditorUtility.DisplayProgressBar("Find Reference", s, i / fileLst.Count);
                i++;
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("查找结束-------------");
        }
    }

    [MenuItem("Assets/检查UIImage九宫格", false, 4061)]
    public static void checkImgeType()
    {
        GameObject selectGo = Selection.activeGameObject;
        if (selectGo)
        {
            int count = 0;
            setImageType(selectGo.transform,ref count);
            if (count > 0) {
                EditorUtility.SetDirty(selectGo);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();               
            }
            Debug.Log("单个UI更换image type完成 UI:  " + selectGo.name + " count: " + count);
            return;
        }
        //递归获取项目中所有的材质球
        List<string> fileLst = new List<string>();
        string subPath = "XNRes/AssetBundle/Prefabs/UI/";
        string path = System.IO.Path.Combine(Application.dataPath, subPath);

        FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
        foreach (string s in fileLst)
        {
            string file = s.Replace(Application.dataPath, "Assets");
            GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
            int count = 0;
            setImageType(go.transform, ref count);
            if (count > 0)
                EditorUtility.SetDirty(go);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        Debug.Log("更换整个UI文件夹image type完成");
    }

    private static void setImageType(Transform go, ref int count)
    {
        if (go == null) return;

        Image img = go.GetComponent<Image>();
        if (img != null)
        {
            if (img.type != Image.Type.Filled) {
                if (img.hasBorder && img.type != Image.Type.Sliced)
                {
                    img.type = Image.Type.Sliced;
                    count++;
                }
            }
        }
        for (int i = 0; i < go.transform.childCount; i++)
        {
            setImageType(go.transform.GetChild(i),ref count);
        }
    }

    private static int GetRefCount(string asset)
    {
        asset = asset.Replace("\\", "/");
        if (!asset.StartsWith("Prefabs"))
        {
            return 0;
        }
        string[] deps = AssetDatabase.GetDependencies(new string[] { asset });
        Dictionary<string, string> dict = new Dictionary<string, string>();
        #region 构造依赖的图集信息
        foreach (string s in deps)
        {
            string tmp = "";
            string abName = "";
            if (s.StartsWith("Atlas"))
            {
                int ind = s.Replace("\\", "/").LastIndexOf("/");
                tmp = s.Substring(0, ind);
                abName = CYUtils.FileUtils.GetFileName("Atlas") + "/" + FileUtils.GetFileName(tmp);
                if (!dict.ContainsKey(abName))
                {
                    dict.Add(abName, s);
                    //Debug.Log(s);
                }
            }
        }
        #endregion
        return dict.Count;
    }
    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + System.IO.Path.GetFullPath(path).Replace(System.IO.Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}
