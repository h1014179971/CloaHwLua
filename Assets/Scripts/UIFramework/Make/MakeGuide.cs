using UnityEngine;
using System.Collections.Generic;
using System;

namespace UIFramework
{
    [Serializable]
    public class GuideUI
    {
        public GameObject go;
        public string hierarchyPath;
        public string speak; //说的话
        public string name; //头像名字
        public string nextNameUI; //下一个弹出的ui

        public GuideUI(GameObject go, string hierarchyPath)
        {
            this.go = go;
            this.hierarchyPath = hierarchyPath;
        }
    }

    public class MakeGuide : MonoBehaviour
    {

        public List<GuideUI> guideList = new List<GuideUI>();

    }
}

