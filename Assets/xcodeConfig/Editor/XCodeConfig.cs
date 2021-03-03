using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;  
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.XCodeEditor;        
using yx;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml;

using Newtonsoft.Json.Linq;       

public class XCodeConfig : MonoBehaviour {

    const string configPath = "Assets/xcodeConfig/Editor/XCodeConfig.json";
    static Hashtable _table;
    
    
    public static void OnPreprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.Android)
        {
            //OnPreprocessBuild_android(buildTarget, path);
        }
    }
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
        if (buildTarget == BuildTarget.iOS) {
            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();

            proj.ReadFromString(File.ReadAllText(projPath));
            //读取配置文件
            string cpath = configPath;
            string json = File.ReadAllText(cpath);
            Hashtable table = json.hashtableFromJson();
            _table = table;
            SetTeamId(proj,table.SGet<string>("teamId"));
            //lib
            SetLibs(proj, table.SGet<Hashtable>("libs"));
            //framework
            SetFrameworks(proj, table.SGet<Hashtable>("frameworks"));
            //building setting
            SetBuildProperties(proj, table.SGet<Hashtable>("properties"));
            //修改代码
            ReviseClass();
            //复制文件
            CopyFiles(proj, path, table.SGet<Hashtable>("copyfiles"));
            //复制文件夹
            CopyFolders(proj, path, table.SGet<Hashtable>("folders"));
            //删除文件夹
            DeleteFolders(proj, path, table.SGet<ArrayList>("deletefolder"));
            //删除hwads指定文件夹下，某些文件
            DeleteFiles(table.SGet<Hashtable>("deleteFiles"));
            //文件编译符号
            //    SetFilesCompileFlag(proj, table.SGet<Hashtable>("filesCompileFlag"));
            //添加 shellscript
            AddShellScript(proj,table.SGet<ArrayList>("addShellScript"));
            //写入
            File.WriteAllText(projPath, proj.WriteToString());
            //plist
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plist.root;

           SetPlist(proj, rootDict, table.SGet<Hashtable>("plist"));

            
            //写入
            plist.WriteToFile(plistPath);
        }
    }
    private static void SetTeamId(PBXProject proj,string teamId)
    {
        string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        proj.SetTeamId(target,teamId);
    }
    private static void AddLibToProject(PBXProject inst, string targetGuid, string lib) {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.AddFileToBuild(targetGuid, fileGuid);
    }
    private static void RemoveLibFromProject(PBXProject inst, string targetGuid, string lib) {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.RemoveFileFromBuild(targetGuid, fileGuid);
    }
    //设置frameworks
    private static void SetFrameworks(PBXProject proj, Hashtable table) {
        if (table!=null) {
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            ArrayList addList = table["+"] as ArrayList;
            if (addList != null) {
                foreach (string i in addList) {
                    proj.AddFrameworkToProject(target, i, false);
                }
            }
            ArrayList removeList = table["-"] as ArrayList;
            if (removeList != null) {
                foreach (string i in removeList) {
                    proj.RemoveFrameworkFromProject(target, i);
                }
            }
        }
    }
    //设置libs
    private static void SetLibs(PBXProject proj, Hashtable table) {
        if (table!=null) {
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            ArrayList addList = table["+"] as ArrayList;
            if (addList != null) {
                foreach (string i in addList) {
                    AddLibToProject(proj, target, i);
                }
            }
            ArrayList removeList = table["-"] as ArrayList;
            if (removeList != null) {
                foreach (string i in removeList) {
                    RemoveLibFromProject(proj, target, i);
                }
            }
        }
    }
    //设置编译属性
    private static void SetBuildProperties(PBXProject proj, Hashtable table) {
        if (table!=null) {
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            Hashtable setTable = table.SGet<Hashtable>("=");
            foreach (DictionaryEntry i in setTable) {
                proj.SetBuildProperty(target, i.Key.ToString(), i.Value.ToString());
            }
            Hashtable addTable = table.SGet<Hashtable>("+");
            foreach (DictionaryEntry i in addTable) {
                ArrayList array = i.Value as ArrayList;
                List<string> list = new List<string>();
                foreach (var flag in array) {
                    list.Add(flag.ToString());
                }
                proj.UpdateBuildProperty(target, i.Key.ToString(), list, null);
            }
            Hashtable removeTable = table.SGet<Hashtable>("-");
            foreach (DictionaryEntry i in removeTable) {
                ArrayList array = i.Value as ArrayList;
                List<string> list = new List<string>();
                foreach (var flag in array) {
                    list.Add(flag.ToString());
                }
                proj.UpdateBuildProperty(target, i.Key.ToString(), null, list);
            }
        }
    }
    //设置plist
    private static void SetPlist(PBXProject proj, PlistElementDict node, Hashtable arg) {
        if (arg != null) {
            foreach (DictionaryEntry i in arg) {
                string key = i.Key.ToString();
                object val = i.Value;
                var vType = i.Value.GetType();
                if (vType == typeof(string)) {
                    node.SetString(key, (string)val);
                }
                else if (vType == typeof(bool)) {
                    node.SetBoolean(key, (bool)val);
                }
                else if (vType == typeof(double)) {
                    int v = int.Parse(val.ToString());
                    node.SetInteger(key, v);
                }
                else if (vType == typeof(ArrayList)) {
                    var t = node.CreateArray(key);
                    var array = val as ArrayList;
                    SetPlist(proj, t, array);
                }
                else if (vType == typeof(Hashtable)) {
                    var t = node.CreateDict(key);
                    var table = val as Hashtable;
                    SetPlist(proj,t, table);
                }
            }
        }
    }
    private static void SetPlist(PBXProject proj, PlistElementArray node, ArrayList arg) {
        if (arg != null) {
            foreach (object i in arg) {
                object val = i;
                var vType = i.GetType();
                if (vType == typeof(string)) {
                    node.AddString((string)val);
                }
                else if (vType == typeof(bool)) {
                    node.AddBoolean((bool)val);
                }
                else if (vType == typeof(double)) {
                    int v = int.Parse(val.ToString());
                    node.AddInteger(v);
                }
                else if (vType == typeof(ArrayList)) {
                    var t = node.AddArray();
                    var array = val as ArrayList;
                    SetPlist(proj, t, array);
                }
                else if (vType == typeof(Hashtable)) {
                    var t = node.AddDict();
                    var table = val as Hashtable;
                    SetPlist(proj, t, table);
                }
            }
        }
    }
    //删掉文件夹
    private static void DeleteFolders(PBXProject proj,string xcodePath,ArrayList arg)
    {
        if (arg != null)
        {
            foreach (object i in arg)
            {
                object val = i;
                var vType = i.GetType();
                if (vType == typeof(string))
                {
                    string deletePath = Path.Combine(xcodePath, val.ToString());
                    if (Directory.Exists(deletePath))
                    {
                        Directory.Delete(deletePath,true);
                    }
                }
                
            }
        }
    }
    //复制文件
    private static void CopyFiles(PBXProject proj, string xcodePath, Hashtable arg) {
        foreach (DictionaryEntry i in arg) {
            string src = Path.Combine(System.Environment.CurrentDirectory, i.Key.ToString());
            string des = Path.Combine(xcodePath, i.Value.ToString());
            CopyFile(proj,xcodePath, src, des);
        }
    }
    //复制文件夹
    private static void CopyFolders(PBXProject proj, string xcodePath, Hashtable arg) {
        foreach (DictionaryEntry i in arg) {
            string src = Path.Combine(System.Environment.CurrentDirectory, i.Key.ToString());
            string des = Path.Combine(xcodePath, i.Value.ToString());
            CopyFolder(src, des);
            AddFolderBuild(proj, xcodePath, i.Value.ToString());
        }
    }
    private static void CopyFile(PBXProject proj, string xcodePath, string src, string des) {
        bool needCopy = NeedCopy(src);
        if (needCopy) {
            if (File.Exists(des))
                File.Delete(des);
            File.Copy(src, des);
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            proj.AddFileToBuild(target, proj.AddFile(des, des.Replace(xcodePath + "/", ""), PBXSourceTree.Absolute));
            AutoAddSearchPath(proj, xcodePath, des);
            Debug.Log("copy file " + src + " -> " + des);
        }
    }
    private static void CopyFolder(string srcPath, string dstPath) {
        if (Directory.Exists(dstPath))
        {
            //Directory.Delete(dstPath); //目录不为空删除会报错
            Directory.Delete(dstPath, true);//目录不为空删除不报错
        }
            
        if (File.Exists(dstPath))
            File.Delete(dstPath);

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath)) {
            if (NeedCopy(Path.GetFileName(file))) {
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));  
            }
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyFolder(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
    //删掉hwads中指定文件
    private static void DeleteFiles(Hashtable arg)
    {
        foreach (DictionaryEntry i in arg)
        {
            string path = Path.Combine(System.Environment.CurrentDirectory, i.Key.ToString());
            ArrayList array = i.Value as ArrayList;
            DeleteFile(path, array);
        }
    }
    private static void DeleteFile(string path,ArrayList array)
    {
        DirectoryInfo sourceDirInfo = new DirectoryInfo(path);
        try
        {
            if (!sourceDirInfo.Exists)//判断所指的文件或文件夹是否存在
            {
                return;
            }
            // 获取文件夹中所有文件和文件夹
            FileSystemInfo[] sourceFiles = sourceDirInfo.GetFileSystemInfos();
            // 对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
            foreach (FileSystemInfo sourceFileSys in sourceFiles)
            {
                FileInfo file = sourceFileSys as FileInfo;
                if (file != null)   // 如果是文件的话，进行文件的复制操作
                {
                    string fileName = file.Name;
                    string filePath = Path.Combine(path, fileName);
                    foreach (object i in array)
                    {
                        object val = i;
                        var vType = i.GetType();
                        if (vType == typeof(string))
                        {
                            string deleteName = val as string;
                            if (fileName.Equals(deleteName))
                            {
                               
                                if(File.Exists(filePath))
                                    File.Delete(filePath);
                            }
                                
                        }
                        
                    }

                }
                else
                {
                    DirectoryInfo directory = sourceFileSys as DirectoryInfo;
                    string directoryName = directory.Name;
                    string directoryPath = Path.Combine(path, directoryName);
                    DeleteFile(directoryPath, array);
                }
            }
        }
        catch (Exception ex)
        {
        }

    }
    private static void AddFolderBuild(PBXProject proj, string xcodePath, string root) {
        //获得源文件下所有目录文件
        string currDir = Path.Combine(xcodePath, root);
        if (root.EndsWith(".framework") || root.EndsWith(".bundle")) {
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            Debug.LogFormat("add framework or bundle to build:{0}->{1}", currDir, root);
            string fileGuid = proj.AddFile(currDir, root, PBXSourceTree.Source);
            proj.AddFileToBuild(target, fileGuid);
            AddEmbedFarmeworks(proj, target, fileGuid, root);
            return;
        }
        List<string> folders = new List<string>(Directory.GetDirectories(currDir));
        foreach (string folder in folders) {
            string name = Path.GetFileName(folder);
            string t_path = Path.Combine(currDir, name);
            string t_projPath = Path.Combine(root, name);
            if (folder.EndsWith(".framework") || folder.EndsWith(".bundle")) {
                string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
                Debug.LogFormat("add framework or bundle to build:{0}->{1}", t_path, t_projPath);
                string fileGuid = proj.AddFile(t_path, t_projPath, PBXSourceTree.Source);
                proj.AddFileToBuild(target, fileGuid);
                AutoAddSearchPath(proj, xcodePath, t_path);
                AddEmbedFarmeworks(proj, target, fileGuid, t_projPath);
            }
            else {
                AddFolderBuild(proj, xcodePath, t_projPath);
            }
        }
        List<string> files = new List<string>(Directory.GetFiles(currDir));
        foreach (string file in files) {
            if (NeedCopy(file)) {
                string name = Path.GetFileName(file);
                string t_path = Path.Combine(currDir, name);
                string t_projPath = Path.Combine(root, name);
                string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
                string fileGuid = proj.AddFile(t_path, t_projPath, PBXSourceTree.Source);
                proj.AddFileToBuild(target, fileGuid);
                AutoAddSearchPath(proj, xcodePath, t_path);
                Debug.Log("add file to build:" + Path.Combine(root, file));
                AddEmbedFarmeworks(proj, target, fileGuid,t_projPath);
            }
        }
    }
    //添加动态库
    private static void AddEmbedFarmeworks(PBXProject proj, string target, string fileGuid,string projPath)
    {
        ArrayList embeds =  _table.SGet<ArrayList>("embeds");
        if (embeds != null)
        {
            foreach (string embed in embeds)
            {
                if (projPath.Contains(embed))
                {
                    PBXProjectExtensions.AddFileToEmbedFrameworks(proj, target, fileGuid);
                }
            }
        }
    }
    //在复制文件加入工程时，当文件中有framework、h、a文件时，自动添加相应的搜索路径
    private static void AutoAddSearchPath(PBXProject proj, string xcodePath, string filePath) {
        if (filePath.EndsWith(".framework")) {//添加框架搜索路径
            string addStr = "$PROJECT_DIR" + Path.GetDirectoryName(filePath.Replace(xcodePath,""));
            Hashtable arg = new Hashtable();
            Hashtable add = new Hashtable();
            arg.Add("+",add);
            arg.Add("=", new Hashtable());
            arg.Add("-", new Hashtable());
            var array = new ArrayList();
            array.Add(addStr);
            add.Add("FRAMEWORK_SEARCH_PATHS", array);
            SetBuildProperties(proj, arg);
        }
        else if(filePath.EndsWith(".h")){//添加头文件搜索路径
            string addStr = "$PROJECT_DIR" + Path.GetDirectoryName(filePath.Replace(xcodePath, ""));
            Hashtable arg = new Hashtable();
            Hashtable add = new Hashtable();
            arg.Add("+", add);
            arg.Add("=", new Hashtable());
            arg.Add("-", new Hashtable());
            var array = new ArrayList();
            array.Add(addStr);
            add.Add("HEADER_SEARCH_PATHS", array);
            SetBuildProperties(proj, arg);
        }
        else if (filePath.EndsWith(".a")) {//添加静态库搜索路径
            string addStr = "$PROJECT_DIR" + Path.GetDirectoryName(filePath.Replace(xcodePath, ""));
            Hashtable arg = new Hashtable();
            Hashtable add = new Hashtable();
            arg.Add("+", add);
            arg.Add("=", new Hashtable());
            arg.Add("-", new Hashtable());
            var array = new ArrayList();
            array.Add(addStr);
            add.Add("LIBRARY_SEARCH_PATHS", array);
            SetBuildProperties(proj, arg);
        }
    }
    private static void SetFilesCompileFlag(PBXProject proj,Hashtable arg) {
        string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        foreach (DictionaryEntry i in arg) {
            string fileProjPath = i.Key.ToString();
            string fguid = proj.FindFileGuidByProjectPath(fileProjPath);
            ArrayList des = i.Value as ArrayList;
            List<string> list = new List<string>();
            foreach (var flag in des) {
                list.Add(flag.ToString());
            }
            proj.SetCompileFlagsForFile(target, fguid, list);
        }
    }
    private static bool NeedCopy(string file) {
        string fileName = Path.GetFileNameWithoutExtension(file);
        string fileEx = Path.GetExtension(file);
        if (fileName.StartsWith(".") || file.EndsWith(".gitkeep") || file.EndsWith(".DS_Store")) {
            return false;
        }
        return true;
    }
    // add shell script
    private static void AddShellScript(PBXProject proj, ArrayList arg)
    {
        string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        if (arg != null)
        {
            foreach (object i in arg)
            {
                Hashtable table = i as Hashtable;
                string shellName = table.SGet<string>("shellName");
                string shellPath = table.SGet<string>("shellPath");
                string shellScript = File.ReadAllText(table.SGet<string>("shellScript"));
                string shell = proj.GetShellScriptBuildPhaseForTarget(target,shellName,shellPath,shellScript);
                if (string.IsNullOrEmpty(shell))
                    proj.AddShellScriptBuildPhase(target, shellName, shellPath, shellScript);
            }
        }
    }
    //修改oc代码
    private static void ReviseClass()
    {
        string path = Path.Combine(System.Environment.CurrentDirectory, "iOSAdd/Files/HwAdsInterface.m");
        XClass hwAdsInterface = new XClass(path);
        hwAdsInterface.Replace("call(\"555555555\")", "call(\"66666666\")");
    }
    /// <summary>
    /// 在生成android apk前，将一些配置写入AndroidManifest.xml
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <param name="path"></param>
    public static void OnPreprocessBuild_android(BuildTarget buildTarget, string path)
    {
        string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);
        if (xmlDoc == null) return;
        XmlNode node = xmlDoc.SelectSingleNode("/manifest");
        node = FindNode(xmlDoc, "/manifest/application", "android:launchMode", "singleTop");
        node.Attributes["android:launchMode"].Value = "singleTop";
    }
    static XmlNode FindNode(XmlDocument xmlDoc, string xpath, string attributeName, string attributeValue)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes(xpath);
        //Debug.Log(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);
            string _attributeValue = node.Attributes[attributeName].Value;
            if (_attributeValue == attributeValue)
            {
                return node;
            }
        }
        return null;
    }
}
