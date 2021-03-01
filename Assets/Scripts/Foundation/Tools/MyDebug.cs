using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using System.Reflection;  
using UnityEditor;  
using UnityEditor.Callbacks;  
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Foundation
{
    public class MyDebug
    {
        public static Dictionary<string, MyDebug> MyDebugs = new Dictionary<string, MyDebug>();

        //是否开启日志系统
        public bool DebugOut = true;    
        private string module;
        private string color;

        public string Module
        {
            get
            {
                return this.module;
            }
        }
        private MyDebug(string module, string color)
        {
            this.module = module;
            this.color = color;
#if RELEASE
            DebugOut = false;
#endif
        }

        public static MyDebug Create(string module, string color = "black")   //用于创建自己喜欢的Log
        {
            if (MyDebug.MyDebugs.ContainsKey(module))
            {
                return MyDebug.MyDebugs[module];
            }
            MyDebug myDebug = new MyDebug(module, color);
            MyDebug.MyDebugs.Add(module, myDebug);

            return myDebug;
        }
        public void Log(string message)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}|<b><color={2}> {3}|      {4}</color></b>",
                    new object[]{
                        DateTime.Now.ToShortTimeString(),
                        "INFO",
                        this.color,
                        this.module,
                        message
                    });
                Debug.Log(text);  //Unity引擎使用
            }
        }

        public void LogError(object message)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}|<color=red>{2}|   {3}</color>",
                    new object[]{
                        DateTime.Now.ToShortTimeString(),
                        "ERROR",
                        this.module,
                        message
                    });
                Debug.LogError(text);  //Unity引擎使用
            }
        }

        public void LogException(Exception exception)
        {
            if (this.DebugOut)
            {
                Debug.LogException(exception);
            }
        }

        public void LogWarning(object message)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}|<color=yellow><b>{2}|    {3}</b></color>", new object[]
                {
                    DateTime.Now.ToShortTimeString(),
                    "WARNING",
                    this.module,
                    message
                });
                Debug.LogWarning(text);
            }
        }

        public void LogFormat(string format, params object[] args)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}<b><color=yellow>{2}|   {3}</color></b>", new object[]
                {
                   DateTime.Now.ToShortTimeString(),
                   "INFO",
                   this.module,
                   format
                });
                Debug.LogFormat(text, args);
            }
        }
        public void LogErrorFormat(string format, params object[] args)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}|<color=red>{2}|    {3}</color>", new object[]
                {
                    DateTime.Now.ToShortTimeString(),
                    "ERROR",
                    this.module,
                    format
                });
                Debug.LogErrorFormat(text, args);
            }
        }
        public void LogWarningFormat(string format, params object[] args)
        {
            if (this.DebugOut)
            {
                string text = string.Format("{0} |{1}|<b><color=yellow>{2}|    {3}</color></b>", new object[]
                {
                    DateTime.Now.ToShortTimeString(),
                    "WARNING",
                    this.module,
                    format
                });
                Debug.LogErrorFormat(text, args);
            }
        }
        public static MyDebug Sys = MyDebug.Create("SYS", "#000000ff");


        public static MyDebug Res = MyDebug.Create("RES", "#008000ff");


        public static MyDebug Net = MyDebug.Create("NET", "#add8e6ff");


        public static MyDebug UI = MyDebug.Create("UI", "#008080ff");
    }  
}


