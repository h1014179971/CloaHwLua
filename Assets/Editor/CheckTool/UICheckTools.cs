using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using CYUtils;
using System.IO;

namespace UIEditor
{
    public class DepAltasInfo
    {
        public string abName;
        public List<string> spriteLst = new List<string>();
    }

    public class UIPrefabInfo
    {
        public string uiPath = "";
        public Dictionary<string, DepAltasInfo> deps = new Dictionary<string, DepAltasInfo>();

        public UnityEngine.Object obj
        {
            get
            {
                if (string.IsNullOrEmpty(uiPath))
                {
                    return null;
                }
                return AssetDatabase.LoadAssetAtPath(uiPath, typeof(GameObject));
            }
        }
    }

    /// <summary>
    /// 主要是用来负责做UI 图集检查的
    /// </summary>
    public class UICheckTools
    {

        public static UIPrefabInfo GetDep(string asset)
        {
            asset = asset.Replace("\\", "/");
            if (!asset.StartsWith("Prefabs"))
            {
                EditorUtility.DisplayDialog("错误", "请选择UI预设文件", "确定");
                return null;
            }
            var info = new UIPrefabInfo();
            info.uiPath = asset;
            string[] deps = AssetDatabase.GetDependencies(new string[] { asset });
            #region 构造依赖的图集信息
            foreach (string s in deps)
            {
                string tmp = "";
                string abName = "";
                if (s.StartsWith("Atlas"))
                {
                    int ind = s.Replace("\\", "/").LastIndexOf("/");
                    tmp = s.Substring(0, ind);
                    abName = CYUtils.FileUtils.GetFileName("Atlas") + "/" + FileUtils.GetFileName(tmp);
                }
                else if (s.StartsWith("Texture"))
                {
                    tmp = FileUtils.GetFileName(s);
                    abName = CYUtils.FileUtils.GetFileName("Texture") + "/" + tmp;
                }
                if (!string.IsNullOrEmpty(abName))
                {
                    if (info.deps.ContainsKey(abName))
                    {
                        info.deps[abName].spriteLst.Add(s);
                    }
                    else
                    {
                        var abInfo = new DepAltasInfo();
                        abInfo.abName = abName;
                        abInfo.spriteLst.Add(s);
                        info.deps.Add(abName, abInfo);
                    }
                }
            }
            #endregion
            return info;
        }

        [MenuItem("Assets/检查UI", false, 4000)]
        public static void CheckUI()
        {
            var obj = Selection.activeObject;
            if (obj == null)
            {
                return;
            }
            var path = AssetDatabase.GetAssetPath(obj);
            var info = GetDep(path);
            UIDepCheckEditor.Instance.ShowUI(info);
            //PackerTools.PublishPC();
        }

