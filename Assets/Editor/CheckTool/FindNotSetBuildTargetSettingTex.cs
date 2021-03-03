using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CYUtils;
using System.IO;

public class FindNotSetBuildTargetSettingTex {

    private static List<string> fileList; //图片路径集合
    private static string texturePath;

    [MenuItem("Assets/图片检查工具/查找选中目录下未设置压缩格式的图片", false, 4011)]
    public static void FindNotSetTex()
    {
        InitData();
        if (fileList != null && fileList.Count > 0)
        {
            int i = 0;
            foreach (var path in fileList)
            {
                CheckHasSetting(path);
                EditorUtility.DisplayProgressBar("Find Not Set Texture", path, i / fileList.Count);
                i++;
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
        else if (string.IsNullOrEmpty(texturePath) == false)
        {
            CheckHasSetting(texturePath);
        }
        else
        {
            Debug.LogError("未选中图片文件或该文件夹未包含图片");
        }
        ClearSelectedData();
        Debug.Log("查找完毕！");
    }

    private static void InitData()
    {
        fileList = new List<string>();
        texturePath = string.Empty;
        string[] guids = Selection.assetGUIDs;
        if (guids == null || guids.Length == 0)
        {
            return;
        }
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Texture selectRes = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
        if (selectRes != null)
        {
            texturePath = path;
        }
        else
        {
            FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", ".psd", ".jpeg", ".tga" });
        }
    }

    private static bool CheckHasSetting(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("路径为空！");
            return false;
        }
        bool isSet = SetTextureEditor.CheckBuildTargetHasSetting(path);
        if (isSet == false)
        {
            Debug.LogError("该图片没有设置纹理压缩格式" + "\n路径为：" + path, AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        return isSet;
    }

    private static void ClearSelectedData()
    {
        fileList.Clear();
        texturePath = string.Empty;
    }
}
