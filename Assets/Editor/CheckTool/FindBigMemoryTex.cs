using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using CYUtils;

public class FindBigMemoryTex : Editor
{

    [MenuItem("Assets/图片检查工具/查找选中目录下占用内存大于500K的图片", false, 4009)]
    public static void FindTextNotSurpport()
    {
        int memorySize = 500 * 1024;
        List<string> fileList = new List<string>();
        string[] guids = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        FileUtils.searchAllFiles(path, fileList, new List<string> { ".png", ".jpg", "psd", "jpeg", "tga" });
        int i = 0;
        foreach (var item in fileList)
        {
            Texture2D tx = (Texture2D)AssetDatabase.LoadAssetAtPath(item, typeof(Texture2D));
            TextureImporter textImport = AssetImporter.GetAtPath(item) as TextureImporter;

            switch (textImport.textureFormat)
            {
                case TextureImporterFormat.ETC2_RGBA8:
                    if (tx.width * tx.height * 40 > memorySize)
                    {
                       //Debug.Log(item, tx);
                    }
                    break;
                case TextureImporterFormat.AutomaticCompressed:
                    if (item.EndsWith(".png"))
                    {
                        if (tx.width * tx.height > memorySize)
                        {
                            Debug.Log(item, tx);
                        }
                    }
                    if(item.EndsWith(".jpg"))
                        if (tx.width * tx.height*0.5f > memorySize)
                        {
                            Debug.Log(item, tx);
                        }
                    break;
                case TextureImporterFormat.DXT5:
                    if (tx.width * tx.height > memorySize)
                    {
                        Debug.Log(item, tx);
                    }
                    break;
                case TextureImporterFormat.Alpha8:
                    if (tx.width * tx.height * 4 > memorySize)
                    {
                        Debug.Log(item, tx);
                    }
                    break;
                case TextureImporterFormat.AutomaticTruecolor:
                    if (tx.width * tx.height * 4 > memorySize)
                    {
                        Debug.Log(item, tx);
                    }
                    break;
                case TextureImporterFormat.RGBA32:
                    if (tx.width * tx.height * 4 > memorySize)
                    {
                        Debug.Log(path, tx);
                    }
                    break;
                case TextureImporterFormat.PVRTC_RGBA4:
                    if (tx.width * tx.height * 32 > memorySize)
                    {
                        Debug.Log(path, tx);
                    }
                    break;

            }
            //if (textImport.textureFormat == TextureImporterFormat.DXT1)
            //{
            //    Debug.Log(item, tx);
            //}
            EditorUtility.DisplayProgressBar("Find Texture 2 power", item, i / fileList.Count);
            i++;
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("查找完成");
    }
}
