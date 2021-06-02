using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace libx
{
    public class AssetBundleWindow : EditorWindow
    {
        /// <summary>
        /// 当前编辑器窗口实例
        /// </summary>
        private static AssetBundleWindow instance;
        private static Dictionary<string, string> objectDic; //<路径，obj名字>
        private static int indexOfFormat = 0;
        private static string[] formatOption = new string[] { "PC", "IOS", "ADNROID" };
        private static BuildTarget target = BuildTarget.StandaloneWindows;
        /// <summary>
        /// 项目根路径	
        /// </summary>
        private static string pathRoot;

        /// <summary>
        /// 滚动窗口初始位置
        /// </summary>
        private static Vector2 scrollPos;
        private static bool isUpLoadRemoteCDN;
        private static string CDNURL;
        private static string CDNUserName;
        private static string CDNPassword;

        private static bool isUpLoadLocalCDN;
        private static string LocalCDNURL;

        private static string PREFCDNURL = "CDNURL";
        private static string PREFCDNUSERNAME = "CDNUserName";
        private static string PREFCDNPASSWORK = "CDNPassword";
        private static string PREFLOCALCDNURL = "LocalCDNURL";


        /// <summary>
        /// 显示当前窗口	
        /// </summary>
        public static void ShowAssetBundleTools()
        {
            Init();
            target = EditorUserBuildSettings.activeBuildTarget;
            //加载Object文件
            LoadObject();
            instance.Show();
        }

        void OnGUI()
        {
            DrawOptions();
            DrawExport();
        }

        /// <summary>
        /// 绘制插件界面配置项
        /// </summary>
        private void DrawOptions()
        {
            

        }

        /// <summary>
        /// 绘制插件界面输出项
        /// </summary>
        private void DrawExport()
        {
            GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("请选择平台11:", GUILayout.Width(85));
            target = (BuildTarget)EditorGUILayout.EnumPopup("请选择平台", target);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("AssetBundle Build"))
            {
                Build();
            }
            GUILayout.BeginHorizontal();
            isUpLoadRemoteCDN = EditorGUILayout.Toggle("是否上传到远程CDN", isUpLoadRemoteCDN);
            GUILayout.EndHorizontal();
            if (isUpLoadRemoteCDN)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("远程CDN地址",GUILayout.Width(100));
                if (EditorPrefs.HasKey(PREFCDNURL))
                    CDNURL = EditorPrefs.GetString(PREFCDNURL);
                CDNURL = EditorGUILayout.TextField(CDNURL);
                EditorPrefs.SetString(PREFCDNURL,CDNURL);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("远程CDN用户名",GUILayout.Width(100));
                if (EditorPrefs.HasKey(PREFCDNUSERNAME))
                    CDNUserName = EditorPrefs.GetString(PREFCDNUSERNAME);
                CDNUserName = EditorGUILayout.TextField(CDNUserName);
                EditorPrefs.SetString(PREFCDNUSERNAME, CDNUserName);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("远程CDN密码", GUILayout.Width(100));
                if (EditorPrefs.HasKey(PREFCDNPASSWORK))
                    CDNPassword = EditorPrefs.GetString(PREFCDNPASSWORK);
                CDNPassword = EditorGUILayout.TextField(CDNPassword);
                EditorPrefs.SetString(PREFCDNPASSWORK, CDNPassword);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                isUpLoadLocalCDN = EditorGUILayout.Toggle("是否上传到本地CDN", isUpLoadLocalCDN);
                GUILayout.EndHorizontal();
                if (isUpLoadLocalCDN)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("本地CDN地址", GUILayout.Width(100));
                    if (EditorPrefs.HasKey(PREFLOCALCDNURL))
                        LocalCDNURL = EditorPrefs.GetString(PREFLOCALCDNURL);
                    LocalCDNURL = EditorGUILayout.TextField(LocalCDNURL);
                    EditorPrefs.SetString(PREFLOCALCDNURL,LocalCDNURL);
                    GUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("只上传CDN"))
            {
                UpLoad();
            }
            if(GUILayout.Button("打包并上传"))
            {
                UpLoadAndBuild();
            }

        }

        private static void Build()
        {
            var watch = new Stopwatch();
            watch.Start();
            ColaFramework.ToolKit.ColaEditHelper.BuildLuaBundle();
            AssetDatabase.SaveAssets();
            BuildScript.ApplyBuildRules();
            BuildScript.BuildAssetBundles(target);
            watch.Stop();
            Debug.Log("BuildAssetBundles " + watch.ElapsedMilliseconds + " ms.");
        }
        private static void UpLoad()
        {
            if (isUpLoadRemoteCDN)
            {
                if(string.IsNullOrEmpty(CDNURL))
                {
                    Debug.LogError($"远程服务器地址{CDNURL}为空");
                    return;
                }
                if (string.IsNullOrEmpty(CDNUserName))
                {
                    Debug.LogError($"远程服务器用户名{CDNUserName}为空");
                    CDNUserName = string.Empty;
                }
                if (string.IsNullOrEmpty(CDNPassword))
                {
                    Debug.LogError($"远程服务器秘密{CDNPassword}为空");
                    CDNPassword = string.Empty;
                }
                BuildScript.CopyAssetBundlesToRemoteCDN(CDNURL,CDNUserName,CDNPassword,target);
                return;
            }
            if (isUpLoadLocalCDN)
            {
                if (string.IsNullOrEmpty(LocalCDNURL))
                {
                    Debug.LogError($"本地服务器地址{LocalCDNURL}为空");
                    return;
                }
                BuildScript.CopyAssetBundlesToLocalCDN(LocalCDNURL,target);
                return;
            }
            if(!isUpLoadRemoteCDN && !isUpLoadLocalCDN)
            {
                Debug.LogError($"请选择上传模式");
            }
        }
        private static void UpLoadAndBuild()
        {
            Build();
            UpLoad();
        }
        private static void DecryptDES()
        {
            var enumerator = objectDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string keyStr = enumerator.Current.Key.Split('.')[0];
                string valueStr = enumerator.Current.Value.Split('.')[0];
                string jsonStr = File.ReadAllText(enumerator.Current.Key);
                //jsonStr = StringEncryption.DecryptDES(jsonStr); //////解密
                File.WriteAllText(keyStr + "_1.json", jsonStr);
            }
        }
        /// <summary>
        /// 转换Excel文件
        /// </summary>
        private static void Convert()
        {
            //BSFramework.ExportAssetBundles.BuildAssetBundleByChoose(objectDic, target);
            //BSFramework.ExportAssetBundles.BuildOneAssetBundleByChoose(objectDic, target);
            //instance.Close();

        }

        /// <summary>
        /// 加载Excel
        /// </summary>
        private static void LoadObject()
        {
            target = EditorUserBuildSettings.activeBuildTarget;
            if (objectDic == null) objectDic = new Dictionary<string, string>();
            //objectDic.Clear();
            //获取选中的对象
            object[] selection = (object[])Selection.objects;
            //判断是否有对象被选中
            if (selection.Length == 0)
                return;
            //遍历每一个对象判断不是Excel文件
            foreach (Object obj in selection)
            {
                string objPath = AssetDatabase.GetAssetPath(obj);
                if (objPath.Length > 0 && !objectDic.ContainsKey(objPath))
                {
                    objectDic.Add(objPath, obj.name);
                }
            }

        }

        private static void Init()
        {
            //获取当前实例
            instance = EditorWindow.GetWindow<AssetBundleWindow>();
            //初始化
            pathRoot = Application.dataPath;
            //注意这里需要对路径进行处理
            //目的是去除Assets这部分字符以获取项目目录
            //我表示Windows的/符号一直没有搞懂
            pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
            //excelList=new List<string>();
            objectDic = new Dictionary<string, string>();
            scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
        }

        void OnSelectionChange()
        {
            //当选择发生变化时重绘窗体
            Show();
            LoadObject();
            Repaint();
        }
    }
}

