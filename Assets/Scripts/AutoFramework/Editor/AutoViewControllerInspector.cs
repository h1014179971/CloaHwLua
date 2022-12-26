#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AutoCode
{
    [CustomEditor(typeof(AutoViewController), true)]
    public class AutoViewControllerInspector : Editor
    {
        [MenuItem("GameObject/AutoFramework/@(Alt+V)Add View Controller &v", false, 0)]
        static void AddView()
        {
            if(Selection.objects == null || Selection.objects.Length <=0)
            {
                Debug.LogError("需要选择 GameObject");
                return;
            }
            var gameObject = Selection.objects.First() as GameObject;

            if (!gameObject)
            {
                Debug.LogError("需要选择 GameObject");
                return;
            }

            var view = gameObject.GetComponent<AutoViewController>();

            if (!view)
            {
                gameObject.AddComponent<AutoViewController>();
            }
        }

        private AutoViewControllerInspectorLocale mLocaleText = new AutoViewControllerInspectorLocale();


        public AutoViewController ViewController => target as AutoViewController;


        private void OnEnable()
        {
            if (string.IsNullOrEmpty(ViewController.ScriptsFolder))
            {
                var setting = AutoCodeGenKitSetting.Load();
                ViewController.ScriptsFolder = setting.ScriptDir;
            }

            if (string.IsNullOrEmpty(ViewController.PrefabFolder))
            {
                var setting = AutoCodeGenKitSetting.Load();
                ViewController.PrefabFolder = setting.PrefabDir;
            }

            if (string.IsNullOrEmpty(ViewController.ScriptName))
            {
                ViewController.ScriptName = ViewController.name;
            }

            if (string.IsNullOrEmpty(ViewController.Namespace))
            {
                var setting = AutoCodeGenKitSetting.Load();
                ViewController.Namespace = setting.Namespace;
            }
        }


        private readonly AutoViewControllerInspectorStyle mStyle = new AutoViewControllerInspectorStyle();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginVertical("box");

            GUILayout.Label(mLocaleText.CodegenPart, mStyle.BigTitleStyle.Value);

            //LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor);
            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.TxtFontDesc, GUILayout.Width(150));
            ViewController.TxtFont = (Font)EditorGUILayout.ObjectField(ViewController.TxtFont, typeof(Font), true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(mLocaleText.ReplaceFont, GUILayout.Height(30)))
            {
                if (ViewController.TxtFont == null)
                    Debug.LogError($"未选择需要替换的字体");
                else
                {
                    Text[] txts = ViewController.gameObject.GetComponentsInChildren<Text>();
                    for(int i = 0; i < txts.Length; i++)
                    {
                        Text txt = txts[i];
                        txt.font = ViewController.TxtFont;
                    }
                    Debug.Log($"字体替换成功");
                }
                

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.Namespace, GUILayout.Width(150));
            ViewController.Namespace = EditorGUILayout.TextArea(ViewController.Namespace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.ScriptName, GUILayout.Width(150));
            ViewController.ScriptName = EditorGUILayout.TextArea(ViewController.ScriptName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.ScriptsFolder, GUILayout.Width(150));
            ViewController.ScriptsFolder =
                EditorGUILayout.TextArea(ViewController.ScriptsFolder, GUILayout.Height(30));

            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField(mLocaleText.DragDescription);
            var sfxPathRect = EditorGUILayout.GetControlRect();
            sfxPathRect.height = 100;
            GUI.Box(sfxPathRect, string.Empty);
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(85));
            if (
                Event.current.type == EventType.DragUpdated
                && sfxPathRect.Contains(Event.current.mousePosition)
            )
            {
                //改变鼠标的外表  
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    if (DragAndDrop.paths[0] != "")
                    {
                        var newPath = DragAndDrop.paths[0];
                        ViewController.ScriptsFolder = newPath;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }
            }


            GUILayout.BeginHorizontal();
            ViewController.GeneratePrefab =
                GUILayout.Toggle(ViewController.GeneratePrefab, mLocaleText.GeneratePrefab);
            GUILayout.EndHorizontal();

            if (ViewController.GeneratePrefab)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(mLocaleText.PrefabGenerateFolder, GUILayout.Width(150));
                ViewController.PrefabFolder =
                    GUILayout.TextArea(ViewController.PrefabFolder, GUILayout.Height(30));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(mLocaleText.DragDescription);

                var dragRect = EditorGUILayout.GetControlRect();
                dragRect.height = 100;
                GUI.Box(dragRect, string.Empty);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Height(85));
                if (
                    Event.current.type == EventType.DragUpdated
                    && dragRect.Contains(Event.current.mousePosition)
                )
                {
                    //改变鼠标的外表  
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        if (DragAndDrop.paths[0] != "")
                        {
                            var newPath = DragAndDrop.paths[0];
                            ViewController.PrefabFolder = newPath;
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }
                    }
                }
            }

            var fileFullPath = ViewController.ScriptsFolder + "/" + ViewController.ScriptName + ".cs";
            if (File.Exists(ViewController.ScriptsFolder + "/" + ViewController.ScriptName + ".cs"))
            {
                var scriptObject = AssetDatabase.LoadAssetAtPath<MonoScript>(fileFullPath);
                if (GUILayout.Button(mLocaleText.OpenScript, GUILayout.Height(30)))
                {
                    AssetDatabase.OpenAsset(scriptObject);
                }

                if (GUILayout.Button(mLocaleText.SelectScript, GUILayout.Height(30)))
                {
                    Selection.activeObject = scriptObject;
                }
            }


            if (GUILayout.Button(mLocaleText.Generate, GUILayout.Height(30)))
            {
                AutoCodeGenKit.Generate(ViewController);
                GUIUtility.ExitGUI();
            }

            GUILayout.EndVertical();
        }
    }
}

#endif
