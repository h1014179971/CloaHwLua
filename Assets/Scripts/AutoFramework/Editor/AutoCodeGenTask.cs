using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoCode
{
    public enum CodeGenTaskStatus
    {
        Search,
        Gen,
        Compile,
        Complete
    }

    public enum GameObjectFrom
    {
        Scene,
        Prefab
    }

    [System.Serializable]
    public class AutoCodeGenTask
    {
        public bool ShowLog = false;

        // state
        public CodeGenTaskStatus Status;

        // input
        public GameObject GameObject;
        public GameObjectFrom From = GameObjectFrom.Scene;


        // search
        public List<StringPair> NameToFullName = new List<StringPair>();
        public List<UIComponentInfo> ComponentTypes = new List<UIComponentInfo>();

        // info
        public string ScriptsFolder;
        public string ClassName;
        public string Namespace;

        // result
        public string MainCode;
        public string DesignerCode;
    }

    [System.Serializable]
    public class StringPair
    {
        public StringPair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key;
        public string Value;
    }
    [System.Serializable]
    public class UIComponentInfo
    {
        public string TypeName;
        public string MemberName;
        public GameObject obj;
        public string PathToRoot;
    }
}

