using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MHEditor.Build
{
    public class IosBuildWindow : EditorWindow
    {
        private static IosBuildWindow instance;
        private static Vector2 scrollPos;
        private enum Channel
        {
            NONE,
            Ohayoo
        }
        private static Channel channel;
        private static ES3File es3File;
        private static IosData iosData;
        private static string configPath;
        private static bool isReplace;

        [MenuItem("MHFrameWork/Ios Build/Replace", false, 1)]
        static void ShowWindow()
        {
            isReplace = true;
            LoadData();
            instance = EditorWindow.GetWindow<IosBuildWindow>();
            scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
            instance.Show();
            
        }
        [MenuItem("MHFrameWork/Ios Build/Append", false, 1)]
        static void SetConfig()
        {
            isReplace = false;
            LoadData();
            instance = EditorWindow.GetWindow<IosBuildWindow>();
            scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
            instance.Show();
            return;
            string path = EditorUtility.OpenFilePanel("Choice Config", System.Environment.CurrentDirectory, "json");
            Debug.Log($"config path=={path}");
            if (!string.IsNullOrEmpty(path))
                ConfigPath = path;
        }
        private static void LoadData()
        {
            es3File = new ES3File("iosBuild.es3");
            if (es3File.KeyExists(channel.ToString()))
                iosData = es3File.Load<IosData>(channel.ToString());
            else
                iosData = new IosData();


        }
        public static string ConfigPath {
            get {
                return "iOSAdd/" + channel.ToString() + "/XCodeConfig.json";
                
            }
            set { configPath = value; }
        }
        private void OnGUI()
        {
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Channel", GUILayout.Width(150));
            Channel ch = (Channel)EditorGUILayout.EnumPopup(channel);
            if (channel != ch)
            {
                channel = ch;
                LoadData();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Version*", GUILayout.Width(150));
            if(iosData !=null)
                iosData.bundleVersion = EditorGUILayout.TextField("", iosData.bundleVersion);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Build Number", GUILayout.Width(150));
            if (iosData != null)
            {
                string buildNumber = EditorGUILayout.TextField("", iosData.buildNumber);
                iosData.buildNumber = buildNumber;
            }
                
            GUILayout.EndHorizontal();
            if (!isReplace)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("UnityXcode Path", GUILayout.Width(150)))
                {
                    if (string.IsNullOrEmpty(iosData.iosPath))
                        iosData.iosPath = EditorUtility.SaveFilePanel("Build Ios", System.Environment.CurrentDirectory, "", "");
                    else
                    {
                        if (Directory.Exists(iosData.iosPath))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(iosData.iosPath);
                            iosData.iosPath = EditorUtility.SaveFilePanel("Build Ios", directoryInfo.Parent.FullName, directoryInfo.Name, "");
                        }
                        else
                            iosData.iosPath = EditorUtility.SaveFilePanel("Build Ios", iosData.iosPath, "", "");
                    }
                }
                if(iosData != null)
                    EditorGUILayout.TextField("",iosData.iosPath);

                GUILayout.EndHorizontal();
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Project"))
            {
                if (isReplace)
                    ExportProject();
                else
                    ExportAppendProject();
            }
            GUILayout.EndHorizontal();
        }
        static void Build()
        {
            PlayerSettings.bundleVersion = iosData.bundleVersion;
            PlayerSettings.iOS.buildNumber = iosData.buildNumber;
        }

        private static void ExportProject()
        {
            Build();
            
            string path = iosData.iosPath;
            if (string.IsNullOrEmpty(iosData.iosPath))
                path = EditorUtility.SaveFilePanel("Build Ios", System.Environment.CurrentDirectory, "", "");
            else
            {
                if (Directory.Exists(iosData.iosPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(iosData.iosPath);
                    path = EditorUtility.SaveFilePanel("Build Ios", directoryInfo.Parent.FullName, directoryInfo.Name, "");
                }
                else
                    path = EditorUtility.SaveFilePanel("Build Ios", iosData.iosPath, "", "");
            }
            if (!string.IsNullOrEmpty(path))
            {
                iosData.iosPath = path;
                SaveData();
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.Android);
                //var options = BuildOptions.AcceptExternalModificationsToPlayer;//指定目录下必须有xcode工程
                var options = BuildOptions.AllowDebugging;
                BuildPipeline.BuildPlayer(GetBuildScenes(), iosData.iosPath, BuildTarget.iOS, options);
                EditorUtility.RevealInFinder(iosData.iosPath);
            }
            
        }
        private static void ExportAppendProject()
        {
            Build();

            string path = iosData.iosPath;
            if (!string.IsNullOrEmpty(path))
            {
                iosData.iosPath = path;
                SaveData();
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.Android);
                string projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(path);
                
                var options = BuildOptions.AllowDebugging;
                if(File.Exists(projPath))
                    options = BuildOptions.AcceptExternalModificationsToPlayer;//指定目录下必须有xcode工程
                
                BuildPipeline.BuildPlayer(GetBuildScenes(), iosData.iosPath, BuildTarget.iOS, options);
                EditorUtility.RevealInFinder(iosData.iosPath);
            }
            else
                Debug.LogError("到处路径为空");

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
            es3File.Save<IosData>(channel.ToString(), iosData);
            es3File.Sync();
        }


        /*********************/
        public class IosData
        {
            public string bundleVersion;
            public string buildNumber;
            public string iosPath;
        }
    }
}


