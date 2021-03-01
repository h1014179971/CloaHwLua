using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CYUtils;

public class CreateUINodeCfg
{
    private static Dictionary<int, Transform> m_nodeTrans = new Dictionary<int, Transform>();
    private static List<string> deallist = new List<string>();
    public static List<string> filelist = new List<string>();
    private static string SavePath = null;
    private static string SUFFIX = "_nodeconfig";

    private static List<string> haveStoreNodeList = new List<string>();


    [MenuItem("Assets/创建UI节点配置", false, 4011)]
    static private void Find()
    {
        SavePath = "Assets/XNRes/ToolConfig/config/data/uinode/";
        FileUtils.CheckDirection(SavePath);
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
            Dictionary<string, GameObject> fileDict = new Dictionary<string, GameObject>();
            bool isSucc = true;
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    if (!fileDict.ContainsKey(go.name))
                    {
                        fileDict.Add(go.name, go);
                        PrefabInfo prefabInfo = FindPrefabInfo(go.name);
                        if (prefabInfo == null || prefabInfo.relateClassPathList.Count == 0)
                        {
                            //Debug.LogWarning("未使用的预设prefabName:" + go.name + "\nfile:" + file);
                            continue;
                        }
                        clearmemory();
                        string className = go.name + SUFFIX;
                        string outPath = System.IO.Path.Combine(SavePath, className + ".lua");
                        FileUtils.DelFile(outPath);
                        string str = className + " = { }";
                        writerFile(str, outPath);
                        List<Transform> transList = new List<Transform>();
                        foreach (Transform trans in go.transform)
                        {
                            transList.Add(trans);
                        }
                        recursiveFind(transList);
                        bool hasWrite = printinScreen(go);
                        if (!hasWrite)
                        {
                            FileUtils.DelFile(outPath);
                        }
                        clearmemory();
                        haveStoreNodeList.Clear();
                    }
                    else
                    {
                        Debug.LogError(go.name + "与其他预设同名，请改名后再生成配置");
                        isSucc = false;
                        break;
                    }
                }
                EditorUtility.DisplayProgressBar("Find Reference", s, i / fileLst.Count);
                i++;
            }
            EditorUtility.ClearProgressBar();
            if (isSucc)
            {
                Debug.Log("创建完成-------------");
            }
            prefabClassPathDic.Clear();
            prefabInfoDic.Clear();
            classPathDic.Clear();
            allLuaClassPathDic.Clear();
            helpIdEnumDic.Clear();
        }
    }

    public static void recursiveFind(List<Transform> transList)
    {
        if(transList!=null&& transList.Count > 0)
        {
            List<Transform> subTransList = new List<Transform>();
            for (int i = 0; i < transList.Count; i++)
            {
                if (transList[i] != null)
                {
                    m_nodeTrans.Add(transList[i].GetInstanceID(), transList[i]);
                    if (transList[i].childCount > 0)
                    {
                        foreach (Transform item in transList[i])
                        {
                            subTransList.Add(item);
                        }
                    }
                }
            }
            if (subTransList.Count > 0)
            {
                recursiveFind(subTransList);
            }
        }
    }
    public static void clearmemory()
    {
        m_nodeTrans.Clear();
    }

    public static bool printinScreen(GameObject rootGo)
    {
        int index = 0;
        bool hasWrite = false;
        foreach (KeyValuePair<int, Transform> item in m_nodeTrans)
        {
            index++;
            bool hasPrint = printinNodeScreen(item.Value.name, item.Value, rootGo);
            if (!hasWrite)
            {
                hasWrite = hasPrint;
            }
            if (index == m_nodeTrans.Count)
            {
                //显示脚本中声明未使用的节点 --TODO
                //ShowNotUsedNode(rootGo.name);
            }
        }
        return hasWrite;
    }

    public static void recursiveNode(Transform tran, GameObject rootGo)
    {
        if (tran == null || rootGo.Equals(tran.gameObject))
        {
            return;
        }
        filelist.Add(tran.name);
        if (tran.parent != null)
        {
            recursiveNode(tran.parent, rootGo);
        }
    }

    public static bool printinNodeScreen(string name, Transform tran, GameObject rootGo)
    {
        string rootName = rootGo.name;
        recursiveNode(tran, rootGo);
        for (int i = filelist.Count - 1; i >= 0; i--)
        {
            string str = filelist[i];
            if (i != 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str = str + "/";
                }
            }
            deallist.Add(str);
        }
        string className = rootName + SUFFIX;
        string showstr = "";
        foreach (var list in deallist)
        {
            showstr += list;
        }
        filelist.Clear();
        deallist.Clear();
        if (!string.IsNullOrEmpty(showstr))
        {
            //Debug.Log("name:" + name + " url:" + showstr);
            string outPath = System.IO.Path.Combine(SavePath, className + ".lua");
            FileUtils.CheckFilePath((outPath));
            string str = className + "['" + name + "']='" + showstr + "'";
            if (CheckNeedWrite(name, rootName))
            {
                if (haveStoreNodeList.Contains(name) == false)
                {
                    haveStoreNodeList.Add(name);
                    writerFile(str, outPath);
                    return true;
                }
                //else
                //{
                //    Debug.Log("=======有相同的节点被写入，请检查=======预设名：" + rootName + "\n节点名为：" + name);
                //}
            }
            else
            {
                //Debug.LogError("没有使用的节点prefabName:" + rootName + "     nodeName:" + name);
            }
        }
        return false;
    }

    private static void writerFile(string showstr, string outPath)
    {
        FileStream outStream = new FileStream(outPath, FileMode.Append);
        StreamWriter writer = new StreamWriter(outStream);
        writer.WriteLine(showstr);
        writer.Close();
    }

    // ======================================================================

    private static string UIInfoPath = "client/scripts/App/Modulus/UI/UIConf/UIInfo.lua";
    private static string UIStencilsPath = "client/scripts/App/Modulus/UI/UIConf/UIStencils.lua";

    private static Dictionary<string, List<string>> prefabClassPathDic;
    private static Dictionary<string, PrefabInfo> prefabInfoDic;
    private static Dictionary<string, string> classPathDic;
    private static Dictionary<string, string> allLuaClassPathDic;
    private static Dictionary<string, string> helpIdEnumDic;
    private static string[] specialNodeArray = new string[] { "closeBtn", "hideBtn" };
    private static string[] allLineStrArray;

    private static bool CheckNeedWrite(string nodeName, string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("prefabName为空！！！");
            return false;
        }
        PrefabInfo prefabInfo = FindPrefabInfo(prefabName);
        if (prefabInfo == null)
        {
            return false;
        }
        List<string> usedNodeList = prefabInfo.usedNodeList;
        List<string> helpIdEnumList = prefabInfo.helpIdEnumList;
        if (usedNodeList.Count == 0 && prefabInfo.isSpecial == false)
        {
            string relateClassPathStr = string.Empty;
            if (prefabInfo.relateClassPathList.Count != 0)
            {
                for (int i = 0; i < prefabInfo.relateClassPathList.Count; i++)
                {
                    relateClassPathStr += prefabInfo.relateClassPathList[i] + "\n";
                }
            }
            else
            {
                relateClassPathStr = "null";
            }
            Debug.LogError("该预设被使用的节点集合为空！请检查！！！PrefabName为：" + prefabName + "\n预设相关类路径为：" + relateClassPathStr);
            return false;
        }
        if(usedNodeList.Contains(nodeName))
        {
            prefabInfo.SetHaveCheckNode(nodeName);
            return true;
        }
        if (prefabInfo.isSpecial)
        {
            int a;
            string lastChar = nodeName.Substring(nodeName.Length - 1);
            if (int.TryParse(lastChar, out a))
            {
                return true;
            }
        }
        if (helpIdEnumList != null & helpIdEnumList.Count > 0)
        {
            StringBuilder suffixNum = new StringBuilder();
            foreach (char c in nodeName)
            {
                if (System.Convert.ToInt32(c) >= 48 && System.Convert.ToInt32(c) <= 57)
                {
                    suffixNum.Append(c);
                }
            }
            if (helpIdEnumList.Contains(suffixNum.ToString()))
            {
                return true;
            }
        }
        return false;
    }

    private static void ShowNotUsedNode(string prefabName)
    {
        string name = prefabName.ToLower();
        PrefabInfo info = FindPrefabInfo(name);
        if (info == null)
        {
            return;
        }
        List<string> realNotUsedNodeList = info.GetRealNotUsedNodeList();
        List<string> classPathList = info.relateClassPathList;
        if ( realNotUsedNodeList != null & realNotUsedNodeList.Count > 0)
        {
            string nodeNameStr = string.Empty;
            string classPathStr = string.Empty;
            for (int i = 0; i < realNotUsedNodeList.Count; i++)
            {
                nodeNameStr += (realNotUsedNodeList[i] + "\n");
            }
            for (int j = 0; j < classPathList.Count; j++)
            {
                classPathStr += (classPathList[j] + "\n");
            }
            Debug.Log("======脚本中声明未使用节点的个数======：" + realNotUsedNodeList.Count + "\n脚本路径为：" + classPathStr + "\n节点：" + nodeNameStr);
        }
    }

    //通过 预设名 找数据
    private static PrefabInfo FindPrefabInfo(string prefabName)
    {
        string name = prefabName.ToLower();
        PrefabInfo info;
        if(prefabInfoDic ==null || prefabInfoDic.Count == 0)
        {
            CreatePrefabInfoDic();
        }
        if(prefabInfoDic.TryGetValue(name, out info))
        {
            return info;
        }
        else
        {
            //Debug.LogError("该预设未被使用！预设名为："+ prefabName);
            return null;
        }
    }

    //最后进入的语句块类型
    public enum LatestStateType
    {
        None,
        ForLoopState,
        IfState,
        InitSelfState,
        InitLayoutState
    }

    private static UsedNodeData GetPrefabUsedNodeList(List<string> prefabClassPathList, out bool isSpecial)
    {
        isSpecial = false;
        UsedNodeData usedNodeData = new UsedNodeData(new List<string>(), new List<string>());
        for (int i = 0; i < prefabClassPathList.Count; i++)
        {
            UsedNodeData data = GetPrefabUsedNodeListByClassPath(prefabClassPathList[i], out isSpecial);
            if (data.usedNodeList != null && data.usedNodeList.Count > 0)
            {
                for (int j = 0; j < data.usedNodeList.Count; j++)
                {
                    if (usedNodeData.usedNodeList.Contains(data.usedNodeList[j]) == false)
                    {
                        usedNodeData.usedNodeList.Add(data.usedNodeList[j]);
                    }
                }
            }
            if (data.usedHelpIdEnumList != null && data.usedHelpIdEnumList.Count > 0)
            {
                for (int j = 0; j < data.usedHelpIdEnumList.Count; j++)
                {
                    if (usedNodeData.usedHelpIdEnumList.Contains(data.usedHelpIdEnumList[j]) == false)
                    {
                        usedNodeData.usedHelpIdEnumList.Add(data.usedHelpIdEnumList[j]);
                    }
                }
            }
        }
        return usedNodeData;
    }

    private static UsedNodeData GetPrefabUsedNodeListByClassPath(string classPath, out bool isSpecial)
    {
        isSpecial = false;
        if (string.IsNullOrEmpty(classPath))
        {
            Debug.LogError("classPath为空！！！");
            return null;
        }
        List<string> realUsedNodeList = new List<string>();
        List<string> helpIdEnumList = new List<string>();
        string parentClassName = string.Empty;
        string[] tempAllLineArray = ReadFile(classPath);
        if (tempAllLineArray != null && tempAllLineArray.Length != 0)
        {
            bool startInitSelf = false;
            bool startInitLayout = false;
            bool haveInitSelf = false;
            int forLoopCount = 0;
            int ifStateCount = 0;
            Stack<LatestStateType> stateStack = new Stack<LatestStateType>();
            string collectedNodeName = string.Empty;
            for (int j = 0; j < tempAllLineArray.Length; j++)
            {
                string lineStr = tempAllLineArray[j].Trim();
                if (lineStr.IndexOf("--") == 0)
                {
                    continue;
                }
                if (lineStr.IndexOf("SimpleClass") != -1)
                {
                    parentClassName = lineStr.Substring(lineStr.IndexOf("(") + 1).TrimEnd(')');
                }
                //进入__init_self
                if (lineStr.IndexOf("__init_self") != -1 && startInitSelf == false)
                {
                    startInitSelf = true;
                    haveInitSelf = true;
                    stateStack.Push(LatestStateType.InitSelfState);
                    continue;
                }
                //进入initLayout
                if (lineStr.IndexOf("initLayout()") != -1)
                {
                    startInitLayout = true;
                    stateStack.Push(LatestStateType.InitLayoutState);
                }
                if (startInitSelf || startInitLayout)
                {
                    //进入一个for循环
                    if (lineStr.IndexOf("for ") != -1)
                    {
                        forLoopCount++;
                        stateStack.Push(LatestStateType.ForLoopState);
                    }
                    //进入一个if语句
                    else if (lineStr.IndexOf("if ") != -1)
                    {
                        ifStateCount++;
                        stateStack.Push(LatestStateType.IfState);
                    }
                    //解析到一个end
                    else if (lineStr.IndexOf("end") == 0)
                    {
                        LatestStateType latestState = LatestStateType.None;
                        if (stateStack.Count != 0)
                        {
                            latestState = stateStack.Pop();
                        }
                        switch (latestState)
                        {
                            //问题
                            case LatestStateType.None:
                                Debug.LogError("未进入约定语句块，解析错误，请检查lua文件：" + classPath + "第" + j + "行");
                                break;
                            //退出一个for循环
                            case LatestStateType.ForLoopState:
                                forLoopCount--;
                                if (forLoopCount < 0)
                                {
                                    Debug.LogError("forLoopCount小于0，解析错误，请检查lua文件：" + classPath + "第" + j + "行");
                                }
                                break;
                            //退出一个if语句
                            case LatestStateType.IfState:
                                ifStateCount--;
                                if (ifStateCount < 0)
                                {
                                    Debug.LogError("ifStateCount小于0，解析错误，请检查lua文件：" + classPath + "第" + j + "行");
                                }
                                break;
                            //退出__init_self
                            case LatestStateType.InitSelfState:
                                startInitSelf = false;
                                break;
                            //退出initLayout
                            case LatestStateType.InitLayoutState:
                                startInitLayout = false;
                                break;
                        }
                    }
                }

                //在__init_self方法中
                if (startInitSelf)
                {
                    if (lineStr.IndexOf("__init_self") != -1)
                    {
                        //Debug.Log("该类有继承关系！路径为：" + classPath + "\n行数为：" + (j + 1));
                        string parentClassPath = GetClassPathByName(parentClassName);
                        if (string.IsNullOrEmpty(parentClassPath) == false)
                        {
                            UsedNodeData tempData = GetPrefabUsedNodeListByClassPath(parentClassPath, out isSpecial);
                            if (tempData.usedNodeList != null && tempData.usedNodeList.Count > 0)
                            {
                                for (int i = 0; i < tempData.usedNodeList.Count; i++)
                                {
                                    if (realUsedNodeList.Contains(tempData.usedNodeList[i]) == false)
                                    {
                                        realUsedNodeList.Add(tempData.usedNodeList[i]);
                                    }
                                }
                            }
                            if (tempData.usedHelpIdEnumList != null && tempData.usedHelpIdEnumList.Count > 0)
                            {
                                for (int i = 0; i < tempData.usedHelpIdEnumList.Count; i++)
                                {
                                    if (helpIdEnumList.Contains(tempData.usedHelpIdEnumList[i]) == false)
                                    {
                                        helpIdEnumList.Add(tempData.usedHelpIdEnumList[i]);
                                    }
                                }
                            }
                        }
                    }
                    if (lineStr.IndexOf("HelpIdEnum.") != -1 && lineStr.IndexOf("HelpIdEnum.btnName") != -1)
                    {
                        string str = lineStr.Substring(lineStr.LastIndexOf("HelpIdEnum."));
                        str = str.Substring(str.IndexOf('.') + 1);
                        str = str.Substring(0, str.IndexOf(']')).Trim();
                        string key = string.Empty;
                        try
                        {
                            //key = str.Substring(0, str.IndexOf(']')).Trim();
                            MatchCollection results = Regex.Matches(str, "[A-Za-z]");
                            foreach (var v in results)
                            {
                                key += v.ToString();
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log("解析出错，classPath：" + classPath + "\n行数为:" + (j + 1) + "\n" + e.ToString());
                        }
                        string value = FindHelpIdEnumValue(key);
                        if (helpIdEnumList.Contains(value) == false)
                        {
                            helpIdEnumList.Add(value);
                        }
                        else
                        {
                            Debug.LogError("helpIdEnum集合中已经包含了该值，value为：" + value);
                        }
                    }
                    else if (lineStr.IndexOf("UIWidgetEnum") != -1 || lineStr.IndexOf("UIStencilEnum") != -1)
                    {
                        //在for循环中 或 在if语句中 或 有包含"self[" 则表示该脚本的节点声明有特殊处理（组拼 等）
                        if (forLoopCount > 0 || ifStateCount > 0 || lineStr.IndexOf("self[") != -1)
                        {
                            isSpecial = true;
                            continue;
                        }
                        string str = lineStr.Substring(lineStr.IndexOf('.') + 1);
                        collectedNodeName = str.Substring(0, str.IndexOf("=")).Trim();
                        if (realUsedNodeList.Contains(collectedNodeName) == false)
                        {
                            realUsedNodeList.Add(collectedNodeName);
                        }
                    }
                }
                //不在__init_self方法中
                else
                {
                    if (lineStr.IndexOf("UIWidgetEnum") != -1 || lineStr.IndexOf("UIStencilEnum") != -1)
                    {
                        isSpecial = true;
                        continue;
                    }
                    if (lineStr.IndexOf("_click_event") != -1)
                    {
                        string nodeName = lineStr.Substring(lineStr.IndexOf(':') + 1).Trim();
                        nodeName = nodeName.Substring(0, nodeName.IndexOf("_click_event"));
                        if (realUsedNodeList.Contains(nodeName) == false)
                        {
                            realUsedNodeList.Add(nodeName);
                        }
                    }
                }
            }
            //没有重写__init_self，在父类中寻找
            if (haveInitSelf == false)
            {
                string parentClassPath = GetClassPathByName(parentClassName);
                //Debug.Log("该类没有重写__init_self！路径为：" + classPath);
                if (string.IsNullOrEmpty(parentClassPath) == false)
                {
                    UsedNodeData tempData = GetPrefabUsedNodeListByClassPath(parentClassPath, out isSpecial);
                    if (tempData.usedNodeList != null && tempData.usedNodeList.Count > 0)
                    {
                        for (int i = 0; i < tempData.usedNodeList.Count; i++)
                        {
                            if (realUsedNodeList.Contains(tempData.usedNodeList[i]) == false)
                            {
                                realUsedNodeList.Add(tempData.usedNodeList[i]);
                            }
                        }
                    }
                    if (tempData.usedHelpIdEnumList != null && tempData.usedHelpIdEnumList.Count > 0)
                    {
                        for (int i = 0; i < tempData.usedHelpIdEnumList.Count; i++)
                        {
                            if (helpIdEnumList.Contains(tempData.usedHelpIdEnumList[i]) == false)
                            {
                                helpIdEnumList.Add(tempData.usedHelpIdEnumList[i]);
                            }
                        }
                    }
                }
            }
        }
        if (specialNodeArray != null && specialNodeArray.Length > 0)
        {
            for (int j = 0; j < specialNodeArray.Length; j++)
            {
                if (realUsedNodeList.Contains(specialNodeArray[j]) == false)
                {
                    realUsedNodeList.Add(specialNodeArray[j]);
                }
            }
        }
        UsedNodeData data = new UsedNodeData(realUsedNodeList, helpIdEnumList);
        return data;
    }

    private static string GetClassPathByName(string className)
    {
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError("className为空！！！");
            return null;
        }
        if(classPathDic==null|| classPathDic.Count == 0)
        {
            Debug.LogError("classPathDic为空！！！");
            return null;
        }
        string classPath;
        if (classPathDic.TryGetValue(className, out classPath) == false)
        {
            if(allLuaClassPathDic==null|| allLuaClassPathDic.Count == 0)
            {
                CreateLuaClassPathDic();
            }
            if (allLuaClassPathDic.TryGetValue(className, out classPath) == false)
            {
                Debug.LogError("没有找到该类的路径！className为：" + className);
            }
        }
        return classPath;
    }

    private static void CreateLuaClassPathDic()
    {
        allLuaClassPathDic = new Dictionary<string, string>();
        List<string> fileList = new List<string>();
        string luaScrSubPath = "client/scripts";
        string uipath = System.IO.Path.Combine(Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")), luaScrSubPath);
        FileUtils.searchAllFiles(uipath, fileList, new List<string> { ".lua" });
        if (fileList != null && fileList.Count != 0)
        {
            string className = string.Empty;
            for (int i = 0; i < fileList.Count; i++)
            {
                className = fileList[i].Substring(fileList[i].LastIndexOf("/") + 1);
                className = className.Substring(0, className.IndexOf("."));
                if (allLuaClassPathDic.ContainsKey(className) == false)
                {
                    allLuaClassPathDic.Add(className, fileList[i]);
                }
            }
        }
    }

    private static void CreateHelpIdEnumDic()
    {
        helpIdEnumDic = new Dictionary<string, string>();
        string classPath = GetClassPathByName("HelpIdEnum");
        if (string.IsNullOrEmpty(classPath))
        {
            Debug.LogWarning("没有找到HelpIDEnum路径！");
            return;
        }
        string[] allLineStrArray = ReadFile(classPath);
        string key = string.Empty;
        string value = string.Empty;
        for (int i = 0; i < allLineStrArray.Length; i++)
        {
            string str = allLineStrArray[i];
            if (str.IndexOf("HelpIdEnum.") != -1)
            {
                key = str.Substring(str.IndexOf('.') + 1);
                key = key.Substring(0, key.IndexOf('=')).Trim();
                value = str.Substring(str.IndexOf('=') + 1);
                if (value.IndexOf("--") != -1)
                {
                    value = value.Substring(0, value.IndexOf("--"));
                }
                value = value.Trim();
            }
            if (string.IsNullOrEmpty(key) == false && string.IsNullOrEmpty(value) == false)
            {
                if (helpIdEnumDic.ContainsKey(key) == false)
                {
                    helpIdEnumDic.Add(key, value);
                }
                else
                {
                    Debug.LogError("helpIdEnumDic中已包含该Key，key为：" + key);
                }
            }
        }
    }

    private static string FindHelpIdEnumValue(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("key为空！！！");
            return null;
        }
        string value = string.Empty;
        if(helpIdEnumDic==null || helpIdEnumDic.Count == 0)
        {
            CreateHelpIdEnumDic();
        }
        if (helpIdEnumDic.TryGetValue(key, out value) == false)
        {
            Debug.LogError("没有在HelpIdEnum中找到对应的值，key为：" + key);
        }
        return value;
    }

    private static void CreatePrefabInfoDic()
    {
        prefabInfoDic = new Dictionary<string, PrefabInfo>();
        if(prefabClassPathDic==null || prefabClassPathDic.Count == 0)
        {
            CreatePrefabClassPathDic();
        }
        List<string> keys = new List<string>(prefabClassPathDic.Keys);
        foreach (string prefabName in keys)
        {
            string name = prefabName.ToLower();
            List<string> classPathList;
            prefabClassPathDic.TryGetValue(prefabName, out classPathList);
            if (classPathList == null || classPathList.Count == 0)
            {
                Debug.LogError("该预设的关联类集合为空！预设名为：" + prefabName);
                continue;
            }
            else
            {
                bool isSpecial = false;
                UsedNodeData data = GetPrefabUsedNodeList(classPathList, out isSpecial);
                if (isSpecial == false && (data.usedNodeList == null || data.usedNodeList.Count == 0))
                {
                    Debug.LogError("该预设没有被使用的节点，请先检查UIInfo/UIStencils中路径是否正确，预设名：" + prefabName);
                    continue;
                }
                else
                {
                    if (prefabInfoDic.ContainsKey(name))
                    {
                        Debug.LogError("该预设已经被添加，请检查！预设名：" + prefabName);
                        continue;
                    }
                    else
                    {
                        PrefabInfo prefabInfo = new PrefabInfo(name, classPathList, data.usedNodeList, data.usedHelpIdEnumList, isSpecial);
                        prefabInfoDic.Add(name, prefabInfo);
                    }
                }
            }
        }
    }

    private static void CreatePrefabClassPathDic()
    {
        prefabClassPathDic = new Dictionary<string, List<string>>();
        classPathDic = new Dictionary<string, string>();
        string[] tempStrArray = new string[] { UIInfoPath, UIStencilsPath };
        for (int i = 0; i < tempStrArray.Length; i++)
        {
            allLineStrArray = ReadFile(tempStrArray[i]);
            for (int j = 0; j < allLineStrArray.Length; j++)
            {
                string prefabName = string.Empty;
                string classPath = string.Empty;
                string className = string.Empty;
                if (allLineStrArray[j].IndexOf("--") != -1) continue;
                if (allLineStrArray[j].IndexOf("srouce") != -1)
                {
                    string nameStr = allLineStrArray[j].Substring(allLineStrArray[j].IndexOf('\"') + 1);
                    string pathStr = nameStr.Substring(nameStr.IndexOf('\"') + 1);
                    pathStr = pathStr.Substring(pathStr.IndexOf('\"') + 1);
                    pathStr = pathStr.Substring(0, pathStr.IndexOf('\"'));
                    nameStr = nameStr.Substring(0, nameStr.IndexOf('\"'));
                    if (string.IsNullOrEmpty(nameStr)) continue;
                    if (nameStr.IndexOf("/") != -1)
                    {
                        prefabName = nameStr.Substring(nameStr.LastIndexOf("/") + 1);
                    }
                    else
                    {
                        prefabName = nameStr;
                    }
                    if (pathStr.IndexOf(".") != -1)
                    {
                        classPath = pathStr.Replace('.', '/');
                        className = pathStr.Substring(pathStr.LastIndexOf('.') + 1);
                    }
                    else
                    {
                        classPath = pathStr;
                        className = pathStr;
                    }
                    if (string.IsNullOrEmpty(classPath)) continue;
                    classPath = "client/scripts/" + classPath + ".lua";
                    if(className == classPath)
                    {
                        Debug.LogError("类名与类路径相同！请确认，类名为：" + className);
                    }
                    if (classPathDic.ContainsKey(className) == false)
                    {
                        classPathDic.Add(className, classPath);
                    }
                    List<string> classPathList;
                    string name = prefabName.ToLower();
                    if (prefabClassPathDic.TryGetValue(name, out classPathList))
                    {
                        if (classPathList.Contains(classPath) == false)
                        {
                            classPathList.Add(classPath);
                        }
                        prefabClassPathDic.TryGetValue(name, out classPathList);
                    }
                    else
                    {
                        classPathList = new List<string>();
                        classPathList.Add(classPath);
                        prefabClassPathDic.Add(name, classPathList);
                    }
                }
            }
        }
    }

    private static string[] ReadFile(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            string[] strArray = new string[] { };
            try
            {
                strArray = File.ReadAllLines(fileName);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("没有找到lua文件fileName:"+ fileName+"\nException caught:" +e.ToString());
            }
            if (strArray.Length != 0)
            {
                return strArray;
            }
        }
        return null;
    }
}

