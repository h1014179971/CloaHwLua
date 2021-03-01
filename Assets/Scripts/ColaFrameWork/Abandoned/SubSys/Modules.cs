//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaAbandoned
{
    /// <summary>
    /// 存储所有的系统
    /// 将新增的系统添加到AllModules列表中即可，ModuleMgr会自动记录\注册所有的系统
    /// </summary>
    public class Modules
    {
        public List<ModuleBase> AllModules;

        public Modules()
        {
            Init();
        }

        private void Init()
        {
            AllModules = new List<ModuleBase>();

            //仿照这个添加新的系统到列表中
            AllModules.Add(new LoginModule());
        }
    }
}