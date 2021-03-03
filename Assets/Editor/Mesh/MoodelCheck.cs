using UnityEngine;
using UnityEditor;
using System.IO;

using System.Collections.Generic;

//using Babybus.Framework.Serialization;
using System.Text;

public class MoodelCheck
{
    [MenuItem("检查模型/关闭法线Normal")]
    static void 关闭所有模型的法线()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.importNormals != ModelImporterNormals.None)
            {
                textureImporter.importNormals = ModelImporterNormals.None;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的法线");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键压缩模型的mesh")]
    static void 一键压缩所有模型的mesh()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.meshCompression != ModelImporterMeshCompression.Off)
            {
                textureImporter.meshCompression = ModelImporterMeshCompression.Off;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个模型");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/选择文件夹压缩模型mesh")]
    static void 选择文件夹压缩模型mesh()
    {
        if (Selection.objects.Length != 1)
        {
            Debug.LogError("选择一个文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        if (!Directory.Exists(path))
        {
            Debug.Log(path);
            return;
        }

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        string[] filePath = GetFilePath(Selection.objects);
        foreach (var guid in AssetDatabase.FindAssets("t:Model", filePath))
        {

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";

            if (textureImporter.meshCompression != ModelImporterMeshCompression.Off)
            {
                textureImporter.meshCompression = ModelImporterMeshCompression.Off;
                textureImporter.SaveAndReimport();
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个模型");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键打开模型的Comperssion")]
    static void 打开模型的Comperssion()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.meshCompression == ModelImporterMeshCompression.Off)
            {
                textureImporter.meshCompression = ModelImporterMeshCompression.Low;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("打开了:" + count + "个模型的Comperssion");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键关闭模型的Comperssion")]
    static void 关闭模型的Comperssion()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.meshCompression != ModelImporterMeshCompression.Off)
            {
                textureImporter.meshCompression = ModelImporterMeshCompression.Off;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的Comperssion");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键去除模型的readWritel")]
    static void 去除模型的readWritel()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.isReadable)
            {
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的法线");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/去除文件夹内模型的readWrite")]
    static void 去除文件夹内模型的()
    {
        if (Selection.objects.Length != 1)
        {
            Debug.LogError("选择一个文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        if (!Directory.Exists(path))
        {
            Debug.Log(path);
            return;
        }

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        string[] filePath = GetFilePath(Selection.objects);
        foreach (var guid in AssetDatabase.FindAssets("t:Model", filePath))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.isReadable)
            {
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个模型");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键去除模型的OptimizeMesh")]
    static void 去除模型的OptimizeMesh()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.optimizeMesh)
            {
                textureImporter.optimizeMesh = false;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的法线");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键去除模型的OptimizeMesh")]
    static void 打开模型的OptimizeMesh()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (!textureImporter.optimizeMesh)
            {
                textureImporter.optimizeMesh = true;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("打开了:" + count + "个模型的OptimizeMesh");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/去除文件夹内模型的OptimizeMesh")]
    static void 去除文件夹内模型的OptimizeMesh()
    {
        if (Selection.objects.Length != 1)
        {
            Debug.LogError("选择一个文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        if (!Directory.Exists(path))
        {
            Debug.Log(path);
            return;
        }

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        string[] filePath = GetFilePath(Selection.objects);
        foreach (var guid in AssetDatabase.FindAssets("t:Model", filePath))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.optimizeMesh)
            {
                textureImporter.optimizeMesh = false;
                textureImporter.SaveAndReimport();
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个模型");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/打开文件夹内模型的OptimizeMesh")]
    static void 打开文件夹内模型的OptimizeMesh()
    {
        if (Selection.objects.Length != 1)
        {
            Debug.LogError("选择一个文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        if (!Directory.Exists(path))
        {
            Debug.Log(path);
            return;
        }

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        string[] filePath = GetFilePath(Selection.objects);
        foreach (var guid in AssetDatabase.FindAssets("t:Model", filePath))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (!textureImporter.optimizeMesh)
            {
                textureImporter.optimizeMesh = true;
                textureImporter.SaveAndReimport();
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个模型");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    public static string[] GetFilePath(Object[] targets)
    {
        var folders = new List<string>();
        for (int i = 0; i < targets.Length; i++)
        {
            string assetPath = AssetDatabase.GetAssetPath(targets[i]);
            if (Directory.Exists(assetPath))
                folders.Add(assetPath);
        }
        return folders.ToArray();
    }
    [MenuItem("检查模型/一键压缩模型的动画为Optimal")]
    static void 一键压缩模型的动画()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.animationCompression != ModelImporterAnimationCompression.Optimal)
            {
                textureImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
                textureImporter.SaveAndReimport();
                count++;
            }
        }
        Debug.Log("压缩了:" + count + "个动画");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键关闭模型的Cameras")]
    static void 关闭模型的Cameras()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.importCameras)
            {
                textureImporter.importCameras = false;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的Cameras");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键关闭模型的Lights")]
    static void 关闭模型的Lights()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (textureImporter.importLights)
            {
                textureImporter.importLights = false;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的Lights");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
    [MenuItem("检查模型/一键打开模型的Hierarchy")]
    static void 打开模型的Hierarchy()
    {

        AssetDatabase.StartAssetEditing();
        var assets = new List<string>();
        int count = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (textureImporter == null)
                continue;

            //int maxsize = 0;
            //TextureImporterFormat textureImporterFormat;
            //string androidPlatform = "Android";
            if (!textureImporter.preserveHierarchy)
            {
                textureImporter.importLights = true;
                textureImporter.SaveAndReimport();
                //Debug.Log(assetPath);
                count++;
            }
        }
        Debug.Log("关闭了:" + count + "个模型的Hierarchy");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }
}