public class PrefabInfo
{
    public string prefabName;                           //预设名
    public List<string> relateClassPathList;        //所有关联类的路径集合
    public List<string> usedNodeList;               //所有被使用节点的集合
    public List<string> haveCheckNodeList;      //被检查过的节点
    public List<string> helpIdEnumList;             //有使用helpIdEnum类组拼节点的集合
    public bool isSpecial;                                  //节点是否有被特殊处理（在__init_self中组拼获取节点）

    public PrefabInfo(string name, List<string> relateClassPath, List<string> usedNode,List<string> helpIdEnum ,  bool special)
    {
        prefabName = name;
        relateClassPathList = relateClassPath;
        usedNodeList = usedNode;
        isSpecial = special;
        helpIdEnumList = helpIdEnum;
        haveCheckNodeList = new List<string>();
    }

    public void SetHaveCheckNode(string nodeName)
    {
        haveCheckNodeList.Add(nodeName);
    }

    public List<string> GetRealNotUsedNodeList()
    {
        List<string> realNotUsedNodeList = new List<string>();
        if (usedNodeList != null && usedNodeList.Count > 0)
        {
            for (int i = 0; i < usedNodeList.Count; i++)
            {
                if (haveCheckNodeList.Contains(usedNodeList[i]) == false)
                {
                    realNotUsedNodeList.Add(usedNodeList[i]);
                }
            }
        }
        return realNotUsedNodeList;
    }
}

public class UsedNodeData
{
    public List<string> usedNodeList;
    public List<string> usedHelpIdEnumList;

    public UsedNodeData(List<string> usedNode, List<string>helpIdEnumList)
    {
        usedNodeList = usedNode;
        usedHelpIdEnumList = helpIdEnumList;
    }
}
