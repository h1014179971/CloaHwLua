using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using CYUtils;

public class UICheckSizeTool
{

    [MenuItem("Assets/查找长或宽大于256的图片", false, 4012)]
    public static void FindImproperTexture()
    {
        List<string> fileList = new List<string>();
        string subPath = "XNRes/AssetBundle/Altas";
        string uipath = System.IO.Path.Combine(Application.dataPath, subPath);
        FileUtils.searchAllFiles(uipath, fileList, new List<string> { ".png", ".jpg" });
        List<Object> objList = new List<Object>();
        int i = 0;
        foreach (var f in fileList)
        {
            FileStream files = new FileStream(f, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            Texture2D tx = new Texture2D(100, 100);
            if (imgByte != null)
            {
                tx.LoadImage(imgByte);
                if(tx.height>256 || tx.width > 256)
                {
                    string filePath = f.Substring(f.IndexOf("Assets"), f.Length - f.IndexOf("Assets"));
                    Debug.LogError("尺寸超标图片路径: " + filePath + "\n图片尺寸为:" + tx.height + "*" + tx.width, AssetDatabase.LoadAssetAtPath((filePath), typeof(Object)));
                }
            }
            i++;
            EditorUtility.DisplayProgressBar("Find Reference", f, i / fileList.Count);
        }
        EditorUtility.ClearProgressBar();
    }
}
