#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AutoCode
{
    public class AutoCodeGenKitSetting : ScriptableObject
    {
        public bool IsDefaultNamespace => Namespace == "UIFramework.Example";
        public string Namespace = "UIFramework.Example";
        public string ScriptDir = "Assets/GameAssets/Scripts/Game";
        public string PrefabDir = "Assets/GameAssets/Prefab";

        private static AutoCodeGenKitSetting mInstance;

        public static AutoCodeGenKitSetting Load()
        {
            if (mInstance) return mInstance;

            var filePath = Dir.Value + FileName;

            if (File.Exists(filePath))
            {
                return mInstance = AssetDatabase.LoadAssetAtPath<AutoCodeGenKitSetting>(filePath);
            }

            return mInstance = CreateInstance<AutoCodeGenKitSetting>();
        }

        public void Save()
        {
            var filePath = Dir.Value + FileName;
            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(this, Dir.Value + FileName);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly Lazy<string> Dir =
            new Lazy<string>(() => "Assets/GameAssets/CodeGenKit/".CreateDirIfNotExists());

        private const string FileName = "Setting.asset";
    }
}

#endif
