using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.XCodeEditor;
using yx;

public class XCodeCheckConfig : EditorWindow
{
    private static string _logStr;
    private static int _logline;
    private static XCodeCheckConfig instance;
    private static Vector2 scrollPos;
    /// <summary>
    /// 配置文件路径
    /// </summary>
    private static string _configPath;
    /// <summary>
    /// xcode工程路径
    /// </summary>
    private static string _xcodePath;
    private static Hashtable _table;
    [MenuItem("MHFrameWork/XCodeCheck",false,1)]
    private static void ShowWindow()
    {
        InitData();
        instance = EditorWindow.GetWindow<XCodeCheckConfig>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        instance.Show();
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("配置表：", GUILayout.Width(150));
        _configPath = EditorGUILayout.TextField("", _configPath);
        if (GUILayout.Button("选择配置表", GUILayout.Width(150)))
        {
            _configPath = EditorUtility.OpenFilePanel("Open existing config json...", Application.dataPath, "json");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("xcode工程路径：", GUILayout.Width(150));
        _xcodePath = EditorGUILayout.TextField("", _xcodePath);
        if (GUILayout.Button("选择xcode工程路径", GUILayout.Width(150)))
        {
            _xcodePath = EditorUtility.OpenFolderPanel("xcode工程路径", Application.dataPath, "");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Check"))
        {
            if(string.IsNullOrEmpty(_xcodePath) || string.IsNullOrEmpty(_configPath))
            {
                Debug.LogError("json配置路径或者xcode工程路径为空");
                return;
            }
            PlayerPrefs.SetString("configPath", _configPath);
            PlayerPrefs.SetString("xcodePath", _xcodePath);
            string json = File.ReadAllText(_configPath);
            Hashtable table = json.hashtableFromJson();
            ChcekInit(_xcodePath,table);
        }
        GUILayout.EndHorizontal();
    }
    public static void InitData()
    {
        _logStr = "";
        _logline = 0;
        _configPath = PlayerPrefs.GetString("configPath");
        _xcodePath = PlayerPrefs.GetString("xcodePath");
    }
    /// <summary>
    /// 开始检测
    /// </summary>
    /// <param name="path">xcode工程路径</param>
    /// <param name="table">json解析后hashtable</param>
    public static void ChcekInit(string path, Hashtable table)
    {
        InitData();
        _table = table;
        string projPath = PBXProject.GetPBXProjectPath(path);
        PBXProject proj = new PBXProject();

        proj.ReadFromString(File.ReadAllText(projPath));
        CheckLibs(proj,table.SGet<Hashtable>("libs"));
        CheckFramework(proj, table.SGet<Hashtable>("frameworks"));
        ChcekBuildProperties(proj, table.SGet<Hashtable>("properties"));
        //plist
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;

        CheckPlist(proj, rootDict, table.SGet<Hashtable>("plist"));
        EditorUtility.DisplayDialog("XCode工程检查信息",_logStr,"确认");
        RecordLog();

    }

    private static void CheckLibs(PBXProject proj, Hashtable table)
    {
        if(table != null)
        {
            string target = proj.TargetGuidByName(proj.GetUnityFrameworkTargetGuid());
            ArrayList addList = table["+"] as ArrayList;
            if(addList != null)
            {
                foreach(string i in addList)
                {
                    if (proj.ContainsFileByProjectPath("Frameworks/"+i))
                        Debug.Log($"{i} lib在工程里");
                    else
                    {
                        OutPutLog($"{i} lib不在工程里");
                        //Debug.LogError($"{i} lib不在工程里");
                    }

                        
                }

            }

        }

    }
    private static void CheckFramework(PBXProject proj,Hashtable table)
    {
        if (table != null)
        {
            string target = proj.TargetGuidByName(proj.GetUnityFrameworkTargetGuid());
            ArrayList addList = table["+"] as ArrayList;
            if(addList != null)
            {
                foreach(string i in addList)
                {
                    if (proj.ContainsFramework(target, i))
                    {
                        Debug.Log($"{i} 在工程里");
                        if(OptionalFarmework(i))
                        {
                            OutPutLog($"检查下{i} 是否是optional 库");
                        }

                    } 
                    else
                    {
                        
                        OutPutLog($"{i}不在工程里");
                        //Debug.LogError($"{i} farmework不在工程里");
                    }
                        

                }

            }

        }

    }
    private static bool OptionalFarmework( string framework)
    {
        ArrayList optionals = _table.SGet<ArrayList>("optionals");
        if (optionals != null)
        {
            foreach (string optional in optionals)
            {
                if (optional == framework)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private static void ChcekBuildProperties(PBXProject proj,Hashtable table)
    {
        if (table != null)
        {
            string target = proj.TargetGuidByName(proj.GetUnityFrameworkTargetGuid());
            Hashtable setTable = table.SGet<Hashtable>("=");
            foreach (DictionaryEntry i in setTable)
            {
                string propertiesValue = proj.GetBuildPropertyForAnyConfig(target,i.Key.ToString());
                if (!string.IsNullOrEmpty(propertiesValue) && propertiesValue.Equals(i.Value.ToString()))
                    Debug.Log($"properties {i.Key.ToString()}={i.Value.ToString()}");
                else
                {
                    OutPutLog($"properties {i.Key.ToString()}!={i.Value.ToString()}");
                    //Debug.LogError($"properties {i.Key.ToString()}!={i.Value.ToString()},{i.Key.ToString()}={propertiesValue}");
                }

                    
            }
            Hashtable addTable = table.SGet<Hashtable>("+");
            foreach (DictionaryEntry i in addTable)
            {
                string propertiesValue = proj.GetBuildPropertyForAnyConfig(target, i.Key.ToString());
                ArrayList array = i.Value as ArrayList;
                List<string> list = new List<string>();
                foreach (var flag in array)
                {
                    if (propertiesValue.Contains(flag.ToString()))
                        Debug.Log($"properties {i.Key.ToString()}已添加{flag.ToString()}");
                    else
                    {
                        OutPutLog($"properties {i.Key.ToString()}未添加{flag.ToString()}");
                        //Debug.LogError($"properties {i.Key.ToString()}未添加{flag.ToString()}");
                    }
                        
                }
            }
            Hashtable removeTable = table.SGet<Hashtable>("-");
            foreach (DictionaryEntry i in removeTable)
            {
                string propertiesValue = proj.GetBuildPropertyForAnyConfig(target, i.Key.ToString());
                ArrayList array = i.Value as ArrayList;
                List<string> list = new List<string>();
                foreach (var flag in array)
                {
                    if (propertiesValue.Equals(flag.ToString()))
                    {
                        OutPutLog($"properties {i.Key.ToString()}未删除{flag.ToString()}");
                        //Debug.LogError($"properties {i.Key.ToString()}未删除{flag.ToString()}");
                    }  
                    else
                        Debug.Log($"properties {i.Key.ToString()}已删除{flag.ToString()}");
                }
            }
        }
    }
    private static void CheckPlist(PBXProject proj,PlistElementDict node,Hashtable arg)
    {
        if (arg != null)
        {
            foreach (DictionaryEntry i in arg)
            {
                string key = i.Key.ToString();
                object val = i.Value;
                var vType = i.Value.GetType();
                if (vType == typeof(string))
                {
                    PlistElement pe = node[key];
                    if (pe == null)
                    {
                        OutPutLog($"plist {i.Key.ToString()}没有赋值");
                        //Debug.LogError($"plist {i.Key.ToString()}没有赋值");
                    }  
                    else
                    {
                        if (!pe.AsString().Equals((string)val))
                        {
                            OutPutLog($"plist {i.Key.ToString()}没有赋值成{(string)val}");
                            //Debug.LogError($"plist {i.Key.ToString()}没有赋值成{(string)val}");
                        }
                        else
                            Debug.Log($"plist {i.Key.ToString()}成功赋值{(string)val}");

                    }

                }
                else if (vType == typeof(bool))
                {
                    PlistElement pe = node[key];
                    if (pe == null)
                    {
                        OutPutLog($"plist {i.Key.ToString()}没有赋值");
                        //Debug.LogError($"plist {i.Key.ToString()}没有赋值");
                    } 
                    else
                    {
                        if (pe.AsBoolean()!= (bool)val)
                        {
                            OutPutLog($"plist {i.Key.ToString()}没有赋值成{(bool)val}");
                            //Debug.LogError($"plist {i.Key.ToString()}没有赋值成{(bool)val}");
                        }  
                        else
                            Debug.Log($"plist {i.Key.ToString()}成功赋值{(bool)val}");

                    }
                }
                else if (vType == typeof(double))
                {
                    int v = int.Parse(val.ToString());
                    PlistElement pe = node[key];
                    if (pe == null)
                    {
                        OutPutLog($"plist {i.Key.ToString()}没有赋值");
                        //Debug.LogError($"plist {i.Key.ToString()}没有赋值");
                    }  
                    else
                    {
                        if (pe.AsInteger() != v)
                        {
                            OutPutLog($"plist {i.Key.ToString()}没有赋值成{v}");
                            //Debug.LogError($"plist {i.Key.ToString()}没有赋值成{v}");
                        } 
                        else
                            Debug.Log($"plist {i.Key.ToString()}成功赋值{v}");

                    }
                }
                else if (vType == typeof(ArrayList))
                {
                    PlistElement pe = node[key];
                    if (pe == null)
                    {
                        OutPutLog($"plist {i.Key.ToString()}没有赋值");
                        //Debug.LogError($"plist {i.Key.ToString()}没有赋值");
                    }  
                    else
                    {
                        PlistElementArray elementArray = pe.AsArray();
                        ArrayList list = val as ArrayList;
                        if (elementArray.values.Count != list.Count)
                        {
                            OutPutLog($"plist {i.Key.ToString()}plist中数组大小与Hashtable的长度不相等，请仔细检查！！！");
                            //Debug.LogError($"plist {i.Key.ToString()}plist中数组大小与Hashtable的长度不相等，请仔细检查！！！");
                        }
                            
                        else
                        {
                            for(int m = 0; m < list.Count; m++)
                            {
                                CheckPlist(proj, elementArray.values[m] as PlistElementDict, list[m] as Hashtable);
                            }
                            
                        }
                    }
                }
                else if (vType == typeof(Hashtable))
                {
                    var table = val as Hashtable;
                    PlistElement pe = node[key];
                    if (pe == null)
                    {
                        OutPutLog($"plist {i.Key.ToString()}没有赋值");
                        //Debug.LogError($"plist {i.Key.ToString()}没有赋值");
                    }  
                    else
                    {
                        CheckPlist(proj,pe as PlistElementDict,table);
                    }
                }
            }
        }
    }

    private static void OutPutLog(string log)
    {
        _logline++;
        _logStr += $"{_logline}.{log}\n";
        Debug.LogError(_logStr);
    }

    private static void RecordLog()
    {
        
        string path = Path.Combine(System.Environment.CurrentDirectory, "xcodeCheckLog.txt");
        FileInfo fs = new FileInfo(path);
        fs.Delete();
        StreamWriter sw;
        if (!fs.Exists)
            sw = fs.CreateText();
        else
            sw = fs.AppendText();
        sw.Write(_logStr);
        sw.Close();
        sw.Dispose();
    }
}
