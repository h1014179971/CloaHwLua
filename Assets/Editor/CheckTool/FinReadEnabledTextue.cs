using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using CYUtils;
using System.Text;

public class FinReadEnabledTextue : Editor
{

    [MenuItem("Assets/图片检查工具/去掉选中目录下勾选了图片的Read勾选", false, 4008)]
    public static void FinReadEnabled()
    {
        List<string> fileList = new List<string>();
        string[] guids = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", "psd", "jpeg", "tga" });
        int i = 0;
        foreach (var item in fileList)
        {
            TextureImporter textImport = AssetImporter.GetAtPath(item) as TextureImporter;
            if (textImport.isReadable == true)
            {
                textImport.isReadable = false;
                AssetDatabase.ImportAsset(item);
                Debug.Log(item, AssetDatabase.LoadAssetAtPath(item, typeof(Object)));
            }
            EditorUtility.DisplayProgressBar("Find Texture isReadable", item, i / fileList.Count);
            i++;
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        Debug.Log("查找完成");
    }
}
