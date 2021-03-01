//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventType = ColaFramework.EventType;

namespace ColaAbandoned
{
    public class LoginModule : ModuleBase
    {
        public LoginModule() : base(ModuleType.Login)
        {

        }

        public override void Init()
        {
            base.Init();
        }

        public void Login()
        {
            GameEventMgr.GetInstance().DispatchEvent("OpenUIWithReturn", EventType.UIMsg, "UILogin");
        }

        public void Logout()
        {

        }

        public override void Exit()
        {
            base.Exit();
        }

        protected override void RegisterHander()
        {
            base.RegisterHander();
        }
    }
}