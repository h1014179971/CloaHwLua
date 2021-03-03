using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class AndroidBuildWindow : EditorWindow
{
    private static AndroidBuildWindow instance;
    private static Vector2 scrollPos;
    private enum Channel
    {
        NONE,
        Ohayoo
    }
    private static Channel channel;
    private static string fromDirectory = Path.Combine(System.Environment.CurrentDirectory, "AndroidAdd");
    private static string toDirectory = Path.Combine(System.Environment.CurrentDirectory, "Assets/Plugins/Android");
    private static List<string> copyFiles = new List<string>();
    private static ES3File es3File;
    private static AndroidData androidData;

    [MenuItem("MHFrameWork/Android Build",false,1)]
    static void ShowWindow()
    {
        instance = EditorWindow.GetWindow<AndroidBuildWindow>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        instance.Show();
        LoadData();
    }
    private static void LoadData()
    {
        es3File = new ES3File("androidBuild.es3");
        if (es3File.KeyExists(channel.ToString()))
            androidData = es3File.Load<AndroidData>(channel.ToString());
        else
            androidData = new AndroidData();
            
            
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("渠道", GUILayout.Width(150));
        Channel ch = (Channel)EditorGUILayout.EnumPopup(channel);
        if(channel != ch)
        {
            channel = ch;
            LoadData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Version*", GUILayout.Width(150));
        androidData.bundleVersion = EditorGUILayout.TextField("", androidData.bundleVersion);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundle Version Code", GUILayout.Width(150));
        string bundleVersionCode = EditorGUILayout.TextField("", androidData.bundleVersionCode.ToString());
        androidData.bundleVersionCode = int.Parse(bundleVersionCode);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Browse Keystore", GUILayout.Width(150)))
        {
            androidData.keystoreName = EditorUtility.OpenFilePanel("Open existing keystor...", Application.dataPath, "keystore");
        }
        EditorGUILayout.LabelField("", androidData.keystoreName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("keystorePass", GUILayout.Width(150));
        androidData.keystorePass = EditorGUILayout.TextField("", androidData.keystorePass);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("keyaliasName", GUILayout.Width(150));
        androidData.keyaliasName = EditorGUILayout.TextField("", androidData.keyaliasName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("keyaliasPass", GUILayout.Width(150));
        androidData.keyaliasPass = EditorGUILayout.TextField("", androidData.keyaliasPass);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build APK"))
        {
            BuildAPK();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Project"))
        {
            ExportProject();
        }
        GUILayout.EndHorizontal();
    }
    static void Build()
    {
        string channelName = channel.ToString();
        string srcDirectory = Path.Combine(fromDirectory, channelName);
        string desDirectory = toDirectory;
        Debug.Log($"src=={srcDirectory};des=={desDirectory}");
        if(Directory.Exists(srcDirectory))
            CopyFiles(srcDirectory, desDirectory);
        else
            Debug.LogError($"没有{srcDirectory}文件夹，您正在打裸包");
        PlayerSettings.bundleVersion = androidData.bundleVersion;
        PlayerSettings.Android.bundleVersionCode = androidData.bundleVersionCode;
        PlayerSettings.Android.keystoreName = androidData.keystoreName;
        PlayerSettings.Android.keystorePass = androidData.keystorePass;
        PlayerSettings.Android.keyaliasName = androidData.keyaliasName;
        PlayerSettings.Android.keyaliasPass = androidData.keyaliasPass;
        AssetDatabase.Refresh();
       
        
    }
    private  static void BuildAPK()
    {
        Build();
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        string parentDirectory = System.Environment.CurrentDirectory;
        string apkName = "";
        if (!string.IsNullOrEmpty(androidData.apkPath))
        {
            FileInfo apkFileInfo = new FileInfo(androidData.apkPath);
            parentDirectory = apkFileInfo.DirectoryName;
            apkName = apkFileInfo.Name;
        }
        androidData.apkPath = EditorUtility.SaveFilePanel("Build Android", parentDirectory, apkName, "apk");
        BuildPipeline.BuildPlayer(GetBuildScenes(), androidData.apkPath, BuildTarget.Android, BuildOptions.None);
        SaveData();
        EditorUtility.RevealInFinder(androidData.apkPath);
        DeleteFiles();
    }
    private static void ExportProject()
    {
        
        Build();
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true; //exportproject=true,导出Androidstudio工程
        string projectPath = EditorUtility.SaveFolderPanel("Build Android", System.Environment.CurrentDirectory,"");
        BuildPipeline.BuildPlayer(GetBuildScenes(), projectPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
        SaveData();
        EditorUtility.RevealInFinder(projectPath);
        DeleteFiles();
    }

    private static void CopyFiles(string srcPath, string dstPath)
    {
        if (File.Exists(dstPath))
            File.Delete(dstPath);
        if(!Directory.Exists(dstPath))
            Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
        {
            string toFilePath = Path.Combine(dstPath, Path.GetFileName(file));
            if (File.Exists(toFilePath))
                File.Delete(toFilePath);
            File.Copy(file, toFilePath);
            copyFiles.Add(toFilePath);
        }
        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyFiles(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
    private static void DeleteFiles()
    {
        for(int i = 0; i < copyFiles.Count; i++)
        {
            string filePath = copyFiles[i];
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    } 
    private static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (!EditorBuildSettings.scenes[i].enabled) continue;
            names.Add(EditorBuildSettings.scenes[i].path);

        }
        return names.ToArray();
    }
    private static void SaveData()
    {
        es3File.Save<AndroidData>(channel.ToString(), androidData);
        es3File.Sync();
    }


    /*********************/
    public class AndroidData 
    {
        public string bundleVersion;
        public int bundleVersionCode;
        public string keystoreName;
        public string keystorePass;
        public string keyaliasName;
        public string keyaliasPass;
        public string apkPath;
    }

}
