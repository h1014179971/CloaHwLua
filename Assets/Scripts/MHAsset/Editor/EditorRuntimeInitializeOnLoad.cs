//
// EditorRuntimeInitializeOnLoad.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2020 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace libx
{

    public class EditorRuntimeInitializeOnLoad : ScriptableObject
    {
        [Tooltip("是否开启模拟Bunlde模式")]
        [Header("OnValueChanged")]
        public  bool SimulateMode;
        [Tooltip("是否开启Debug日志模式")]
        [Header("OnValueChanged")]
        public bool DebugMode = Application.platform == RuntimePlatform.WindowsEditor ? true : false;
        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize()
        {
            Assets.runtimeMode = GetSetting().SimulateMode;
            AppConst.DebugMode = GetSetting().DebugMode;
            if (Assets.runtimeMode) return;
            Assets.basePath = BuildScript.outputPath + Path.DirectorySeparatorChar;     
            Assets.loadDelegate = AssetDatabase.LoadAssetAtPath; 
            var assets = new List<string>();
            var rules = BuildScript.GetBuildRules();
            foreach (var asset in rules.scenesInBuild)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                assets.Add(path); 
            } 
            foreach (var rule in rules.rules)
            {
                if (rule.searchPattern.Contains("*.unity"))
                {
                    assets.AddRange(rule.GetAssets());
                }
            }  
            //var scenes = new EditorBuildSettingsScene[assets.Count];
            //for (var index = 0; index < assets.Count; index++)
            //{
            //    var asset = assets[index]; 
            //    scenes[index] = new EditorBuildSettingsScene(asset, true);
            //}
            //EditorBuildSettings.scenes = scenes;
        }

        [InitializeOnLoadMethod]
        private static void OnEditorInitialize()
        {
            EditorUtility.ClearProgressBar();
            // BuildScript.GetManifest();
            //BuildScript.GetBuildRules();
            var buildRules =  BuildScript.GetBuildRules();
            BuildRule[] rules = buildRules.rules;
            for(int i = 0;i< rules.Length; i++)
            {
                Delivery.EditorGameLauncher._searchPath.Add(rules[i].searchPath);
            }
            
        }
        private static EditorRuntimeInitializeOnLoad GetSetting()
        {
            return GetAsset<EditorRuntimeInitializeOnLoad>("Assets/Plugins/AppSetting.asset");
        }
        private static T GetAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }
}
#endif