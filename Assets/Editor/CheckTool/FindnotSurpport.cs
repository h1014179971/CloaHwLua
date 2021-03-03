using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CYUtils;

public class FindnotSurpport : Editor
{
    [MenuItem("Assets/图片检查工具/查找选中目录下不是2的幂次方且长或宽大于220的图片", false, 4003)]
    public static void FindTextNotSurpport()
    {
        List<string> fileList = new List<string>();
        string[] guids = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", "psd", "jpeg", "tga" });
        int i = 0;
        foreach (var item in fileList)
        {
            Texture2D tx = (Texture2D)AssetDatabase.LoadAssetAtPath(item, typeof(Texture2D));
            if (isLargeTex(tx) && (!getNumber(tx.width) || !getNumber(tx.height)))
            {
                Debug.Log(item, tx);
            }
            EditorUtility.DisplayProgressBar("Find Texture 2 power", item, i / fileList.Count);
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找完成");
    }
    private static bool getNumber(int num)
    {
        if (num < 1)
            return false;
        return (num & num - 1) == 0;
    }

    private static bool isLargeTex(Texture2D tex)
    {
        if (tex == null)
        {
            return false;
        }

        if (tex.width >= 220 || tex.height >= 220)
        {
            return true;
        }

        return false;
    }
}
