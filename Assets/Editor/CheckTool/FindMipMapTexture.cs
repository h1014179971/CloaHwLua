using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using CYUtils;
using System.Text;

public class FindMipMapTexture :Editor {

    [MenuItem("Assets/图片检查工具/找到选中目录下勾选了MipMap的图片", false, 4008)]
    public static void FindTextMipMap()
    {
        List<string> fileList = new List<string>();
        string[] guids = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", "psd", "jpeg", "tga" });
        int i = 0;
        foreach (var item in fileList)
        {
            TextureImporter textImport = AssetImporter.GetAtPath(item) as TextureImporter;
            if (textImport.mipmapEnabled == true)
            {
                Debug.Log(item,AssetDatabase.LoadAssetAtPath(item, typeof(Object)));
            }
            EditorUtility.DisplayProgressBar("Find Texture 2 power", item, i / fileList.Count);
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找完成");
    }
}