        [MenuItem("Assets/修复Text美术字RichText设置错误", false, 4001)]
        public static void RemoveRickText()
        {
            //递归获取项目中所有的材质球
            List<string> fileLst = new List<string>();
            string subPath = "XNRes/AssetBundle/Prefabs/UI/";
            string path = System.IO.Path.Combine(Application.dataPath, subPath);

            string fPath = "Assets/XNRes/AssetBundle/Font/FZY4JW/FZY4JW.TTF";
            Font font = AssetDatabase.LoadAssetAtPath(fPath, typeof(Font)) as Font;
            if (font == null)
            {
                Debug.LogError("FZY4JW Font is null");
                return;
            }

            FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                List<Text> texts = new List<Text>();
                getTextComponent(go.transform, ref texts);
                for (int i = 0; i < texts.Count; i++)
                {
                    Text t = texts[i];
                    if (t.font != null)
                    {
                        if (!t.font.dynamic && t.supportRichText == true)
                        {
                            Debug.Log("go - " + go + " Text = " + t.gameObject + " Art font not support RichText");
                            t.supportRichText = false;
                            t.lineSpacing = 0;
                            EditorUtility.SetDirty(go);
                        }

                        if (!t.font.dynamic && t.resizeTextForBestFit == true)
                        {
                            Debug.Log("go - " + go + " Text = " + t.gameObject + " is BestFit");
                            t.resizeTextForBestFit = false;
                            EditorUtility.SetDirty(go);
                        }

                        if (t.font.name == "Arial")
                        {
                            Debug.Log("go - " + go + " Text = " + t.gameObject + " is has Arial");
                            t.font = font;
                            EditorUtility.SetDirty(go);
                        }
                        //if (t.font.dynamic)
                        //{
                        //    int size = t.fontSize;
                        //    t.fontSize = t.fontSize - 1;
                        //    EditorUtility.SetDirty(go);
                        //}
                    }
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            Debug.Log("check over");
        }

        [MenuItem("Assets/查找挂载的废弃脚本", false, 4002)]
        public static void MissingScriptFinder()
        {
            List<string> fileLst = new List<string>();
            string subPath = "XNRes/AssetBundle/Prefabs/";
            string path = System.IO.Path.Combine(Application.dataPath, subPath);

            FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
            foreach (string s in fileLst)
            {
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                //Debug.Log(go.name);
                FindMissionRefInGo(go);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            Debug.Log("废弃脚本查找完成");
        }

        private static void FindMissionRefInGo(GameObject go)
        {
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            MonoBehaviour c = null;
            for (int i = 0; i < components.Length; i++)
            {
                // Missing components will be null, we can't find their type, etc.
                c = components[i];
                if (components[i] == null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(go);
                    if (assetPath != "" && assetPath != null)
                    {
                        Debug.LogError("missing script: " + GetHierarchyName(go.transform) + "-->" + assetPath);
                    }
                    else
                    {
                        Debug.LogError("missing script: " + GetHierarchyName(go.transform));
                    }
                    continue;
                }
            }
            foreach (Transform t in go.transform)
            {
                FindMissionRefInGo(t.gameObject);
            }
        }

        private static void getTextComponent(Transform trans, ref List<Text> texts)
        {
            Text t = trans.GetComponent<Text>();
            if (t != null)
                texts.Add(t);
            for (int i = 0; i < trans.childCount; i++)
            {
                getTextComponent(trans.GetChild(i), ref texts);
            }
        }

        public static string GetHierarchyName(Transform t)
        {
            if (t == null)
                return "";

            var pname = GetHierarchyName(t.parent);
            if (pname != "")
            {
                return pname + "/" + t.gameObject.name;
            }
            return t.gameObject.name;
        }
    }

    //UI依赖显示
    public class UIDepCheckEditor : EditorWindow
    {
        static private UIDepCheckEditor _instance;
        private UIPrefabInfo _info;
        private UnityEngine.Object obj;
        private DepAltasInfo selectAltasInfo;
        private Vector2 pos = Vector2.zero;
        public static UIDepCheckEditor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (UIDepCheckEditor)EditorWindow.GetWindow(typeof(UIDepCheckEditor));
#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
                    _instance.title = "UI依赖检查";
#else
					_instance.titleContent = new GUIContent("UI依赖检查");
#endif
                    _instance.maxSize = new Vector2(500, 400);
                    _instance.minSize = new Vector2(500, 400);
                }
                return UIDepCheckEditor._instance;
            }
        }
        void OnGUI()
        {
            //EditorGUILayout.ObjectField
            EditorGUILayout.BeginHorizontal();
            obj = EditorGUILayout.ObjectField("检查预设", obj, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("检查"))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (path != _info.uiPath)
                {
                    _info = UICheckTools.GetDep(path);
                    selectAltasInfo = null;
                }
                return;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("依赖图片资源", GUILayout.Width(100));
            foreach (var dep in _info.deps)
            {
                if (GUILayout.Button(dep.Key))
                {
                    selectAltasInfo = dep.Value;
                }
            }
            if (selectAltasInfo != null)
            {
                EditorGUILayout.LabelField(selectAltasInfo.abName + " 详情:");
                pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(500));
                foreach (var s in selectAltasInfo.spriteLst)
                {
                    EditorGUILayout.LabelField(s);
                    var sp = AssetDatabase.LoadAssetAtPath(s, typeof(Texture));
                    sp = EditorGUILayout.ObjectField("", sp, typeof(Texture), false) as Texture;
                }

                EditorGUILayout.EndScrollView();
            }
        }

        public void ShowUI(UIPrefabInfo info)
        {
            _info = info;
            obj = _info.obj;
            this.Show();
        }

    }
}
